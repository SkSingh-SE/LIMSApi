using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISubGroupRepository
    {
        Task AddSubGroup(SubGroupMaster model);
        Task UpdateSubGroup(SubGroupMaster model);
        Task DeleteSubGroup(SubGroupMaster model);
        Task<SubGroupMaster> GetSubGroupById(long id);
        Task<PagedResponse<object>> GetAllSubGroups(PageFilter filter);

        Task<List<DropdwonSelector>> GetSubGroupDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
