using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<CountryService> _logger;

        public CountryService(ICountryRepository countryRepository, ILogger<CountryService> logger)
        {
            _countryRepository = countryRepository;
            _logger = logger;
        }

        public async Task<CountryMaster> CreateCountry(CountryMaster country)
        {
            if (string.IsNullOrWhiteSpace(country.Name))
                throw new ArgumentException("Country name should not be empty!");

            bool exists = await _countryRepository.ExistsByName(country.Name);
            if (exists)
                throw new InvalidOperationException("Country already exists!");

            await _countryRepository.AddCountry(country);
            _logger.LogInformation("Country '{CountryName}' created successfully.", country.Name);
            return country;
        }

        public async Task ModifyCountry(CountryMaster country)
        {
            if (country.ID == 0)
                throw new ArgumentException("Country ID should not be empty!");

            bool exists = await _countryRepository.ExistsByNameAndNotId(country.Name, country.ID);
            if (exists)
                throw new InvalidOperationException("Same Country already exists!");

            var existingCountry = await _countryRepository.GetCountryById(country.ID);
            if (existingCountry == null)
                throw new InvalidOperationException("Country not found!");

            existingCountry.Name = country.Name;
            existingCountry.Code = country.Code;
            existingCountry.ModifiedOn = DateTime.UtcNow;

            await _countryRepository.UpdateCountry(existingCountry);
            _logger.LogInformation("Country '{CountryName}' updated successfully.", country.Name);
        }

        public async Task RemoveCountry(long id)
        {
            var existingCountry = await _countryRepository.GetCountryById(id);
            if (existingCountry == null)
                throw new Exception("Country not found!");

            existingCountry.IsActive = false;
            existingCountry.ModifiedOn = DateTime.UtcNow;

            await _countryRepository.UpdateCountry(existingCountry);
            _logger.LogInformation("Country with ID '{CountryId}' deleted successfully.", id);
        }

        public async Task<CountryMaster> GetCountryDetails(long id)
        {
            var country = await _countryRepository.GetCountryById(id);
            if (country == null)
                throw new Exception("Country not found!");

            return country;
        }
        public async Task<CountryMaster?> GetCountryByName(string name)
        {
            return await _countryRepository.GetByName(name);
        }

        public async Task<PagedResponse<object>> FetchCountries(PageFilter filter)
        {
            return await _countryRepository.GetAllCountries(filter);
        }
    }
}
