using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Repositories.Interface
{
    public interface ISiteActivityRepository
    {
        Task AddSiteActivity(SiteActivity model);
        Task AddMultipleSiteActivities(List<SiteActivity> activities);
        Task UpdateSiteActivity(SiteActivity model);
        Task DeleteSiteActivity(long id);
        Task<SiteActivity> GetSiteActivityById(long id);
        Task<PagedResponse<object>> GetAllSiteActivitys(PageFilter filter);
    }
}
