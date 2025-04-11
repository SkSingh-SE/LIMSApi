using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISubContractorService
    {
        Task CreateSubContractor(SubContractorMaster model);
        Task ModifySubContractor(SubContractorMaster model);
        Task RemoveSubContractor(long id);
        Task<SubContractorMaster> GetSubContractorDetails(long id);
        Task<PagedResponse<object>> FetchSubContractorList(PageFilter filter);

        Task<List<DropdwonSelector>> GetSubContractorDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
