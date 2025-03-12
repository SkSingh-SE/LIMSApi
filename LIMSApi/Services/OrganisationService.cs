using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly ILogger<OrganisationService> _logger;

        public OrganisationService(IOrganisationRepository organisationRepo, ILogger<OrganisationService> logger)
        {
            _organisationRepository = organisationRepo;
            _logger = logger;
        }

        public async Task CreateOrganisation(OrganisationMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Organisation name should not be empty!");

            bool exists = await _organisationRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Organisation already exists!");

            await _organisationRepository.AddOrganisation(model);
            _logger.LogInformation("Organisation '{OrganisationName}' created successfully.", model.Name);
        }

        public async Task ModifyOrganisation(OrganisationMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Organisation ID should not be empty!");

            bool exists = await _organisationRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Organisation already exists!");

            var existingOrganisation = await _organisationRepository.GetOrganisationById(model.ID);
            if (existingOrganisation == null)
                throw new InvalidOperationException("Organisation not found!");

            existingOrganisation.Name = model.Name;
            existingOrganisation.Description = model.Description;
            existingOrganisation.ModifiedOn = DateTime.UtcNow;

            await _organisationRepository.UpdateOrganisation(existingOrganisation);
            _logger.LogInformation("Organisation '{OrganisationName}' updated successfully.", model.Name);
        }

        public async Task RemoveOrganisation(long id)
        {
            var existingOrganisation = await _organisationRepository.GetOrganisationById(id);
            if (existingOrganisation == null)
                throw new InvalidOperationException("Organisation not found!");

            existingOrganisation.IsActive = false;
            existingOrganisation.ModifiedOn = DateTime.UtcNow;

            await _organisationRepository.UpdateOrganisation(existingOrganisation);
            _logger.LogInformation("Organisation with ID '{OrganisationId}' deleted successfully.", id);
        }

        public async Task<OrganisationMaster> GetOrganisationDetails(long id)
        {
            var classification = await _organisationRepository.GetOrganisationById(id);
            if (classification == null)
                throw new InvalidOperationException("Organisation not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchOrganisationList(PageFilter filter)
        {
            return await _organisationRepository.GetAllOrganisations(filter);
        }

        public async Task<List<DropdwonSelector>> GetOrganisationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _organisationRepository.GetOrganisationDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
