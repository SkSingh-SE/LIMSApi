using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IMenuRepository
    {
        Task<MenuMaster> AddMenu(MenuMaster model);
        Task<MenuMaster> UpdateMenu(MenuMaster model);
        Task DeleteMenuTree(long id);
        Task<MenuMaster> GetMenuById(long id);
        Task<PagedResponse<object>> GetAllMenus(PageFilter filter);
        Task<PagedResponse<object>> GetAllSubMenus(PageFilter filter);
        Task<List<CustomDropdown>> GetMenuDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<CustomDropdown>> GetSubMenuDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
        Task<bool> UpdateMenuPermission(long menuId, List<PermissionMaster> updatedPermissions);
        Task<List<PermissionMaster>> GetMenuPermissions(long menuId);
        Task<List<MenuMaster>> GetMenuTree();
    }
}
