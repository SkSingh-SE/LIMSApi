using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Helpers
{
    public interface IEffectiveConfigurationResolver
    {
        Task<UniversalPlanPreviewResponseDto> ResolveEffectiveConfigurationAsync(UniversalPlanPreviewRequestDto request);
        Task<TestExecutionConfigSnapshotDto> ResolveSnapshotDtoAsync(UniversalTestGroup utg);
    }
}
