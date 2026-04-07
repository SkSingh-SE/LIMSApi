using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface ISamplePreparationService
    {
        Task<PagedResponse<SamplePreparationListDto>> GetAllAsync(PageFilter filter);
        Task<SamplePreparationDetailDto> GetByIdAsync(long id);
        Task<SamplePreparationDetailDto> GetOrCreateBySampleAsync(long sampleId);
        Task<SamplePreparationDetailDto> CreateAsync(SamplePreparationCreateDto dto);
        Task<SamplePreparationDetailDto> UpdateAsync(SamplePreparationUpdateDto dto);
        Task UpdateStatusAsync(long id, SamplePreparationStatusDto dto);
        Task RefreshChargeTotalsAsync(long sampleId);
    }
}
