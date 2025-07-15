using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IConfigurationRepository
    {
        Task<PagedResponse<object>> GetAllConfigurations(PageFilter filter);
        Task<Configuration> GetConfigurationByKey(string key);
        Task AddConfiguration(Configuration Configuration);
        Task UpdateConfiguration(Configuration Configuration);
        Task<bool> IsExistKeyName(string key);
        Task<bool> IsExistKeyAndId(string key, int Id);

    }
}
