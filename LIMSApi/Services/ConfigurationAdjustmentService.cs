using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class ConfigurationAdjustmentService : IConfigurationAdjustmentService
    {
        private readonly LIMSContext _context;
        private readonly IConfigurationAdjustmentRepository _repo;
        private readonly IUniversalTestGroupService _utgService;
        private readonly IBranchContext _branchContext;

        public ConfigurationAdjustmentService(
            LIMSContext context,
            IConfigurationAdjustmentRepository repo,
            IUniversalTestGroupService utgService,
            IBranchContext branchContext)
        {
            _context = context;
            _repo = repo;
            _utgService = utgService;
            _branchContext = branchContext;
        }

        private static string NormalizeSection(string? section)
        {
            if (string.IsNullOrWhiteSpace(section)) return "Equipment";
            var s = section.Trim();
            if (s.Equals("Equipment", StringComparison.OrdinalIgnoreCase) || s.Equals("Equipments", StringComparison.OrdinalIgnoreCase)) return "Equipment";
            if (s.Equals("Layout", StringComparison.OrdinalIgnoreCase) || s.Equals("ExecutionLayout", StringComparison.OrdinalIgnoreCase)) return "ExecutionLayout";
            if (s.Equals("TestMethod", StringComparison.OrdinalIgnoreCase) || s.Equals("TestMethodSpecification", StringComparison.OrdinalIgnoreCase) || s.Equals("Method", StringComparison.OrdinalIgnoreCase)) return "TestMethod";
            if (s.Equals("Specification", StringComparison.OrdinalIgnoreCase) || s.Equals("Spec", StringComparison.OrdinalIgnoreCase) || s.Equals("SpecificationHeader", StringComparison.OrdinalIgnoreCase) || s.Equals("Grade", StringComparison.OrdinalIgnoreCase)) return "Specification";
            return s;
        }

        private static string NormalizeChangeType(string? changeType)
        {
            if (string.IsNullOrWhiteSpace(changeType)) return "REPLACE";
            var c = changeType.Trim().ToUpperInvariant();
            if (c == "ADD") return "ADD";
            if (c == "REMOVE") return "REMOVE";
            if (c == "CHANGE") return "CHANGE";
            if (c == "REPLACE") return "REPLACE";
            if (c == "SUBSTITUTE") return "REPLACE";
            if (c == "OVERRIDE") return "OVERRIDE";
            return c;
        }

        private static string NormalizeVal(string? val)
        {
            if (string.IsNullOrWhiteSpace(val)) return string.Empty;
            var trimmed = val.Trim();
            if (decimal.TryParse(trimmed, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
            {
                return d.ToString("G29", System.Globalization.CultureInfo.InvariantCulture);
            }
            return trimmed;
        }

        private async Task<UniversalTestGroup> ValidateAccessAndLoadUtgAsync(long utgId, BranchAction action = BranchAction.View)
        {
            var utg = await _context.UniversalTestGroups
                .Include(u => u.Branch)
                .Include(u => u.LaboratoryTest)
                .Include(u => u.SampleTestPlan).ThenInclude(p => p.SampleDetail).ThenInclude(s => s.SampleInward)
                .Include(u => u.TestMethodSpecification)
                .Include(u => u.TestMethodSpecificationVersion)
                .Include(u => u.SpecificationHeader)
                .Include(u => u.SpecificationGrade)
                .Include(u => u.SpecificationVersion)
                .Include(u => u.ExecutionLayout)
                .FirstOrDefaultAsync(u => u.ID == utgId && u.IsActive);

            if (utg == null)
                throw new KeyNotFoundException($"Universal Test Group {utgId} not found.");

            var loggedInUser = LoggedInUserProvider.CurrentUser;
            string tenantCode = loggedInUser?.CompanyCode ?? string.Empty;
            if (!string.IsNullOrEmpty(utg.CompanyCode) && !string.IsNullOrEmpty(tenantCode) && utg.CompanyCode != tenantCode)
                throw new UnauthorizedAccessException("Access denied. Test group does not belong to your organization.");

            if (!_branchContext.IsAuthorizedForBranch(utg.BranchID, action))
                throw new UnauthorizedAccessException($"Access denied to branch {utg.BranchID}.");

            if (action != BranchAction.View)
            {
                if (utg.Status == "Verified" || utg.Status == "Approved")
                    throw new InvalidOperationException($"Cannot adjust configuration: Test group is in '{utg.Status}' status. Adjustments are immutable once reviewed or approved.");

                var isFinalized = await _context.UniversalTestResults.AnyAsync(r => r.UniversalTestGroupID == utgId && r.IsActive && (r.ResultStatus == "Verified" || r.ResultStatus == "Approved"));
                if (isFinalized)
                    throw new InvalidOperationException("Cannot adjust configuration: A verified or approved test result already exists for this test group.");
            }

            return utg;
        }

        public async Task<ConfigurationAdjustmentDetailDto?> GetActiveAdjustmentAsync(long utgId)
        {
            await ValidateAccessAndLoadUtgAsync(utgId, BranchAction.View);

            var adj = await _repo.GetActiveByTestGroupIdAsync(utgId);
            if (adj == null) return null;

            return await MapDetailDtoAsync(adj);
        }

        public async Task<ConfigurationAdjustmentDetailDto?> GetByIdAsync(long id)
        {
            var adj = await _repo.GetByIdAsync(id);
            if (adj == null) return null;

            await ValidateAccessAndLoadUtgAsync(adj.UniversalTestGroupID, BranchAction.View);
            return await MapDetailDtoAsync(adj);
        }

        public async Task<List<ConfigurationAdjustmentDetailDto>> GetHistoryAsync(long utgId)
        {
            await ValidateAccessAndLoadUtgAsync(utgId, BranchAction.View);

            var list = await _repo.GetHistoryByTestGroupIdAsync(utgId);
            var result = new List<ConfigurationAdjustmentDetailDto>();
            foreach (var adj in list)
            {
                result.Add(await MapDetailDtoAsync(adj));
            }
            return result;
        }

        public async Task<List<ConfigurationAdjustmentItemDto>> GetDifferenceAuditAsync(
            long utgId, string? section = null, string? changeType = null)
        {
            await ValidateAccessAndLoadUtgAsync(utgId, BranchAction.View);

            var items = await _repo.GetAuditItemsByTestGroupIdAsync(utgId, section, changeType);
            var empIds = items.Select(i => i.CreatedBy)
                .Concat(items.Where(i => i.AuthorizedBy.HasValue).Select(i => i.AuthorizedBy!.Value))
                .Distinct()
                .ToList();

            var empMap = await _context.EmployeeMasters
                .Where(e => empIds.Contains(e.ID))
                .ToDictionaryAsync(e => e.ID, e => e.Name.Trim());

            return items.Select(i => new ConfigurationAdjustmentItemDto
            {
                ID = i.ID,
                ConfigurationAdjustmentID = i.ConfigurationAdjustmentID,
                UniversalTestGroupID = i.UniversalTestGroupID,
                Section = i.Section,
                EntityType = i.EntityType,
                EntityID = i.EntityID,
                EntityCode = i.EntityCode,
                EntityName = i.EntityName,
                FieldName = i.FieldName,
                ChangeType = i.ChangeType,
                PlannedValue = i.PlannedValue,
                EffectiveValue = i.EffectiveValue,
                PreviousAdjustedValue = i.PreviousAdjustedValue,
                NewAdjustedValue = i.NewAdjustedValue,
                Reason = i.Reason,
                AuthorizationStatus = i.AuthorizationStatus,
                AuthorizedBy = i.AuthorizedBy,
                AuthorizedByName = i.AuthorizedBy.HasValue && empMap.TryGetValue(i.AuthorizedBy.Value, out var aName) ? aName : null,
                AuthorizedOn = i.AuthorizedOn,
                CreatedBy = i.CreatedBy,
                CreatedByName = empMap.TryGetValue(i.CreatedBy, out var cName) ? cName : null,
                CreatedOn = i.CreatedOn
            }).ToList();
        }

        public async Task<ComprehensiveDifferenceAuditDto> GetComprehensiveDifferenceAuditAsync(long utgId)
        {
            var utg = await ValidateAccessAndLoadUtgAsync(utgId, BranchAction.View);

            // 1. Load Planned Baseline
            PlannedConfigurationSnapshotDto? planned = null;
            if (!string.IsNullOrWhiteSpace(utg.PlannedConfigurationJson))
            {
                try
                {
                    planned = JsonSerializer.Deserialize<PlannedConfigurationSnapshotDto>(
                        utg.PlannedConfigurationJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch { }
            }

            // 2. Load Effective Configuration
            var eff = await _utgService.GetEffectiveConfigurationAsync(utgId);

            // 3. Load Active Adjustment and Adjusted Configuration (include unapproved for previewing comparison)
            var activeAdj = await _repo.GetActiveByTestGroupIdAsync(utgId);
            ConfigurationAdjustmentDetailDto? activeAdjDto = null;
            if (activeAdj != null)
            {
                activeAdjDto = await MapDetailDtoAsync(activeAdj);
            }
            var adjConfig = await GetAdjustedConfigurationAsync(utgId, includeUnapproved: true);

            // 4. Load History
            var history = await GetHistoryAsync(utgId);

            // 5. Load Logged Audit Adjustment Items
            var auditItems = await GetDifferenceAuditAsync(utgId);
            var itemMap = auditItems
                .GroupBy(i => $"{NormalizeSection(i.Section)}|{i.FieldName.Trim().ToUpperInvariant()}")
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CreatedOn).First());

            var comparisonList = new List<DifferenceAuditComparisonItemDto>();

            // Helper to get audit item match
            ConfigurationAdjustmentItemDto? FindAuditItem(string section, string fieldName)
            {
                var normSec = NormalizeSection(section);
                var normField = fieldName.Trim().ToUpperInvariant();
                if (itemMap.TryGetValue($"{normSec}|{normField}", out var itm))
                    return itm;
                return auditItems.FirstOrDefault(i =>
                    NormalizeSection(i.Section) == normSec &&
                    i.FieldName.Trim().Equals(fieldName.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            void AddComparison(string section, string entityType, long? entityId, string? entityCode, string? entityName, string fieldName, string? pVal, string? eVal, string? aVal)
            {
                string pNorm = NormalizeVal(pVal);
                string eNorm = NormalizeVal(eVal);
                string aNorm = NormalizeVal(aVal);

                string classification = "UNCHANGED";
                if (string.IsNullOrWhiteSpace(pNorm) && string.IsNullOrWhiteSpace(eNorm) && !string.IsNullOrWhiteSpace(aNorm))
                {
                    classification = "ADDED";
                }
                else if (!string.Equals(pNorm, aNorm, StringComparison.OrdinalIgnoreCase) || !string.Equals(eNorm, aNorm, StringComparison.OrdinalIgnoreCase) || !string.Equals(pNorm, eNorm, StringComparison.OrdinalIgnoreCase))
                {
                    classification = "CHANGED";
                }

                var match = FindAuditItem(section, fieldName);

                comparisonList.Add(new DifferenceAuditComparisonItemDto
                {
                    Section = section,
                    EntityType = entityType,
                    EntityID = entityId,
                    EntityCode = entityCode,
                    EntityName = entityName,
                    FieldName = fieldName,
                    Classification = classification,
                    PlannedValue = pVal,
                    EffectiveValue = eVal,
                    AdjustedValue = aVal,
                    Reason = match?.Reason,
                    AuthorizationStatus = match?.AuthorizationStatus ?? (activeAdj != null ? activeAdj.Status : "Planned Baseline"),
                    CreatedByName = match?.CreatedByName ?? activeAdjDto?.CreatedByName,
                    CreatedOn = match?.CreatedOn ?? activeAdj?.CreatedOn,
                    AuthorizedByName = match?.AuthorizedByName ?? activeAdjDto?.ApprovedByName ?? activeAdjDto?.RejectedByName,
                    AuthorizedOn = match?.AuthorizedOn ?? (activeAdj?.Status == "Approved" ? activeAdj?.ApprovedOn : (activeAdj?.Status == "Rejected" ? activeAdj?.ModifiedOn : null))
                });
            }

            // --- 6. POPULATE CONTROLLED DEVIATION AREAS (Planning + Test-Setup) ---

            // A. TEST METHOD SPECIFICATION
            string plannedMethod = planned?.TestMethodName ?? planned?.TestMethodCode ?? utg.TestMethodSpecification?.Name ?? "None";
            string effectiveMethod = utg.TestMethodSpecification?.Name ?? plannedMethod;
            string adjustedMethod = adjConfig?.TestMethodName ?? effectiveMethod;
            AddComparison("TestMethod", "TestMethod", adjConfig?.TestMethodSpecificationID ?? utg.TestMethodSpecificationID, adjConfig?.TestMethodCode ?? utg.TestMethodSpecification?.Code, adjConfig?.TestMethodName ?? utg.TestMethodSpecification?.Name, "TestMethodSpecification", plannedMethod, effectiveMethod, adjustedMethod);

            // B. SPECIFICATION & GRADE
            string plannedSpecGrade = (planned?.SpecificationTitle ?? utg.SpecificationHeader?.Title ?? utg.SpecificationHeader?.AliasName ?? "None") + (string.IsNullOrEmpty(planned?.GradeName ?? utg.SpecificationGrade?.Grade) ? "" : $" (Grade: {planned?.GradeName ?? utg.SpecificationGrade?.Grade})");
            string effectiveSpecGrade = (utg.SpecificationHeader?.Title ?? utg.SpecificationHeader?.AliasName ?? planned?.SpecificationTitle ?? "None") + (string.IsNullOrEmpty(utg.SpecificationGrade?.Grade ?? planned?.GradeName) ? "" : $" (Grade: {utg.SpecificationGrade?.Grade ?? planned?.GradeName})");
            string adjustedSpecGrade = (adjConfig?.SpecificationName ?? utg.SpecificationHeader?.Title ?? utg.SpecificationHeader?.AliasName ?? planned?.SpecificationTitle ?? "None") + (string.IsNullOrEmpty(adjConfig?.SpecificationGradeName ?? utg.SpecificationGrade?.Grade ?? planned?.GradeName) ? "" : $" (Grade: {adjConfig?.SpecificationGradeName ?? utg.SpecificationGrade?.Grade ?? planned?.GradeName})");
            AddComparison("Specification", "Specification", adjConfig?.SpecificationHeaderID ?? utg.SpecificationHeaderID, adjConfig?.SpecificationCode ?? utg.SpecificationHeader?.Code, adjConfig?.SpecificationName ?? utg.SpecificationHeader?.Title ?? utg.SpecificationHeader?.AliasName, "SpecificationGrade", plannedSpecGrade, effectiveSpecGrade, adjustedSpecGrade);

            // C. EXECUTION LAYOUT
            string plannedLayout = planned?.ExecutionLayoutCode ?? "DEFAULT";
            string effectiveLayout = eff?.Layout?.EffectiveLayoutCode ?? "DEFAULT";
            string adjustedLayout = adjConfig?.Layout?.EffectiveLayoutCode ?? effectiveLayout;
            AddComparison("ExecutionLayout", "ExecutionLayout", adjConfig?.Layout?.EffectiveExecutionLayoutID ?? eff?.Layout?.EffectiveExecutionLayoutID, adjConfig?.Layout?.EffectiveLayoutCode ?? eff?.Layout?.EffectiveLayoutCode, adjConfig?.Layout?.EffectiveLayoutName ?? eff?.Layout?.EffectiveLayoutName, "ExecutionLayout", plannedLayout, effectiveLayout, adjustedLayout);

            // D. EQUIPMENT ASSIGNMENTS
            if (eff?.Equipment != null && eff.Equipment.Any())
            {
                foreach (var eqE in eff.Equipment)
                {
                    var plannedEq = planned?.EquipmentRequirements?.FirstOrDefault(r => r.EquipmentRequirementMasterID == eqE.EquipmentRequirementID);
                    var adjustedEq = adjConfig?.Equipment?.FirstOrDefault(r => r.EquipmentRequirementID == eqE.EquipmentRequirementID);

                    string pVal = plannedEq?.EquipmentName ?? "Unassigned";
                    string eVal = eqE.Name ?? "Unassigned";
                    string aVal = adjustedEq?.Name ?? eVal;

                    AddComparison("Equipment", "Equipment", eqE.EquipmentRequirementID, eqE.RequirementCode, eqE.RequirementName, eqE.RequirementName ?? eqE.RequirementCode ?? "EquipmentAssignment", pVal, eVal, aVal);
                }
            }
            else
            {
                AddComparison("Equipment", "Equipment", null, "EQUIPMENT", "Equipment Requirement", "EquipmentAssignment", "None", "None", "None");
            }

            int total = comparisonList.Count;
            int added = comparisonList.Count(x => x.Classification == "ADDED");
            int removed = comparisonList.Count(x => x.Classification == "REMOVED");
            int changed = comparisonList.Count(x => x.Classification == "CHANGED");
            int unchanged = comparisonList.Count(x => x.Classification == "UNCHANGED");

            return new ComprehensiveDifferenceAuditDto
            {
                UniversalTestGroupID = utgId,
                AdjustmentNumber = activeAdj?.AdjustmentNumber ?? 0,
                AdjustmentStatus = activeAdj != null ? activeAdj.Status : "No Adjustment",
                TotalItems = total,
                AddedCount = added,
                RemovedCount = removed,
                ChangedCount = changed,
                UnchangedCount = unchanged,
                ComparisonItems = comparisonList,
                AdjustmentItems = auditItems,
                History = history
            };
        }

        public async Task<ConfigurationAdjustmentDetailDto> SaveDraftAsync(ConfigurationAdjustmentDraftDto draft)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));
            if (string.IsNullOrWhiteSpace(draft.OverallReason))
                throw new ArgumentException("Overall adjustment reason is mandatory and cannot be whitespace.");

            var utg = await ValidateAccessAndLoadUtgAsync(draft.UniversalTestGroupID, BranchAction.Edit);

            // Phase 6 boundary check: once execution has started, Phase 5 is locked
            if (await _repo.HasExecutionStartedAsync(draft.UniversalTestGroupID))
                throw new InvalidOperationException("Test execution has already commenced for this Universal Test Group. Configuration adjustments are locked.");

            var loggedInUser = LoggedInUserProvider.CurrentUser;
            long currentUserId = loggedInUser?.EmployeeID ?? 0;
            string companyCode = loggedInUser?.CompanyCode ?? utg.CompanyCode ?? "LIMS";

            var activeAdj = await _repo.GetActiveByTestGroupIdAsync(draft.UniversalTestGroupID);
            ConfigurationAdjustment adjToSave;

            if (activeAdj != null && activeAdj.Status == "Draft")
            {
                // Optimistic Concurrency Check
                if (!string.IsNullOrWhiteSpace(draft.ConcurrencyToken) &&
                    !string.Equals(activeAdj.ConcurrencyToken, draft.ConcurrencyToken, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("The configuration adjustment has been modified by another user. Please refresh and try again.");
                }

                // Update existing draft
                activeAdj.OverallReason = draft.OverallReason.Trim();
                activeAdj.ConcurrencyToken = Guid.NewGuid().ToString("N");
                activeAdj.ModifiedBy = currentUserId;
                activeAdj.ModifiedOn = DateTime.UtcNow;

                // Remove existing items and re-add
                _context.ConfigurationAdjustmentItems.RemoveRange(activeAdj.Items);
                activeAdj.Items.Clear();

                foreach (var itemDto in draft.Items)
                {
                    if (string.IsNullOrWhiteSpace(itemDto.Reason))
                        throw new ArgumentException($"Reason is mandatory for adjustment item on field '{itemDto.FieldName}'.");

                    activeAdj.Items.Add(new ConfigurationAdjustmentItem
                    {
                        ConfigurationAdjustmentID = activeAdj.ID,
                        UniversalTestGroupID = utg.ID,
                        Section = itemDto.Section,
                        EntityType = itemDto.EntityType,
                        EntityID = itemDto.EntityID,
                        EntityCode = itemDto.EntityCode,
                        EntityName = itemDto.EntityName,
                        FieldName = itemDto.FieldName,
                        ChangeType = itemDto.ChangeType,
                        PlannedValue = itemDto.PlannedValue,
                        EffectiveValue = itemDto.EffectiveValue,
                        PreviousAdjustedValue = itemDto.PreviousAdjustedValue,
                        NewAdjustedValue = itemDto.NewAdjustedValue ?? itemDto.AdjustedValue,
                        Reason = itemDto.Reason.Trim(),

                        AuthorizationStatus = "Pending",
                        BranchID = utg.BranchID,
                        CompanyCode = companyCode,
                        CreatedBy = currentUserId,
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }

                await _repo.UpdateAsync(activeAdj);
                adjToSave = activeAdj;
            }
            else
            {
                int nextNo = await _repo.GetNextAdjustmentNumberAsync(draft.UniversalTestGroupID);
                adjToSave = new ConfigurationAdjustment
                {
                    UniversalTestGroupID = utg.ID,
                    AdjustmentNumber = nextNo,
                    Status = "Draft",
                    OverallReason = draft.OverallReason.Trim(),
                    BranchID = utg.BranchID,
                    CompanyCode = companyCode,
                    ConcurrencyToken = Guid.NewGuid().ToString("N"),
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true
                };

                foreach (var itemDto in draft.Items)
                {
                    if (string.IsNullOrWhiteSpace(itemDto.Reason))
                        throw new ArgumentException($"Reason is mandatory for adjustment item on field '{itemDto.FieldName}'.");

                    adjToSave.Items.Add(new ConfigurationAdjustmentItem
                    {
                        UniversalTestGroupID = utg.ID,
                        Section = itemDto.Section,
                        EntityType = itemDto.EntityType,
                        EntityID = itemDto.EntityID,
                        EntityCode = itemDto.EntityCode,
                        EntityName = itemDto.EntityName,
                        FieldName = itemDto.FieldName,
                        ChangeType = itemDto.ChangeType,
                        PlannedValue = itemDto.PlannedValue,
                        EffectiveValue = itemDto.EffectiveValue,
                        PreviousAdjustedValue = itemDto.PreviousAdjustedValue,
                        NewAdjustedValue = itemDto.NewAdjustedValue ?? itemDto.AdjustedValue,
                        Reason = itemDto.Reason.Trim(),

                        AuthorizationStatus = "Pending",
                        BranchID = utg.BranchID,
                        CompanyCode = companyCode,
                        CreatedBy = currentUserId,
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true
                    });
                }

                await _repo.CreateAsync(adjToSave);
            }

            return await MapDetailDtoAsync(adjToSave);
        }

        public async Task<AdjustmentValidationResultDto> ValidateAdjustmentAsync(long utgId, List<ConfigurationAdjustmentItemDto> items)
        {
            var utg = await ValidateAccessAndLoadUtgAsync(utgId, BranchAction.View);
            var result = new AdjustmentValidationResultDto { IsValid = true, CanApply = true };

            if (!items.Any())
            {
                result.Warnings.Add("No deviation items provided.");
                return result;
            }

            foreach (var item in items)
            {
                var itemVal = new AdjustmentItemValidationDto
                {
                    Section = item.Section,
                    FieldName = item.FieldName,
                    EntityCode = item.EntityCode,
                    IsValid = true
                };

                // Rule 1: Mandatory Reason
                if (string.IsNullOrWhiteSpace(item.Reason))
                {
                    itemVal.IsValid = false;
                    itemVal.IsBlocking = true;
                    itemVal.Message = "Reason is mandatory for this adjustment.";
                    result.BlockingErrors.Add($"[{item.Section}] Reason is mandatory for field '{item.FieldName}'.");
                    result.IsValid = false;
                    result.CanApply = false;
                    result.ItemValidations.Add(itemVal);
                    continue;
                }

                var normSec = NormalizeSection(item.Section);

                // RULE 2: STRICT PROHIBITION OF SCIENTIFIC PARAMETERS / FORMULAS / LIMITS / MU / CRITERIA
                if (normSec.Equals("Parameters", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Parameter", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Requirements", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Requirement", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Conditions", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Condition", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Factors", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Factor", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Uncertainty", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("AcceptanceCriteria", StringComparison.OrdinalIgnoreCase) ||
                    normSec.Equals("Acceptance", StringComparison.OrdinalIgnoreCase))
                {
                    itemVal.IsValid = false;
                    itemVal.IsBlocking = true;
                    itemVal.Message = $"Execution deviation is not permitted for scientific section '{item.Section}'. Parameters, formulas, limits, conditions, factors, uncertainty, and acceptance criteria are authoritative and Read-Only.";
                    result.BlockingErrors.Add($"[{item.Section}] Scientific configuration is immutable in Phase 5. Execution deviation is restricted to Equipment, ExecutionLayout, TestMethod, and Specification.");
                    result.IsValid = false;
                    result.CanApply = false;
                    result.ItemValidations.Add(itemVal);
                    continue;
                }

                var adjustedVal = item.NewAdjustedValue ?? item.AdjustedValue;

                // RULE 3: VALIDATE ALLOWED DEVIATION CATEGORIES
                switch (normSec)
                {
                    case "Equipment":
                        if (long.TryParse(adjustedVal, out var newEqId) || item.EntityID.HasValue)
                        {
                            var eqIdToCheck = long.TryParse(adjustedVal, out var parsedEqId) ? parsedEqId : item.EntityID!.Value;
                            var eq = await _context.EquipmentMasters
                                .Include(e => e.Calibrations)
                                .FirstOrDefaultAsync(e => e.ID == eqIdToCheck && e.IsActive && e.BranchID == utg.BranchID);

                            if (eq == null)
                            {
                                itemVal.IsValid = false;
                                itemVal.IsBlocking = true;
                                itemVal.Message = $"Equipment {eqIdToCheck} is inactive, does not exist, or does not belong to branch {utg.BranchID}.";
                            }
                            else if (eq.CalibrationRequired)
                            {
                                var latestCal = eq.Calibrations?.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                                var due = latestCal?.CalibrationDueDate ?? eq.NextCalibrationDueDate;
                                if (!due.HasValue || due.Value < DateTime.UtcNow)
                                {
                                    itemVal.IsValid = false;
                                    itemVal.IsBlocking = true;
                                    itemVal.Message = $"Equipment '{eq.Name}' has expired or unconfigured calibration.";
                                }
                            }
                        }
                        else
                        {
                            itemVal.IsValid = false;
                            itemVal.IsBlocking = true;
                            itemVal.Message = "A valid alternate Equipment ID must be specified.";
                        }
                        break;

                    case "ExecutionLayout":
                        if (long.TryParse(adjustedVal, out var newLayId) || item.EntityID.HasValue)
                        {
                            var layIdToCheck = long.TryParse(adjustedVal, out var parsedLayId) ? parsedLayId : item.EntityID!.Value;
                            var lay = await _context.ExecutionLayoutMasters.FindAsync(layIdToCheck);
                            if (lay == null || !lay.IsActive)
                            {
                                itemVal.IsValid = false;
                                itemVal.IsBlocking = true;
                                itemVal.Message = $"Execution layout {layIdToCheck} is inactive or not found.";
                            }
                        }
                        else
                        {
                            itemVal.IsValid = false;
                            itemVal.IsBlocking = true;
                            itemVal.Message = "A valid alternate Execution Layout ID must be specified.";
                        }
                        break;

                    case "TestMethod":
                        if (long.TryParse(adjustedVal, out var newTmId) || item.EntityID.HasValue)
                        {
                            var tmIdToCheck = long.TryParse(adjustedVal, out var parsedTmId) ? parsedTmId : item.EntityID!.Value;
                            var tm = await _context.TestMethodSpecifications.FindAsync(tmIdToCheck);
                            if (tm == null || !tm.IsActive || tm.IsDisabled)
                            {
                                itemVal.IsValid = false;
                                itemVal.IsBlocking = true;
                                itemVal.Message = $"Test Method Specification {tmIdToCheck} is inactive, disabled, or not found.";
                            }
                        }
                        else
                        {
                            itemVal.IsValid = false;
                            itemVal.IsBlocking = true;
                            itemVal.Message = "A valid alternate Test Method Specification ID must be specified.";
                        }
                        break;

                    case "Specification":
                        if (long.TryParse(adjustedVal, out var newSpecId) || item.EntityID.HasValue)
                        {
                            var specIdToCheck = long.TryParse(adjustedVal, out var parsedSpecId) ? parsedSpecId : item.EntityID!.Value;
                            var spec = await _context.SpecificationHeaders.FindAsync(specIdToCheck);
                            if (spec == null || !spec.IsActive)
                            {
                                itemVal.IsValid = false;
                                itemVal.IsBlocking = true;
                                itemVal.Message = $"Specification {specIdToCheck} is inactive or not found.";
                            }
                        }
                        else
                        {
                            itemVal.IsValid = false;
                            itemVal.IsBlocking = true;
                            itemVal.Message = "A valid alternate Specification ID must be specified.";
                        }
                        break;

                    default:
                        itemVal.IsValid = false;
                        itemVal.IsBlocking = true;
                        itemVal.Message = $"Unsupported section '{item.Section}'. Allowed categories: Equipment, ExecutionLayout, TestMethod, Specification.";
                        break;
                }

                if (!itemVal.IsValid)
                {
                    result.IsValid = false;
                    if (itemVal.IsBlocking)
                    {
                        result.CanApply = false;
                        result.BlockingErrors.Add(itemVal.Message ?? "Validation failed.");
                    }
                }

                result.ItemValidations.Add(itemVal);
            }

            return result;
        }

        public async Task<ConfigurationAdjustmentDetailDto> ApplyAdjustmentAsync(ApplyAdjustmentRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var utg = await ValidateAccessAndLoadUtgAsync(request.UniversalTestGroupID, BranchAction.Edit);

            if (await _repo.HasExecutionStartedAsync(request.UniversalTestGroupID))
                throw new InvalidOperationException("Test execution has already commenced for this Universal Test Group. Configuration adjustments are locked.");

            var adj = await _repo.GetByIdAsync(request.ConfigurationAdjustmentID);
            if (adj == null || adj.UniversalTestGroupID != request.UniversalTestGroupID)
                throw new KeyNotFoundException($"Configuration adjustment {request.ConfigurationAdjustmentID} not found for this test group.");

            if (adj.Status != "Draft")
                throw new InvalidOperationException($"Adjustment is already in '{adj.Status}' status and cannot be applied again.");

            // Optimistic concurrency check
            if (!string.IsNullOrWhiteSpace(request.ConcurrencyToken) &&
                !string.Equals(adj.ConcurrencyToken, request.ConcurrencyToken, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Concurrency conflict: The adjustment has been modified by another user. Please refresh.");
            }

            // Run validation
            var itemsDto = adj.Items.Select(i => new ConfigurationAdjustmentItemDto
            {
                Section = i.Section,
                EntityType = i.EntityType,
                EntityID = i.EntityID,
                EntityCode = i.EntityCode,
                FieldName = i.FieldName,
                ChangeType = i.ChangeType,
                PlannedValue = i.PlannedValue,
                EffectiveValue = i.EffectiveValue,
                NewAdjustedValue = i.NewAdjustedValue,
                Reason = i.Reason
            }).ToList();

            var validation = await ValidateAdjustmentAsync(request.UniversalTestGroupID, itemsDto);
            if (!validation.CanApply)
            {
                throw new InvalidOperationException($"Cannot apply adjustment due to blocking errors: {string.Join("; ", validation.BlockingErrors)}");
            }

            // Load Phase 4 Effective Configuration
            var eff = await _utgService.GetEffectiveConfigurationAsync(request.UniversalTestGroupID);

            // Execute Single Authoritative BuildAdjustedConfiguration engine
            var adjustedConfig = await BuildAdjustedConfigurationInternalAsync(eff, adj, adj.Items);

            var loggedInUser = LoggedInUserProvider.CurrentUser;
            adj.Status = "Applied";
            adj.AppliedBy = loggedInUser?.EmployeeID ?? 0;
            adj.AppliedOn = DateTime.UtcNow;
            adj.ConcurrencyToken = Guid.NewGuid().ToString("N");
            adj.AdjustedConfigurationJson = JsonSerializer.Serialize(adjustedConfig, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            await _repo.UpdateAsync(adj);

            return await MapDetailDtoAsync(adj, adjustedConfig);
        }

        public async Task<ConfigurationAdjustmentDetailDto> ApproveAdjustmentAsync(ApproveAdjustmentRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var utg = await ValidateAccessAndLoadUtgAsync(request.UniversalTestGroupID, BranchAction.Approve);

            if (await _repo.HasExecutionStartedAsync(request.UniversalTestGroupID))
                throw new InvalidOperationException("Test execution has already commenced for this Universal Test Group. Configuration adjustments are locked.");

            var adj = await _repo.GetByIdAsync(request.ConfigurationAdjustmentID);
            if (adj == null || adj.UniversalTestGroupID != request.UniversalTestGroupID)
                throw new KeyNotFoundException($"Configuration adjustment {request.ConfigurationAdjustmentID} not found for this test group.");

            if (adj.Status != "Applied")
                throw new InvalidOperationException($"Only an 'Applied' adjustment can be approved. Current status: '{adj.Status}'.");

            // Optimistic concurrency check
            if (!string.IsNullOrWhiteSpace(request.ConcurrencyToken) &&
                !string.Equals(adj.ConcurrencyToken, request.ConcurrencyToken, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Concurrency conflict: The adjustment has been modified by another user. Please refresh.");
            }

            var loggedInUser = LoggedInUserProvider.CurrentUser;
            long currentUserId = loggedInUser?.EmployeeID ?? 0;

            // STRICT FOUR-EYES SEGREGATION OF DUTIES (ZERO ADMIN BYPASS)
            if (adj.CreatedBy == currentUserId || (adj.AppliedBy.HasValue && adj.AppliedBy.Value == currentUserId))
            {
                throw new InvalidOperationException("Segregation of duties violation: The user who created or applied this adjustment cannot approve it. An independent authorized reviewer must approve.");
            }

            // Re-validate against current live baseline
            var itemsDto = adj.Items.Select(i => new ConfigurationAdjustmentItemDto
            {
                Section = i.Section,
                EntityType = i.EntityType,
                EntityID = i.EntityID,
                EntityCode = i.EntityCode,
                FieldName = i.FieldName,
                ChangeType = i.ChangeType,
                PlannedValue = i.PlannedValue,
                EffectiveValue = i.EffectiveValue,
                NewAdjustedValue = i.NewAdjustedValue,
                Reason = i.Reason
            }).ToList();

            var validation = await ValidateAdjustmentAsync(request.UniversalTestGroupID, itemsDto);
            if (!validation.CanApply)
            {
                throw new InvalidOperationException($"Cannot approve adjustment due to blocking validation errors against current baseline: {string.Join("; ", validation.BlockingErrors)}");
            }

            // Mark items approved
            foreach (var item in adj.Items)
            {
                item.AuthorizationStatus = "Approved";
                item.AuthorizedBy = currentUserId;
                item.AuthorizedOn = DateTime.UtcNow;
            }

            // Load Phase 4 Effective Configuration and re-render final adjusted handoff
            var eff = await _utgService.GetEffectiveConfigurationAsync(request.UniversalTestGroupID);
            var adjustedConfig = await BuildAdjustedConfigurationInternalAsync(eff, adj, adj.Items);
            adjustedConfig.AdjustmentStatus = "Approved";

            adj.Status = "Approved";
            adj.ApprovedBy = currentUserId;
            adj.ApprovedOn = DateTime.UtcNow;
            adj.ApprovalRemarks = request.ApprovalRemarks?.Trim();
            adj.ConcurrencyToken = Guid.NewGuid().ToString("N");
            adj.AdjustedConfigurationJson = JsonSerializer.Serialize(adjustedConfig, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            await _repo.UpdateAsync(adj);

            return await MapDetailDtoAsync(adj, adjustedConfig);
        }

        public async Task<ConfigurationAdjustmentDetailDto> RejectAdjustmentAsync(RejectAdjustmentRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
                throw new ArgumentException("Rejection reason is mandatory and cannot be whitespace.");

            var utg = await ValidateAccessAndLoadUtgAsync(request.UniversalTestGroupID, BranchAction.Approve);

            if (await _repo.HasExecutionStartedAsync(request.UniversalTestGroupID))
                throw new InvalidOperationException("Test execution has already commenced for this Universal Test Group. Configuration adjustments are locked.");

            var adj = await _repo.GetByIdAsync(request.ConfigurationAdjustmentID);
            if (adj == null || adj.UniversalTestGroupID != request.UniversalTestGroupID)
                throw new KeyNotFoundException($"Configuration adjustment {request.ConfigurationAdjustmentID} not found for this test group.");

            if (adj.Status != "Applied")
                throw new InvalidOperationException($"Only an 'Applied' adjustment can be rejected. Current status: '{adj.Status}'.");

            // Optimistic concurrency check
            if (!string.IsNullOrWhiteSpace(request.ConcurrencyToken) &&
                !string.Equals(adj.ConcurrencyToken, request.ConcurrencyToken, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Concurrency conflict: The adjustment has been modified by another user. Please refresh.");
            }

            var loggedInUser = LoggedInUserProvider.CurrentUser;
            long currentUserId = loggedInUser?.EmployeeID ?? 0;

            // STRICT FOUR-EYES SEGREGATION OF DUTIES (ZERO ADMIN BYPASS)
            if (adj.CreatedBy == currentUserId || (adj.AppliedBy.HasValue && adj.AppliedBy.Value == currentUserId))
            {
                throw new InvalidOperationException("Segregation of duties violation: The user who created or applied this adjustment cannot reject it. An independent authorized reviewer must review.");
            }

            // User Correction 1: Do not populate ApprovedBy/ApprovedOn for a rejected adjustment.
            adj.Status = "Rejected";
            adj.ApprovedBy = null;
            adj.ApprovedOn = null;
            adj.ApprovalRemarks = request.RejectionReason.Trim();
            adj.ModifiedBy = currentUserId;
            adj.ModifiedOn = DateTime.UtcNow;
            adj.ConcurrencyToken = Guid.NewGuid().ToString("N");

            foreach (var item in adj.Items)
            {
                item.AuthorizationStatus = "Rejected";
                item.AuthorizedBy = currentUserId;
                item.AuthorizedOn = DateTime.UtcNow;
                item.ModifiedBy = currentUserId;
                item.ModifiedOn = DateTime.UtcNow;
            }

            await _repo.UpdateAsync(adj);

            return await MapDetailDtoAsync(adj);
        }

        public async Task<DeviationLookupResultDto> GetDeviationLookupOptionsAsync(long utgId, string deviationCategory)
        {
            var utg = await ValidateAccessAndLoadUtgAsync(utgId, BranchAction.View);
            var eff = await _utgService.GetEffectiveConfigurationAsync(utgId);
            var normCat = NormalizeSection(deviationCategory);

            var result = new DeviationLookupResultDto
            {
                UniversalTestGroupID = utgId,
                DeviationCategory = normCat
            };

            switch (normCat)
            {
                case "Equipment":
                    var currentEq = eff.Equipment?.FirstOrDefault();
                    result.CurrentEntityID = currentEq?.EquipmentID;
                    result.CurrentValueDisplay = currentEq != null
                        ? $"{currentEq.RequirementName}: {currentEq.Name ?? "Unassigned"} ({currentEq.ReadinessStatus})"
                        : "No equipment assigned";

                    var allBranchEq = await _context.EquipmentMasters
                        .Include(e => e.Calibrations)
                        .Include(e => e.EquipmentType)
                        .Where(e => e.BranchID == utg.BranchID && e.IsActive)
                        .OrderBy(e => e.Name)
                        .ToListAsync();

                    foreach (var eq in allBranchEq)
                    {
                        var latestCal = eq.Calibrations?.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                        var dueDate = latestCal?.CalibrationDueDate ?? eq.NextCalibrationDueDate;
                        bool isCalValid = !eq.CalibrationRequired || (dueDate.HasValue && dueDate.Value >= DateTime.UtcNow);

                        result.AvailableOptions.Add(new DeviationOptionDto
                        {
                            ID = eq.ID,
                            Code = eq.EquipmentNo ?? eq.ID.ToString(),
                            Name = eq.Name,
                            Description = $"Model: {eq.ModelNo ?? "N/A"} | Cal: {(isCalValid ? "Valid" : "Expired/Due")}",
                            SecondaryInfo = eq.EquipmentType?.Name,
                            SecondaryID = currentEq?.EquipmentRequirementID,
                            IsCurrent = currentEq != null && currentEq.EquipmentID == eq.ID
                        });
                    }
                    break;

                case "ExecutionLayout":
                    result.CurrentEntityID = eff.Layout.EffectiveExecutionLayoutID;
                    result.CurrentValueDisplay = $"{eff.Layout.EffectiveLayoutCode} - {eff.Layout.EffectiveLayoutName} ({eff.Layout.EffectiveRendererType})";

                    var layouts = await _context.ExecutionLayoutMasters
                        .Where(l => l.IsActive)
                        .OrderBy(l => l.Code)
                        .ToListAsync();

                    foreach (var lay in layouts)
                    {
                        result.AvailableOptions.Add(new DeviationOptionDto
                        {
                            ID = lay.ID,
                            Code = lay.Code,
                            Name = lay.Name,
                            Description = $"Renderer: {lay.RendererType}",
                            IsCurrent = eff.Layout.EffectiveExecutionLayoutID == lay.ID
                        });
                    }
                    break;

                case "TestMethod":
                    result.CurrentEntityID = utg.TestMethodSpecificationID;
                    result.CurrentValueDisplay = utg.TestMethodSpecification != null
                        ? $"{utg.TestMethodSpecification.Code} - {utg.TestMethodSpecification.Name}"
                        : "None";

                    var methods = await _context.TestMethodSpecifications
                        .Where(m => m.IsActive && !m.IsDisabled)
                        .OrderBy(m => m.Name)
                        .ToListAsync();

                    foreach (var tm in methods)
                    {
                        result.AvailableOptions.Add(new DeviationOptionDto
                        {
                            ID = tm.ID,
                            Code = tm.Code ?? tm.TestMethodStandard,
                            Name = tm.Name,
                            Description = tm.DisplayTitle ?? tm.Description,
                            IsCurrent = utg.TestMethodSpecificationID == tm.ID
                        });
                    }
                    break;

                case "Specification":
                    result.CurrentEntityID = utg.SpecificationHeaderID;
                    result.CurrentValueDisplay = utg.SpecificationHeader != null
                        ? $"{utg.SpecificationHeader.Code} - {utg.SpecificationHeader.Title ?? utg.SpecificationHeader.AliasName} {(utg.SpecificationGrade != null ? ("Grade: " + utg.SpecificationGrade.Grade) : "")}"
                        : "None";

                    var specs = await _context.SpecificationHeaders
                        .Include(s => s.Grades)
                        .Where(s => s.IsActive)
                        .OrderBy(s => s.Title ?? s.AliasName)
                        .ToListAsync();

                    foreach (var s in specs)
                    {
                        var gradeNames = s.Grades?.Select(g => g.Grade).ToList();
                        string gradesStr = gradeNames != null && gradeNames.Any() ? $"Grades: {string.Join(", ", gradeNames)}" : "";

                        result.AvailableOptions.Add(new DeviationOptionDto
                        {
                            ID = s.ID,
                            Code = s.Code ?? "",
                            Name = s.Title ?? s.AliasName,
                            Description = string.IsNullOrEmpty(gradesStr) ? s.Description : $"{s.Description} | {gradesStr}",
                            IsCurrent = utg.SpecificationHeaderID == s.ID
                        });
                    }
                    break;

                default:
                    throw new ArgumentException($"Unsupported deviation category: '{deviationCategory}'. Allowed categories: Equipment, ExecutionLayout, TestMethod, Specification.");
            }

            return result;
        }

        public async Task<ConfigurationAdjustmentDetailDto> RequestExecutionDeviationAsync(ExecutionDeviationRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new ArgumentException("Deviation reason is mandatory and cannot be whitespace.");

            var utg = await ValidateAccessAndLoadUtgAsync(request.UniversalTestGroupID, BranchAction.Edit);

            if (await _repo.HasExecutionStartedAsync(request.UniversalTestGroupID))
                throw new InvalidOperationException("Test execution has already commenced for this Universal Test Group. Configuration adjustments and deviations are locked.");

            var normCat = NormalizeSection(request.DeviationCategory);
            var lookup = await GetDeviationLookupOptionsAsync(request.UniversalTestGroupID, normCat);

            var selectedOption = lookup.AvailableOptions.FirstOrDefault(o => o.ID == request.SelectedAlternativeID);
            if (selectedOption == null)
                throw new ArgumentException($"Selected alternative ID {request.SelectedAlternativeID} is not a valid active option for category '{normCat}'.");

            long currentUserId = LoggedInUserProvider.CurrentUser?.EmployeeID ?? 0;
            string companyCode = LoggedInUserProvider.CurrentUser?.CompanyCode ?? utg.CompanyCode ?? "LIMS";

            string fieldName = normCat switch
            {
                "Equipment" => "EquipmentAssignment",
                "ExecutionLayout" => "ExecutionLayoutID",
                "TestMethod" => "TestMethodSpecificationID",
                "Specification" => "SpecificationHeaderID",
                _ => throw new ArgumentException($"Unsupported deviation category: '{normCat}'.")
            };

            long targetEntityId = request.TargetEntityID ?? selectedOption.SecondaryID ?? lookup.CurrentEntityID ?? request.SelectedAlternativeID;

            string reasonWithEvidence = string.IsNullOrWhiteSpace(request.EvidenceReference)
                ? request.Reason.Trim()
                : $"{request.Reason.Trim()} [Evidence: {request.EvidenceReference.Trim()}]";

            var itemDto = new ConfigurationAdjustmentItemDto
            {
                UniversalTestGroupID = utg.ID,
                Section = normCat,
                EntityType = normCat,
                EntityID = targetEntityId,
                EntityCode = selectedOption.Code,
                EntityName = selectedOption.Name,
                FieldName = fieldName,
                ChangeType = "REPLACE",
                PlannedValue = lookup.CurrentValueDisplay,
                EffectiveValue = lookup.CurrentValueDisplay,
                NewAdjustedValue = request.SelectedAlternativeID.ToString(),
                Reason = reasonWithEvidence,
                AuthorizationStatus = "Pending",
                CreatedBy = currentUserId,
                CreatedOn = DateTime.UtcNow
            };

            var draftDto = new ConfigurationAdjustmentDraftDto
            {
                UniversalTestGroupID = utg.ID,
                OverallReason = $"Execution Deviation: {normCat} replaced with {selectedOption.Name} ({selectedOption.Code}). Reason: {request.Reason.Trim()}",
                ConcurrencyToken = request.ConcurrencyToken,
                Items = new List<ConfigurationAdjustmentItemDto> { itemDto }
            };

            var savedDetail = await SaveDraftAsync(draftDto);

            if (request.SubmitForApproval)
            {
                var applyReq = new ApplyAdjustmentRequestDto
                {
                    UniversalTestGroupID = utg.ID,
                    ConfigurationAdjustmentID = savedDetail.ID,
                    ConcurrencyToken = savedDetail.ConcurrencyToken
                };
                return await ApplyAdjustmentAsync(applyReq);
            }

            return savedDetail;
        }

        public async Task<AdjustedConfigurationDto> GetAdjustedConfigurationAsync(long utgId, bool includeUnapproved = false)
        {
            var utg = await ValidateAccessAndLoadUtgAsync(utgId, BranchAction.View);
            var activeAdj = await _repo.GetActiveByTestGroupIdAsync(utgId);

            // Phase 6 execution consumes AdjustedConfigurationJson ONLY when Status == "Approved"
            if (activeAdj != null && activeAdj.Status != "Approved" && !includeUnapproved)
            {
                throw new InvalidOperationException($"Configuration adjustment Rev {activeAdj.AdjustmentNumber} is currently '{activeAdj.Status}' and is not approved. Phase 6 execution requires an Approved configuration.");
            }

            if (activeAdj == null)
            {
                // Fallback to Phase 4 Effective Configuration as baseline
                var eff = await _utgService.GetEffectiveConfigurationAsync(utgId);
                return new AdjustedConfigurationDto
                {
                    UniversalTestGroupID = utgId,
                    AdjustmentID = 0,
                    AdjustmentNumber = 0,
                    AdjustmentStatus = "No Adjustment",
                    TestMethodSpecificationID = utg.TestMethodSpecificationID,
                    TestMethodCode = utg.TestMethodSpecification?.Code,
                    TestMethodName = utg.TestMethodSpecification?.Name,
                    SpecificationHeaderID = utg.SpecificationHeaderID,
                    SpecificationCode = utg.SpecificationHeader?.Code,
                    SpecificationName = utg.SpecificationHeader?.Title ?? utg.SpecificationHeader?.AliasName,
                    SpecificationGradeID = utg.SpecificationGradeID,
                    SpecificationGradeName = utg.SpecificationGrade?.Grade,
                    Parameters = eff.Parameters,
                    Requirements = eff.Requirements,
                    Conditions = eff.Conditions,
                    Equipment = eff.Equipment,
                    Factors = eff.Factors,
                    Uncertainty = eff.Uncertainty,
                    AcceptanceCriteria = eff.AcceptanceCriteria,
                    Layout = eff.Layout,
                    Validation = eff.Validation,
                    GeneratedAtUtc = DateTime.UtcNow
                };
            }

            if (!string.IsNullOrWhiteSpace(activeAdj.AdjustedConfigurationJson))
            {
                try
                {
                    var cached = JsonSerializer.Deserialize<AdjustedConfigurationDto>(
                        activeAdj.AdjustedConfigurationJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (cached != null) return cached;
                }
                catch { }
            }

            var effConfig = await _utgService.GetEffectiveConfigurationAsync(utgId);
            return await BuildAdjustedConfigurationInternalAsync(effConfig, activeAdj, activeAdj.Items);
        }

        /// <summary>
        /// Single Authoritative BuildAdjustedConfiguration Engine.
        /// Consumes: Phase 4 Effective Configuration + ConfigurationAdjustment + Applied Items.
        /// Parameters, formulas, limits, conditions, factors, uncertainty, and acceptance criteria are authoritative and Read-Only.
        /// Strictly applies Equipment, ExecutionLayout, TestMethod, and Specification deviations.
        /// </summary>
        private async Task<AdjustedConfigurationDto> BuildAdjustedConfigurationInternalAsync(
            EffectiveConfigurationDto eff,
            ConfigurationAdjustment adj,
            IEnumerable<ConfigurationAdjustmentItem> items)
        {
            var itemList = items.ToList();

            // 1. Clone Equipment and Layout for adjustment
            var adjEquipment = eff.Equipment.Select(e => new EffectiveEquipmentRowDto
            {
                EquipmentID = e.EquipmentID,
                Name = e.Name,
                Model = e.Model,
                EquipmentType = e.EquipmentType,
                EquipmentTypeID = e.EquipmentTypeID,
                EquipmentTypeName = e.EquipmentTypeName,
                IsRequired = e.IsRequired,
                IsMandatory = e.IsMandatory,
                EquipmentRequirementID = e.EquipmentRequirementID,
                RequirementCode = e.RequirementCode,
                RequirementName = e.RequirementName,
                MatchingEquipmentCount = e.MatchingEquipmentCount,
                AvailableEquipmentCount = e.AvailableEquipmentCount,
                ReadinessStatus = e.ReadinessStatus,
                BlockingReason = e.BlockingReason,
                Status = e.Status,
                CalibrationStatus = e.CalibrationStatus,
                CalibratedOn = e.CalibratedOn,
                ValidUpto = e.ValidUpto,
                CalibrationNo = e.CalibrationNo
            }).ToList();

            var adjLayout = new EffectiveLayoutDto
            {
                PlannedExecutionLayoutID = eff.Layout.PlannedExecutionLayoutID,
                PlannedLayoutCode = eff.Layout.PlannedLayoutCode,
                PlannedLayoutName = eff.Layout.PlannedLayoutName,
                PlannedRendererType = eff.Layout.PlannedRendererType,
                EffectiveExecutionLayoutID = eff.Layout.EffectiveExecutionLayoutID,
                EffectiveLayoutCode = eff.Layout.EffectiveLayoutCode,
                EffectiveLayoutName = eff.Layout.EffectiveLayoutName,
                EffectiveRendererType = eff.Layout.EffectiveRendererType,
                LayoutResolutionLevel = eff.Layout.LayoutResolutionLevel,
                ResolutionStatus = eff.Layout.ResolutionStatus,
                Reason = eff.Layout.Reason
            };

            // 2. Planning baseline headers
            PlannedConfigurationSnapshotDto? planned = null;
            if (!string.IsNullOrWhiteSpace(adj.UniversalTestGroup?.PlannedConfigurationJson))
            {
                try
                {
                    planned = JsonSerializer.Deserialize<PlannedConfigurationSnapshotDto>(
                        adj.UniversalTestGroup.PlannedConfigurationJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch { }
            }

            long? tmId = planned?.TestMethodSpecificationID ?? adj.UniversalTestGroup?.TestMethodSpecificationID;
            string? tmCode = planned?.TestMethodCode ?? adj.UniversalTestGroup?.TestMethodSpecification?.Code;
            string? tmName = planned?.TestMethodName ?? adj.UniversalTestGroup?.TestMethodSpecification?.Name;
            long? tmVerId = planned?.TestMethodSpecificationVersionID ?? adj.UniversalTestGroup?.TestMethodSpecificationVersionID;
            string? tmVerNum = planned?.TestMethodVersion ?? adj.UniversalTestGroup?.TestMethodSpecificationVersion?.Version;

            long? specId = planned?.SpecificationHeaderID ?? adj.UniversalTestGroup?.SpecificationHeaderID;
            string? specCode = planned?.SpecificationTitle ?? adj.UniversalTestGroup?.SpecificationHeader?.Code;
            string? specName = planned?.SpecificationTitle ?? adj.UniversalTestGroup?.SpecificationHeader?.Title ?? adj.UniversalTestGroup?.SpecificationHeader?.AliasName;
            long? specVerId = planned?.SpecificationVersionID ?? adj.UniversalTestGroup?.SpecificationVersionID;
            string? specVerNum = planned?.SpecificationVersionNumber ?? adj.UniversalTestGroup?.SpecificationVersion?.Version;
            long? specGradeId = planned?.SpecificationGradeID ?? adj.UniversalTestGroup?.SpecificationGradeID;
            string? specGradeName = planned?.GradeName ?? adj.UniversalTestGroup?.SpecificationGrade?.Grade;

            int addedCount = 0, removedCount = 0, changedCount = 0, replacedCount = 0, overriddenCount = 0;

            // 3. Apply Controlled Deviations
            foreach (var item in itemList)
            {
                switch (item.ChangeType)
                {
                    case "ADD": addedCount++; break;
                    case "REMOVE": removedCount++; break;
                    case "CHANGE": changedCount++; break;
                    case "REPLACE": replacedCount++; break;
                    case "OVERRIDE": overriddenCount++; break;
                }

                var normSec = NormalizeSection(item.Section);

                switch (normSec)
                {
                    case "Equipment":
                        if (long.TryParse(item.NewAdjustedValue, out var newEqId))
                        {
                            var newEq = await _context.EquipmentMasters
                                .Include(e => e.Calibrations)
                                .Include(e => e.EquipmentType)
                                .FirstOrDefaultAsync(e => e.ID == newEqId);

                            if (newEq != null)
                            {
                                var eqTarget = adjEquipment.FirstOrDefault(e => e.EquipmentRequirementID == item.EntityID || e.EquipmentID == item.EntityID)
                                    ?? adjEquipment.FirstOrDefault();

                                if (eqTarget != null)
                                {
                                    eqTarget.EquipmentID = newEq.ID;
                                    eqTarget.Name = newEq.Name;
                                    eqTarget.Model = newEq.ModelNo;
                                    var latestCal = newEq.Calibrations?.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                                    eqTarget.CalibratedOn = latestCal?.CalibrationDate;
                                    eqTarget.ValidUpto = latestCal?.CalibrationDueDate ?? newEq.NextCalibrationDueDate;
                                    eqTarget.CalibrationNo = latestCal?.Certificate ?? newEq.EquipmentNo;
                                    eqTarget.CalibrationStatus = (!newEq.CalibrationRequired) ? "Not Required" :
                                        (eqTarget.ValidUpto.HasValue && eqTarget.ValidUpto.Value >= DateTime.UtcNow ? "Valid" : "Expired");
                                    eqTarget.ReadinessStatus = (eqTarget.CalibrationStatus == "Valid" || eqTarget.CalibrationStatus == "Not Required") ? "READY" : "WARNING";
                                    eqTarget.Status = "Adjusted (Replaced)";
                                }
                            }
                        }
                        break;

                    case "ExecutionLayout":
                        if (long.TryParse(item.NewAdjustedValue, out var newLayId))
                        {
                            var newLay = await _context.ExecutionLayoutMasters.FindAsync(newLayId);
                            if (newLay != null)
                            {
                                adjLayout.EffectiveExecutionLayoutID = newLay.ID;
                                adjLayout.EffectiveLayoutCode = newLay.Code;
                                adjLayout.EffectiveLayoutName = newLay.Name;
                                adjLayout.EffectiveRendererType = newLay.RendererType;
                                adjLayout.ResolutionStatus = "ADJUSTED_LAYOUT";
                                adjLayout.Reason = $"Execution deviation: {item.Reason}";
                            }
                        }
                        break;

                    case "TestMethod":
                        if (long.TryParse(item.NewAdjustedValue, out var newTmId))
                        {
                            var newTm = await _context.TestMethodSpecifications
                                .Include(m => m.Versions)
                                .FirstOrDefaultAsync(m => m.ID == newTmId);

                            if (newTm != null)
                            {
                                tmId = newTm.ID;
                                tmCode = newTm.Code ?? newTm.TestMethodStandard;
                                tmName = newTm.Name;
                                var activeVer = newTm.Versions?.FirstOrDefault();
                                if (activeVer != null)
                                {
                                    tmVerId = activeVer.ID;
                                    tmVerNum = activeVer.Version;
                                }
                            }
                        }
                        break;

                    case "Specification":
                        if (long.TryParse(item.NewAdjustedValue, out var newSpId))
                        {
                            var newSp = await _context.SpecificationHeaders
                                .Include(s => s.Grades)
                                .Include(s => s.Versions)
                                .FirstOrDefaultAsync(s => s.ID == newSpId);

                            if (newSp != null)
                            {
                                specId = newSp.ID;
                                specCode = newSp.Code;
                                specName = newSp.Title ?? newSp.AliasName;
                                var activeVer = newSp.Versions?.FirstOrDefault();
                                if (activeVer != null)
                                {
                                    specVerId = activeVer.ID;
                                    specVerNum = activeVer.Version;
                                }
                                var firstGrade = newSp.Grades?.FirstOrDefault();
                                if (firstGrade != null)
                                {
                                    specGradeId = firstGrade.ID;
                                    specGradeName = firstGrade.Grade;
                                }
                            }
                        }
                        break;
                }
            }

            return new AdjustedConfigurationDto
            {
                UniversalTestGroupID = eff.UniversalTestGroupID,
                ConfigurationAdjustmentID = adj.ID,
                AdjustmentNumber = adj.AdjustmentNumber,
                AdjustmentStatus = adj.Status,
                TestMethodSpecificationID = tmId,
                TestMethodCode = tmCode,
                TestMethodName = tmName,
                TestMethodSpecificationVersionID = tmVerId,
                TestMethodVersionNumber = tmVerNum,
                SpecificationHeaderID = specId,
                SpecificationCode = specCode,
                SpecificationName = specName,
                SpecificationVersionID = specVerId,
                SpecificationVersionNumber = specVerNum,
                SpecificationGradeID = specGradeId,
                SpecificationGradeName = specGradeName,

                // Scientific parameters, requirements, conditions, factors, MU, and criteria are mirrored 100% Read-Only
                Parameters = eff.Parameters,
                Requirements = eff.Requirements,
                Conditions = eff.Conditions,
                Equipment = adjEquipment,
                Factors = eff.Factors,
                Uncertainty = eff.Uncertainty,
                AcceptanceCriteria = eff.AcceptanceCriteria,
                Layout = adjLayout,
                Validation = eff.Validation,
                AddedCount = addedCount,
                RemovedCount = removedCount,
                ChangedCount = changedCount,
                ReplacedCount = replacedCount,
                OverriddenCount = overriddenCount,
                GeneratedAtUtc = DateTime.UtcNow
            };
        }

        private async Task<ConfigurationAdjustmentDetailDto> MapDetailDtoAsync(
            ConfigurationAdjustment adj,
            AdjustedConfigurationDto? adjustedConfig = null)
        {
            var empIds = new List<long> { adj.CreatedBy };
            if (adj.AppliedBy.HasValue) empIds.Add(adj.AppliedBy.Value);
            if (adj.ApprovedBy.HasValue) empIds.Add(adj.ApprovedBy.Value);
            if (adj.ModifiedBy.HasValue) empIds.Add(adj.ModifiedBy.Value);

            foreach (var item in adj.Items)
            {
                empIds.Add(item.CreatedBy);
                if (item.AuthorizedBy.HasValue) empIds.Add(item.AuthorizedBy.Value);
                if (item.ModifiedBy.HasValue) empIds.Add(item.ModifiedBy.Value);
            }

            var empMap = await _context.EmployeeMasters
                .Where(e => empIds.Distinct().Contains(e.ID))
                .ToDictionaryAsync(e => e.ID, e => e.Name.Trim());

            if (adjustedConfig == null && !string.IsNullOrWhiteSpace(adj.AdjustedConfigurationJson))
            {
                try
                {
                    adjustedConfig = JsonSerializer.Deserialize<AdjustedConfigurationDto>(
                        adj.AdjustedConfigurationJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch { }
            }

            return new ConfigurationAdjustmentDetailDto
            {
                ID = adj.ID,
                UniversalTestGroupID = adj.UniversalTestGroupID,
                AdjustmentNumber = adj.AdjustmentNumber,
                Status = adj.Status,
                OverallReason = adj.OverallReason,
                CreatedBy = adj.CreatedBy,
                CreatedByName = empMap.TryGetValue(adj.CreatedBy, out var cName) ? cName : null,
                CreatedOn = adj.CreatedOn,
                AppliedBy = adj.AppliedBy,
                AppliedByName = adj.AppliedBy.HasValue && empMap.TryGetValue(adj.AppliedBy.Value, out var apName) ? apName : null,
                AppliedOn = adj.AppliedOn,
                ApprovedBy = adj.Status == "Approved" ? adj.ApprovedBy : null,
                ApprovedByName = adj.Status == "Approved" && adj.ApprovedBy.HasValue && empMap.TryGetValue(adj.ApprovedBy.Value, out var avName) ? avName : null,
                ApprovedOn = adj.Status == "Approved" ? adj.ApprovedOn : null,
                ApprovalRemarks = adj.Status == "Approved" ? adj.ApprovalRemarks : null,
                RejectedBy = adj.Status == "Rejected" ? adj.ModifiedBy : null,
                RejectedByName = adj.Status == "Rejected" && adj.ModifiedBy.HasValue && empMap.TryGetValue(adj.ModifiedBy.Value, out var rjName) ? rjName : null,
                RejectedOn = adj.Status == "Rejected" ? adj.ModifiedOn : null,
                RejectionReason = adj.Status == "Rejected" ? adj.ApprovalRemarks : null,
                BranchID = adj.BranchID,
                BranchName = adj.Branch?.Name,
                ConcurrencyToken = adj.ConcurrencyToken,
                Items = adj.Items.Select(i => new ConfigurationAdjustmentItemDto
                {
                    ID = i.ID,
                    ConfigurationAdjustmentID = i.ConfigurationAdjustmentID,
                    UniversalTestGroupID = i.UniversalTestGroupID,
                    Section = i.Section,
                    EntityType = i.EntityType,
                    EntityID = i.EntityID,
                    EntityCode = i.EntityCode,
                    EntityName = i.EntityName,
                    FieldName = i.FieldName,
                    ChangeType = i.ChangeType,
                    PlannedValue = i.PlannedValue,
                    EffectiveValue = i.EffectiveValue,
                    PreviousAdjustedValue = i.PreviousAdjustedValue,
                    NewAdjustedValue = i.NewAdjustedValue,
                    Reason = i.Reason,
                    AuthorizationStatus = i.AuthorizationStatus,
                    AuthorizedBy = i.AuthorizedBy,
                    AuthorizedByName = i.AuthorizedBy.HasValue && empMap.TryGetValue(i.AuthorizedBy.Value, out var aName) ? aName : null,
                    AuthorizedOn = i.AuthorizedOn,
                    CreatedBy = i.CreatedBy,
                    CreatedByName = empMap.TryGetValue(i.CreatedBy, out var icName) ? icName : null,
                    CreatedOn = i.CreatedOn
                }).ToList(),
                AdjustedConfiguration = adjustedConfig
            };
        }
    }
}
