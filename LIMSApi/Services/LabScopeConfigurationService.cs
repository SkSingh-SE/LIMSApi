using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class LabScopeConfigurationService : ILabScopeConfigurationService
    {
        private readonly LIMSContext _context;
        private readonly ILabScopeRepository _repository;
        private readonly INablScopeValidationService _scopeValidator;
        private readonly IBranchContext _branchContext;
        private readonly ILogger<LabScopeConfigurationService> _logger;
        private readonly LoggedInUserDTO _loggedInUser;

        private string CompanyCode => _loggedInUser.CompanyCode ?? "LIMS";

        public LabScopeConfigurationService(
            LIMSContext context,
            ILabScopeRepository repository,
            INablScopeValidationService scopeValidator,
            IBranchContext branchContext,
            ILogger<LabScopeConfigurationService> logger)
        {
            _context = context;
            _repository = repository;
            _scopeValidator = scopeValidator;
            _branchContext = branchContext;
            _logger = logger;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        private static bool IsScopeEffective(LabScopeMaster scope, DateTime referenceDateUtc)
        {
            if (scope.ValidFrom.HasValue && referenceDateUtc < scope.ValidFrom.Value) return false;
            if (scope.ValidUntil.HasValue && referenceDateUtc > scope.ValidUntil.Value) return false;
            return true;
        }

        private static string ValidityStateOf(LabScopeMaster scope, DateTime referenceDateUtc)
        {
            if (!scope.ValidFrom.HasValue && !scope.ValidUntil.HasValue) return "Open";
            if (scope.ValidFrom.HasValue && referenceDateUtc < scope.ValidFrom.Value) return "Scheduled";
            if (scope.ValidUntil.HasValue && referenceDateUtc > scope.ValidUntil.Value) return "Expired";
            return "Valid";
        }

        private static void ValidateValidityWindow(DateTime? validFrom, DateTime? validUntil)
        {
            if (validFrom.HasValue && validUntil.HasValue && validFrom.Value.Date > validUntil.Value.Date)
                throw new ArgumentException("Valid-From cannot be later than Valid-Until.");
        }

        private long ResolveMutationBranch(long? requestedBranchId, BranchAction action)
        {
            if (requestedBranchId.HasValue && requestedBranchId.Value > 0)
            {
                if (!_branchContext.IsAuthorizedForBranch(requestedBranchId.Value, action))
                    throw new UnauthorizedAccessException($"Not authorized for branch {requestedBranchId.Value}.");
                return requestedBranchId.Value;
            }
            return _branchContext.RequireCurrentBranchId();
        }

        private void RequireViewBranch(long? branchId)
        {
            if (branchId.HasValue && branchId.Value > 0
                && !_branchContext.IsAuthorizedForBranch(branchId.Value, BranchAction.View))
                throw new UnauthorizedAccessException($"Not authorized to view branch {branchId.Value}.");
        }

        private async Task<string> EmployeeNameAsync(long? employeeId)
        {
            if (!employeeId.HasValue || employeeId.Value <= 0) return "-";
            var name = await _context.EmployeeMasters
                .Where(e => e.ID == employeeId.Value)
                .Select(e => e.Name)
                .FirstOrDefaultAsync();
            return string.IsNullOrWhiteSpace(name) ? "-" : name!;
        }

        private async Task<AccreditationContextDto> ResolveAccreditationAsync(long? branchId, DateTime referenceDateUtc)
        {
            var result = new AccreditationContextDto();
            long? orgId = null;
            try
            {
                if (branchId.HasValue && branchId.Value > 0)
                    orgId = _branchContext.ResolveTenantContext(branchId.Value).OrganizationID;
                else
                    orgId = _branchContext.RequireOrganizationId();
            }
            catch (InvalidOperationException) { return result; }

            var cert = await _context.NablAccreditations
                .Where(n => n.IsActive && n.CompanyCode == CompanyCode && n.OrganizationId == orgId
                    && (n.BranchID == null || (branchId.HasValue && n.BranchID == branchId.Value)))
                .OrderByDescending(n => n.ExpiryDate)
                .FirstOrDefaultAsync();
            if (cert == null) return result;

            result.AccreditationID = cert.Id;
            result.CertificateNumber = cert.CertificateNumber;
            result.IssueDate = cert.IssueDate == default ? null : cert.IssueDate;
            result.ExpiryDate = cert.ExpiryDate == default ? null : cert.ExpiryDate;
            result.LogoPath = cert.LogoPath;
            result.IsEffective = result.IssueDate.HasValue && result.ExpiryDate.HasValue
                && result.IssueDate.Value <= referenceDateUtc && result.ExpiryDate.Value >= referenceDateUtc;
            return result;
        }

        public async Task<PagedResponse<ScopeListItemDto>> QueryScopesAsync(ScopeListRequest request)
        {
            var status = (request.Status ?? "active").Trim().ToLowerInvariant();
            var now = DateTime.UtcNow;

            long? branchFilter = request.BranchID;
            if (branchFilter.HasValue && branchFilter.Value > 0)
            {
                RequireViewBranch(branchFilter);
            }

            var authorizedOnly = new List<long>();
            if ((!branchFilter.HasValue || branchFilter.Value <= 0) && !_branchContext.CanViewAllBranches)
                authorizedOnly = _branchContext.AuthorizedBranchIDs.ToList();

            var query = _context.LabScopeMasters
                .Where(ls => ls.CompanyCode == CompanyCode);

            if (status == "active") query = query.Where(ls => ls.IsActive);
            else if (status == "inactive") query = query.Where(ls => !ls.IsActive);

            if (branchFilter.HasValue && branchFilter.Value > 0)
                query = query.Where(ls => ls.BranchID == branchFilter.Value);
            else if (authorizedOnly.Any())
                query = query.Where(ls => ls.BranchID.HasValue && authorizedOnly.Contains(ls.BranchID.Value));

            if (request.LaboratoryTestID.HasValue && request.LaboratoryTestID.Value > 0)
                query = query.Where(ls => ls.LaboratoryTestID == request.LaboratoryTestID.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim();
                query = query.Where(ls => _context.LaboratoryTests
                    .Any(t => t.ID == ls.LaboratoryTestID && t.Name.Contains(search)));
            }

            var validity = (request.Validity ?? "all").Trim().ToLowerInvariant();
            if (validity == "valid")
                query = query.Where(ls => (ls.ValidFrom == null || ls.ValidFrom <= now)
                    && (ls.ValidUntil == null || ls.ValidUntil >= now));
            else if (validity == "expired")
                query = query.Where(ls => ls.ValidUntil != null && ls.ValidUntil < now);

            var accreditation = (request.Accreditation ?? "all").Trim().ToLowerInvariant();
            if (accreditation == "covered" || accreditation == "uncovered")
            {
                var orgId = _loggedInUser.OrganizationID;
                var coveredBranchIds = await _context.NablAccreditations
                    .Where(n => n.IsActive && n.CompanyCode == CompanyCode
                        && (orgId == null || n.OrganizationId == orgId)
                        && n.IssueDate != default && n.ExpiryDate != default
                        && n.IssueDate <= now && n.ExpiryDate >= now)
                    .Select(n => n.BranchID)
                    .ToListAsync();
                bool globalCovered = coveredBranchIds.Any(b => b == null);
                var coveredBranches = coveredBranchIds.Where(b => b.HasValue).Select(b => b!.Value).ToHashSet();
                if (accreditation == "covered")
                    query = query.Where(ls => globalCovered || (ls.BranchID.HasValue && coveredBranches.Contains(ls.BranchID.Value)));
                else
                    query = query.Where(ls => !globalCovered && (!ls.BranchID.HasValue || !coveredBranches.Contains(ls.BranchID.Value)));
            }

            var totalRecords = await query.CountAsync();
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

            var sortCol = (request.SortByColumn ?? "modifiedOn").Trim().ToLowerInvariant();
            var desc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = sortCol switch
            {
                "laboratorytestname" => desc
                    ? query.OrderByDescending(ls => _context.LaboratoryTests.Where(t => t.ID == ls.LaboratoryTestID).Select(t => t.Name).FirstOrDefault())
                    : query.OrderBy(ls => _context.LaboratoryTests.Where(t => t.ID == ls.LaboratoryTestID).Select(t => t.Name).FirstOrDefault()),
                "branchname" => desc
                    ? query.OrderByDescending(ls => _context.Branches.Where(b => b.ID == ls.BranchID).Select(b => b.Name).FirstOrDefault())
                    : query.OrderBy(ls => _context.Branches.Where(b => b.ID == ls.BranchID).Select(b => b.Name).FirstOrDefault()),
                "validfrom" => desc ? query.OrderByDescending(ls => ls.ValidFrom) : query.OrderBy(ls => ls.ValidFrom),
                "validuntil" => desc ? query.OrderByDescending(ls => ls.ValidUntil) : query.OrderBy(ls => ls.ValidUntil),
                _ => desc ? query.OrderByDescending(ls => ls.ModifiedOn) : query.OrderBy(ls => ls.ModifiedOn),
            };

            var page = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var scopeIds = page.Select(x => x.ID).ToList();
            var specNames = await _context.LabScopeSpecifications
                .Where(s => scopeIds.Contains(s.LabScopeID))
                .Join(_context.TestMethodSpecifications, s => s.TestMethodSpecificationID, t => t.ID,
                    (s, t) => new { s.LabScopeID, t.Name })
                .ToListAsync();
            var specNameMap = specNames.GroupBy(x => x.LabScopeID)
                .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(x => x.Name)));
            var paramCounts = await _context.LabScopeSpecifications
                .Where(s => scopeIds.Contains(s.LabScopeID))
                .SelectMany(s => s.Parameters)
                .GroupBy(p => p.LabScopeSpecificationID)
                .Select(g => new { SpecId = g.Key, Count = g.Count() })
                .ToListAsync();
            var specToScope = await _context.LabScopeSpecifications
                .Where(s => scopeIds.Contains(s.LabScopeID))
                .Select(s => new { s.ID, s.LabScopeID })
                .ToListAsync();
            var countByScope = specToScope
                .GroupJoin(paramCounts, s => s.ID, p => p.SpecId, (s, ps) => new { s.LabScopeID, Count = ps.Sum(p => p.Count) })
                .GroupBy(x => x.LabScopeID)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));

            var branchIdList = page.Where(x => x.BranchID.HasValue).Select(x => x.BranchID!.Value).Distinct().ToList();
            var branchNames = await _context.Branches
                .Where(b => branchIdList.Contains(b.ID))
                .ToDictionaryAsync(b => b.ID, b => b.Name);
            var testIdList = page.Select(x => x.LaboratoryTestID).Distinct().ToList();
            var testNames = await _context.LaboratoryTests
                .Where(t => testIdList.Contains(t.ID))
                .ToDictionaryAsync(t => t.ID, t => t.Name);
            var empIds = page.SelectMany(x => new[] { x.CreatedBy, x.ModifiedBy ?? 0 }).Where(id => id > 0).Distinct().ToList();
            var empNames = await _context.EmployeeMasters
                .Where(e => empIds.Contains(e.ID))
                .ToDictionaryAsync(e => e.ID, e => e.Name);

            var items = new List<ScopeListItemDto>();
            foreach (var ls in page)
            {
                var covered = await IsAccreditationCoveredAsync(ls.BranchID, now);
                items.Add(new ScopeListItemDto
                {
                    ID = ls.ID,
                    LaboratoryTestID = ls.LaboratoryTestID,
                    LaboratoryTestName = testNames.GetValueOrDefault(ls.LaboratoryTestID, "-"),
                    BranchID = ls.BranchID,
                    BranchName = ls.BranchID.HasValue && branchNames.TryGetValue(ls.BranchID.Value, out var bn) ? bn : "-",
                    MethodsSummary = specNameMap.GetValueOrDefault(ls.ID, ""),
                    ParameterCount = countByScope.GetValueOrDefault(ls.ID, 0),
                    ValidFrom = ls.ValidFrom,
                    ValidUntil = ls.ValidUntil,
                    ValidityState = ValidityStateOf(ls, now),
                    AccreditationCovered = covered,
                    IsActive = ls.IsActive,
                    CreatedByName = empNames.GetValueOrDefault(ls.CreatedBy, "-"),
                    CreatedOn = ls.CreatedOn,
                    ModifiedByName = ls.ModifiedBy.HasValue && empNames.TryGetValue(ls.ModifiedBy.Value, out var mn) ? mn : empNames.GetValueOrDefault(ls.CreatedBy, "-"),
                    ModifiedOn = ls.ModifiedOn
                });
            }

            return new PagedResponse<ScopeListItemDto>(items, totalRecords, pageNumber, pageSize);
        }

        private async Task<bool> IsAccreditationCoveredAsync(long? branchId, DateTime referenceDateUtc)
        {
            var ctx = await ResolveAccreditationForBranchAsync(branchId, referenceDateUtc);
            return ctx.IsEffective;
        }

        private async Task<AccreditationContextDto> ResolveAccreditationForBranchAsync(long? branchId, DateTime referenceDateUtc)
        {
            return await ResolveAccreditationAsync(branchId, referenceDateUtc);
        }

        public async Task<AccreditationContextDto> GetAccreditationContextAsync(long? branchId)
        {
            if (branchId.HasValue && branchId.Value > 0) RequireViewBranch(branchId);
            return await ResolveAccreditationAsync(branchId, DateTime.UtcNow);
        }

        public async Task<ScopeDetailDto> GetScopeDetailsAsync(long id)
        {
            var scope = await _repository.GetLabScopeById(id);
            if (scope == null || scope.CompanyCode != CompanyCode)
                throw new KeyNotFoundException("Lab scope not found.");
            RequireViewBranch(scope.BranchID);

            var now = DateTime.UtcNow;
            var testName = await _context.LaboratoryTests.Where(t => t.ID == scope.LaboratoryTestID).Select(t => t.Name).FirstOrDefaultAsync() ?? "-";
            var branchName = scope.BranchID.HasValue
                ? await _context.Branches.Where(b => b.ID == scope.BranchID.Value).Select(b => b.Name).FirstOrDefaultAsync() ?? "-"
                : "-";

            var methodIds = scope.Specifications.Select(s => s.TestMethodSpecificationID).Distinct().ToList();
            var methodNames = await _context.TestMethodSpecifications.Where(t => methodIds.Contains(t.ID)).ToDictionaryAsync(t => t.ID, t => t.Name);
            var versionIds = scope.Specifications.Where(s => s.TestMethodSpecificationVersionID.HasValue).Select(s => s.TestMethodSpecificationVersionID!.Value).Distinct().ToList();
            var versionNames = await _context.TestMethodSpecificationVersions.Where(v => versionIds.Contains(v.ID)).ToDictionaryAsync(v => v.ID, v => v.Version);
            var paramIds = scope.Specifications.SelectMany(s => s.Parameters).Select(p => p.ParameterID).Distinct().ToList();
            var paramNames = await _context.ParameterMasters.Where(p => paramIds.Contains(p.ID)).ToDictionaryAsync(p => p.ID, p => p.Name);
            var unitIds = scope.Specifications.SelectMany(s => s.Parameters).Select(p => p.ParameterUnitID).Distinct().ToList();
            var unitNames = await _context.ParameterUnitMasters.Where(u => unitIds.Contains(u.ID)).ToDictionaryAsync(u => u.ID, u => u.Name);
            var equipIds = scope.Specifications.SelectMany(s => s.Parameters).SelectMany(p => p.Equipments).Select(e => e.EquipmentID).Distinct().ToList();
            var equipNames = await _context.EquipmentMasters.Where(e => equipIds.Contains(e.ID)).ToDictionaryAsync(e => e.ID, e => e.Name);

            var dto = new ScopeDetailDto
            {
                ID = scope.ID,
                LaboratoryTestID = scope.LaboratoryTestID,
                LaboratoryTestName = testName,
                BranchID = scope.BranchID,
                BranchName = branchName,
                IsActive = scope.IsActive,
                ValidFrom = scope.ValidFrom,
                ValidUntil = scope.ValidUntil,
                NextReviewDate = scope.NextReviewDate,
                ScopeRemarks = scope.ScopeRemarks,
                ValidityState = ValidityStateOf(scope, now),
                CreatedByName = await EmployeeNameAsync(scope.CreatedBy),
                CreatedOn = scope.CreatedOn,
                ModifiedByName = await EmployeeNameAsync(scope.ModifiedBy ?? scope.CreatedBy),
                ModifiedOn = scope.ModifiedOn,
                CompanyCode = scope.CompanyCode,
                Accreditation = await ResolveAccreditationAsync(scope.BranchID, now)
            };

            foreach (var spec in scope.Specifications)
            {
                var methodDto = new ScopeMethodDto
                {
                    ID = spec.ID,
                    TestMethodSpecificationID = spec.TestMethodSpecificationID,
                    TestMethodName = methodNames.GetValueOrDefault(spec.TestMethodSpecificationID, "-"),
                    TestMethodSpecificationVersionID = spec.TestMethodSpecificationVersionID,
                    TestMethodVersion = spec.TestMethodSpecificationVersionID.HasValue
                        ? versionNames.GetValueOrDefault(spec.TestMethodSpecificationVersionID.Value, "-") : null
                };
                foreach (var p in spec.Parameters)
                {
                    methodDto.Parameters.Add(new ScopeParameterDto
                    {
                        ID = p.ID,
                        ParameterID = p.ParameterID,
                        ParameterName = paramNames.GetValueOrDefault(p.ParameterID, "-"),
                        ParameterUnitID = p.ParameterUnitID,
                        ParameterUnitName = unitNames.GetValueOrDefault(p.ParameterUnitID, "-"),
                        ScopeType = p.QualitativeQuantitative,
                        IsUnderISO = p.IsUnderISO,
                        LowerOperator = p.LowerLimit,
                        LowerLimitValue = p.LowerLimitValue,
                        UpperOperator = p.UpperLimit,
                        UpperLimitValue = p.UpperLimitValue,
                        EquipmentIDs = p.Equipments.Select(e => e.EquipmentID).ToList(),
                        EquipmentNames = p.Equipments.Select(e => equipNames.GetValueOrDefault(e.EquipmentID, "-")).ToList()
                    });
                }
                dto.Methods.Add(methodDto);
            }

            var logs = await _context.LabScopeChangeLogs
                .Where(l => l.LabScopeID == id)
                .OrderByDescending(l => l.ChangedOn)
                .Take(100)
                .ToListAsync();
            foreach (var log in logs)
            {
                dto.ChangeHistory.Add(new ScopeChangeHistoryDto
                {
                    ID = log.ID,
                    ChangeType = log.ChangeType,
                    EntityName = log.EntityName,
                    OldValue = log.OldValue,
                    NewValue = log.NewValue,
                    ChangedBy = log.ChangedBy,
                    ChangedByName = await EmployeeNameAsync(log.ChangedBy),
                    ChangedOn = log.ChangedOn
                });
            }

            return dto;
        }

        public async Task<long> CreateScopeAsync(ScopeCreateDto dto)
        {
            if (dto.LaboratoryTestID <= 0)
                throw new ArgumentException("Laboratory test is required.");
            var branchId = ResolveMutationBranch(dto.BranchID, BranchAction.Create);
            ValidateValidityWindow(dto.ValidFrom, dto.ValidUntil);

            var testExists = await _context.LaboratoryTests
                .AnyAsync(t => t.ID == dto.LaboratoryTestID && t.IsActive && t.CompanyCode == CompanyCode);
            if (!testExists)
                throw new ArgumentException("Selected laboratory test is not available.");

            var duplicate = await _context.LabScopeMasters.AnyAsync(x =>
                x.LaboratoryTestID == dto.LaboratoryTestID && x.IsActive
                && x.CompanyCode == CompanyCode
                && (x.BranchID == branchId || (x.BranchID == null && branchId == 0)));
            if (duplicate)
                throw new InvalidOperationException("A scope already exists for this laboratory test in this branch.");

            var entity = BuildScopeEntity(new LabScopeMaster(), dto, branchId, isNew: true);
            await _repository.AddLabScope(entity);

            _context.LabScopeChangeLogs.Add(new LabScopeChangeLog
            {
                LabScopeID = entity.ID,
                ChangeType = "Created",
                EntityName = $"LabTestID: {entity.LaboratoryTestID}",
                NewValue = $"{entity.Specifications.Count} methods, {entity.Specifications.SelectMany(s => s.Parameters).Count()} parameters",
                ChangedBy = _loggedInUser.EmployeeID,
                ChangedOn = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation("Lab scope created for test {TestId} in branch {BranchId}.", entity.LaboratoryTestID, branchId);
            return entity.ID;
        }

        public async Task ModifyScopeAsync(ScopeUpdateDto dto)
        {
            if (dto.ID <= 0)
                throw new ArgumentException("Lab scope ID is required.");
            var existing = await _repository.GetLabScopeById(dto.ID);
            if (existing == null || existing.CompanyCode != CompanyCode)
                throw new KeyNotFoundException("Lab scope not found.");

            var branchId = ResolveMutationBranch(dto.BranchID ?? existing.BranchID, BranchAction.Edit);
            ValidateValidityWindow(dto.ValidFrom, dto.ValidUntil);

            if (existing.LaboratoryTestID != dto.LaboratoryTestID || existing.BranchID != branchId)
            {
                var duplicate = await _context.LabScopeMasters.AnyAsync(x =>
                    x.LaboratoryTestID == dto.LaboratoryTestID && x.IsActive && x.ID != dto.ID
                    && x.CompanyCode == CompanyCode
                    && (x.BranchID == branchId || (x.BranchID == null && branchId == 0)));
                if (duplicate)
                    throw new InvalidOperationException("A scope already exists for this laboratory test in this branch.");
            }

            var logs = new List<LabScopeChangeLog>();
            if (existing.BranchID != branchId)
                logs.Add(ScopeLog(dto.ID, "BranchChanged", $"BranchID: {existing.BranchID}", $"BranchID: {branchId}"));
            if (existing.ValidFrom != dto.ValidFrom || existing.ValidUntil != dto.ValidUntil)
                logs.Add(ScopeLog(dto.ID, "ValidityChanged", $"{existing.ValidFrom:yyyy-MM-dd}..{existing.ValidUntil:yyyy-MM-dd}", $"{dto.ValidFrom:yyyy-MM-dd}..{dto.ValidUntil:yyyy-MM-dd}"));

            existing.LaboratoryTestID = dto.LaboratoryTestID;
            existing.BranchID = branchId;
            existing.ValidFrom = dto.ValidFrom;
            existing.ValidUntil = dto.ValidUntil;
            existing.NextReviewDate = dto.NextReviewDate;
            existing.ScopeRemarks = dto.ScopeRemarks?.Trim();
            existing.IsActive = dto.IsActive;
            existing.ModifiedBy = _loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;

            SyncMethods(existing, dto.Methods, logs);
            if (logs.Any()) _context.LabScopeChangeLogs.AddRange(logs);
            await _repository.UpdateLabScope(existing);
            _logger.LogInformation("Lab scope {Id} updated with {Count} logged changes.", dto.ID, logs.Count);
        }

        public async Task<bool> ToggleScopeStatusAsync(long id, bool activate)
        {
            var existing = await _repository.GetLabScopeById(id);
            if (existing == null || existing.CompanyCode != CompanyCode)
                throw new KeyNotFoundException("Lab scope not found.");
            ResolveMutationBranch(existing.BranchID, BranchAction.Edit);

            if (activate && !existing.IsActive)
            {
                var branchId = existing.BranchID ?? 0;
                var duplicate = await _context.LabScopeMasters.AnyAsync(x =>
                    x.LaboratoryTestID == existing.LaboratoryTestID && x.IsActive && x.ID != id
                    && x.CompanyCode == CompanyCode
                    && (x.BranchID == existing.BranchID || (x.BranchID == null && branchId == 0)));
                if (duplicate)
                    throw new InvalidOperationException("Cannot activate: another active scope already covers this test in this branch.");
            }

            existing.IsActive = activate;
            existing.ModifiedBy = _loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repository.UpdateLabScope(existing);
            _context.LabScopeChangeLogs.Add(ScopeLog(id, activate ? "Reactivated" : "Deactivated", null, null));
            await _context.SaveChangesAsync();
            return existing.IsActive;
        }

        public async Task RemoveScopeAsync(long id)
        {
            var existing = await _repository.GetLabScopeById(id);
            if (existing == null || existing.CompanyCode != CompanyCode)
                throw new KeyNotFoundException("Lab scope not found.");
            ResolveMutationBranch(existing.BranchID, BranchAction.Delete);

            await DeleteValidationHelper.ValidateDeleteAsync<LabScopeMaster>(_context, id, "Lab Scope");
            existing.IsActive = false;
            existing.ModifiedBy = _loggedInUser.EmployeeID;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repository.UpdateLabScope(existing);
            _context.LabScopeChangeLogs.Add(ScopeLog(id, "Deactivated", null, null));
            await _context.SaveChangesAsync();
        }

        public async Task<ScopeDecisionDto> ValidateAsync(ScopeValidateRequest request)
        {
            if (request.LaboratoryTestID <= 0)
                throw new ArgumentException("Laboratory test is required.");
            if (request.ParameterID <= 0)
                throw new ArgumentException("Parameter is required.");
            if (request.BranchID.HasValue && request.BranchID.Value > 0)
                RequireViewBranch(request.BranchID);
            var refDate = request.ReferenceDate ?? DateTime.UtcNow;

            NablScopeCheckResult check;
            if (request.Value.HasValue)
                check = await _scopeValidator.CheckParameterScope(
                    request.LaboratoryTestID, request.ParameterID, request.Value.Value, request.BranchID, refDate);
            else
            {
                var covered = await _scopeValidator.CheckParameterScopeExists(
                    request.LaboratoryTestID, request.ParameterID, request.BranchID, refDate);
                check = new NablScopeCheckResult
                {
                    ParameterId = request.ParameterID,
                    ScopeStatus = covered ? "WithinScope" : "NotAccredited"
                };
            }

            var dto = new ScopeDecisionDto
            {
                ScopeStatus = check.ScopeStatus,
                LabScopeSpecParamId = check.LabScopeSpecParamId,
                ScopeLowerLimit = check.NablLowerLimit,
                ScopeUpperLimit = check.NablUpperLimit
            };

            if (check.LabScopeSpecParamId.HasValue)
            {
                var specInfo = await _context.LabScopeSpecificationParameters
                    .Where(p => p.ID == check.LabScopeSpecParamId.Value)
                    .Join(_context.LabScopeSpecifications, p => p.LabScopeSpecificationID, s => s.ID,
                        (p, s) => new { s.LabScopeID, s.TestMethodSpecificationID, s.TestMethodSpecificationVersionID })
                    .FirstOrDefaultAsync();
                if (specInfo != null)
                {
                    dto.LabScopeID = specInfo.LabScopeID;
                    dto.MethodName = await _context.TestMethodSpecifications
                        .Where(t => t.ID == specInfo.TestMethodSpecificationID).Select(t => t.Name).FirstOrDefaultAsync();
                    if (specInfo.TestMethodSpecificationVersionID.HasValue)
                        dto.MethodVersion = await _context.TestMethodSpecificationVersions
                            .Where(v => v.ID == specInfo.TestMethodSpecificationVersionID!.Value).Select(v => v.Version).FirstOrDefaultAsync();
                }
            }

            var accred = await ResolveAccreditationAsync(request.BranchID, refDate);
            dto.AccreditationEffective = accred.IsEffective;
            dto.AccreditationCertificate = accred.CertificateNumber;
            dto.Reason = check.ScopeStatus switch
            {
                "WithinScope" => request.Value.HasValue
                    ? $"Value lies within the accredited scope range{(dto.MethodName != null ? $" ({dto.MethodName})" : "")}."
                    : "Parameter is covered by the effective accredited scope.",
                "OutsideScope" => "Parameter is accredited, but the value lies outside the accredited scope range.",
                _ => "No effective accredited scope covers this laboratory test and parameter."
            };
            return dto;
        }

        public async Task<ScopePreviewDto> PreviewAsync(ScopePreviewRequest request)
        {
            if (request.LaboratoryTestID <= 0)
                throw new ArgumentException("Laboratory test is required.");
            if (request.BranchID.HasValue && request.BranchID.Value > 0)
                RequireViewBranch(request.BranchID);
            var refDate = request.ReferenceDate ?? DateTime.UtcNow;

            var scopes = await _context.LabScopeMasters
                .Include(ls => ls.Specifications)
                    .ThenInclude(s => s.Parameters)
                .Where(ls => ls.LaboratoryTestID == request.LaboratoryTestID && ls.IsActive
                    && ls.CompanyCode == CompanyCode
                    && (!request.BranchID.HasValue || request.BranchID.Value <= 0 || ls.BranchID == null || ls.BranchID == request.BranchID.Value)
                    && (ls.ValidFrom == null || refDate >= ls.ValidFrom)
                    && (ls.ValidUntil == null || refDate <= ls.ValidUntil))
                .ToListAsync();

            var dto = new ScopePreviewDto
            {
                LaboratoryTestID = request.LaboratoryTestID,
                Accreditation = await ResolveAccreditationAsync(request.BranchID, refDate)
            };
            if (!scopes.Any()) return dto;

            var methodIds = scopes.SelectMany(s => s.Specifications).Select(s => s.TestMethodSpecificationID).Distinct().ToList();
            var methodNames = await _context.TestMethodSpecifications.Where(t => methodIds.Contains(t.ID)).ToDictionaryAsync(t => t.ID, t => t.Name);
            var versionIds = scopes.SelectMany(s => s.Specifications).Where(s => s.TestMethodSpecificationVersionID.HasValue).Select(s => s.TestMethodSpecificationVersionID!.Value).Distinct().ToList();
            var versionNames = await _context.TestMethodSpecificationVersions.Where(v => versionIds.Contains(v.ID)).ToDictionaryAsync(v => v.ID, v => v.Version);
            var paramIds = scopes.SelectMany(s => s.Specifications).SelectMany(s => s.Parameters).Select(p => p.ParameterID).Distinct().ToList();
            var paramInfo = await _context.ParameterMasters.Where(p => paramIds.Contains(p.ID)).ToDictionaryAsync(p => p.ID, p => new { p.Name });
            var unitIds = scopes.SelectMany(s => s.Specifications).SelectMany(s => s.Parameters).Select(p => p.ParameterUnitID).Distinct().ToList();
            var unitNames = await _context.ParameterUnitMasters.Where(u => unitIds.Contains(u.ID)).ToDictionaryAsync(u => u.ID, u => u.Name);

            foreach (var scope in scopes)
            {
                foreach (var spec in scope.Specifications)
                {
                    foreach (var p in spec.Parameters)
                    {
                        dto.Parameters.Add(new ScopePreviewParameterDto
                        {
                            ParameterID = p.ParameterID,
                            ParameterName = paramInfo.GetValueOrDefault(p.ParameterID)?.Name ?? "-",
                            ParameterUnitName = unitNames.GetValueOrDefault(p.ParameterUnitID, "-"),
                            ScopeType = p.QualitativeQuantitative,
                            ScopeStatus = "WithinScope",
                            LabScopeID = scope.ID,
                            MethodName = methodNames.GetValueOrDefault(spec.TestMethodSpecificationID, "-"),
                            MethodVersion = spec.TestMethodSpecificationVersionID.HasValue
                                ? versionNames.GetValueOrDefault(spec.TestMethodSpecificationVersionID.Value, "-") : null
                        });
                    }
                }
            }
            dto.InScopeCount = dto.Parameters.Count;
            return dto;
        }

        private static LabScopeChangeLog ScopeLog(long scopeId, string changeType, string? oldValue, string? newValue)
        {
            return new LabScopeChangeLog
            {
                LabScopeID = scopeId,
                ChangeType = changeType,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedBy = LoggedInUserProvider.CurrentUser.EmployeeID,
                ChangedOn = DateTime.UtcNow
            };
        }

        private LabScopeMaster BuildScopeEntity(LabScopeMaster entity, ScopeCreateDto dto, long branchId, bool isNew)
        {
            entity.LaboratoryTestID = dto.LaboratoryTestID;
            entity.BranchID = branchId;
            entity.ValidFrom = dto.ValidFrom;
            entity.ValidUntil = dto.ValidUntil;
            entity.NextReviewDate = dto.NextReviewDate;
            entity.ScopeRemarks = dto.ScopeRemarks?.Trim();
            if (isNew)
            {
                entity.CompanyCode = CompanyCode;
                entity.IsActive = true;
                entity.CreatedBy = _loggedInUser.EmployeeID;
                entity.CreatedOn = DateTime.UtcNow;
            }
            entity.ModifiedBy = _loggedInUser.EmployeeID;
            entity.ModifiedOn = DateTime.UtcNow;
            SyncMethods(entity, dto.Methods, new List<LabScopeChangeLog>());
            return entity;
        }

        private void SyncMethods(LabScopeMaster entity, List<ScopeMethodDto> incoming, List<LabScopeChangeLog> logs)
        {
            var toRemove = entity.Specifications.Where(x => !incoming.Any(y => y.ID == x.ID && x.ID > 0)).ToList();
            foreach (var spec in toRemove)
            {
                logs.Add(new LabScopeChangeLog
                {
                    LabScopeID = entity.ID,
                    ChangeType = "SpecificationRemoved",
                    EntityName = $"SpecID: {spec.TestMethodSpecificationID}",
                    OldValue = $"{spec.Parameters.Count} parameters",
                    ChangedBy = _loggedInUser.EmployeeID,
                    ChangedOn = DateTime.UtcNow
                });
                entity.Specifications.Remove(spec);
            }

            foreach (var incomingSpec in incoming)
            {
                var methodExists = _context.TestMethodSpecifications
                    .Any(t => t.ID == incomingSpec.TestMethodSpecificationID && t.IsActive);
                if (!methodExists)
                    throw new ArgumentException($"Test method {incomingSpec.TestMethodSpecificationID} is not available.");
                if (incomingSpec.TestMethodSpecificationVersionID.HasValue)
                {
                    var versionOk = _context.TestMethodSpecificationVersions.Any(v =>
                        v.ID == incomingSpec.TestMethodSpecificationVersionID!.Value
                        && v.TestMethodSpecificationID == incomingSpec.TestMethodSpecificationID);
                    if (!versionOk)
                        throw new ArgumentException("Selected method version does not belong to the selected method.");
                }

                var existingSpec = incomingSpec.ID > 0
                    ? entity.Specifications.FirstOrDefault(x => x.ID == incomingSpec.ID)
                    : null;
                if (existingSpec == null)
                {
                    logs.Add(new LabScopeChangeLog
                    {
                        LabScopeID = entity.ID,
                        ChangeType = "SpecificationAdded",
                        EntityName = $"SpecID: {incomingSpec.TestMethodSpecificationID}",
                        NewValue = $"{incomingSpec.Parameters.Count} parameters",
                        ChangedBy = _loggedInUser.EmployeeID,
                        ChangedOn = DateTime.UtcNow
                    });
                    existingSpec = new LabScopeSpecification { LabScopeID = entity.ID };
                    entity.Specifications.Add(existingSpec);
                }
                existingSpec.TestMethodSpecificationID = incomingSpec.TestMethodSpecificationID;
                existingSpec.TestMethodSpecificationVersionID = incomingSpec.TestMethodSpecificationVersionID;
                existingSpec.ModifiedBy = _loggedInUser.EmployeeID;
                existingSpec.ModifiedOn = DateTime.UtcNow;

                SyncParameters(existingSpec, incomingSpec.Parameters, logs);
            }
        }

        private void SyncParameters(LabScopeSpecification spec, List<ScopeParameterDto> incoming, List<LabScopeChangeLog> logs)
        {
            var toRemove = spec.Parameters.Where(p => !incoming.Any(ip => ip.ID == p.ID && p.ID > 0)).ToList();
            foreach (var param in toRemove)
            {
                logs.Add(new LabScopeChangeLog
                {
                    LabScopeID = spec.LabScopeID,
                    ChangeType = "ParameterRemoved",
                    EntityName = $"ParamID: {param.ParameterID}",
                    OldValue = $"Limits: {param.LowerLimitValue} - {param.UpperLimitValue}",
                    ChangedBy = _loggedInUser.EmployeeID,
                    ChangedOn = DateTime.UtcNow
                });
                spec.Parameters.Remove(param);
            }

            foreach (var incomingParam in incoming)
            {
                var paramExists = _context.ParameterMasters.Any(p => p.ID == incomingParam.ParameterID && p.IsActive);
                if (!paramExists)
                    throw new ArgumentException($"Parameter {incomingParam.ParameterID} is not available.");
                var unitExists = _context.ParameterUnitMasters.Any(u => u.ID == incomingParam.ParameterUnitID && u.IsActive);
                if (!unitExists)
                    throw new ArgumentException($"Parameter unit {incomingParam.ParameterUnitID} is not available.");
                if (incomingParam.EquipmentIDs.Any())
                {
                    var equipCount = _context.EquipmentMasters.Count(e => incomingParam.EquipmentIDs.Contains(e.ID) && e.IsActive);
                    if (equipCount != incomingParam.EquipmentIDs.Distinct().Count())
                        throw new ArgumentException("One or more selected equipment records are not available.");
                }

                var existingParam = incomingParam.ID > 0
                    ? spec.Parameters.FirstOrDefault(p => p.ID == incomingParam.ID)
                    : null;
                if (existingParam == null)
                {
                    logs.Add(new LabScopeChangeLog
                    {
                        LabScopeID = spec.LabScopeID,
                        ChangeType = "ParameterAdded",
                        EntityName = $"ParamID: {incomingParam.ParameterID}",
                        NewValue = $"Limits: {incomingParam.LowerLimitValue} - {incomingParam.UpperLimitValue}, ISO: {incomingParam.IsUnderISO}",
                        ChangedBy = _loggedInUser.EmployeeID,
                        ChangedOn = DateTime.UtcNow
                    });
                    existingParam = new LabScopeSpecificationParameter { LabScopeSpecificationID = spec.ID };
                    spec.Parameters.Add(existingParam);
                }
                else if (existingParam.LowerLimitValue != incomingParam.LowerLimitValue
                    || existingParam.UpperLimitValue != incomingParam.UpperLimitValue)
                {
                    logs.Add(new LabScopeChangeLog
                    {
                        LabScopeID = spec.LabScopeID,
                        ChangeType = "LimitsChanged",
                        EntityName = $"ParamID: {existingParam.ParameterID}",
                        OldValue = $"{existingParam.LowerLimitValue} - {existingParam.UpperLimitValue}",
                        NewValue = $"{incomingParam.LowerLimitValue} - {incomingParam.UpperLimitValue}",
                        ChangedBy = _loggedInUser.EmployeeID,
                        ChangedOn = DateTime.UtcNow
                    });
                }

                existingParam.ParameterID = incomingParam.ParameterID;
                existingParam.ParameterUnitID = incomingParam.ParameterUnitID;
                existingParam.QualitativeQuantitative = string.IsNullOrWhiteSpace(incomingParam.ScopeType) ? "Quantitative" : incomingParam.ScopeType.Trim();
                existingParam.IsUnderISO = incomingParam.IsUnderISO;
                existingParam.LowerLimit = incomingParam.LowerOperator;
                existingParam.LowerLimitValue = incomingParam.LowerLimitValue;
                existingParam.UpperLimit = incomingParam.UpperOperator;
                existingParam.UpperLimitValue = incomingParam.UpperLimitValue;
                existingParam.ModifiedBy = _loggedInUser.EmployeeID;
                existingParam.ModifiedOn = DateTime.UtcNow;

                if (existingParam.Equipments == null)
                    existingParam.Equipments = new List<LabScopeSpecificationParameterEquipment>();
                var wanted = incomingParam.EquipmentIDs.Distinct().ToList();
                foreach (var gone in existingParam.Equipments.Where(e => !wanted.Contains(e.EquipmentID)).ToList())
                    existingParam.Equipments.Remove(gone);
                foreach (var id in wanted.Where(id => !existingParam.Equipments.Any(e => e.EquipmentID == id)))
                    existingParam.Equipments.Add(new LabScopeSpecificationParameterEquipment { EquipmentID = id });
            }
        }
    }
}
