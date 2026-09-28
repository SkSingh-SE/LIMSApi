using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class NablScopeValidationService : INablScopeValidationService
    {
        private readonly LIMSContext _db;
        private readonly LoggedInUserDTO _loggedInUser;

        public NablScopeValidationService(LIMSContext db)
        {
            _db = db;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public Task<NablScopeCheckResult> CheckParameterScope(long laboratoryTestId, long parameterId, decimal value)
            => CheckParameterScope(laboratoryTestId, parameterId, value, null, null);

        private static bool IsScopeEffective(LabScopeMaster labScope, long? branchId, DateTime referenceDateUtc)
        {
            if (branchId.HasValue && labScope.BranchID.HasValue && labScope.BranchID.Value != branchId.Value)
                return false;
            if (labScope.ValidFrom.HasValue && referenceDateUtc < labScope.ValidFrom.Value)
                return false;
            if (labScope.ValidUntil.HasValue && referenceDateUtc > labScope.ValidUntil.Value)
                return false;
            return true;
        }

        public async Task<NablScopeCheckResult> CheckParameterScope(long laboratoryTestId, long parameterId, decimal value, long? branchId, DateTime? referenceDateUtc, long? testMethodId = null, long? testMethodVersionId = null)
        {
            var refDate = referenceDateUtc ?? DateTime.UtcNow;
            
            var query = _db.LabScopeMasters
                .Include(ls => ls.Specifications)
                    .ThenInclude(s => s.Parameters)
                .Where(ls => ls.LaboratoryTestID == laboratoryTestId && ls.IsActive
                    && ls.CompanyCode == _loggedInUser.CompanyCode);

            var candidates = await query.ToListAsync();
            candidates = candidates.Where(ls => IsScopeEffective(ls, branchId, refDate)).ToList();

            if (!candidates.Any())
            {
                // Hierarchical fallback: if laboratoryTestId is a SubGroup or AnalysisType, check parent Master LaboratoryTestID
                var subGroup = await _db.LaboratoryTestSubGroups.FirstOrDefaultAsync(sg => sg.ID == laboratoryTestId);
                if (subGroup != null)
                {
                    candidates = await _db.LabScopeMasters
                        .Include(ls => ls.Specifications)
                            .ThenInclude(s => s.Parameters)
                        .Where(ls => ls.LaboratoryTestID == subGroup.LaboratoryTestID && ls.IsActive
                            && ls.CompanyCode == _loggedInUser.CompanyCode)
                        .ToListAsync();
                    candidates = candidates.Where(ls => IsScopeEffective(ls, branchId, refDate)).ToList();
                }
                else
                {
                    var analysisType = await _db.LaboratoryTestAnalysisTypes.Include(at => at.SubGroup).FirstOrDefaultAsync(at => at.ID == laboratoryTestId);
                    if (analysisType?.SubGroup != null)
                    {
                        candidates = await _db.LabScopeMasters
                            .Include(ls => ls.Specifications)
                                .ThenInclude(s => s.Parameters)
                            .Where(ls => ls.LaboratoryTestID == analysisType.SubGroup.LaboratoryTestID && ls.IsActive
                                && ls.CompanyCode == _loggedInUser.CompanyCode)
                            .ToListAsync();
                        candidates = candidates.Where(ls => IsScopeEffective(ls, branchId, refDate)).ToList();
                    }
                }
            }

            if (!candidates.Any())
            {
                return new NablScopeCheckResult
                {
                    ParameterId = parameterId,
                    Value = value,
                    ScopeStatus = "NotAccredited"
                };
            }

            // Deterministic Scope Precedence
            var branchSpecific = candidates.Where(ls => ls.BranchID.HasValue && ls.BranchID.Value == branchId).ToList();
            var globalSpecific = candidates.Where(ls => !ls.BranchID.HasValue).ToList();

            List<LabScopeMaster> effectiveCandidates = branchSpecific.Any() ? branchSpecific : globalSpecific;

            if (effectiveCandidates.Count > 1)
            {
                throw new InvalidOperationException($"Ambiguous Lab Scope: Found {effectiveCandidates.Count} active effective scopes for LaboratoryTestID {laboratoryTestId}. Please deactivate overlapping scopes.");
            }

            var labScope = effectiveCandidates.FirstOrDefault();

            if (labScope == null)
            {
                return new NablScopeCheckResult
                {
                    ParameterId = parameterId,
                    Value = value,
                    ScopeStatus = "NotAccredited"
                };
            }

            var specsQuery = labScope.Specifications.AsEnumerable();

            if (testMethodId.HasValue && testMethodId.Value > 0)
            {
                specsQuery = specsQuery.Where(s => s.TestMethodSpecificationID == testMethodId.Value);
            }

            if (testMethodVersionId.HasValue && testMethodVersionId.Value > 0)
            {
                specsQuery = specsQuery.Where(s => s.TestMethodSpecificationVersionID == testMethodVersionId.Value || !s.TestMethodSpecificationVersionID.HasValue);
            }

            // G7: Sort specs so version-matched ones are checked first (prefer specific version over generic)
            var sortedSpecs = specsQuery
                .OrderByDescending(s => s.TestMethodSpecificationVersionID.HasValue)
                .ToList();

            // Search ALL specifications — if value is within ANY spec's range, it's in scope
            // Track the widest range across all specs for display
            NablScopeCheckResult? bestMatch = null;
            decimal? widestLower = null;
            decimal? widestUpper = null;

            foreach (var spec in sortedSpecs)
            {
                var scopeParam = spec.Parameters.FirstOrDefault(p => p.ParameterID == parameterId);
                if (scopeParam == null) continue;

                decimal? lower = scopeParam.LowerLimitValue ?? ParseDecimal(scopeParam.LowerLimit);
                decimal? upper = scopeParam.UpperLimitValue ?? ParseDecimal(scopeParam.UpperLimit);

                // Track widest range for display
                if (widestLower == null || (lower.HasValue && lower < widestLower)) widestLower = lower;
                if (widestUpper == null || (upper.HasValue && upper > widestUpper)) widestUpper = upper;

                // If both limits are null, treat as accredited but unchecked
                if (!lower.HasValue && !upper.HasValue)
                {
                    return new NablScopeCheckResult
                    {
                        ParameterId = parameterId,
                        Value = value,
                        ScopeStatus = "WithinScope",
                        IsUnderISO = scopeParam.IsUnderISO,
                        LabScopeSpecParamId = scopeParam.ID
                    };
                }

                bool withinScope = true;
                if (lower.HasValue)
                    withinScope = withinScope && (scopeParam.LowerLimit == ">" ? value > lower.Value : value >= lower.Value);
                if (upper.HasValue)
                    withinScope = withinScope && (scopeParam.UpperLimit == "<" ? value < upper.Value : value <= upper.Value);

                // If within THIS spec's range, immediately return success
                if (withinScope)
                {
                    return new NablScopeCheckResult
                    {
                        ParameterId = parameterId,
                        Value = value,
                        NablLowerLimit = lower,
                        NablUpperLimit = upper,
                        ScopeStatus = "WithinScope",
                        IsUnderISO = scopeParam.IsUnderISO,
                        LabScopeSpecParamId = scopeParam.ID
                    };
                }

                // Keep track of best match for "OutsideScope" result
                bestMatch = new NablScopeCheckResult
                {
                    ParameterId = parameterId,
                    Value = value,
                    NablLowerLimit = widestLower,
                    NablUpperLimit = widestUpper,
                    ScopeStatus = "OutsideScope",
                    IsUnderISO = scopeParam.IsUnderISO,
                    LabScopeSpecParamId = scopeParam.ID
                };
            }

            // Parameter was found in scope but value is outside ALL specs' ranges
            if (bestMatch != null) return bestMatch;

            return new NablScopeCheckResult
            {
                ParameterId = parameterId,
                Value = value,
                ScopeStatus = "NotAccredited"
            };
        }

        public Task<List<NablScopeCheckResult>> CheckAllParameters(long testResultHeaderId)
            => CheckAllParameters(testResultHeaderId, null, null);

        public async Task<List<NablScopeCheckResult>> CheckAllParameters(long testResultHeaderId, long? branchId, DateTime? referenceDateUtc)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == testResultHeaderId);

            if (header == null) return new List<NablScopeCheckResult>();

            var results = new List<NablScopeCheckResult>();

            foreach (var param in header.Parameters)
            {
                if (param.Value.HasValue)
                {
                    var result = await CheckParameterScope(header.LaboratoryTestID, param.ParameterID, param.Value.Value, branchId, referenceDateUtc);
                    result.ParameterName = param.ParameterName;
                    results.Add(result);
                }
                else
                {
                    // No value entered yet — still check if this test+parameter combo
                    // exists in LabScopeMaster so we show scope coverage correctly
                    bool scopeExists = await CheckParameterScopeExists(header.LaboratoryTestID, param.ParameterID, branchId, referenceDateUtc);
                    results.Add(new NablScopeCheckResult
                    {
                        ParameterId = param.ParameterID,
                        ParameterName = param.ParameterName,
                        Value = null,
                        ScopeStatus = scopeExists ? "WithinScope" : "NotAccredited"
                    });
                }
            }

            return results;
        }

        public async Task<UncertaintyResult?> GetUncertaintyForParameter(long laboratoryTestId, long parameterId)
        {
            // Get parameter name from ParameterMaster
            var paramMaster = await _db.ParameterMasters.FindAsync(parameterId);
            if (paramMaster == null) return null;

            // Get laboratory test name/method
            var labTest = await _db.LaboratoryTests.FindAsync(laboratoryTestId);
            if (labTest == null) return null;

            // G2: Search by exact match first, then Contains fallback
            var paramName = paramMaster.Name.Trim();
            // Rely on SQL Server CI collation — no LOWER() on columns (keeps indexes sargable).
            var uncertainty = await _db.NablMeasurementUncertainties
                .Where(u => u.IsActive && u.TestParameter != null
                    && u.TestParameter.Trim() == paramName)
                .FirstOrDefaultAsync();

            // Fallback to Contains only if exact match not found
            if (uncertainty == null)
            {
                uncertainty = await _db.NablMeasurementUncertainties
                    .Where(u => u.IsActive && u.TestParameter != null
                        && u.TestParameter.Contains(paramName))
                    .FirstOrDefaultAsync();
            }

            if (uncertainty == null) return null;

            return new UncertaintyResult
            {
                ParameterId = parameterId,
                ParameterName = paramMaster.Name,
                ExpandedUncertainty = uncertainty.ExpandedUncertainty,
                CombinedUncertainty = uncertainty.CombinedUncertainty,
                CoverageFactor = uncertainty.CoverageFactor,
                ConfidenceLevel = uncertainty.ConfidenceLevel?.ToString("F1") + "%",
                NablMeasurementUncertaintyId = uncertainty.ID
            };
        }

        public async Task<List<UncertaintyResult>> GetAllUncertainties(long testResultHeaderId)
        {
            var header = await _db.TestResultHeaders
                .Include(h => h.Parameters)
                .FirstOrDefaultAsync(h => h.ID == testResultHeaderId);

            if (header == null) return new List<UncertaintyResult>();

            var results = new List<UncertaintyResult>();

            foreach (var param in header.Parameters)
            {
                var uncertainty = await GetUncertaintyForParameter(header.LaboratoryTestID, param.ParameterID);
                if (uncertainty != null)
                {
                    results.Add(uncertainty);
                }
            }

            return results;
        }

        /// <summary>
        /// Checks whether a LabScopeSpecificationParameter record exists for the given
        /// laboratoryTestId + parameterId combination, without checking value ranges.
        /// </summary>
        public Task<bool> CheckParameterScopeExists(long laboratoryTestId, long parameterId)
            => CheckParameterScopeExists(laboratoryTestId, parameterId, null, null);

        public async Task<bool> CheckParameterScopeExists(long laboratoryTestId, long parameterId, long? branchId, DateTime? referenceDateUtc, long? testMethodId = null, long? testMethodVersionId = null)
        {
            var refDate = referenceDateUtc ?? DateTime.UtcNow;
            var query = _db.LabScopeMasters
                .Where(ls => ls.LaboratoryTestID == laboratoryTestId && ls.IsActive
                    && ls.CompanyCode == _loggedInUser.CompanyCode
                    && (!branchId.HasValue || ls.BranchID == null || ls.BranchID == branchId.Value)
                    && (ls.ValidFrom == null || refDate >= ls.ValidFrom)
                    && (ls.ValidUntil == null || refDate <= ls.ValidUntil));

            var candidates = await query.ToListAsync();

            if (!candidates.Any())
            {
                var subGroup = await _db.LaboratoryTestSubGroups.FirstOrDefaultAsync(sg => sg.ID == laboratoryTestId);
                long? parentTestId = subGroup?.LaboratoryTestID;
                if (!parentTestId.HasValue)
                {
                    var analysisType = await _db.LaboratoryTestAnalysisTypes.Include(at => at.SubGroup).FirstOrDefaultAsync(at => at.ID == laboratoryTestId);
                    parentTestId = analysisType?.SubGroup?.LaboratoryTestID;
                }
                if (parentTestId.HasValue)
                {
                    candidates = await _db.LabScopeMasters
                        .Where(ls => ls.LaboratoryTestID == parentTestId.Value && ls.IsActive
                            && ls.CompanyCode == _loggedInUser.CompanyCode
                            && (!branchId.HasValue || ls.BranchID == null || ls.BranchID == branchId.Value)
                            && (ls.ValidFrom == null || refDate >= ls.ValidFrom)
                            && (ls.ValidUntil == null || refDate <= ls.ValidUntil))
                        .ToListAsync();
                }
            }

            if (!candidates.Any()) return false;

            // Deterministic Scope Precedence
            var branchSpecific = candidates.Where(ls => ls.BranchID.HasValue && ls.BranchID.Value == branchId).ToList();
            var globalSpecific = candidates.Where(ls => !ls.BranchID.HasValue).ToList();

            List<LabScopeMaster> effectiveCandidates = branchSpecific.Any() ? branchSpecific : globalSpecific;

            if (effectiveCandidates.Count > 1) return false; // Ambiguous

            var labScope = effectiveCandidates.FirstOrDefault();
            if (labScope == null) return false;

            var specsQuery = _db.LabScopeSpecifications.Where(s => s.LabScopeID == labScope.ID);

            if (testMethodId.HasValue && testMethodId.Value > 0)
            {
                specsQuery = specsQuery.Where(s => s.TestMethodSpecificationID == testMethodId.Value);
            }

            if (testMethodVersionId.HasValue && testMethodVersionId.Value > 0)
            {
                specsQuery = specsQuery.Where(s => s.TestMethodSpecificationVersionID == testMethodVersionId.Value || s.TestMethodSpecificationVersionID == null);
            }

            var exists = await specsQuery
                .SelectMany(s => s.Parameters)
                .AnyAsync(p => p.ParameterID == parameterId);

            return exists;
        }

        private static decimal? ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (decimal.TryParse(value.Trim(), out var result)) return result;
            return null;
        }
    }
}
