using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IVendorService
    {
        Task CreateVendor(VendorMaster model);
        Task ModifyVendor(VendorMaster model);
        Task RemoveVendor(long id);
        Task<VendorMaster> GetVendorDetails(long id);
        Task<PagedResponse<object>> FetchVendorList(PageFilter filter);

        Task<List<DropdwonSelector>> GetVendorDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
