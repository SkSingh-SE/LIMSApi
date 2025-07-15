using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IMenuRepository
    {
        Task AddMenu(MenuMaster model);
        Task UpdateMenu(MenuMaster model);
        Task DeleteMenuTree(long id);
        Task<MenuMaster> GetMenuById(long id);
        Task<PagedResponse<object>> GetAllMenus(PageFilter filter);
        Task<List<DropdwonSelector>> GetMenuDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
