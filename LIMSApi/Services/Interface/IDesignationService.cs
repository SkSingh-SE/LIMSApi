using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IDesignationService
    {
        Task CreateDesignation(DesignationMaster model);
        Task ModifyDesignation(DesignationMaster model);
        Task RemoveDesignation(long id);
        Task<DesignationMaster> GetDesignationDetails(long id);
        Task<PagedResponse<object>> FetchDesignationList(PageFilter filter);

        Task<List<DropdwonSelector>> GetDesignationDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
