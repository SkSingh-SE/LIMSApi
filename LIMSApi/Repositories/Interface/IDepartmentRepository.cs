using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IDepartmentRepository
    {
        Task AddDepartment(DepartmentMaster model);
        Task UpdateDepartment(DepartmentMaster model);
        Task DeleteDepartment(long id);
        Task<DepartmentMaster> GetDepartmentById(long id);
        Task<PagedResponse<object>> GetAllDepartments(PageFilter filter);

        Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
