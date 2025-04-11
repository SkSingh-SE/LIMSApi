using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IDispatchModeService
    {
        Task CreateDispatchMode(DispatchModeMaster model);
        Task ModifyDispatchMode(DispatchModeMaster model);
        Task RemoveDispatchMode(long id);
        Task<DispatchModeMaster> GetDispatchModeDetails(long id);
        Task<PagedResponse<object>> FetchDispatchModeList(PageFilter filter);

        Task<List<DropdwonSelector>> GetDispatchModeDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
