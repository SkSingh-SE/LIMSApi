using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICustomerRepository
    {
        Task AddCustomer(Customer model);
        Task UpdateCustomer(Customer model);
        Task DeleteCustomer(Customer model);
        Task<Customer> GetCustomerById(long id);
        Task<PagedResponse<object>> GetAllCustomers(PageFilter filter);

        Task<List<DropdwonSelector>> GetCustomerDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
