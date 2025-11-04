using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IMaterialTestMappingService
    {
        Task<PagedResponse<object>> GetAllMappings(PageFilter filter);
        Task<MaterialTestMapping?> GetMapping(long id);
        Task CreateMapping(MaterialTestMapping dto);
        Task UpdateMapping(MaterialTestMapping dto);
        Task<List<DropdwonSelector>> GetSuggestedTestsAsync(TestSuggestionRequest request);
    }
}
