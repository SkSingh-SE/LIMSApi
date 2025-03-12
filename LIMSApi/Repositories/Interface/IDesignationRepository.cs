using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IDesignationRepository
    {
        Task AddDesignation(DesignationMaster model);
        Task UpdateDesignation(DesignationMaster model);
        Task DeleteDesignation(long id);
        Task<DesignationMaster> GetDesignationById(long id);
        Task<PagedResponse<object>> GetAllDesignations(PageFilter filter);

        Task<List<DropdwonSelector>> GetDesignationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
