using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IItemMasterService
    {
        Task CreateItem(ItemMaster model);
        Task ModifyItem(ItemMaster model);
        Task RemoveItem(long id);
        Task<ItemMaster> GetItemDetails(long id);
        Task<PagedResponse<object>> FetchItemList(PageFilter filter);

        Task<List<DropdwonSelector>> GetItemDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
