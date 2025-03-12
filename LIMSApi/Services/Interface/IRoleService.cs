using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IRoleService
    {
        Task CreateRole(RoleMaster model);
        Task ModifyRole(RoleMaster model);
        Task RemoveRole(long id);
        Task<RoleMaster> GetRoleDetails(long id);
        Task<PagedResponse<object>> FetchRoleList(PageFilter filter);

        Task<List<DropdwonSelector>> GetRoleDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
