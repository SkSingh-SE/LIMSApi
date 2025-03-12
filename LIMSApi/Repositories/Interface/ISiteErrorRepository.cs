using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISiteErrorRepository
    {
        Task AddSiteError(SiteError model);
        Task UpdateSiteError(SiteError model);
        Task DeleteSiteError(long id);
        Task<SiteError> GetSiteErrorById(long id);
        Task<PagedResponse<object>> GetAllSiteErrors(PageFilter filter);
    }
}
