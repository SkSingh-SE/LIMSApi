using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IProductSizeMasterRepository
    {
        Task AddProductSize(ProductSizeMaster model);
        Task UpdateProductSize(ProductSizeMaster model);
        Task DeleteProductSize(ProductSizeMaster model);
        Task<ProductSizeMaster?> GetProductSizeById(long id);
        Task<PagedResponse<object>> GetAllProductSizes(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductSizeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string displayName);
        Task<bool> ExistsByNameAndNotId(string displayName, long id);
    }
}
