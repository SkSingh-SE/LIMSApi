using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface IConfigurationService
    {
        Task<PagedResponse<object>> FetchConfigurationList(PageFilter filter);
        Task UpdateConfiguration(Configuration Configuration);
        Task CreateConfiguration(Configuration Configuration);
        Task<Configuration> GetConfigurationByKey(string Key);

    }
}
