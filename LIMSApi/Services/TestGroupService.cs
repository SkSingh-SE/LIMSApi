using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class TestGroupService : ITestGroupService
    {
        private readonly ITestGroupRepository _testGroupRepository;
        private readonly ILogger<TestGroupService> _logger;
        private LoggedInUserDTO loggedInUser;

        public TestGroupService(ITestGroupRepository testGroupRepo, ILogger<TestGroupService> logger)
        {
            _testGroupRepository = testGroupRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateTestGroup(TestGroup model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestGroup name should not be empty!");

            bool exists = await _testGroupRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("TestGroup already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _testGroupRepository.AddTestGroup(model);
            _logger.LogInformation("TestGroup '{TestGroupName}' created successfully.", model.Name);
        }

        public async Task ModifyTestGroup(TestGroup model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestGroup ID should not be empty!");

            bool exists = await _testGroupRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same TestGroup already exists!");

            var existingTestGroup = await _testGroupRepository.GetTestGroupById(model.ID);
            if (existingTestGroup == null)
                throw new InvalidOperationException("TestGroup not found!");


            existingTestGroup.Name = model.Name;
            existingTestGroup.Sample = model.Sample;
            existingTestGroup.ModifiedOn = DateTime.UtcNow;
            existingTestGroup.ModifiedBy = loggedInUser.EmployeeID;

            // remove unwanted mappings
            if (existingTestGroup.TestGroupMappings.Any())
            {
                var mappingsToRemove = existingTestGroup.TestGroupMappings.Where(mapping => !model.TestGroupMappings.Any(m => m.ID == mapping.ID)).ToList();
                foreach (var mapping in mappingsToRemove)
                {
                    existingTestGroup.TestGroupMappings.Remove(mapping);
                }
            }

            if (model.TestGroupMappings != null && model.TestGroupMappings.Any())
            {
                // Add or update mappings
                foreach (var mapping in model.TestGroupMappings)
                {
                    mapping.TestGroupID = model.ID;

                    var existingMapping = existingTestGroup.TestGroupMappings
                        .FirstOrDefault(m => m.ID == mapping.ID);

                    if (existingMapping != null)
                    {
                        existingMapping.TestGroupID = model.ID;
                        existingMapping.TestMethodID = mapping.TestMethodID;

                    }
                    else
                    {
                        existingTestGroup.TestGroupMappings.Add(mapping);
                    }
                }
            }
            await _testGroupRepository.UpdateTestGroup(existingTestGroup);

            _logger.LogInformation("TestGroup '{TestGroupName}' updated successfully.", model.Name);
        }

        public async Task RemoveTestGroup(long id)
        {
            var existingTestGroup = await _testGroupRepository.GetTestGroupById(id);
            if (existingTestGroup == null)
                throw new InvalidOperationException("TestGroup not found!");

            existingTestGroup.IsActive = false;
            existingTestGroup.ModifiedOn = DateTime.UtcNow;
            existingTestGroup.ModifiedBy = loggedInUser.EmployeeID;

            await _testGroupRepository.DeleteTestGroup(existingTestGroup);
            _logger.LogInformation("TestGroup with ID '{TestGroupId}' deleted successfully.", id);
        }

        public async Task<TestGroup> GetTestGroupDetails(long id)
        {
            var classification = await _testGroupRepository.GetTestGroupById(id);
            if (classification == null)
                throw new InvalidOperationException("TestGroup not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchTestGroupList(PageFilter filter)
        {
            return await _testGroupRepository.GetAllTestGroups(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestGroupDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testGroupRepository.GetTestGroupDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
