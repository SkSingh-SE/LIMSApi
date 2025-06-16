using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IGroupService
    {
        Task CreateGroup(GroupMaster model);
        Task ModifyGroup(GroupMaster model);
        Task RemoveGroup(long id);
        Task<GroupMaster> GetGroupDetails(long id);
        Task<PagedResponse<object>> FetchGroupList(PageFilter filter);

        Task<List<DropdwonSelector>> GetGroupDropdown(string? searchTerm, int pageNo, int pageSize, long? id = null);
    }
}
