using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IAnalysisTechniqueService
    {
        Task CreateAnalysisTechnique(AnalysisTechniqueCreateDto dto);
        Task ModifyAnalysisTechnique(AnalysisTechniqueUpdateDto dto);
        Task<bool> ToggleAnalysisTechniqueStatus(long id);
        Task<AnalysisTechniqueDetailDto> GetAnalysisTechniqueDetails(long id);
        Task<PagedResponse<object>> FetchAnalysisTechniqueList(PageFilter filter);
        Task<List<DropdwonSelector>> GetAnalysisTechniqueDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
