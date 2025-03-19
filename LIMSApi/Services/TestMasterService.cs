using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TestMasterService : ITestMasterService
    {
        private readonly ITestMasterRepository _testMasterRepository;
        private readonly ILogger<TestMasterService> _logger;
        private LoggedInUserDTO loggedInUser;

        public TestMasterService(ITestMasterRepository testMasterRepo, ILogger<TestMasterService> logger)
        {
            _testMasterRepository = testMasterRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateTestMaster(TestMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestMaster name should not be empty!");

            bool exists = await _testMasterRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("TestMaster already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _testMasterRepository.AddTestMaster(model);
            _logger.LogInformation("TestMaster '{TestMasterName}' created successfully.", model.Name);
        }

        public async Task ModifyTestMaster(TestMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestMaster ID should not be empty!");

            bool exists = await _testMasterRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same TestMaster already exists!");

            var existingTestMaster = await _testMasterRepository.GetTestMasterById(model.ID);
            if (existingTestMaster == null)
                throw new InvalidOperationException("TestMaster not found!");


            existingTestMaster.Name = model.Name;
            existingTestMaster.TestCaption = model.TestCaption;
            existingTestMaster.TestDuration = model.TestDuration;
            existingTestMaster.InvoiceCaption = model.InvoiceCaption;
            existingTestMaster.LabDepartmentID = model.LabDepartmentID;
            existingTestMaster.ModifiedOn = DateTime.UtcNow;
            existingTestMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _testMasterRepository.UpdateTestMaster(existingTestMaster);
            _logger.LogInformation("TestMaster '{TestMasterName}' updated successfully.", model.Name);
        }

        public async Task RemoveTestMaster(long id)
        {
            var existingTestMaster = await _testMasterRepository.GetTestMasterById(id);
            if (existingTestMaster == null)
                throw new InvalidOperationException("TestMaster not found!");

            existingTestMaster.IsActive = false;
            existingTestMaster.ModifiedOn = DateTime.UtcNow;
            existingTestMaster.ModifiedBy = loggedInUser.EmployeeID;

            await _testMasterRepository.UpdateTestMaster(existingTestMaster);
            _logger.LogInformation("TestMaster with ID '{TestMasterId}' deleted successfully.", id);
        }

        public async Task<TestMaster> GetTestMasterDetails(long id)
        {
            var classification = await _testMasterRepository.GetTestMasterById(id);
            if (classification == null)
                throw new InvalidOperationException("TestMaster not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchTestMasterList(PageFilter filter)
        {
            return await _testMasterRepository.GetAllTestMasters(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testMasterRepository.GetTestMasterDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
