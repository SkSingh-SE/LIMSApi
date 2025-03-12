using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IStandardOrganizationService
    {
        Task CreateStandardOrganization(StandardOrganizationMaster model);
        Task ModifyStandardOrganization(StandardOrganizationMaster model);
        Task RemoveStandardOrganization(long id);
        Task<StandardOrganizationMaster> GetStandardOrganizationDetails(long id);
        Task<PagedResponse<object>> FetchStandardOrganizationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetStandardOrganizationDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
