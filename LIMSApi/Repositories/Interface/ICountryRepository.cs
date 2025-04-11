using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICountryRepository
    {
        Task AddCountry(CountryMaster country);
        Task UpdateCountry(CountryMaster country);
        Task DeleteCountry(long id);
        Task<CountryMaster> GetCountryById(long id);
        Task<PagedResponse<object>> GetAllCountries(PageFilter filter);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
        Task<CountryMaster?> GetByName(string name);
    }
}
