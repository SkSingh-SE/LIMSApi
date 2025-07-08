using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IConfigurationService
    {
        Task UpdateConfiguration(Configuration Configuration);
        Task CreateConfiguration(Configuration Configuration);
        Task<Configuration> GetConfigurationByKey(string Key);

    }
}
