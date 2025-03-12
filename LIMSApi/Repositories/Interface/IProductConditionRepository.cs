using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IProductConditionRepository
    {
        Task AddProductCondition(ProductConditionMaster model);
        Task UpdateProductCondition(ProductConditionMaster model);
        Task DeleteProductCondition(long id);
        Task<ProductConditionMaster> GetProductConditionById(long id);
        Task<PagedResponse<object>> GetAllProductConditions(PageFilter filter);

        Task<List<DropdwonSelector>> GetProductConditionDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
