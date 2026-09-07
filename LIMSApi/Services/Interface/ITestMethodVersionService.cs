using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface ITestMethodVersionService
    {
        Task<PagedResponse<TestMethodVersionListItemDto>> GetPagedVersionsAsync(TestMethodVersionFilterDto filter);
        Task<TestMethodVersionDetailDto> GetVersionDetailsAsync(long id);
        Task<TestMethodVersionListItemDto> CreateVersionAsync(TestMethodVersionCreateDto dto);
        Task<TestMethodVersionListItemDto> UpdateVersionAsync(long id, TestMethodVersionUpdateDto dto);
        Task<bool> SetDefaultVersionAsync(long id);
        Task<List<TestMethodVersionParameterDto>> GetVersionParametersAsync(long id);
        Task<bool> SaveVersionParametersAsync(long id, SaveVersionParametersDto dto);
        Task<List<TestMethodVersionDropdownDto>> GetDropdownByMethodIdAsync(long methodId);
    }
}
