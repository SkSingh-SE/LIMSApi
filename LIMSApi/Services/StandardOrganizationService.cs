using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class StandardOrganizationService : IStandardOrganizationService
    {
        private readonly IStandardOrganizationRepository _standardOrganizationRepository;
        private readonly ILogger<StandardOrganizationService> _logger;

        public StandardOrganizationService(IStandardOrganizationRepository standardOrganizationRepo, ILogger<StandardOrganizationService> logger)
        {
            _standardOrganizationRepository = standardOrganizationRepo;
            _logger = logger;
        }

        public async Task CreateStandardOrganization(StandardOrganizationMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("StandardOrganization name should not be empty!");

            bool exists = await _standardOrganizationRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("StandardOrganization already exists!");

            await _standardOrganizationRepository.AddStandardOrganization(model);
            _logger.LogInformation("StandardOrganization '{StandardOrganizationName}' created successfully.", model.Name);
        }

        public async Task ModifyStandardOrganization(StandardOrganizationMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("StandardOrganization ID should not be empty!");

            bool exists = await _standardOrganizationRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same StandardOrganization already exists!");

            var existingStandardOrganization = await _standardOrganizationRepository.GetStandardOrganizationById(model.ID);
            if (existingStandardOrganization == null)
                throw new InvalidOperationException("StandardOrganization not found!");

            existingStandardOrganization.Name = model.Name;
            existingStandardOrganization.NumberType = model.NumberType;
            existingStandardOrganization.ModifiedOn = DateTime.UtcNow;

            await _standardOrganizationRepository.UpdateStandardOrganization(existingStandardOrganization);
            _logger.LogInformation("StandardOrganization '{StandardOrganizationName}' updated successfully.", model.Name);
        }

        public async Task RemoveStandardOrganization(long id)
        {
            var existingStandardOrganization = await _standardOrganizationRepository.GetStandardOrganizationById(id);
            if (existingStandardOrganization == null)
                throw new InvalidOperationException("StandardOrganization not found!");

            existingStandardOrganization.IsActive = false;
            existingStandardOrganization.ModifiedOn = DateTime.UtcNow;

            await _standardOrganizationRepository.UpdateStandardOrganization(existingStandardOrganization);
            _logger.LogInformation("StandardOrganization with ID '{StandardOrganizationId}' deleted successfully.", id);
        }

        public async Task<StandardOrganizationMaster> GetStandardOrganizationDetails(long id)
        {
            var classification = await _standardOrganizationRepository.GetStandardOrganizationById(id);
            if (classification == null)
                throw new InvalidOperationException("StandardOrganization not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchStandardOrganizationList(PageFilter filter)
        {
            return await _standardOrganizationRepository.GetAllStandardOrganizations(filter);
        }

        public async Task<List<DropdwonSelector>> GetStandardOrganizationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _standardOrganizationRepository.GetStandardOrganizationDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
