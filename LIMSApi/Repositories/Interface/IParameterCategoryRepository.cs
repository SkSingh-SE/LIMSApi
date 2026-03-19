using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IParameterCategoryRepository
    {
        Task AddParameterCategory(ParameterCategoryMaster model);
        Task UpdateParameterCategory(ParameterCategoryMaster model);
        Task DeleteParameterCategory(long id);
        Task<ParameterCategoryMaster> GetParameterCategoryById(long id);
        Task<PagedResponse<object>> GetAllParameterCategories(PageFilter filter);
        Task<List<DropdwonSelector>> GetParameterCategoryDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
