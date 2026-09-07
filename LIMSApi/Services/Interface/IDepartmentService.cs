using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IDepartmentService
    {
        Task CreateDepartment(DepartmentCreateDto model);
        Task ModifyDepartment(DepartmentUpdateDto model);
        Task RemoveDepartment(long id);
        Task<bool> ToggleDepartmentStatus(long id);
        Task<DepartmentDetailDto> GetDepartmentDetails(long id);
        Task<PagedResponse<object>> FetchDepartmentList(PageFilter filter);
        Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<object>> GetBranchDisciplines(long branchId);
        Task<List<object>> GetAuthorizedBranches();
    }
}
