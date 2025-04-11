using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICustomerTypeRepository
    {
        Task AddCustomerType(CustomerTypeMaster model);
        Task UpdateCustomerType(CustomerTypeMaster model);
        Task DeleteCustomerType(CustomerTypeMaster model);
        Task<CustomerTypeMaster> GetCustomerTypeById(long id);
        Task<PagedResponse<object>> GetAllCustomerTypes(PageFilter filter);

        Task<List<DropdwonSelector>> GetCustomerTypeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
