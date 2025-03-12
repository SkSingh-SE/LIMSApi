using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IDepartmentService
    {
        Task CreateDepartment(DepartmentMaster model);
        Task ModifyDepartment(DepartmentMaster model);
        Task RemoveDepartment(long id);
        Task<DepartmentMaster> GetDepartmentDetails(long id);
        Task<PagedResponse<object>> FetchDepartmentList(PageFilter filter);

        Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
