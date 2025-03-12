using System.Linq;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SiteActivityService : ISiteActivityService
    {
        private readonly ISiteActivityRepository _siteActivityRepository;
        private readonly ILogger<SiteActivityService> _logger;

        public SiteActivityService(ISiteActivityRepository siteActivityRepo, ILogger<SiteActivityService> logger)
        {
            _siteActivityRepository = siteActivityRepo;
            _logger = logger;
        }

        public async Task CreateSiteActivity(SiteActivity model)
        {
            
            await _siteActivityRepository.AddSiteActivity(model);
            _logger.LogInformation("SiteActivity '{SiteActivityName}' created successfully.", model.ModuleName);
        }
        public async Task CreateMultipleSiteActivities(List<SiteActivity> activities)
        {

            await _siteActivityRepository.AddMultipleSiteActivities(activities);
            _logger.LogInformation("Multiple SiteActivity '{SiteActivityNames}' added successfully.", string.Join(",", activities.Select(x => x.ModuleName).ToList()));
        }

        public async Task ModifySiteActivity(SiteActivity model)
        {
           
            var existingSiteActivity = await _siteActivityRepository.GetSiteActivityById(model.ID);
            if (existingSiteActivity == null)
                throw new InvalidOperationException("SiteActivity not found!");

            
            existingSiteActivity.ModifiedOn = DateTime.UtcNow;

            await _siteActivityRepository.UpdateSiteActivity(existingSiteActivity);
            _logger.LogInformation("SiteActivity '{SiteActivityName}' updated successfully.", model.ModuleName);
        }

        public async Task RemoveSiteActivity(long id)
        {
            await _siteActivityRepository.DeleteSiteActivity(id);
            _logger.LogInformation("SiteActivity with ID '{SiteActivityId}' deleted successfully.", id);
        }

        public async Task<SiteActivity> GetSiteActivityDetails(long id)
        {
            var classification = await _siteActivityRepository.GetSiteActivityById(id);
            if (classification == null)
                throw new InvalidOperationException("SiteActivity not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSiteActivityList(PageFilter filter)
        {
            return await _siteActivityRepository.GetAllSiteActivitys(filter);
        }

       
    }
}
