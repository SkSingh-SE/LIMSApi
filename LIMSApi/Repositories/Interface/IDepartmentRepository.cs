using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IDepartmentRepository
    {
        Task AddDepartment(DepartmentMaster model);
        Task UpdateDepartment(DepartmentMaster model);
        Task DeleteDepartment(long id);
        Task<DepartmentMaster?> GetDepartmentById(long id);
        Task<PagedResponse<object>> GetAllDepartments(PageFilter filter);
        Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo, int pageSize);

        Task<bool> BranchExistsAndActive(long branchId);
        Task<bool> DisciplineExistsAndActive(long disciplineId);
        Task<bool> BranchSupportsDiscipline(long branchId, long disciplineId);
        Task<bool> ExistsByCodeInBranch(string code, long branchId);
        Task<bool> ExistsByCodeInBranchAndNotId(string code, long branchId, long id);
        Task<bool> ExistsByNameInBranch(string name, long branchId);
        Task<bool> ExistsByNameInBranchAndNotId(string name, long branchId, long id);
        Task<List<object>> GetBranchDisciplines(long branchId);
        Task<List<object>> GetAuthorizedBranches();
    }
}
