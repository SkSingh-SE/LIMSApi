using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IEmployeeService
    {
        Task CreateEmployee(EmployeeMaster model);
        Task ModifyEmployee(EmployeeMaster model);
        Task RemoveEmployee(long id);
        Task<EmployeeMaster> GetEmployeeDetails(long id);
        Task<PagedResponse<object>> FetchEmployeeList(PageFilter filter);

        Task<List<DropdwonSelector>> GetEmployeeDropdown(string? searchTerm, int pageNo, int pageSize);

        // Qualification Methods
        Task AddEmployeeQualification(EmployeeQualification qualification);
        Task UpdateEmployeeQualification(EmployeeQualification qualification);
        Task DeleteEmployeeQualification(long id);
        Task<List<EmployeeQualification>> GetEmployeeQualifications(long employeeId);

        // Document Methods
        Task AddEmployeeDocument(EmployeeDocument document);
        Task DeleteEmployeeDocument(long id);
        Task<EmployeeDocument?> GetEmployeeDocumentById(long id);
        Task<List<EmployeeDocument>> GetEmployeeDocuments(long employeeId);
    }
}
