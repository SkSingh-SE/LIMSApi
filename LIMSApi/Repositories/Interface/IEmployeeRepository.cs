using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task<EmployeeMaster> AddEmployee(EmployeeMaster model);
        Task UpdateEmployee(EmployeeMaster model);
        Task DeleteEmployee(long id);
        Task<EmployeeMaster> GetEmployeeById(long id);
        Task<PagedResponse<object>> GetAllEmployees(PageFilter filter);

        Task<List<DropdwonSelector>> GetEmployeeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByEmail(string email);
        Task<bool> ExistsByEmailAndNotId(string email, long id);


        // Employee Qualification Management
        Task AddEmployeeQualification(EmployeeQualification qualification);
        Task UpdateEmployeeQualification(EmployeeQualification qualification);
        Task<EmployeeQualification?> GetEmployeeQualificationById(long Id);
        Task DeleteEmployeeQualification(long id);
        Task<List<EmployeeQualification>> GetEmployeeQualifications(long employeeId);

        // Employee Document Management
        Task AddEmployeeDocument(EmployeeDocument document);
        Task UpdateEmployeeDocument(EmployeeDocument document);
        Task DeleteEmployeeDocument(long id);
        Task<EmployeeDocument?> GetEmployeeDocumentById(long id);
        Task<List<EmployeeDocument>> GetEmployeeDocuments(long employeeId);

        // Org Chart
        Task<OrgNodeDto?> GetOrgChartAsync();
        Task<List<OrgNodeDto>> GetDirectReportsAsync(long managerId);
    }
}
