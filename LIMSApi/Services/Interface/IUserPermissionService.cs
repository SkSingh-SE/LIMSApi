using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IUserPermissionService
    {
        Task SaveUserPermissions(long userId, List<UserPermissionUpdateDto> updatedPermissions);

        Task<List<MenuPermissionGroupDto>> GetUserPermissions(long userId);
        Task<List<MenuPermissionGroupDto>> GetAllPermissions();
        Task<List<DropdwonSelector>> GetPermissionDropdown(string? searchTerm, int pageNo, int pageSize );
    }
}
