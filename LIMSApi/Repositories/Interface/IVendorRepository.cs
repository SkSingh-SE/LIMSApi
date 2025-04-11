using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IVendorRepository
    {
        Task AddVendor(VendorMaster model);
        Task UpdateVendor(VendorMaster model);
        Task DeleteVendor(long id);
        Task<VendorMaster> GetVendorById(long id);
        Task<PagedResponse<object>> GetAllVendors(PageFilter filter);

        Task<List<DropdwonSelector>> GetVendorDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
