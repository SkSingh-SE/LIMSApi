using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class NablScopeValidationService : INablScopeValidationService
    {
        private readonly LIMSContext _db;

        public NablScopeValidationService(LIMSContext db)
        {
            _db = db;
        }

        public async Task<NablScopeCheckResult> CheckParameterScope(long laboratoryTestId, long parameterId, decimal value)
        {
            // Find LabScopeMaster for this laboratory test (active, with parameters)
            var labScope = await _db.LabScopeMasters
                .Include(ls => ls.Specifications)
                    .ThenInclude(s => s.Parameters)
                .Where(ls => ls.LaboratoryTestID == laboratoryTestId && ls.IsActive)
                .OrderByDescending(ls => ls.Specifications.SelectMany(s => s.Parameters).Count())
                .FirstOrDefaultAsync();

            if (labScope == null)
            {
                return new NablScopeCheckResult
                {
                    ParameterId = parameterId,
                    Value = value,
                    ScopeStatus = "NotAccredited"
                };
            }

            // Search ALL specifications — if value is within ANY spec's range, it's in scope
            // Track the widest range across all specs for display
            NablScopeCheckResult? bestMatch = null;
            decimal? widestLower = null;
            decimal? widestUpper = null;

            foreach (var spec in labScope.Specifications)
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

        public async Task<List<NablScopeCheckResult>> CheckAllParameters(long testResultHeaderId)
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
                    var result = await CheckParameterScope(header.LaboratoryTestID, param.ParameterID, param.Value.Value);
                    result.ParameterName = param.ParameterName;
                    results.Add(result);
                }
                else
                {
                    // No value entered yet — still check if this test+parameter combo
                    // exists in LabScopeMaster so we show scope coverage correctly
                    bool scopeExists = await CheckParameterScopeExists(header.LaboratoryTestID, param.ParameterID);
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

            // Search NablMeasurementUncertainty by parameter name and test method
            var uncertainty = await _db.NablMeasurementUncertainties
                .Where(u => u.IsActive &&
                    u.TestParameter != null &&
                    u.TestParameter.ToLower().Contains(paramMaster.Name.ToLower()))
                .FirstOrDefaultAsync();

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
        public async Task<bool> CheckParameterScopeExists(long laboratoryTestId, long parameterId)
        {
            return await _db.LabScopeMasters
                .Where(ls => ls.LaboratoryTestID == laboratoryTestId && ls.IsActive)
                .SelectMany(ls => ls.Specifications)
                .SelectMany(s => s.Parameters)
                .AnyAsync(p => p.ParameterID == parameterId);
        }

        private static decimal? ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (decimal.TryParse(value.Trim(), out var result)) return result;
            return null;
        }
    }
}
