using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICountryService
    {
        Task CreateCountry(CountryMaster country);
        Task ModifyCountry(CountryMaster country);
        Task RemoveCountry(long id);
        Task<CountryMaster> GetCountryDetails(long id);
        Task<PagedResponse<object>> FetchCountries(PageFilter filter);
    }
}
