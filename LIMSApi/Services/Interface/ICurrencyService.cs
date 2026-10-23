using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICurrencyService
    {
        Task CreateCurrency(CurrencyMaster model);
        Task ModifyCurrency(CurrencyMaster model);
        Task RemoveCurrency(long id);
        Task<CurrencyMaster> GetCurrencyDetails(long id);
        Task<PagedResponse<CurrencyMaster>> FetchCurrencies(PageFilter filter);

        Task<List<DropdwonSelector>> GetCurrencyDropdown(string? searchTerm, int pageNo, int pageSize);
        Task<CurrencyMaster?> GetDefaultCurrency();
        Task SetDefaultCurrency(long id);
    }
}
