using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICurrencyRepository
    {
        Task AddCurrency(CurrencyMaster currency);
        Task UpdateCurrency(CurrencyMaster currency);
        Task DeleteCurrency(long id);
        Task<CurrencyMaster> GetCurrencyById(long id);
        Task<PagedResponse<CurrencyMaster>> GetAllCurrencys(PageFilter filter);

        Task<List<DropdwonSelector>> GetCurrencyDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long Id);
        Task<CurrencyMaster?> GetDefaultCurrency();
        Task SetDefaultCurrency(long id);
    }
}
