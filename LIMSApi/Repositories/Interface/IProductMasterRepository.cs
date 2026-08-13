using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IProductMasterRepository
    {
        Task Add(ProductMaster model);
        Task Update(ProductMaster model);
        Task Delete(ProductMaster model);
        Task<ProductMaster?> GetById(long id);
        Task<ProductMaster?> GetDetailsById(long id);
        Task<PagedResponse<object>> GetAll(PageFilter filter);
        Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20, long metalId = 0);
        Task<bool> ExistsByName(string productName);
        Task<bool> ExistsByNameAndNotId(string productName, long id);
    }
}
