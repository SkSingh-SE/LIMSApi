using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger<GroupService> _logger;
        private LoggedInUserDTO loggedInUser;

        public GroupService(IGroupRepository GroupRepo, ILogger<GroupService> logger)
        {
            _groupRepository = GroupRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateGroup(GroupMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Group name should not be empty!");

            bool exists = await _groupRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Group already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _groupRepository.AddGroup(model);
            _logger.LogInformation("Group '{GroupName}' created successfully.", model.Name);
        }

        public async Task ModifyGroup(GroupMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Group ID should not be empty!");

            bool exists = await _groupRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Group already exists!");

            var existingGroup = await _groupRepository.GetGroupById(model.ID);
            if (existingGroup == null)
                throw new InvalidOperationException("Group not found!");


            existingGroup.Name = model.Name;
            existingGroup.Description = model.Description;
            existingGroup.ModifiedOn = DateTime.UtcNow;
            existingGroup.ModifiedBy = loggedInUser.EmployeeID;
            existingGroup.DisciplineID = model.DisciplineID;

            await _groupRepository.UpdateGroup(existingGroup);
            _logger.LogInformation("Group '{GroupName}' updated successfully.", model.Name);
        }

        public async Task RemoveGroup(long id)
        {
            var existingGroup = await _groupRepository.GetGroupById(id);
            if (existingGroup == null)
                throw new InvalidOperationException("Group not found!");

            existingGroup.IsActive = false;
            existingGroup.ModifiedOn = DateTime.UtcNow;
            existingGroup.ModifiedBy = loggedInUser.EmployeeID;

            await _groupRepository.DeleteGroup(existingGroup);
            _logger.LogInformation("Group with ID '{GroupId}' deleted successfully.", id);
        }

        public async Task<GroupMaster> GetGroupDetails(long id)
        {
            var classification = await _groupRepository.GetGroupById(id);
            if (classification == null)
                throw new InvalidOperationException("Group not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchGroupList(PageFilter filter)
        {
            return await _groupRepository.GetAllGroups(filter);
        }

        public async Task<List<DropdwonSelector>> GetGroupDropdown(string? searchTerm, int pageNo, int pageSize, long? id = null)
        {
            return await _groupRepository.GetGroupDropdown(searchTerm, pageNo, pageSize,id);
        }
    }
}
