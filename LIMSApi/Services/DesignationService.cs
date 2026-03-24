using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _designationRepository;
        private readonly ILogger<DesignationService> _logger;
        private readonly LIMSContext _context;
        private readonly LoggedInUserDTO loggedInUserDTO;

        public DesignationService(IDesignationRepository designationRepo, ILogger<DesignationService> logger, LIMSContext context)
        {
            _designationRepository = designationRepo;
            _logger = logger;
            _context = context;
            loggedInUserDTO = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateDesignation(DesignationMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Designation name should not be empty!");

            bool exists = await _designationRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Designation already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUserDTO.EmployeeID;
            model.CompanyCode = loggedInUserDTO.CompanyCode;
            await _designationRepository.AddDesignation(model);
            _logger.LogInformation("Designation '{DesignationName}' created successfully.", model.Name);
        }

        public async Task ModifyDesignation(DesignationMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Designation ID should not be empty!");

            bool exists = await _designationRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Designation already exists!");

            var existingDesignation = await _designationRepository.GetDesignationById(model.ID);
            if (existingDesignation == null)
                throw new InvalidOperationException("Designation not found!");

            existingDesignation.Name = model.Name;
            existingDesignation.Description = model.Description;
            existingDesignation.RoleID = model.RoleID;
            existingDesignation.ModifiedOn = DateTime.UtcNow;
            existingDesignation.ModifiedBy = loggedInUserDTO?.EmployeeID ?? 0;

            await _designationRepository.UpdateDesignation(existingDesignation);
            _logger.LogInformation("Designation '{DesignationName}' updated successfully.", model.Name);
        }

        public async Task RemoveDesignation(long id)
        {
            var existingDesignation = await _designationRepository.GetDesignationById(id);
            if (existingDesignation == null)
                throw new InvalidOperationException("Designation not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<DesignationMaster>(_context, id, "Designation");

            existingDesignation.IsActive = false;
            existingDesignation.ModifiedOn = DateTime.UtcNow;
            existingDesignation.ModifiedBy = loggedInUserDTO?.EmployeeID ?? 0;

            await _designationRepository.UpdateDesignation(existingDesignation);
            _logger.LogInformation("Designation with ID '{DesignationId}' deleted successfully.", id);
        }

        public async Task<DesignationMaster> GetDesignationDetails(long id)
        {
            var classification = await _designationRepository.GetDesignationById(id);
            if (classification == null)
                throw new InvalidOperationException("Designation not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchDesignationList(PageFilter filter)
        {
            return await _designationRepository.GetAllDesignations(filter);
        }

        public async Task<List<DropdwonSelector>> GetDesignationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _designationRepository.GetDesignationDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
