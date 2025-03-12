using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SiteErrorService : ISiteErrorService
    {
        private readonly ISiteErrorRepository _siteErrorRepository;
        private readonly ILogger<SiteErrorService> _logger;

        public SiteErrorService(ISiteErrorRepository siteErrorRepo, ILogger<SiteErrorService> logger)
        {
            _siteErrorRepository = siteErrorRepo;
            _logger = logger;
        }

        public async Task CreateSiteError(SiteError model)
        {
            
            await _siteErrorRepository.AddSiteError(model);
            _logger.LogInformation("SiteError '{SiteErrorName}' created successfully.", model.Description);
        }

        public async Task ModifySiteError(SiteError model)
        {
           
            var existingSiteError = await _siteErrorRepository.GetSiteErrorById(model.ID);
            if (existingSiteError == null)
                throw new InvalidOperationException("SiteError not found!");

            
            existingSiteError.ModifiedOn = DateTime.UtcNow;

            await _siteErrorRepository.UpdateSiteError(existingSiteError);
            _logger.LogInformation("SiteError '{SiteErrorName}' updated successfully.", model.Description);
        }

        public async Task RemoveSiteError(long id)
        {
            await _siteErrorRepository.DeleteSiteError(id);
            _logger.LogInformation("SiteError with ID '{SiteErrorId}' deleted successfully.", id);
        }

        public async Task<SiteError> GetSiteErrorDetails(long id)
        {
            var classification = await _siteErrorRepository.GetSiteErrorById(id);
            if (classification == null)
                throw new InvalidOperationException("SiteError not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSiteErrorList(PageFilter filter)
        {
            return await _siteErrorRepository.GetAllSiteErrors(filter);
        }

       
    }
}
