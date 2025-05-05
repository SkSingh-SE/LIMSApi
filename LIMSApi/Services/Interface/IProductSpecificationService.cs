using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IProductSpecificationService
    {
        Task CreateProductSpecification(ProductSpecification model);
        Task ModifyProductSpecification(ProductSpecification model);
        Task RemoveProductSpecification(long id);
        Task<ProductSpecification> GetProductSpecificationDetails(long id);
        Task<PagedResponse<object>> FetchProductSpecificationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetProductSpecificationDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
