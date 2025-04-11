using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICustomerTypeService
    {
        Task CreateCustomerType(CustomerTypeMaster model);
        Task ModifyCustomerType(CustomerTypeMaster model);
        Task RemoveCustomerType(long id);
        Task<CustomerTypeMaster> GetCustomerTypeDetails(long id);
        Task<PagedResponse<object>> FetchCustomerTypeList(PageFilter filter);

        Task<List<DropdwonSelector>> GetCustomerTypeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
