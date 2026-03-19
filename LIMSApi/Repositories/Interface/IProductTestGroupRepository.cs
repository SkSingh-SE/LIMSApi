using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IProductTestGroupRepository
    {
        Task AddProductTestGroup(ProductTestGroup model);
        Task UpdateProductTestGroup(ProductTestGroup model);
        Task DeleteProductTestGroup(long id);
        Task<ProductTestGroup?> GetProductTestGroupById(long id);
        Task<PagedResponse<object>> GetAllProductTestGroups(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductTestGroupDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<object>> GetByProductSpecificationId(long productSpecId);
    }
}
