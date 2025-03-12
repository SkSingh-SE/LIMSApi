using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISiteActivityService
    {
        Task CreateSiteActivity(SiteActivity model);
        Task CreateMultipleSiteActivities(List<SiteActivity> activities);
        Task ModifySiteActivity(SiteActivity model);
        Task RemoveSiteActivity(long id);
        Task<SiteActivity> GetSiteActivityDetails(long id);
        Task<PagedResponse<object>> FetchSiteActivityList(PageFilter filter);

    }
}
