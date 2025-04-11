using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IIndustryMasterService
    {
        Task CreateIndustry(IndustryMaster model);
        Task ModifyIndustry(IndustryMaster model);
        Task RemoveIndustry(long id);
        Task<IndustryMaster> GetIndustryDetails(long id);
        Task<PagedResponse<object>> FetchIndustryList(PageFilter filter);

        Task<List<DropdwonSelector>> GetIndustryDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
