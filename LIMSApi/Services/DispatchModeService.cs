using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class DispatchModeService : IDispatchModeService
    {
        private readonly IDispatchModeRepository _customerTypeRepository;
        private readonly ILogger<DispatchModeService> _logger;
        private LoggedInUserDTO loggedInUser;

        public DispatchModeService(IDispatchModeRepository DispatchModeRepo, ILogger<DispatchModeService> logger)
        {
            _customerTypeRepository = DispatchModeRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateDispatchMode(DispatchModeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("DispatchMode name should not be empty!");

            bool exists = await _customerTypeRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("DispatchMode already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _customerTypeRepository.AddDispatchMode(model);
            _logger.LogInformation("DispatchMode '{DispatchModeName}' created successfully.", model.Name);
        }

        public async Task ModifyDispatchMode(DispatchModeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("DispatchMode ID should not be empty!");

            bool exists = await _customerTypeRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same DispatchMode already exists!");

            var existingDispatchMode = await _customerTypeRepository.GetDispatchModeById(model.ID);
            if (existingDispatchMode == null)
                throw new InvalidOperationException("DispatchMode not found!");


            existingDispatchMode.Name = model.Name;
            existingDispatchMode.Description = model.Description;
            existingDispatchMode.ModifiedOn = DateTime.UtcNow;
            existingDispatchMode.ModifiedBy = loggedInUser.EmployeeID;

            await _customerTypeRepository.UpdateDispatchMode(existingDispatchMode);
            _logger.LogInformation("DispatchMode '{DispatchModeName}' updated successfully.", model.Name);
        }

        public async Task RemoveDispatchMode(long id)
        {
            var existingDispatchMode = await _customerTypeRepository.GetDispatchModeById(id);
            if (existingDispatchMode == null)
                throw new InvalidOperationException("DispatchMode not found!");

            existingDispatchMode.IsActive = false;
            existingDispatchMode.ModifiedOn = DateTime.UtcNow;
            existingDispatchMode.ModifiedBy = loggedInUser.EmployeeID;

            await _customerTypeRepository.DeleteDispatchMode(existingDispatchMode);
            _logger.LogInformation("DispatchMode with ID '{DispatchModeId}' deleted successfully.", id);
        }

        public async Task<DispatchModeMaster> GetDispatchModeDetails(long id)
        {
            var classification = await _customerTypeRepository.GetDispatchModeById(id);
            if (classification == null)
                throw new InvalidOperationException("DispatchMode not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchDispatchModeList(PageFilter filter)
        {
            return await _customerTypeRepository.GetAllDispatchModes(filter);
        }

        public async Task<List<DropdwonSelector>> GetDispatchModeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _customerTypeRepository.GetDispatchModeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
