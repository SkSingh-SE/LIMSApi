using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IUniversalTestGroupService
    {
        Task<List<UniversalTestGroupListItemDto>> GetAllForCurrentUserAsync();
        Task<List<UniversalTestGroupListItemDto>> GetTestGroupsForSampleAsync(long sampleId);
        Task<List<UniversalTestGroupListItemDto>> GetTestGroupsForInwardAsync(long inwardId);
        Task<List<UniversalTestGroupListItemDto>> GetTestGroupsForPlanAsync(long sampleTestPlanId);
        Task<UniversalTestGroupDetailDto> GetTestGroupDetailAsync(long testGroupId);
        Task<EffectiveConfigurationDto> GetEffectiveConfigurationAsync(long testGroupId);
        Task<ValidationSummaryDto> GetValidationAsync(long testGroupId);
    }
}
