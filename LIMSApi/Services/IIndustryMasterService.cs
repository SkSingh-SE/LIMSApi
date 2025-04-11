using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class IndustryMasterService : IIndustryMasterService
    {
        private readonly IIndustryMasterRepository _itemRepository;
        private readonly ILogger<IndustryMasterService> _logger;
        private LoggedInUserDTO loggedInUser;

        public IndustryMasterService(IIndustryMasterRepository itemMasterRepository, ILogger<IndustryMasterService> logger)
        {
            _itemRepository = itemMasterRepository;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateIndustry(IndustryMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("IndustryMaster name should not be empty!");

            bool exists = await _itemRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("IndustryMaster already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _itemRepository.AddIndustry(model);
            _logger.LogInformation("IndustryMaster '{IndustryMasterName}' created successfully.", model.Name);
        }

        public async Task ModifyIndustry(IndustryMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("IndustryMaster ID should not be empty!");

            bool exists = await _itemRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same IndustryMaster already exists!");

            var existingIndustryMaster = await _itemRepository.GetIndustryById(model.ID);
            if (existingIndustryMaster == null)
                throw new InvalidOperationException("IndustryMaster not found!");


            existingIndustryMaster.Name = model.Name;
            existingIndustryMaster.Description = model.Description;
            existingIndustryMaster.ModifiedOn = DateTime.UtcNow;
            existingIndustryMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.UpdateIndustry(existingIndustryMaster);
            _logger.LogInformation("IndustryMaster '{IndustryMasterName}' updated successfully.", model.Name);
        }

        public async Task RemoveIndustry(long id)
        {
            var existingIndustryMaster = await _itemRepository.GetIndustryById(id);
            if (existingIndustryMaster == null)
                throw new InvalidOperationException("IndustryMaster not found!");

            existingIndustryMaster.IsActive = false;
            existingIndustryMaster.ModifiedOn = DateTime.UtcNow;
            existingIndustryMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _itemRepository.DeleteIndustry(existingIndustryMaster);
            _logger.LogInformation("IndustryMaster with ID '{IndustryMasterId}' deleted successfully.", id);
        }

        public async Task<IndustryMaster> GetIndustryDetails(long id)
        {
            var classification = await _itemRepository.GetIndustryById(id);
            if (classification == null)
                throw new InvalidOperationException("IndustryMaster not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchIndustryList(PageFilter filter)
        {
            return await _itemRepository.GetAllIndustrys(filter);
        }

        public async Task<List<DropdwonSelector>> GetIndustryDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _itemRepository.GetIndustryDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
