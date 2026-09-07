using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IUniversalPlanService
    {
        Task<UniversalPlanWorkspaceDto> GetPlanWorkspaceAsync(long inwardId, long sampleId);
        Task<List<UniversalPlanSampleSummaryDto>> GetInwardSamplesOverviewAsync(long inwardId);
        Task<List<TestMethodVersionOptionDto>> GetMethodVersionsAsync(long methodId);
        Task<UniversalPlanPreviewResponseDto> PreviewTestConfigurationAsync(UniversalPlanPreviewRequestDto request);
        Task<UniversalPlanConfirmResultDto> SaveDraftPlanAsync(UniversalPlanSaveDto dto);
        Task<UniversalPlanValidationSummaryDto> ValidatePlanAsync(UniversalPlanSaveDto dto);
        Task<UniversalPlanConfirmResultDto> CreateTestGroupsAsync(UniversalPlanConfirmDto dto);
        Task<UniversalPlanCopyResultDto> CopyPlanToSamplesAsync(UniversalPlanCopyRequestDto dto);
    }
}
