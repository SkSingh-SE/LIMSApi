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
            // Find LabScopeMaster for this laboratory test
            var labScope = await _db.LabScopeMasters
                .Include(ls => ls.Specifications)
                    .ThenInclude(s => s.Parameters)
                .FirstOrDefaultAsync(ls => ls.LaboratoryTestID == laboratoryTestId);

            if (labScope == null)
            {
                return new NablScopeCheckResult
                {
                    ParameterId = parameterId,
                    Value = value,
                    ScopeStatus = "NotAccredited"
                };
            }

            // Search across all specifications for the matching parameter
            foreach (var spec in labScope.Specifications)
            {
                var scopeParam = spec.Parameters.FirstOrDefault(p => p.ParameterID == parameterId);
                if (scopeParam != null)
                {
                    decimal? lower = ParseDecimal(scopeParam.LowerLimit);
                    decimal? upper = ParseDecimal(scopeParam.UpperLimit);

                    bool withinScope = true;
                    if (lower.HasValue) withinScope = withinScope && value >= lower.Value;
                    if (upper.HasValue) withinScope = withinScope && value <= upper.Value;

                    // If both limits are null, we can't check scope — treat as accredited but unchecked
                    if (!lower.HasValue && !upper.HasValue)
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

                    return new NablScopeCheckResult
                    {
                        ParameterId = parameterId,
                        Value = value,
                        NablLowerLimit = lower,
                        NablUpperLimit = upper,
                        ScopeStatus = withinScope ? "WithinScope" : "OutsideScope",
                        IsUnderISO = scopeParam.IsUnderISO,
                        LabScopeSpecParamId = scopeParam.ID
                    };
                }
            }

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
                    results.Add(new NablScopeCheckResult
                    {
                        ParameterId = param.ParameterID,
                        ParameterName = param.ParameterName,
                        Value = null,
                        ScopeStatus = "NotAccredited"
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

        private static decimal? ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (decimal.TryParse(value.Trim(), out var result)) return result;
            return null;
        }
    }
}
