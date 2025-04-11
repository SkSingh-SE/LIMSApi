using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IOEMService
    {
        Task CreateOEM(OEMMaster model);
        Task ModifyOEM(OEMMaster model);
        Task RemoveOEM(long id);
        Task<OEMMaster> GetOEMDetails(long id);
        Task<PagedResponse<object>> FetchOEMList(PageFilter filter);

        Task<List<DropdwonSelector>> GetOEMDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
