using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface INablScopeValidationService
    {
        Task<NablScopeCheckResult> CheckParameterScope(long laboratoryTestId, long parameterId, decimal value);
        Task<NablScopeCheckResult> CheckParameterScope(long laboratoryTestId, long parameterId, decimal value, long? branchId, DateTime? referenceDateUtc, long? testMethodId = null, long? testMethodVersionId = null);
        Task<List<NablScopeCheckResult>> CheckAllParameters(long testResultHeaderId);
        Task<List<NablScopeCheckResult>> CheckAllParameters(long testResultHeaderId, long? branchId, DateTime? referenceDateUtc);
        Task<bool> CheckParameterScopeExists(long laboratoryTestId, long parameterId);
        Task<bool> CheckParameterScopeExists(long laboratoryTestId, long parameterId, long? branchId, DateTime? referenceDateUtc, long? testMethodId = null, long? testMethodVersionId = null);
        Task<UncertaintyResult?> GetUncertaintyForParameter(long laboratoryTestId, long parameterId);
        Task<List<UncertaintyResult>> GetAllUncertainties(long testResultHeaderId);
    }
}
