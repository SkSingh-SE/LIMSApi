using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IOrganisationRepository
    {
        Task AddOrganisation(OrganisationMaster model);
        Task UpdateOrganisation(OrganisationMaster model);
        Task DeleteOrganisation(long id);
        Task<OrganisationMaster> GetOrganisationById(long id);
        Task<PagedResponse<object>> GetAllOrganisations(PageFilter filter);

        Task<List<DropdwonSelector>> GetOrganisationDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
    }
}
