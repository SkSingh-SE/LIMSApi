using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICoolingMediumRepository
    {
        Task AddCoolingMedium(CoolingMediumMaster model);
        Task UpdateCoolingMedium(CoolingMediumMaster model);
        Task DeleteCoolingMedium(long id);
        Task<CoolingMediumMaster> GetCoolingMediumById(long id);
        Task<PagedResponse<object>> GetAllCoolingMediums(PageFilter filter);
        Task<List<DropdwonSelector>> GetCoolingMediumDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
