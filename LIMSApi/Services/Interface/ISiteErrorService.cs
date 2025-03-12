using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.Services.Interface
{
    public interface ISiteErrorService
    {
        Task CreateSiteError(SiteError model);
        Task ModifySiteError(SiteError model);
        Task RemoveSiteError(long id);
        Task<SiteError> GetSiteErrorDetails(long id);
        Task<PagedResponse<object>> FetchSiteErrorList(PageFilter filter);

       
    }
}
