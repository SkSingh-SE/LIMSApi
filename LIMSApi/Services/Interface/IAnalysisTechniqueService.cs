using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IAnalysisTechniqueService
    {
        Task CreateAnalysisTechnique(AnalysisTechniqueMaster model);
        Task ModifyAnalysisTechnique(AnalysisTechniqueMaster model);
        Task RemoveAnalysisTechnique(long id);
        Task<AnalysisTechniqueMaster> GetAnalysisTechniqueDetails(long id);
        Task<PagedResponse<object>> FetchAnalysisTechniqueList(PageFilter filter);
        Task<List<DropdwonSelector>> GetAnalysisTechniqueDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
