using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IRoleRepository
    {
        Task AddRole(RoleMaster model);
        Task UpdateRole(RoleMaster model);
        Task DeleteRole(long id);
        Task<RoleMaster> GetRoleById(long id);
        Task<PagedResponse<object>> GetAllRoles(PageFilter filter);

        Task<List<DropdwonSelector>> GetRoleDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
