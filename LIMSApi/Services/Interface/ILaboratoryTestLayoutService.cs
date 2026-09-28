using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface ILaboratoryTestLayoutService
    {
        Task<List<LaboratoryTestLayoutItemDto>> GetLayoutsAsync(long testId);
        Task<LaboratoryTestLayoutItemDto> AddLayoutAsync(long testId, LaboratoryTestLayoutCreateDto dto);
        Task<LaboratoryTestLayoutItemDto> UpdateLayoutAsync(long testId, long assignmentId, LaboratoryTestLayoutUpdateDto dto);
        Task<bool> ToggleLayoutStatusAsync(long testId, long assignmentId);
        Task<EffectiveLayoutResponseDto> ResolveEffectiveLayoutAsync(long testId, long? methodId, long? versionId);
    }
}
