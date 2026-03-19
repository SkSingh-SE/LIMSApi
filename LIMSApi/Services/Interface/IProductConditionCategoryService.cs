using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IProductConditionCategoryService
    {
        Task CreateProductConditionCategory(ProductConditionCategoryMaster model);
        Task ModifyProductConditionCategory(ProductConditionCategoryMaster model);
        Task RemoveProductConditionCategory(long id);
        Task<ProductConditionCategoryMaster> GetProductConditionCategoryDetails(long id);
        Task<PagedResponse<object>> FetchProductConditionCategoryList(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductConditionCategoryDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
