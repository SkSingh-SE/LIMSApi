using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IProductFormRepository
    {
        Task AddProductForm(ProductFormMaster model);
        Task UpdateProductForm(ProductFormMaster model);
        Task DeleteProductForm(long id);
        Task<ProductFormMaster> GetProductFormById(long id);
        Task<PagedResponse<object>> GetAllProductForms(PageFilter filter);
        Task<List<DropdwonSelector>> GetProductFormDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
