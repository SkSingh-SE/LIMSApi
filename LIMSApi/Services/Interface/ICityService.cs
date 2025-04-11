using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ICityService
    {
        Task<CityMaster> CreateCity(CityMaster country);
        Task ModifyCity(CityMaster country);
        Task RemoveCity(long id);
        Task<CityMaster> GetCityDetails(long id);
        Task<PagedResponse<object>> FetchCities(PageFilter filter);
        Task<CityMaster?> GetCityByName(string name);
    }
}
