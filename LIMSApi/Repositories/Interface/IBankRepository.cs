using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IBankRepository
    {
        Task AddBank(BankMaster model);
        Task UpdateBank(BankMaster model);
        Task DeleteBank(BankMaster model);
        Task<BankMaster> GetBankById(long id);
        Task<PagedResponse<object>> GetAllBanks(PageFilter filter);

        Task<List<DropdwonSelector>> GetBankDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
        Task<bool> ExistsByAccountNumber(string accountNumber);
        Task<bool> ExistsByAccountNumberAndNotId(string accountNumber, long id);
    }
}
