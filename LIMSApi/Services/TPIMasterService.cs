using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TPIMasterService : ITPIMasterService
    {
        private readonly ITPIMasterRepository _itemRepository;
        private readonly ILogger<TPIMasterService> _logger;
        private LoggedInUserDTO loggedInUser;

        public TPIMasterService(ITPIMasterRepository itemMasterRepository, ILogger<TPIMasterService> logger)
        {
            _itemRepository = itemMasterRepository;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateTPI(TPIMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.AgencyName))
                throw new ArgumentException("TPIMaster name should not be empty!");

            bool exists = await _itemRepository.ExistsByName(model.AgencyName);
            if (exists)
                throw new InvalidOperationException("TPIMaster already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _itemRepository.AddTPI(model);
            _logger.LogInformation("TPIMaster '{TPIMasterName}' created successfully.", model.AgencyName);
        }

        public async Task ModifyTPI(TPIMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TPIMaster ID should not be empty!");

            bool exists = await _itemRepository.ExistsByNameAndNotId(model.AgencyName, model.ID);
            if (exists)
                throw new InvalidOperationException("Same TPIMaster already exists!");

            var existingTPIMaster = await _itemRepository.GetTPIById(model.ID);
            if (existingTPIMaster == null)
                throw new InvalidOperationException("TPIMaster not found!");


            existingTPIMaster.AgencyName = model.AgencyName;
            existingTPIMaster.EmailId = model.EmailId;
            existingTPIMaster.ContactNo = model.ContactNo;
            existingTPIMaster.ModifiedOn = DateTime.UtcNow;
            existingTPIMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.UpdateTPI(existingTPIMaster);
            _logger.LogInformation("TPIMaster '{TPIMasterName}' updated successfully.", model.AgencyName);
        }

        public async Task RemoveTPI(long id)
        {
            var existingTPIMaster = await _itemRepository.GetTPIById(id);
            if (existingTPIMaster == null)
                throw new InvalidOperationException("TPIMaster not found!");

            existingTPIMaster.IsActive = false;
            existingTPIMaster.ModifiedOn = DateTime.UtcNow;
            existingTPIMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.DeleteTPI(existingTPIMaster);
            _logger.LogInformation("TPIMaster with ID '{TPIMasterId}' deleted successfully.", id);
        }

        public async Task<TPIMaster> GetTPIDetails(long id)
        {
            var classification = await _itemRepository.GetTPIById(id);
            if (classification == null)
                throw new InvalidOperationException("TPIMaster not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchTPIList(PageFilter filter)
        {
            return await _itemRepository.GetAllTPIs(filter);
        }

        public async Task<List<DropdwonSelector>> GetTPIDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _itemRepository.GetTPIDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
