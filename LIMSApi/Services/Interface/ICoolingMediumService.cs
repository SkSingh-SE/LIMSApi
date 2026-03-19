using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICoolingMediumService
    {
        Task CreateCoolingMedium(CoolingMediumMaster model);
        Task ModifyCoolingMedium(CoolingMediumMaster model);
        Task RemoveCoolingMedium(long id);
        Task<CoolingMediumMaster> GetCoolingMediumDetails(long id);
        Task<PagedResponse<object>> FetchCoolingMediumList(PageFilter filter);
        Task<List<DropdwonSelector>> GetCoolingMediumDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
