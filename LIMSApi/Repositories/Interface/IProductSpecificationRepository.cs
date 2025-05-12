using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IProductSpecificationRepository
    {
        Task AddProductSpecification(ProductSpecification model);
        Task UpdateProductSpecification(ProductSpecification model);
        Task DeleteProductSpecification(long id);
        Task<ProductSpecification> GetProductSpecificationById(long id);
        Task<PagedResponse<object>> GetAllProductSpecifications(PageFilter filter);
        Task<PagedResponse<object>> GetAllCustomProductSpecifications(PageFilter filter);

        Task<List<DropdwonSelector>> GetProductSpecificationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
