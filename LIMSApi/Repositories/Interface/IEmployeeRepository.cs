using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task AddEmployee(EmployeeMaster model);
        Task UpdateEmployee(EmployeeMaster model);
        Task DeleteEmployee(long id);
        Task<EmployeeMaster> GetEmployeeById(long id);
        Task<PagedResponse<object>> GetAllEmployees(PageFilter filter);

        Task<List<DropdwonSelector>> GetEmployeeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByEmail(string email);
        Task<bool> ExistsByEmailAndNotId(string email, long id);
    }
}
