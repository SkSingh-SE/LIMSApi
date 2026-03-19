using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IProductFormService
    {
        Task CreateProductForm(ProductFormMaster model);
        Task ModifyProductForm(ProductFormMaster model);
        Task RemoveProductForm(long id);
        Task<ProductFormMaster> GetProductFormDetails(long id);
        Task<PagedResponse<object>> FetchProductFormList(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductFormDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
