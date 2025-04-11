using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IDispatchModeRepository
    {
        Task AddDispatchMode(DispatchModeMaster model);
        Task UpdateDispatchMode(DispatchModeMaster model);
        Task DeleteDispatchMode(DispatchModeMaster model);
        Task<DispatchModeMaster> GetDispatchModeById(long id);
        Task<PagedResponse<object>> GetAllDispatchModes(PageFilter filter);

        Task<List<DropdwonSelector>> GetDispatchModeDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
