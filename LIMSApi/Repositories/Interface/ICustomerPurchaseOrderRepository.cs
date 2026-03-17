using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICustomerPurchaseOrderRepository
    {
        Task<CustomerPurchaseOrder> Add(CustomerPurchaseOrder model);
        Task<CustomerPurchaseOrder> Update(CustomerPurchaseOrder model);
        Task<CustomerPurchaseOrder> Delete(long id);
        Task<CustomerPurchaseOrder?> GetById(long id);
        Task<PagedResponse<object>> GetAll(PageFilter filter);
        Task<List<DropdwonSelector>> GetDropdown(long customerId, string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByPONumber(string poNumber, long customerId);
        Task<bool> ExistsByPONumberAndNotId(string poNumber, long customerId, long id);
    }
}
