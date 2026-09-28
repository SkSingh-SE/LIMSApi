using System.Text.Json;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class UniversalTestGroupService : IUniversalTestGroupService
    {
        private readonly LIMSContext _context;
        private readonly IEffectiveConfigurationResolver _resolver;
        private readonly IBranchContext _branchContext;
        private readonly ILaboratoryTestLayoutService _layoutService;

        public UniversalTestGroupService(
            LIMSContext context,
            IEffectiveConfigurationResolver resolver,
            IBranchContext branchContext,
            ILaboratoryTestLayoutService layoutService)
        {
            _context = context;
            _resolver = resolver;
            _branchContext = branchContext;
            _layoutService = layoutService;
        }

        private async Task ValidateTenantForInwardAsync(long inwardId)
        {
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            var inward = await _context.SampleInwards.AsNoTracking().FirstOrDefaultAsync(i => i.ID == inwardId);
            if (inward == null) throw new KeyNotFoundException($"Sample Inward {inwardId} not found.");
            string tenantCompanyCode = loggedInUser?.CompanyCode ?? string.Empty;
            if (!string.IsNullOrEmpty(inward.CompanyCode) && inward.CompanyCode != tenantCompanyCode)
                throw new UnauthorizedAccessException("Access denied. Case does not belong to your organization.");
        }

        private async Task ValidateAccessToTestGroupAsync(long branchId)
        {
            if (!_branchContext.IsAuthorizedForBranch(branchId, BranchAction.View))
                throw new UnauthorizedAccessException($"Access denied to branch {branchId}.");
        }

        public async Task<List<UniversalTestGroupListItemDto>> GetAllForCurrentUserAsync()
        {
            var orgId = _branchContext.CurrentOrganizationID;
            var allowedBranchIds = _branchContext.AuthorizedBranchIDs;
            var query = _context.UniversalTestGroups
                .Include(u => u.LaboratoryTest).ThenInclude(t => t.Discipline)
                .Include(u => u.Branch)
                .Include(u => u.SampleTestPlan).ThenInclude(p => p.SampleDetail).ThenInclude(s => s.SampleInward)
                .Include(u => u.TestExecutions)
                .Where(u => u.IsActive);

            if (orgId.HasValue)
                query = query.Where(u => u.OrganizationID == orgId.Value);

            if (!_branchContext.CanViewAllBranches && allowedBranchIds.Any())
                query = query.Where(u => allowedBranchIds.Contains(u.BranchID));

            var groups = await query.OrderByDescending(u => u.CreatedOn).Take(100).ToListAsync();
            var deptMap = await BuildDeptMapAsync(groups);
            return groups.Select(u => ToListDto(u, deptMap)).ToList();
        }

        public async Task<List<UniversalTestGroupListItemDto>> GetTestGroupsForInwardAsync(long inwardId)
        {
            await ValidateTenantForInwardAsync(inwardId);
            var groups = await _context.UniversalTestGroups
                .Include(u => u.LaboratoryTest).ThenInclude(t => t.Discipline)
                .Include(u => u.Branch)
                .Include(u => u.SampleTestPlan).ThenInclude(p => p.SampleDetail)
                .Include(u => u.TestExecutions)
                .Where(u => u.IsActive && u.SampleTestPlan.SampleDetail.InwardID == inwardId)
                .OrderBy(u => u.ID)
                .ToListAsync();

            var deptMap = await BuildDeptMapAsync(groups);

            return groups.Select(u => ToListDto(u, deptMap)).ToList();
        }

        public async Task<List<UniversalTestGroupListItemDto>> GetTestGroupsForSampleAsync(long sampleId)
        {
            var sample = await _context.SampleDetails.AsNoTracking()
                .Include(s => s.SampleInward)
                .FirstOrDefaultAsync(s => s.ID == sampleId);
            if (sample == null) throw new KeyNotFoundException($"Sample {sampleId} not found.");
            await ValidateTenantForInwardAsync(sample.InwardID);

            var groups = await _context.UniversalTestGroups
                .Include(u => u.LaboratoryTest).ThenInclude(t => t.Discipline)
                .Include(u => u.Branch)
                .Include(u => u.SampleTestPlan)
                .Include(u => u.TestExecutions)
                .Where(u => u.IsActive && u.SampleTestPlan.SampleID == sampleId)
                .OrderBy(u => u.ID)
                .ToListAsync();

            var deptMap = await BuildDeptMapAsync(groups);
            return groups.Select(u => ToListDto(u, deptMap)).ToList();
        }

        public async Task<List<UniversalTestGroupListItemDto>> GetTestGroupsForPlanAsync(long sampleTestPlanId)
        {
            var plan = await _context.TestPlans.AsNoTracking()
                .Include(p => p.SampleDetail).ThenInclude(s => s.SampleInward)
                .FirstOrDefaultAsync(p => p.ID == sampleTestPlanId);
            if (plan == null) throw new KeyNotFoundException($"Sample Test Plan {sampleTestPlanId} not found.");
            await ValidateTenantForInwardAsync(plan.SampleDetail.InwardID);

            var groups = await _context.UniversalTestGroups
                .Include(u => u.LaboratoryTest).ThenInclude(t => t.Discipline)
                .Include(u => u.Branch)
                .Include(u => u.SampleTestPlan)
                .Include(u => u.TestExecutions)
                .Where(u => u.IsActive && u.SampleTestPlanID == sampleTestPlanId)
                .OrderBy(u => u.ID)
                .ToListAsync();

            var deptMap = await BuildDeptMapAsync(groups);
            return groups.Select(u => ToListDto(u, deptMap)).ToList();
        }

        public async Task<UniversalTestGroupDetailDto> GetTestGroupDetailAsync(long testGroupId)
        {
            var utg = await LoadUtgAsync(testGroupId);
            await ValidateAccessToTestGroupAsync(utg.BranchID);

            var detail = await MapDetailAsync(utg);
            return detail;
        }

        public async Task<EffectiveConfigurationDto> GetEffectiveConfigurationAsync(long testGroupId)
        {
            var utg = await LoadUtgAsync(testGroupId);

            // Strict Tenant Isolation
            var loggedInUser = LoggedInUserProvider.CurrentUser;
            string tenantCompanyCode = loggedInUser?.CompanyCode ?? string.Empty;
            if (!string.IsNullOrEmpty(utg.CompanyCode) && !string.IsNullOrEmpty(tenantCompanyCode) && utg.CompanyCode != tenantCompanyCode)
                throw new UnauthorizedAccessException("Access denied. Test group does not belong to your organization.");

            await ValidateAccessToTestGroupAsync(utg.BranchID);

            var detail = await MapDetailAsync(utg);

            // Deserialize Planned Baseline from UTG.PlannedConfigurationJson
            PlannedConfigurationSnapshotDto? plannedBaseline = null;
            if (!string.IsNullOrWhiteSpace(utg.PlannedConfigurationJson))
            {
                try
                {
                    plannedBaseline = JsonSerializer.Deserialize<PlannedConfigurationSnapshotDto>(
                        utg.PlannedConfigurationJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch { }
            }

            // Build resolver request using PINNED IDs from UTG — never re-resolve from latest/default
            var sampleId = utg.SampleTestPlan.SampleID;
            var request = new UniversalPlanPreviewRequestDto
            {
                SampleID = sampleId,
                LaboratoryTestID = utg.LaboratoryTestID,
                SpecificationHeaderID = utg.SpecificationHeaderID,
                SpecificationVersionID = utg.SpecificationVersionID,
                SpecificationGradeID = utg.SpecificationGradeID,
                TestMethodSpecificationID = utg.TestMethodSpecificationID,
                TestMethodSpecificationVersionID = utg.TestMethodSpecificationVersionID,
                BranchID = utg.BranchID,
                ReferenceDate = utg.CreatedOn != default ? utg.CreatedOn : DateTime.UtcNow
            };

            var preview = await _resolver.ResolveEffectiveConfigurationAsync(request);
            var snapshot = await _resolver.ResolveSnapshotDtoAsync(utg);

            // Preload ParameterMasters for precision, symbols, and formulas
            var liveParamsByPid = preview.Parameters.ToDictionary(p => p.ParameterID, p => p);
            var allPids = (plannedBaseline?.Parameters?.Select(p => p.ParameterID) ?? preview.Parameters.Select(p => p.ParameterID)).Distinct().ToList();
            var pmMasters = await _context.ParameterMasters
                .Include(pm => pm.ParameterUnit)
                .Where(pm => allPids.Contains(pm.ID))
                .ToDictionaryAsync(pm => pm.ID);

            // Track parameter membership drift
            var plannedPids = plannedBaseline?.Parameters?.Select(p => p.ParameterID).ToHashSet() ?? new HashSet<long>();
            var livePids = preview.Parameters.Select(p => p.ParameterID).ToHashSet();
            var addedParamCodes = new List<string>();
            var removedParamCodes = new List<string>();

            if (plannedPids.Any())
            {
                foreach (var lp in preview.Parameters)
                {
                    if (!plannedPids.Contains(lp.ParameterID))
                        addedParamCodes.Add(lp.ParameterCode);
                }
                foreach (var pp in plannedBaseline!.Parameters)
                {
                    if (!livePids.Contains(pp.ParameterID))
                        removedParamCodes.Add(pp.ParameterCode);
                }
            }

            // Lock 1 & Lock 2: Planned parameter scope is executable scope; planned values remain Effective
            var paramRows = new List<EffectiveParameterRowDto>();
            var sourceParams = (plannedBaseline != null && plannedBaseline.Parameters.Any())
                ? plannedBaseline.Parameters
                : preview.Parameters;

            foreach (var p in sourceParams)
            {
                liveParamsByPid.TryGetValue(p.ParameterID, out var liveP);
                pmMasters.TryGetValue(p.ParameterID, out var pm);

                // Effective Values: PLANNED VALUES REMAIN EFFECTIVE CONTRACT
                string effUnit = p.ParameterUnit ?? "";
                string effFormula = p.Equation ?? "";
                decimal? effMin = p.MinValue;
                decimal? effMax = p.MaxValue;
                decimal? effMinTol = p.MinTolerance;
                decimal? effMaxTol = p.MaxTolerance;
                string? effAcceptance = p.AcceptanceCriteria;
                int? effPrecision = (pm?.InputType == "Decimal") ? pm.DecimalPrecision : null;

                // Live Master Comparison Values
                string? masterUnit = liveP?.ParameterUnit ?? pm?.ParameterUnit?.Symbol ?? pm?.ParameterUnit?.Name;
                string? masterFormula = liveP?.Equation ?? pm?.Formula;
                decimal? masterMin = liveP?.MinValue;
                decimal? masterMax = liveP?.MaxValue;
                int? masterPrecision = (pm?.InputType == "Decimal") ? pm.DecimalPrecision : null;

                string driftStatus = "UNCHANGED";
                string? driftCategory = null;

                if (liveP == null)
                {
                    driftStatus = "MISSING";
                    driftCategory = "Scientific";
                }
                else
                {
                    bool unitDiff = !string.IsNullOrWhiteSpace(effUnit) && !string.IsNullOrWhiteSpace(masterUnit) && !string.Equals(effUnit, masterUnit, StringComparison.OrdinalIgnoreCase);
                    bool formulaDiff = !string.IsNullOrWhiteSpace(effFormula) && !string.IsNullOrWhiteSpace(masterFormula) && !string.Equals(effFormula, masterFormula, StringComparison.Ordinal);
                    bool limitsDiff = (effMin != masterMin) || (effMax != masterMax);
                    bool precisionDiff = (effPrecision != masterPrecision);

                    if (unitDiff || formulaDiff)
                    {
                        driftStatus = "CHANGED";
                        driftCategory = "Scientific";
                    }
                    else if (limitsDiff)
                    {
                        driftStatus = "CHANGED";
                        driftCategory = "Compliance";
                    }
                    else if (precisionDiff)
                    {
                        driftStatus = "CHANGED";
                        driftCategory = "Operational";
                    }
                }

                string unitSource = p.SpecificationLineID.HasValue && !string.IsNullOrWhiteSpace(effUnit)
                    ? "Specification Requirement (Planned)"
                    : "Parameter Master (Planned)";
                if (preview.IsStandardlessTest) unitSource = "Parameter Master (Planned)";

                paramRows.Add(new EffectiveParameterRowDto
                {
                    ParameterID = p.ParameterID,
                    ParameterCode = p.ParameterCode,
                    ParameterName = p.ParameterName,
                    Symbol = pm?.Symbol,
                    Unit = effUnit,
                    UnitSource = unitSource,
                    InputType = p.InputType ?? pm?.InputType ?? "Decimal",
                    DecimalPrecision = effPrecision,
                    CalculationRole = pm?.CalculationRole,
                    IsCalculated = !string.IsNullOrWhiteSpace(effFormula) || pm?.IsCalculated == true,
                    Formula = effFormula,
                    IsMandatory = p.IsMandatory,
                    IsReportable = p.IsReportable,
                    RequirementText = p.RequirementText,
                    MinValue = effMin,
                    MaxValue = effMax,
                    MinTolerance = effMinTol,
                    MaxTolerance = effMaxTol,
                    AcceptanceCriteria = effAcceptance,
                    ResolutionStatus = p.ResolutionStatus.ToString(),
                    ResolutionReason = p.ResolutionReason,
                    RequirementSource = p.HasRequirement ? $"Specification Version {preview.SpecificationVersionNumber}" : (preview.IsStandardlessTest ? "N/A — Standardless" : "Not Configured"),
                    SpecificationLineID = p.SpecificationLineID,
                    Status = p.IsMandatory ? "Required" : "Optional",
                    MasterUnit = masterUnit,
                    MasterFormula = masterFormula,
                    MasterMinValue = masterMin,
                    MasterMaxValue = masterMax,
                    MasterDecimalPrecision = masterPrecision,
                    MasterDriftStatus = driftStatus,
                    DriftCategory = driftCategory
                });
            }

            // Requirements (detail view)
            var reqRows = paramRows.Select(p => new EffectiveRequirementRowDto
            {
                ParameterID = p.ParameterID,
                ParameterCode = p.ParameterCode,
                ParameterName = p.ParameterName,
                RequirementType = p.SpecificationLineID.HasValue ? "Specification Limit" : (preview.IsStandardlessTest ? "N/A" : "None"),
                Min = p.MinValue,
                Max = p.MaxValue,
                MinTolerance = p.MinTolerance,
                MaxTolerance = p.MaxTolerance,
                AcceptanceCriteria = p.AcceptanceCriteria,
                Equation = p.Formula,
                ConditionContext = p.RequirementText,
                Source = p.RequirementSource,
                SpecificationLineID = p.SpecificationLineID,
                ResolutionStatus = p.ResolutionStatus,
                Status = p.Status
            }).ToList();

            // Conditions
            var condRows = preview.Conditions.Select(c => new EffectiveConditionRowDto
            {
                ConditionMasterID = c.ConditionMasterID,
                ConditionCode = c.ConditionCode,
                ConditionName = c.ConditionName,
                Category = c.Category,
                Unit = c.ParameterUnit,
                Operator = "=",
                ConfiguredValue = c.ConfiguredValue,
                RequirementContext = c.HasConfiguration ? "Configured" : "Not Configured",
                IsMandatory = c.IsMandatory,
                HasConfiguration = c.HasConfiguration,
                Status = c.HasConfiguration ? "CONFIGURED" : (c.IsMandatory ? "MISSING" : "NOT_CONFIGURED")
            }).ToList();

            // Method
            var methodDto = new EffectiveMethodDto
            {
                TestMethodSpecificationID = preview.TestMethodSpecificationID,
                TestMethodCode = preview.TestMethodCode,
                TestMethodName = preview.TestMethodName,
                StandardReference = preview.TestMethodStandard,
                TestMethodSpecificationVersionID = preview.TestMethodSpecificationVersionID,
                Version = preview.TestMethodVersion,
                Status = preview.AvailableMethodVersions.FirstOrDefault(v => v.ID == preview.TestMethodSpecificationVersionID)?.Status ?? "Active",
                EffectiveDate = preview.AvailableMethodVersions.FirstOrDefault(v => v.ID == preview.TestMethodSpecificationVersionID)?.EffectiveDate,
                SupersededDate = preview.AvailableMethodVersions.FirstOrDefault(v => v.ID == preview.TestMethodSpecificationVersionID)?.SupersededDate,
                IsSuperseded = preview.IsSupersededMethodVersion,
                IsDefault = preview.AvailableMethodVersions.FirstOrDefault(v => v.ID == preview.TestMethodSpecificationVersionID)?.IsDefault ?? false,
                Source = "Universal Test Group"
            };

            // Lock 3: Layout Resolution & Operational Drift
            var liveLayout = await _layoutService.ResolveEffectiveLayoutAsync(
                utg.LaboratoryTestID,
                utg.TestMethodSpecificationID,
                utg.TestMethodSpecificationVersionID);

            long? plannedLayoutId = plannedBaseline?.ExecutionLayoutID ?? utg.ExecutionLayoutID;
            string? plannedLayoutCode = plannedBaseline?.ExecutionLayoutCode ?? utg.ExecutionLayout?.Code;
            string? plannedLayoutName = plannedBaseline?.ExecutionLayoutName ?? utg.ExecutionLayout?.Name;
            string? plannedRenderer = plannedBaseline?.RendererType ?? utg.ExecutionLayout?.RendererType;

            long? effLayoutId = liveLayout?.ExecutionLayoutID;
            string? effLayoutCode = liveLayout?.LayoutCode;
            string? effLayoutName = liveLayout?.LayoutName;
            string? effRenderer = liveLayout?.RendererType;
            string layoutLevel = liveLayout?.ResolutionLevel ?? "Unassigned";

            string layoutResolutionStatus = "UNCHANGED";
            string? layoutReason = null;

            if (!effLayoutId.HasValue || layoutLevel == "Unassigned")
            {
                layoutResolutionStatus = "UNASSIGNED";
                layoutReason = "No layout assigned for this Laboratory Test / Method combination.";
            }
            else if (plannedLayoutId.HasValue && plannedLayoutId.Value != effLayoutId.Value)
            {
                layoutResolutionStatus = "LAYOUT_DRIFT";
                layoutReason = $"Assigned layout changed from '{plannedLayoutCode}' to '{effLayoutCode}' at resolution level '{layoutLevel}'.";
            }
            else
            {
                layoutResolutionStatus = "UNCHANGED";
                layoutReason = $"Resolved layout matches planned baseline at level '{layoutLevel}'.";
            }

            var layoutDto = new EffectiveLayoutDto
            {
                PlannedExecutionLayoutID = plannedLayoutId,
                PlannedLayoutCode = plannedLayoutCode,
                PlannedLayoutName = plannedLayoutName,
                PlannedRendererType = plannedRenderer,
                EffectiveExecutionLayoutID = effLayoutId,
                EffectiveLayoutCode = effLayoutCode,
                EffectiveLayoutName = effLayoutName,
                EffectiveRendererType = effRenderer,
                LayoutResolutionLevel = layoutLevel,
                ResolutionStatus = layoutResolutionStatus,
                Reason = layoutReason,
                ExecutionLayout = preview.ExecutionLayout,
                UnmappedParameterIDs = preview.UnmappedParameters.Select(p => p.ParameterID).ToList()
            };

            // Equipment (Phase 1D)
            var activeRequirements = await _context.EquipmentRequirementMasters
                .Include(r => r.EquipmentType)
                .AsNoTracking()
                .Where(r => r.IsActive
                    && (string.IsNullOrEmpty(utg.CompanyCode) || r.CompanyCode == utg.CompanyCode)
                    && r.LaboratoryTestID == utg.LaboratoryTestID
                    && (r.TestMethodSpecificationID == null || r.TestMethodSpecificationID == utg.TestMethodSpecificationID))
                .OrderBy(r => r.DisplayOrder).ThenBy(r => r.Code)
                .ToListAsync();

            var branchEquipments = await _context.EquipmentMasters
                .Include(e => e.Calibrations)
                .Include(e => e.EquipmentType)
                .AsNoTracking()
                .Where(e => e.IsActive && e.BranchID == utg.BranchID && (string.IsNullOrEmpty(utg.CompanyCode) || e.CompanyCode == utg.CompanyCode))
                .ToListAsync();

            var equipRows = new List<EffectiveEquipmentRowDto>();
            var now = DateTime.UtcNow;

            foreach (var req in activeRequirements)
            {
                var matchingList = branchEquipments.Where(e => e.EquipmentTypeID == req.EquipmentTypeID).ToList();
                if (req.EquipmentID.HasValue) matchingList = matchingList.Where(e => e.ID == req.EquipmentID.Value).ToList();

                int matchingCount = matchingList.Count;
                var validList = matchingList.Where(e =>
                {
                    if (!e.CalibrationRequired) return true;
                    var latestCal = e.Calibrations?.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                    var due = latestCal?.CalibrationDueDate ?? e.NextCalibrationDueDate;
                    return due.HasValue && due.Value >= now;
                }).ToList();

                int availableCount = validList.Count;
                var firstValid = validList.FirstOrDefault();
                var firstAny = matchingList.FirstOrDefault();
                var selectedEq = firstValid ?? firstAny;

                string calStatus = "Not Required";
                DateTime? calDue = null;
                DateTime? calOn = null;
                string? calNo = null;

                if (selectedEq != null)
                {
                    var latestCal = selectedEq.Calibrations?.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                    calDue = latestCal?.CalibrationDueDate ?? selectedEq.NextCalibrationDueDate;
                    calOn = latestCal?.CalibrationDate;
                    calNo = latestCal?.Certificate ?? selectedEq.EquipmentNo;

                    if (!selectedEq.CalibrationRequired) calStatus = "Not Required";
                    else if (!calDue.HasValue) calStatus = "Not Configured";
                    else if (calDue.Value < now) calStatus = "Expired";
                    else calStatus = "Valid";
                }

                string readinessStatus = "READY";
                string? blockReason = null;

                if (matchingCount == 0)
                {
                    readinessStatus = req.IsMandatory ? "BLOCKED" : "WARNING";
                    blockReason = $"No active equipment of type '{req.EquipmentType?.Name ?? req.Code}' found in Branch '{utg.Branch?.Name}'.";
                }
                else if (availableCount == 0)
                {
                    readinessStatus = req.IsMandatory ? "BLOCKED" : "WARNING";
                    blockReason = "Equipment available in branch but calibration is expired or not configured.";
                }

                equipRows.Add(new EffectiveEquipmentRowDto
                {
                    EquipmentID = selectedEq?.ID,
                    Name = selectedEq?.Name ?? req.Name,
                    Model = selectedEq?.ModelNo,
                    EquipmentType = req.EquipmentType?.Name,
                    EquipmentTypeID = req.EquipmentTypeID,
                    EquipmentTypeName = req.EquipmentType?.Name,
                    IsRequired = req.IsMandatory,
                    IsMandatory = req.IsMandatory,
                    EquipmentRequirementID = req.ID,
                    RequirementCode = req.Code,
                    RequirementName = req.Name,
                    MatchingEquipmentCount = matchingCount,
                    AvailableEquipmentCount = availableCount,
                    ReadinessStatus = readinessStatus,
                    BlockingReason = blockReason,
                    Status = req.IsMandatory ? "Mandatory" : "Optional",
                    CalibrationStatus = calStatus,
                    CalibratedOn = calOn,
                    ValidUpto = calDue,
                    CalibrationNo = calNo
                });
            }

            // Factors (Phase 1E)
            var paramIds = paramRows.Select(p => p.ParameterID).ToList();
            var activeFactors = await _context.FactorConversionMasters
                .Include(f => f.InputParameter)
                .Include(f => f.OutputParameter)
                .AsNoTracking()
                .Where(f => f.IsActive
                    && f.CompanyCode == utg.CompanyCode
                    && paramIds.Contains(f.InputParameterID)
                    && (f.LaboratoryTestID == null || f.LaboratoryTestID == utg.LaboratoryTestID)
                    && (f.TestMethodSpecificationID == null || f.TestMethodSpecificationID == utg.TestMethodSpecificationID)
                    && (f.TestMethodSpecificationVersionID == null || f.TestMethodSpecificationVersionID == utg.TestMethodSpecificationVersionID))
                .OrderBy(f => f.DisplayOrder).ThenBy(f => f.Code)
                .ToListAsync();

            var winningFactors = activeFactors
                .GroupBy(f => f.InputParameterID)
                .Select(g => g
                    .OrderByDescending(f =>
                        f.TestMethodSpecificationVersionID != null ? 3 :
                        f.TestMethodSpecificationID != null ? 2 :
                        f.LaboratoryTestID != null ? 1 : 0)
                    .ThenBy(f => f.DisplayOrder)
                    .ThenBy(f => f.Code)
                    .First())
                .OrderBy(f => f.DisplayOrder).ThenBy(f => f.Code)
                .ToList();

            var factorRows = winningFactors.Select(fc => new EffectiveFactorRowDto
            {
                FactorConversionID = fc.ID,
                Code = fc.Code,
                Name = fc.Name,
                Description = fc.Description,
                FactorType = fc.FactorType switch
                {
                    "DIVISION" => "Division Factor",
                    "ADDITIVE_OFFSET" => "Additive Offset",
                    "SUBTRACTIVE_OFFSET" => "Subtractive Offset",
                    _ => "Multiplication Factor"
                },
                FactorValue = fc.FactorValue,
                InputParameterID = fc.InputParameterID,
                InputParameterCode = fc.InputParameter?.Code,
                InputParameterName = fc.InputParameter?.Name,
                OutputParameterID = fc.OutputParameterID,
                OutputParameterCode = fc.OutputParameter?.Code,
                OutputParameterName = fc.OutputParameter?.Name,
                AppliedOn = fc.AppliedOn,
                IsMandatory = fc.IsMandatory,
                ScopeLevel = fc.TestMethodSpecificationVersionID != null ? "Version" :
                             fc.TestMethodSpecificationID != null ? "Method" :
                             fc.LaboratoryTestID != null ? "Test" : "Global",
                Status = "CONFIGURED"
            }).ToList();

            // Measurement Uncertainty (Phase 1F Only)
            EffectiveUncertaintyDto? uncDto = null;
            var muCandidates = await _context.MeasurementUncertaintyMasters
                .Include(x => x.ParameterUnit)
                .AsNoTracking()
                .Where(x => x.IsActive
                    && x.CompanyCode == utg.CompanyCode
                    && x.ParameterID == null
                    && (x.LaboratoryTestID == null || x.LaboratoryTestID == utg.LaboratoryTestID)
                    && (x.TestMethodSpecificationID == null || x.TestMethodSpecificationID == utg.TestMethodSpecificationID)
                    && (x.TestMethodSpecificationVersionID == null || x.TestMethodSpecificationVersionID == utg.TestMethodSpecificationVersionID))
                .OrderByDescending(x => (x.LaboratoryTestID != null ? 1 : 0)
                    + (x.TestMethodSpecificationID != null ? 1 : 0)
                    + (x.TestMethodSpecificationVersionID != null ? 1 : 0))
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.Code)
                .ToListAsync();

            var bestMu = muCandidates.FirstOrDefault();
            if (bestMu != null)
            {
                uncDto = new EffectiveUncertaintyDto
                {
                    IsConfigured = true,
                    MeasurementUncertaintyMasterID = bestMu.ID,
                    MasterCode = bestMu.Code,
                    MasterName = bestMu.Name,
                    UncertaintyType = bestMu.UncertaintyType,
                    CombinedUncertainty = bestMu.CombinedUncertainty,
                    ExpandedUncertainty = bestMu.ExpandedUncertainty,
                    Value = bestMu.ExpandedUncertainty ?? bestMu.CombinedUncertainty,
                    Unit = bestMu.ParameterUnit?.Symbol ?? bestMu.ParameterUnit?.Name,
                    CoverageFactor = bestMu.CoverageFactor,
                    ConfidenceLevel = bestMu.ConfidenceLevel,
                    Basis = bestMu.Basis,
                    ComponentsJson = bestMu.ComponentsJson,
                    Remarks = bestMu.Description ?? "As per ISO 17025",
                    Status = "CONFIGURED"
                };
            }
            else
            {
                uncDto = new EffectiveUncertaintyDto
                {
                    IsConfigured = false,
                    Status = "NOT_CONFIGURED"
                };
            }

            // Lock 4: Acceptance Criteria Resolution & Source
            var activeCriteria = await _context.AcceptanceCriteriaMasters
                .AsNoTracking()
                .Where(a => a.IsActive && (string.IsNullOrEmpty(utg.CompanyCode) || a.CompanyCode == utg.CompanyCode))
                .ToListAsync();

            var decisionRule = utg.SampleTestPlan?.SampleDetail?.SampleInward?.DecisionRule;
            EffectiveAcceptanceCriteriaDto? acDto = null;
            AcceptanceCriteriaMaster? matchedAc = null;
            string resSource = "Not Configured";

            if (!string.IsNullOrWhiteSpace(decisionRule))
            {
                matchedAc = activeCriteria.FirstOrDefault(a =>
                    string.Equals(a.Name, decisionRule, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(a.Code, decisionRule, StringComparison.OrdinalIgnoreCase));
                if (matchedAc != null)
                {
                    resSource = $"Sample Inward Decision Rule ('{decisionRule}')";
                }
                else if (string.Equals(decisionRule, "Not Applicable", StringComparison.OrdinalIgnoreCase))
                {
                    resSource = "Sample Inward Decision Rule ('Not Applicable')";
                }
            }

            if (matchedAc == null && !string.Equals(decisionRule, "Not Applicable", StringComparison.OrdinalIgnoreCase))
            {
                matchedAc = activeCriteria.FirstOrDefault(a => a.Code == "DEFAULT" || a.Code == "ALL_REQUIRED_PARAMETERS_PASS");
                if (matchedAc != null)
                {
                    resSource = $"Enterprise Default ('{matchedAc.Code}')";
                }
            }

            if (matchedAc != null)
            {
                acDto = new EffectiveAcceptanceCriteriaDto
                {
                    IsConfigured = true,
                    AcceptanceCriteriaID = matchedAc.ID,
                    Code = matchedAc.Code,
                    Name = matchedAc.Name,
                    Description = matchedAc.Description,
                    EvaluationType = matchedAc.EvaluationType,
                    ComparisonType = matchedAc.ComparisonType,
                    DecisionRule = matchedAc.DecisionRule,
                    RoundingRule = matchedAc.RoundingRule,
                    RoundingPrecision = 0.01m,
                    ResolutionSource = resSource,
                    Status = "RESOLVED"
                };
            }
            else
            {
                acDto = new EffectiveAcceptanceCriteriaDto
                {
                    IsConfigured = false,
                    ResolutionSource = resSource,
                    Status = string.Equals(decisionRule, "Not Applicable", StringComparison.OrdinalIgnoreCase) ? "N/A" : "NOT_CONFIGURED"
                };
            }

            // Lock 5: Single Service 17-Gate Validation Decision
            var validation = Evaluate17GateValidation(
                preview.ValidationSummary,
                preview.IsStandardlessTest,
                layoutDto,
                paramRows,
                reqRows,
                condRows,
                equipRows,
                factorRows,
                uncDto,
                acDto,
                addedParamCodes,
                removedParamCodes,
                utg,
                preview);

            return new EffectiveConfigurationDto
            {
                UniversalTestGroupID = utg.ID,
                TestGroup = detail,
                PlannedBaseline = plannedBaseline,
                EffectiveConfiguration = preview,
                SnapshotPreview = snapshot,
                Parameters = paramRows,
                Requirements = reqRows,
                Conditions = condRows,
                Method = methodDto,
                Equipment = equipRows,
                Factors = factorRows,
                Uncertainty = uncDto,
                AcceptanceCriteria = acDto,
                Layout = layoutDto,
                Validation = validation
            };
        }

        public async Task<ValidationSummaryDto> GetValidationAsync(long testGroupId)
        {
            var eff = await GetEffectiveConfigurationAsync(testGroupId);
            return eff.Validation;
        }

        private async Task<LIMSApi.Models.UniversalTestGroup> LoadUtgAsync(long id)
        {
            var utg = await _context.UniversalTestGroups
                .Include(u => u.LaboratoryTest).ThenInclude(t => t.Discipline)
                .Include(u => u.LaboratoryTest).ThenInclude(t => t.LabDepartment)
                .Include(u => u.TestMethodSpecification)
                .Include(u => u.TestMethodSpecificationVersion)
                .Include(u => u.SpecificationHeader)
                .Include(u => u.SpecificationVersion)
                .Include(u => u.SpecificationGrade).ThenInclude(g => g.SpecificationHeader)
                .Include(u => u.Branch)
                .Include(u => u.ExecutionLayout)
                .Include(u => u.Department)
                .Include(u => u.SampleTestPlan).ThenInclude(p => p.SampleDetail).ThenInclude(s => s.ProductMaster)
                .Include(u => u.SampleTestPlan).ThenInclude(p => p.SampleDetail).ThenInclude(s => s.SampleInward)
                .Include(u => u.SampleTestPlan).ThenInclude(p => p.SampleDetail).ThenInclude(s => s.SpecificationGrade)
                .Include(u => u.Organization)
                .Include(u => u.TestExecutions)
                .FirstOrDefaultAsync(u => u.ID == id && u.IsActive);
            if (utg == null) throw new KeyNotFoundException($"Universal Test Group {id} not found.");
            return utg;
        }

        private async Task<UniversalTestGroupDetailDto> MapDetailAsync(LIMSApi.Models.UniversalTestGroup utg)
        {
            var sample = utg.SampleTestPlan.SampleDetail;
            var inward = sample.SampleInward;
            // Department resolution (authoritative BranchID + DisciplineID)
            long? deptId = null;
            string? deptName = null;
            string routingSource = string.Empty;
            if (utg.Department != null)
            {
                deptId = utg.Department.ID;
                deptName = utg.Department.Name;
                routingSource = $"Branch ({utg.Branch?.Name}) + Department ({utg.Department.Name})";
            }
            else if (utg.LaboratoryTest.DisciplineID.HasValue)
            {
                var dept = await _context.DepartmentMasters.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.BranchID == utg.BranchID && d.DisciplineID == utg.LaboratoryTest.DisciplineID.Value && d.IsActive);
                if (dept != null)
                {
                    deptId = dept.ID;
                    deptName = dept.Name;
                    routingSource = $"Branch ({utg.Branch?.Name}) + Discipline ({utg.LaboratoryTest.Discipline?.Name})";
                }
                else
                {
                    routingSource = "DEPARTMENT NOT CONFIGURED";
                }
            }
            else
            {
                routingSource = "DEPARTMENT NOT CONFIGURED";
            }

            // CreatedBy / ModifiedBy names
            string? createdByName = null;
            string? modifiedByName = null;
            if (utg.CreatedBy != 0)
            {
                var emp = await _context.EmployeeMasters.AsNoTracking().FirstOrDefaultAsync(e => e.ID == utg.CreatedBy);
                createdByName = emp?.Name ?? $"Emp#{utg.CreatedBy}";
            }
            if (utg.ModifiedBy.HasValue)
            {
                var emp2 = await _context.EmployeeMasters.AsNoTracking().FirstOrDefaultAsync(e => e.ID == utg.ModifiedBy.Value);
                modifiedByName = emp2?.Name ?? $"Emp#{utg.ModifiedBy.Value}";
            }

            // Sibling groups for same sample
            var siblings = await _context.UniversalTestGroups.AsNoTracking()
                .Where(u => u.IsActive && u.SampleTestPlanID == utg.SampleTestPlanID && u.ID != utg.ID)
                .Select(u => u.ID).ToListAsync();

            bool hasExecution = utg.TestExecutions.Any();
            var latestExec = utg.TestExecutions.OrderByDescending(e => e.ID).FirstOrDefault();

            var activeAdj = await _context.ConfigurationAdjustments
                .AsNoTracking()
                .Where(a => a.UniversalTestGroupID == utg.ID && a.IsActive)
                .OrderByDescending(a => a.AdjustmentNumber)
                .Select(a => new { a.ID, a.Status, a.AdjustmentNumber })
                .FirstOrDefaultAsync();

            bool canOpen = utg.Status == "Pending" || utg.Status == "InProgress" || hasExecution;
            string canOpenReason = hasExecution
                ? $"Execution {latestExec?.Status ?? utg.Status} (View)"
                : canOpen
                    ? "Ready for Execution"
                    : $"Status is {utg.Status}";

            return new UniversalTestGroupDetailDto
            {
                ID = utg.ID,
                SampleTestPlanID = utg.SampleTestPlanID,
                SampleID = sample.ID,
                SampleNo = sample.SampleNo,
                InwardCaseNo = inward.CaseNo,
                InwardID = inward.ID,
                InwardDate = inward.CollectionTime,
                CustomerName = await _context.Customers.AsNoTracking().Where(c => c.ID == inward.CustomerID).Select(c => c.Name).FirstOrDefaultAsync() ?? string.Empty,
                CustomerID = inward.CustomerID,
                SampleDetails = sample.Details,
                ProductMasterID = sample.ProductMasterID,
                ProductName = sample.ProductMaster?.ProductName ?? sample.ProductMaster?.DisplayTitle,
                SpecificationGradeID = utg.SpecificationGradeID ?? sample.SpecificationGradeID,
                GradeName = utg.SpecificationGrade?.Grade ?? sample.SpecificationGrade?.Grade,
                SpecificationHeaderID = utg.SpecificationHeaderID,
                SpecificationTitle = utg.SpecificationHeader?.DisplayTitle ?? utg.SpecificationHeader?.AliasName ?? utg.SpecificationHeader?.Code,
                StandardReference = utg.SpecificationHeader?.StandardReference,
                SpecificationVersionID = utg.SpecificationVersionID,
                SpecificationVersionName = utg.SpecificationVersion?.Version,
                SpecificationVersionStatus = utg.SpecificationVersion?.Status.ToString(),
                IsSupersededSpecVersion = utg.SpecificationVersion?.Status == VersionStatus.Superseded,
                IsStandardless = !utg.SpecificationHeaderID.HasValue,
                LaboratoryTestID = utg.LaboratoryTestID,
                LaboratoryTestCode = utg.LaboratoryTest.Code ?? string.Empty,
                LaboratoryTestName = utg.LaboratoryTest.Name,
                LaboratoryTestDescription = utg.LaboratoryTest.Description,
                DisciplineID = utg.LaboratoryTest.DisciplineID,
                DisciplineName = utg.LaboratoryTest.Discipline?.Name,
                LabDepartmentID = utg.LaboratoryTest.LabDepartmentID,
                TestMethodSpecificationID = utg.TestMethodSpecificationID,
                TestMethodCode = utg.TestMethodSpecification?.Code,
                TestMethodName = utg.TestMethodSpecification?.Name,
                TestMethodStandard = utg.TestMethodSpecification?.TestMethodStandard,
                TestMethodSpecificationVersionID = utg.TestMethodSpecificationVersionID,
                TestMethodVersion = utg.TestMethodSpecificationVersion?.Version,
                TestMethodVersionStatus = utg.TestMethodSpecificationVersion?.Status.ToString(),
                IsSupersededMethodVersion = utg.TestMethodSpecificationVersion?.Status == VersionStatus.Superseded,
                MethodEffectiveDate = utg.TestMethodSpecificationVersion?.EffectiveDate,
                BranchID = utg.BranchID,
                BranchName = utg.Branch?.Name,
                DepartmentID = deptId,
                DepartmentName = deptName,
                DepartmentRoutingSource = routingSource,
                ExecutionLayoutID = utg.ExecutionLayoutID,
                ExecutionLayoutCode = utg.ExecutionLayout?.Code,
                ExecutionLayoutName = utg.ExecutionLayout?.Name,
                RendererType = utg.ExecutionLayout?.RendererType,
                PlannedConfigurationJson = utg.PlannedConfigurationJson,
                Status = utg.Status,
                CompanyCode = utg.CompanyCode,
                OrganizationID = utg.OrganizationID,
                CreatedOn = utg.CreatedOn,
                CreatedByName = createdByName,
                ModifiedOn = utg.ModifiedOn,
                ModifiedByName = modifiedByName,
                HasExecution = hasExecution,
                LatestExecutionID = latestExec?.ID,
                LatestExecutionStatus = latestExec?.Status,
                HasSnapshot = hasExecution,
                CanOpenExecution = canOpen,
                CanOpenExecutionReason = canOpenReason,
                HasAdjustment = activeAdj != null,
                AdjustmentStatus = activeAdj?.Status,
                AdjustmentNumber = activeAdj?.AdjustmentNumber.ToString(),
                AdjustmentID = activeAdj?.ID,
                SiblingTestGroupIDs = siblings
            };
        }

        private UniversalTestGroupListItemDto ToListDto(LIMSApi.Models.UniversalTestGroup u, Dictionary<(long branchId, long disciplineId), string> deptMap)
        {
            string? deptName = null;
            if (u.LaboratoryTest.DisciplineID.HasValue)
            {
                deptMap.TryGetValue((u.BranchID, u.LaboratoryTest.DisciplineID.Value), out deptName);
            }
            return new UniversalTestGroupListItemDto
            {
                ID = u.ID,
                SampleTestPlanID = u.SampleTestPlanID,
                SampleID = u.SampleTestPlan.SampleID,
                SampleNo = u.SampleTestPlan.SampleNo,
                InwardCaseNo = u.SampleTestPlan.SampleDetail?.SampleInward?.CaseNo ?? string.Empty,
                InwardID = u.SampleTestPlan.SampleDetail?.InwardID ?? 0,
                LaboratoryTestID = u.LaboratoryTestID,
                LaboratoryTestCode = u.LaboratoryTest?.Code ?? string.Empty,
                LaboratoryTestName = u.LaboratoryTest?.Name ?? string.Empty,
                DisciplineName = u.LaboratoryTest?.Discipline?.Name,
                Status = u.Status,
                BranchID = u.BranchID,
                BranchName = u.Branch?.Name,
                DepartmentName = deptName,
                HasExecution = u.TestExecutions.Any(),
                LatestExecutionID = u.TestExecutions.OrderByDescending(e => e.ID).Select(e => (long?)e.ID).FirstOrDefault(),
                ExecutionStatus = u.TestExecutions.OrderByDescending(e => e.ID).Select(e => e.Status).FirstOrDefault(),
                HasSnapshot = u.TestExecutions.Any(),
                CreatedOn = u.CreatedOn
            };
        }

        private async Task<Dictionary<(long, long), string>> BuildDeptMapAsync(List<LIMSApi.Models.UniversalTestGroup> groups)
        {
            var branchIds = groups.Select(g => g.BranchID).Distinct().ToList();
            var discIds = groups.Where(g => g.LaboratoryTest.DisciplineID.HasValue).Select(g => g.LaboratoryTest.DisciplineID!.Value).Distinct().ToList();
            if (!branchIds.Any() || !discIds.Any()) return new();
            var depts = await _context.DepartmentMasters.AsNoTracking()
                .Where(d => branchIds.Contains(d.BranchID) && d.DisciplineID.HasValue && discIds.Contains(d.DisciplineID.Value) && d.IsActive)
                .ToListAsync();
            // Key (BranchID, DisciplineID) is NOT unique in DB — multiple departments can share same discipline.
            // Group by key and pick first active department to avoid "An item with the same key has already been added" (Key: (1, 1)).
            return depts.GroupBy(d => (d.BranchID, d.DisciplineID!.Value))
                        .ToDictionary(g => g.Key, g => g.First().Name);
        }

        private string MapResolutionToStatus(SpecificationResolutionStatus s) => s switch
        {
            SpecificationResolutionStatus.RESOLVED => "RESOLVED",
            SpecificationResolutionStatus.NOT_CONFIGURED => "NOT_CONFIGURED",
            SpecificationResolutionStatus.MANDATORY_MISSING => "BLOCKED",
            SpecificationResolutionStatus.VERSION_MISMATCH => "VERSION_MISMATCH",
            SpecificationResolutionStatus.GRADE_MISMATCH => "GRADE_MISMATCH",
            SpecificationResolutionStatus.PARAMETER_MISMATCH => "PARAMETER_MISMATCH",
            SpecificationResolutionStatus.SPECIFICATION_NOT_APPLICABLE => "N/A",
            SpecificationResolutionStatus.AMBIGUOUS_CONFIGURATION => "BLOCKED",
            SpecificationResolutionStatus.INVALID_CONFIGURATION => "INVALID_CONFIGURATION",
            _ => "NOT_CONFIGURED"
        };

        private ValidationSummaryDto Evaluate17GateValidation(
            UniversalPlanValidationSummaryDto vs,
            bool isStandardless,
            EffectiveLayoutDto layoutDto,
            List<EffectiveParameterRowDto> paramRows,
            List<EffectiveRequirementRowDto> reqRows,
            List<EffectiveConditionRowDto> condRows,
            List<EffectiveEquipmentRowDto> equipRows,
            List<EffectiveFactorRowDto> factorRows,
            EffectiveUncertaintyDto uncDto,
            EffectiveAcceptanceCriteriaDto acDto,
            List<string> addedParamCodes,
            List<string> removedParamCodes,
            LIMSApi.Models.UniversalTestGroup utg,
            UniversalPlanPreviewResponseDto preview)
        {
            var items = new List<ValidationItemDto>();

            // Gate 1: Test Definition
            items.Add(new ValidationItemDto
            {
                Check = "Test Definition",
                Source = "LaboratoryTests",
                Status = vs.TestDefinitionPass ? "PASS" : "BLOCKED",
                Message = vs.TestDefinitionPass ? "Laboratory test is active and verified." : (vs.TestDefinitionMessage ?? "Laboratory test definition is invalid or inactive.")
            });

            // Gate 2: Specification
            items.Add(new ValidationItemDto
            {
                Check = "Specification",
                Source = "SpecificationHeaders",
                Status = isStandardless ? "N/A" : (vs.SpecificationPass ? "PASS" : "BLOCKED"),
                Message = isStandardless ? "Standardless testing — specification header not applicable." : (vs.SpecificationPass ? "Specification header is active." : (vs.SpecificationMessage ?? "Specification header is missing or inactive."))
            });

            // Gate 3: Specification Version
            string specVerStatus = "PASS";
            string specVerMsg = $"Pinned Specification Version {preview.SpecificationVersionNumber} is active.";
            if (isStandardless)
            {
                specVerStatus = "N/A";
                specVerMsg = "Standardless testing — specification version not applicable.";
            }
            else if (utg.SpecificationVersion?.Status == VersionStatus.Superseded || preview.IsSupersededSpecVersion)
            {
                specVerStatus = "WARNING";
                specVerMsg = $"Pinned Specification Version is SUPERSEDED ({preview.SpecificationVersionNumber}). Execution permitted under frozen plan.";
            }
            else if (!vs.SpecificationVersionPass)
            {
                specVerStatus = "BLOCKED";
                specVerMsg = vs.SpecificationVersionMessage ?? "Specification version is missing or invalid.";
            }
            items.Add(new ValidationItemDto
            {
                Check = "Specification Version",
                Source = "SpecificationVersions",
                Status = specVerStatus,
                Message = specVerMsg
            });

            // Gate 4: Product Grade
            items.Add(new ValidationItemDto
            {
                Check = "Product Grade",
                Source = "SpecificationGrades",
                Status = isStandardless ? "N/A" : (vs.ProductGradePass ? "PASS" : "BLOCKED"),
                Message = isStandardless ? "Standardless testing — specification grade not applicable." : (vs.ProductGradePass ? $"Product grade '{preview.GradeName}' is valid and mapped to specification." : (vs.ProductGradeMessage ?? "Product grade is invalid or does not belong to specification."))
            });

            // Gate 5: Test Method
            items.Add(new ValidationItemDto
            {
                Check = "Test Method",
                Source = "TestMethodSpecifications",
                Status = vs.TestMethodPass ? "PASS" : "BLOCKED",
                Message = vs.TestMethodPass ? $"Test method '{preview.TestMethodCode}' is active and mapped to test." : (vs.TestMethodMessage ?? "Test method is inactive or not mapped to test.")
            });

            // Gate 6: Method Version
            string methVerStatus = "PASS";
            string methVerMsg = $"Pinned Test Method Version {preview.TestMethodVersion} is active.";
            if (utg.TestMethodSpecificationVersion?.Status == VersionStatus.Superseded || preview.IsSupersededMethodVersion)
            {
                methVerStatus = "WARNING";
                methVerMsg = $"Pinned Test Method Version is SUPERSEDED ({preview.TestMethodVersion}). Execution permitted under frozen plan.";
            }
            else if (!vs.MethodVersionPass)
            {
                methVerStatus = "BLOCKED";
                methVerMsg = vs.MethodVersionMessage ?? "Test method version is invalid or missing.";
            }
            items.Add(new ValidationItemDto
            {
                Check = "Method Version",
                Source = "TestMethodSpecificationVersions",
                Status = methVerStatus,
                Message = methVerMsg
            });

            // Gate 7: Parameters
            string paramStatus = "PASS";
            string paramMsg = $"All {paramRows.Count} planned parameters verified in scope.";
            if (!vs.MandatoryParametersPass)
            {
                paramStatus = "BLOCKED";
                paramMsg = vs.MandatoryParametersMessage ?? "Mandatory parameters are missing.";
            }
            else if (addedParamCodes.Any() || removedParamCodes.Any())
            {
                var driftDetails = new List<string>();
                if (addedParamCodes.Any()) driftDetails.Add($"New in master: {string.Join(", ", addedParamCodes)}");
                if (removedParamCodes.Any()) driftDetails.Add($"Removed in master: {string.Join(", ", removedParamCodes)}");
                paramStatus = "WARNING";
                paramMsg = $"Parameter master drift detected. Planned executable scope retained. ({string.Join("; ", driftDetails)})";
            }
            items.Add(new ValidationItemDto
            {
                Check = "Parameters",
                Source = "LaboratoryTestParameters",
                Status = paramStatus,
                Message = paramMsg
            });

            // Gate 8: Units
            string unitStatus = "PASS";
            string unitMsg = "All parameter units are configured and authoritative.";
            var missingUnits = paramRows.Where(p => p.IsMandatory && string.IsNullOrWhiteSpace(p.Unit)).ToList();
            if (missingUnits.Any())
            {
                unitStatus = "BLOCKED";
                unitMsg = $"Mandatory parameters missing units: {string.Join(", ", missingUnits.Select(p => p.ParameterCode))}";
            }
            else if (paramRows.Any(p => p.MasterDriftStatus == "CHANGED" && !string.IsNullOrWhiteSpace(p.MasterUnit) && !string.Equals(p.Unit, p.MasterUnit, StringComparison.OrdinalIgnoreCase)))
            {
                unitStatus = "WARNING";
                unitMsg = "Unit drift detected between planned baseline and master. Planned units remain effective.";
            }
            items.Add(new ValidationItemDto
            {
                Check = "Units",
                Source = "ParameterUnitMasters",
                Status = unitStatus,
                Message = unitMsg
            });

            // Gate 9: Requirements
            string reqStatus = "PASS";
            string reqMsg = "All specification requirement limits resolved successfully.";
            if (isStandardless)
            {
                reqStatus = "N/A";
                reqMsg = "Standardless testing — specification limits not applicable.";
            }
            else
            {
                var missingReqs = paramRows.Where(p => p.IsMandatory && (p.ResolutionStatus == "MANDATORY_MISSING" || p.ResolutionStatus == "NOT_CONFIGURED" || (!p.MinValue.HasValue && !p.MaxValue.HasValue && string.IsNullOrWhiteSpace(p.RequirementText) && string.IsNullOrWhiteSpace(p.Formula)))).ToList();
                if (missingReqs.Any())
                {
                    reqStatus = "BLOCKED";
                    reqMsg = $"Specification requirement limits missing for mandatory parameters: {string.Join(", ", missingReqs.Select(p => p.ParameterCode))}";
                }
                else if (paramRows.Any(p => p.MasterDriftStatus == "CHANGED" && p.DriftCategory == "Compliance"))
                {
                    reqStatus = "WARNING";
                    reqMsg = "Specification requirement limits have changed in master since planning. Planned limits remain effective.";
                }
            }
            items.Add(new ValidationItemDto
            {
                Check = "Requirements",
                Source = "SpecificationLines",
                Status = reqStatus,
                Message = reqMsg
            });

            // Gate 10: Conditions
            string condStatus = "PASS";
            string condMsg = $"All {condRows.Count} test conditions configured.";
            if (condRows.Count == 0)
            {
                condStatus = "N/A";
                condMsg = "No test conditions required for this test.";
            }
            else
            {
                var missingConds = condRows.Where(c => c.IsMandatory && !c.HasConfiguration && string.IsNullOrWhiteSpace(c.ConfiguredValue)).ToList();
                if (missingConds.Any())
                {
                    condStatus = "BLOCKED";
                    condMsg = $"Mandatory test conditions unconfigured: {string.Join(", ", missingConds.Select(c => c.ConditionCode))}";
                }
            }
            items.Add(new ValidationItemDto
            {
                Check = "Conditions",
                Source = "LaboratoryTestConditions",
                Status = condStatus,
                Message = condMsg
            });

            // Gate 11: Branch
            items.Add(new ValidationItemDto
            {
                Check = "Branch",
                Source = "Branches",
                Status = vs.BranchPass ? "PASS" : "BLOCKED",
                Message = vs.BranchPass ? $"Operating branch '{utg.Branch?.Name}' verified active." : (vs.BranchMessage ?? "Operating branch is invalid or inactive.")
            });

            // Gate 12: Department
            items.Add(new ValidationItemDto
            {
                Check = "Department",
                Source = "DepartmentMasters",
                Status = vs.DepartmentRoutingPass ? "PASS" : "BLOCKED",
                Message = vs.DepartmentRoutingPass ? $"Department resolved: '{preview.DepartmentName}'." : (vs.DepartmentRoutingMessage ?? "Department routing could not be resolved for Branch and Discipline.")
            });

            // Gate 13: Execution Layout
            string layoutStatus = "PASS";
            string layoutMsg = $"Execution layout '{layoutDto.EffectiveLayoutName ?? layoutDto.EffectiveLayoutCode}' assigned at level '{layoutDto.LayoutResolutionLevel}'.";
            if (layoutDto.ResolutionStatus == "UNASSIGNED")
            {
                layoutStatus = "WARNING";
                layoutMsg = "No execution layout assigned. Default dynamic renderer will be used.";
            }
            else if (layoutDto.ResolutionStatus == "LAYOUT_DRIFT")
            {
                layoutStatus = "WARNING";
                layoutMsg = layoutDto.Reason ?? "Execution layout assignment has changed since planning.";
            }
            items.Add(new ValidationItemDto
            {
                Check = "Execution Layout",
                Source = "ExecutionLayoutMasters",
                Status = layoutStatus,
                Message = layoutMsg
            });

            // Gate 14: Equipment Readiness
            string equipStatus = "PASS";
            string equipMsg = $"All {equipRows.Count} equipment requirements satisfied with active, calibrated instruments in branch.";
            if (equipRows.Count == 0)
            {
                equipStatus = "N/A";
                equipMsg = "No equipment requirements configured for this test.";
            }
            else
            {
                var blockedEquip = equipRows.Where(e => e.IsMandatory && e.ReadinessStatus == "BLOCKED").ToList();
                var warningEquip = equipRows.Where(e => e.ReadinessStatus == "WARNING").ToList();
                if (blockedEquip.Any())
                {
                    equipStatus = "BLOCKED";
                    equipMsg = string.Join("; ", blockedEquip.Select(e => e.BlockingReason ?? $"Equipment '{e.RequirementName}' unavailable/uncalibrated in branch."));
                }
                else if (warningEquip.Any())
                {
                    equipStatus = "WARNING";
                    equipMsg = string.Join("; ", warningEquip.Select(e => e.BlockingReason ?? $"Optional equipment '{e.RequirementName}' unavailable/uncalibrated in branch."));
                }
            }
            items.Add(new ValidationItemDto
            {
                Check = "Equipment Readiness",
                Source = "EquipmentMasters",
                Status = equipStatus,
                Message = equipMsg
            });

            // Gate 15: Factor Conversions
            string factorStatus = "PASS";
            string factorMsg = $"{factorRows.Count} factor conversion rule(s) configured (Precedence: Version > Method > Test > Global).";
            if (factorRows.Count == 0)
            {
                factorStatus = "N/A";
                factorMsg = "No factor conversions configured or applicable for participating parameters.";
            }
            items.Add(new ValidationItemDto
            {
                Check = "Factor Conversions",
                Source = "FactorConversionMasters",
                Status = factorStatus,
                Message = factorMsg
            });

            // Gate 16: Measurement Uncertainty
            string uncStatus = "PASS";
            string uncMsg = $"Uncertainty rule '{uncDto.MasterName ?? uncDto.MasterCode}' configured (U={uncDto.ExpandedUncertainty ?? uncDto.CombinedUncertainty} {uncDto.Unit}, k={uncDto.CoverageFactor}, CL={uncDto.ConfidenceLevel}%).";
            if (!uncDto.IsConfigured)
            {
                uncStatus = "N/A";
                uncMsg = "Measurement uncertainty not configured for this test/method.";
            }
            items.Add(new ValidationItemDto
            {
                Check = "Measurement Uncertainty",
                Source = "MeasurementUncertaintyMasters",
                Status = uncStatus,
                Message = uncMsg
            });

            // Gate 17: Acceptance Criteria
            string acStatus = "PASS";
            string acMsg = $"Acceptance criteria '{acDto.Name}' resolved via {acDto.ResolutionSource} (Decision Rule: {acDto.DecisionRule ?? "Default"}).";
            if (!acDto.IsConfigured)
            {
                if (acDto.Status == "N/A")
                {
                    acStatus = "N/A";
                    acMsg = $"Acceptance criteria not applicable ({acDto.ResolutionSource}).";
                }
                else
                {
                    acStatus = "WARNING";
                    acMsg = $"Acceptance criteria not configured ({acDto.ResolutionSource}). Default evaluation will apply.";
                }
            }
            items.Add(new ValidationItemDto
            {
                Check = "Acceptance Criteria",
                Source = "AcceptanceCriteriaMasters",
                Status = acStatus,
                Message = acMsg
            });

            int blockingCount = items.Count(i => i.Status == "BLOCKED" || i.Status == "BLOCKING");
            int warningCount = items.Count(i => i.Status == "WARNING");
            string overallStatus = blockingCount > 0 ? "BLOCKED" : (warningCount > 0 ? "WARNING" : "READY");
            bool allPassed = (blockingCount == 0 && warningCount == 0);

            var blockingErrors = items
                .Where(i => (i.Status == "BLOCKED" || i.Status == "BLOCKING") && !string.IsNullOrWhiteSpace(i.Message))
                .Select(i => $"{i.Check}: {i.Message}")
                .ToList();

            var warnings = items
                .Where(i => i.Status == "WARNING" && !string.IsNullOrWhiteSpace(i.Message))
                .Select(i => $"{i.Check}: {i.Message}")
                .ToList();

            return new ValidationSummaryDto
            {
                OverallStatus = overallStatus,
                BlockingCount = blockingCount,
                WarningCount = warningCount,
                AllPassed = allPassed,
                Items = items,
                BlockingErrors = blockingErrors,
                Warnings = warnings
            };
        }
    }
}
