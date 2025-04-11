using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IOEMRepository
    {
        Task AddOEM(OEMMaster model);
        Task UpdateOEM(OEMMaster model);
        Task DeleteOEM(long id);
        Task<OEMMaster> GetOEMById(long id);
        Task<PagedResponse<object>> GetAllOEMs(PageFilter filter);

        Task<List<DropdwonSelector>> GetOEMDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
