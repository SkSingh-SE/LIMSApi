using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IGroupRepository
    {
        Task AddGroup(GroupMaster model);
        Task UpdateGroup(GroupMaster model);
        Task DeleteGroup(GroupMaster model);
        Task<GroupMaster> GetGroupById(long id);
        Task<PagedResponse<object>> GetAllGroups(PageFilter filter);

        Task<List<DropdwonSelector>> GetGroupDropdown(string? searchTerm, int pageNo, int pageSize, long? id = null);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
