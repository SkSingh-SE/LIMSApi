using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISubGroupService
    {
        Task CreateSubGroup(SubGroupMaster model);
        Task ModifySubGroup(SubGroupMaster model);
        Task RemoveSubGroup(long id);
        Task<SubGroupMaster> GetSubGroupDetails(long id);
        Task<PagedResponse<object>> FetchSubGroupList(PageFilter filter);

        Task<List<DropdwonSelector>> GetSubGroupDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<List<DropdwonSelector>> GetDropdownByGroupId(long id);
    }
}
