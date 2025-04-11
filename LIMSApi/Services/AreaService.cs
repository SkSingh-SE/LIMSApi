using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Newtonsoft.Json;

namespace LIMSApi.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly ILogger<AreaService> _logger;
        private readonly ICountryService countryService;
        private readonly IStateService stateService;
        private readonly ICityService cityService;

        public AreaService(IAreaRepository areaRepository, ILogger<AreaService> logger, ICountryService countryService, IStateService stateService, ICityService cityService )
        {
            _areaRepository = areaRepository;
            _logger = logger;
            this.countryService = countryService;
            this.stateService = stateService;
            this.cityService = cityService;
        }

        public async Task CreateArea(AreaMaster area)
        {
            if (string.IsNullOrWhiteSpace(area.Name))
                throw new ArgumentException("Area name should not be empty!");

            bool exists = await _areaRepository.ExistsByName(area.Name);
            if (exists)
                throw new InvalidOperationException("Area already exists!");

            await _areaRepository.AddArea(area);
            _logger.LogInformation("Area '{AreaName}' created successfully.", area.Name);
        }

        public async Task ModifyArea(AreaMaster area)
        {
            if (area.ID == 0)
                throw new ArgumentException("Area ID should not be empty!");

            bool exists = await _areaRepository.ExistsByNameAndNotId(area.Name, area.ID);
            if (exists)
                throw new InvalidOperationException("Same Area already exists!");

            var existingArea = await _areaRepository.GetAreaById(area.ID);
            if (existingArea == null)
                throw new InvalidOperationException("Area not found!");

            existingArea.Name = area.Name;
            existingArea.Code = area.Code;
            existingArea.ModifiedOn = DateTime.UtcNow;

            await _areaRepository.UpdateArea(existingArea);
            _logger.LogInformation("Area '{AreaName}' updated successfully.", area.Name);
        }

        public async Task RemoveArea(long id)
        {
            var existingArea = await _areaRepository.GetAreaById(id);
            if (existingArea == null)
                throw new InvalidOperationException("Area not found!");

            existingArea.IsActive = false;
            existingArea.ModifiedOn = DateTime.UtcNow;

            await _areaRepository.UpdateArea(existingArea);
            _logger.LogInformation("Area with ID '{AreaId}' deleted successfully.", id);
        }

        public async Task<AreaMaster> GetAreaDetails(long id)
        {
            var area = await _areaRepository.GetAreaById(id);
            if (area == null)
                throw new InvalidOperationException("Area not found!");

            return area;
        }

        public async Task<PagedResponse<AreaMaster>> FetchAreas(PageFilter filter)
        {
            return await _areaRepository.GetAllAreas(filter);
        }

        public async Task<List<AreaDropdownDTO>> GetAreaWithPincode(string pincode)
        {
            var areaList = await _areaRepository.GetAreaWithPincode(pincode);
            if(areaList == null || areaList.Count == 0)
            {
                areaList = new List<AreaDropdownDTO>();
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://api.postalpincode.in/pincode/{pincode}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<List<PostalApiResponse>>(content);

                        if (apiResponse != null && apiResponse[0].Status == "Success")
                        {
                            foreach(var item in apiResponse[0].PostOffice)
                            {
                                var existingCountry = await countryService.GetCountryByName(item.Country);
                                if (existingCountry == null)
                                {
                                    var country = new CountryMaster
                                    {
                                        Name = item.Country,
                                        IsActive = true,
                                        CreatedOn = DateTime.UtcNow
                                    };
                                    existingCountry = await countryService.CreateCountry(country);

                                }
                                var existingState = await stateService.GetStateByName(item.State);
                                if (existingState == null)
                                {
                                    var state = new StateMaster
                                    {
                                        Name = item.State,
                                        CountryID = existingCountry.ID,
                                        IsActive = true,
                                        CreatedOn = DateTime.UtcNow
                                    };
                                    existingState = await stateService.CreateState(state);
                                }

                                var existingCity = await cityService.GetCityByName(item.Block);
                                if (existingCity == null)
                                {
                                    var city = new CityMaster
                                    {
                                        Name = item.Block,
                                        StateID = existingState.ID,
                                        IsActive = true,
                                        CreatedOn = DateTime.UtcNow
                                    };
                                    existingCity = await cityService.CreateCity(city);
                                }

                                var area = new AreaMaster
                                {
                                    Name = item.Name,
                                    Code = item.Name,
                                    CityID = existingCity.ID,
                                    Pincode = item.Pincode,
                                    IsActive = true,
                                    CreatedOn = DateTime.UtcNow
                                };
                                var existingArea = await _areaRepository.GetAreaByName(area.Name);
                                if(existingArea == null)
                                {
                                    existingArea = await _areaRepository.AddArea(area);
                                    areaList.Add(new AreaDropdownDTO
                                    {
                                        AreaId = existingArea.ID,
                                        AreaName = existingArea.Name,
                                        CityId = existingCity.ID,
                                        CityName = existingCity.Name,
                                        StateId = existingState.ID,
                                        StateName = existingState.Name,
                                        CountryId = existingCountry.ID,
                                        CountryName = existingCountry.Name
                                    });
                                }
                                else
                                {
                                    areaList.Add(new AreaDropdownDTO
                                    {
                                        AreaId = existingArea.ID,
                                        AreaName = existingArea.Name,
                                        CityId = existingCity.ID,
                                        CityName = existingCity.Name,
                                        StateId = existingState.ID,
                                        StateName = existingState.Name,
                                        CountryId = existingCountry.ID,
                                        CountryName = existingCountry.Name
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return areaList;
            
        }
    }
}
