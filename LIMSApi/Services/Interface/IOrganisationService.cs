using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IOrganisationService
    {
        Task CreateOrganisation(OrganisationMaster model);
        Task ModifyOrganisation(OrganisationMaster model);
        Task RemoveOrganisation(long id);
        Task<OrganisationMaster> GetOrganisationDetails(long id);
        Task<PagedResponse<object>> FetchOrganisationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetOrganisationDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
