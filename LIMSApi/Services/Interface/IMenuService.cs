using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IMenuService
    {
        Task CreateMenu(MenuMaster model);
        Task ModifyMenu(MenuMaster model);
        Task RemoveMenu(long id);
        Task<MenuMaster> GetMenuDetails(long id);
        Task<PagedResponse<object>> FetchMenuList(PageFilter filter);

        Task<List<CustomDropdown>> GetMenuDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
