using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISupplierService
    {
        Task CreateSupplier(SupplierMaster model);
        Task ModifySupplier(SupplierMaster model);
        Task RemoveSupplier(long id);
        Task<SupplierMaster> GetSupplierDetails(long id);
        Task<PagedResponse<object>> FetchSupplierList(PageFilter filter);

        Task<List<DropdwonSelector>> GetSupplierDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
