using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IUOMRepository
    {
        Task AddUOM(UOMMaster model);
        Task UpdateUOM(UOMMaster model);
        Task DeleteUOM(long id);
        Task<UOMMaster> GetUOMById(long id);
        Task<PagedResponse<object>> GetAllUOMs(PageFilter filter);

        Task<List<DropdwonSelector>> GetUOMDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
