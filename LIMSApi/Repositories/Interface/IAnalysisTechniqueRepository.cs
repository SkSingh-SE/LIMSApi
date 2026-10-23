using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IAnalysisTechniqueRepository
    {
        Task AddAnalysisTechnique(AnalysisTechniqueMaster model);
        Task UpdateAnalysisTechnique(AnalysisTechniqueMaster model);
        Task DeleteAnalysisTechnique(AnalysisTechniqueMaster model);
        Task<AnalysisTechniqueMaster?> GetAnalysisTechniqueById(long id);
        Task<PagedResponse<object>> GetAllAnalysisTechniques(PageFilter filter);
        Task<List<DropdwonSelector>> GetAnalysisTechniqueDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
        Task<bool> ExistsByCode(string code);
        Task<bool> ExistsByCodeAndNotId(string code, long id);
    }
}
