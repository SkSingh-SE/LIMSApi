using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IParameterCategoryService
    {
        Task CreateParameterCategory(ParameterCategoryMaster model);
        Task ModifyParameterCategory(ParameterCategoryMaster model);
        Task RemoveParameterCategory(long id);
        Task<ParameterCategoryMaster> GetParameterCategoryDetails(long id);
        Task<PagedResponse<object>> FetchParameterCategoryList(PageFilter filter);
        Task<List<DropdwonSelector>> GetParameterCategoryDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
