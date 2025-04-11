using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISupplierRepository
    {
        Task AddSupplier(SupplierMaster model);
        Task UpdateSupplier(SupplierMaster model);
        Task DeleteSupplier(long id);
        Task<SupplierMaster> GetSupplierById(long id);
        Task<PagedResponse<object>> GetAllSuppliers(PageFilter filter);

        Task<List<DropdwonSelector>> GetSupplierDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
