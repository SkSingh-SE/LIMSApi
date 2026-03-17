using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface ICustomerPurchaseOrderService
    {
        Task<PagedResponse<object>> GetAll(PageFilter filter);
        Task<CustomerPurchaseOrderDto?> GetById(long id);
        Task<CustomerPurchaseOrderDto> Create(CreateCustomerPurchaseOrderDto dto);
        Task<CustomerPurchaseOrderDto> Update(long id, CreateCustomerPurchaseOrderDto dto);
        Task<bool> Delete(long id);
        Task<List<DropdwonSelector>> GetDropdown(long customerId, string? searchTerm, int pageNo, int pageSize);
        Task<POUtilizationDto?> GetUtilization(long id);
    }
}
