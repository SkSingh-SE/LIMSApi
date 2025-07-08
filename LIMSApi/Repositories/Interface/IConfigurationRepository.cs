using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface IConfigurationRepository
    {
        Task<Configuration> GetConfigurationByKey(string key);
        Task AddConfiguration(Configuration Configuration);
        Task UpdateConfiguration(Configuration Configuration);

    }
}
