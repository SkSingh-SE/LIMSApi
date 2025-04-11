using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IItemMasterRepository
    {
        Task AddItem(ItemMaster model);
        Task UpdateItem(ItemMaster model);
        Task DeleteItem(ItemMaster model);
        Task<ItemMaster> GetItemById(long id);
        Task<PagedResponse<object>> GetAllItems(PageFilter filter);

        Task<List<DropdwonSelector>> GetItemDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
