using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IProductConditionCategoryRepository
    {
        Task AddProductConditionCategory(ProductConditionCategoryMaster model);
        Task UpdateProductConditionCategory(ProductConditionCategoryMaster model);
        Task DeleteProductConditionCategory(long id);
        Task<ProductConditionCategoryMaster> GetProductConditionCategoryById(long id);
        Task<PagedResponse<object>> GetAllProductConditionCategorys(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductConditionCategoryDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
