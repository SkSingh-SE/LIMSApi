using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IAreaRepository
    {
        Task<AreaMaster> AddArea(AreaMaster area);
        Task UpdateArea(AreaMaster area);
        Task DeleteArea(long id);
        Task<AreaMaster> GetAreaById(long id);
        Task<PagedResponse<AreaMaster>> GetAllAreas(PageFilter filter);

        Task<List<AreaDropdownDTO>> GetAreaWithPincode(string pincode);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
        Task<AreaMaster> GetAreaByName(string name);
    }
}
