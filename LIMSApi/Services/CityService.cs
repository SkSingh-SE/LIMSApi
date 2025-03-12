using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<CityService> _logger;

        public CityService(ICityRepository cityRepository, ILogger<CityService> logger)
        {
            _cityRepository = cityRepository;
            _logger = logger;
        }

        public async Task CreateCity(CityMaster city)
        {
            if (string.IsNullOrWhiteSpace(city.Name))
                throw new ArgumentException("City name should not be empty!");

            bool exists = await _cityRepository.ExistsByName(city.Name);
            if (exists)
                throw new InvalidOperationException("City already exists!");

            await _cityRepository.AddCity(city);
            _logger.LogInformation("City '{CityName}' created successfully.", city.Name);
        }

        public async Task ModifyCity(CityMaster city)
        {
            if (city.ID == 0)
                throw new ArgumentException("City ID should not be empty!");

            bool exists = await _cityRepository.ExistsByNameAndNotId(city.Name, city.ID);
            if (exists)
                throw new InvalidOperationException("Same City already exists!");

            var existingCity = await _cityRepository.GetCityById(city.ID);
            if (existingCity == null)
                throw new InvalidOperationException("City not found!");

            existingCity.Name = city.Name;
            existingCity.Code = city.Code;
            existingCity.ModifiedOn = DateTime.UtcNow;

            await _cityRepository.UpdateCity(existingCity);
            _logger.LogInformation("City '{CityName}' updated successfully.", city.Name);
        }

        public async Task RemoveCity(long id)
        {
            var existingCity = await _cityRepository.GetCityById(id);
            if (existingCity == null)
                throw new InvalidOperationException("City not found!");

            existingCity.IsActive = false;
            existingCity.ModifiedOn = DateTime.UtcNow;

            await _cityRepository.UpdateCity(existingCity);
            _logger.LogInformation("City with ID '{CityId}' deleted successfully.", id);
        }

        public async Task<CityMaster> GetCityDetails(long id)
        {
            var city = await _cityRepository.GetCityById(id);
            if (city == null)
                throw new InvalidOperationException("City not found!");

            return city;
        }

        public async Task<PagedResponse<object>> FetchCities(PageFilter filter)
        {
            return await _cityRepository.GetAllCities(filter);
        }
    }
}
