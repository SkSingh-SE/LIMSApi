using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface INablScopeValidationService
    {
        Task<NablScopeCheckResult> CheckParameterScope(long laboratoryTestId, long parameterId, decimal value);
        Task<List<NablScopeCheckResult>> CheckAllParameters(long testResultHeaderId);
        Task<bool> CheckParameterScopeExists(long laboratoryTestId, long parameterId);
        Task<UncertaintyResult?> GetUncertaintyForParameter(long laboratoryTestId, long parameterId);
        Task<List<UncertaintyResult>> GetAllUncertainties(long testResultHeaderId);
    }
}
