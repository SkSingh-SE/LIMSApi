using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IStandardOrganizationRepository
    {
        Task AddStandardOrganization(StandardOrganizationMaster model);
        Task UpdateStandardOrganization(StandardOrganizationMaster model);
        Task DeleteStandardOrganization(long id);
        Task<StandardOrganizationMaster> GetStandardOrganizationById(long id);
        Task<PagedResponse<object>> GetAllStandardOrganizations(PageFilter filter);

        Task<List<DropdwonSelector>> GetStandardOrganizationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
    }
}
