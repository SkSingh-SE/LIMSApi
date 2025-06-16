using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SubGroupService : ISubGroupService
    {
        private readonly ISubGroupRepository _subGroupRepository;
        private readonly ILogger<SubGroupService> _logger;
        private LoggedInUserDTO loggedInUser;

        public SubGroupService(ISubGroupRepository SubGroupRepo, ILogger<SubGroupService> logger)
        {
            _subGroupRepository = SubGroupRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateSubGroup(SubGroupMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("SubGroup name should not be empty!");

            bool exists = await _subGroupRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("SubGroup already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _subGroupRepository.AddSubGroup(model);
            _logger.LogInformation("SubGroup '{SubGroupName}' created successfully.", model.Name);
        }

        public async Task ModifySubGroup(SubGroupMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SubGroup ID should not be empty!");

            bool exists = await _subGroupRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same SubGroup already exists!");

            var existingSubGroup = await _subGroupRepository.GetSubGroupById(model.ID);
            if (existingSubGroup == null)
                throw new InvalidOperationException("SubGroup not found!");


            existingSubGroup.Name = model.Name;
            existingSubGroup.Description = model.Description;
            existingSubGroup.ModifiedOn = DateTime.UtcNow;
            existingSubGroup.ModifiedBy = loggedInUser.EmployeeID;
            existingSubGroup.GroupID = model.GroupID;

            await _subGroupRepository.UpdateSubGroup(existingSubGroup);
            _logger.LogInformation("SubGroup '{SubGroupName}' updated successfully.", model.Name);
        }

        public async Task RemoveSubGroup(long id)
        {
            var existingSubGroup = await _subGroupRepository.GetSubGroupById(id);
            if (existingSubGroup == null)
                throw new InvalidOperationException("SubGroup not found!");

            existingSubGroup.IsActive = false;
            existingSubGroup.ModifiedOn = DateTime.UtcNow;
            existingSubGroup.ModifiedBy = loggedInUser.EmployeeID;

            await _subGroupRepository.DeleteSubGroup(existingSubGroup);
            _logger.LogInformation("SubGroup with ID '{SubGroupId}' deleted successfully.", id);
        }

        public async Task<SubGroupMaster> GetSubGroupDetails(long id)
        {
            var classification = await _subGroupRepository.GetSubGroupById(id);
            if (classification == null)
                throw new InvalidOperationException("SubGroup not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSubGroupList(PageFilter filter)
        {
            return await _subGroupRepository.GetAllSubGroups(filter);
        }

        public async Task<List<DropdwonSelector>> GetSubGroupDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _subGroupRepository.GetSubGroupDropdown(searchTerm, pageNo, pageSize);
        } 
        public async Task<List<DropdwonSelector>> GetDropdownByGroupId(long id)
        {
            return await _subGroupRepository.GetDropdownByGroupId(id);
        }
    }
}
