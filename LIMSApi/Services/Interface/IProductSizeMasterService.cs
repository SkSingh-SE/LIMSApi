using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IProductSizeMasterService
    {
        Task CreateProductSize(ProductSizeMaster model);
        Task ModifyProductSize(ProductSizeMaster model);
        Task RemoveProductSize(long id);
        Task<ProductSizeMaster> GetProductSizeDetails(long id);
        Task<PagedResponse<object>> FetchProductSizeList(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductSizeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
