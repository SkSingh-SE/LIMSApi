using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IAreaRepository
    {
        Task AddArea(AreaMaster area);
        Task UpdateArea(AreaMaster area);
        Task DeleteArea(long id);
        Task<AreaMaster> GetAreaById(long id);
        Task<PagedResponse<AreaMaster>> GetAllAreas(PageFilter filter);

        Task<List<DropdwonSelector>> GetAreaWithPincode(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
