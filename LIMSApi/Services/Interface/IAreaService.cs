using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IAreaService
    {
        Task CreateArea(AreaMaster country);
        Task ModifyArea(AreaMaster country);
        Task RemoveArea(long id);
        Task<AreaMaster> GetAreaDetails(long id);
        Task<PagedResponse<AreaMaster>> FetchAreas(PageFilter filter);

        Task<List<DropdwonSelector>> GetAreaWithPincode(string? searchTerm, int pageNo, int pageSize);
    }
}
