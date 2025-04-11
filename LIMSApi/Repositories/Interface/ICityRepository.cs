using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ICityRepository
    {
        Task AddCity(CityMaster city);
        Task DeleteCity(long id);
        Task UpdateCity(CityMaster city);
        Task<CityMaster> GetCityById(long cityId);
        Task<PagedResponse<object>> GetAllCities(PageFilter filter);
        Task<bool> ExistsByName(string name);
        Task<bool> ExistsByNameAndNotId(string name, long id);
        Task<CityMaster?> GetByName(string name);

    }
}
