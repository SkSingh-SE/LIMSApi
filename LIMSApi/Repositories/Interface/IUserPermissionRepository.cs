using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IUserPermissionRepository
    {
        Task SaveUserPermissions(long userId, List<UserPermissionUpdateDto> updatedPermissions);

        Task<List<MenuPermissionGroupDto>> GetUserPermissions(long userId);
        Task<List<MenuPermissionGroupDto>> GetAllPermissions();
        Task<List<DropdwonSelector>> GetPermissionDropdown(string? searchTerm, int pageNo = 0, int pageSize = 20);
    }
}
