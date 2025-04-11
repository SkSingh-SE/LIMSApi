using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ITPIMasterService
    {
        Task CreateTPI(TPIMaster model);
        Task ModifyTPI(TPIMaster model);
        Task RemoveTPI(long id);
        Task<TPIMaster> GetTPIDetails(long id);
        Task<PagedResponse<object>> FetchTPIList(PageFilter filter);

        Task<List<DropdwonSelector>> GetTPIDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
