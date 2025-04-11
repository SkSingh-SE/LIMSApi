using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IBankService
    {
        Task CreateBank(BankMaster model);
        Task ModifyBank(BankMaster model);
        Task RemoveBank(long id);
        Task<BankMaster> GetBankDetails(long id);
        Task<PagedResponse<object>> FetchBankList(PageFilter filter);

        Task<List<DropdwonSelector>> GetBankDropdown(string? searchTerm, int pageNo, int pageSize);
    }
}
