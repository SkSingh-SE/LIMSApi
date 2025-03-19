using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TestMethodStandardService : ITestMethodStandardService
    {
        private readonly ITestMethodStandardRepository _testMethodStandardRepository;
        private readonly ILogger<TestMethodStandardService> _logger;
        private LoggedInUserDTO loggedInUser;

        public TestMethodStandardService(ITestMethodStandardRepository testMethodStandardRepo, ILogger<TestMethodStandardService> logger)
        {
            _testMethodStandardRepository = testMethodStandardRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateTestMethodStandard(TestMethodStandard model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestMethodStandard name should not be empty!");

            bool exists = await _testMethodStandardRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("TestMethodStandard already exists!");

            model.Name = model.StandardOrganisationID + "_" + model.TestMethodCode + "_" + model.Year;
            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _testMethodStandardRepository.AddTestMethodStandard(model);
            _logger.LogInformation("TestMethodStandard '{TestMethodStandardName}' created successfully.", model.Name);
        }

        public async Task ModifyTestMethodStandard(TestMethodStandard model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestMethodStandard ID should not be empty!");

            bool exists = await _testMethodStandardRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same TestMethodStandard already exists!");

            var existingTestMethodStandard = await _testMethodStandardRepository.GetTestMethodStandardById(model.ID);
            if (existingTestMethodStandard == null)
                throw new InvalidOperationException("TestMethodStandard not found!");


            existingTestMethodStandard.Name = model.Name;
            existingTestMethodStandard.Description = model.Description;
            existingTestMethodStandard.Caption = model.Caption;
            existingTestMethodStandard.Description = model.Description;
            existingTestMethodStandard.TestMethodCode = model.TestMethodCode;
            existingTestMethodStandard.Year = model.Year;
            existingTestMethodStandard.ModifiedOn = DateTime.UtcNow;
            existingTestMethodStandard.ModifiedBy = loggedInUser.EmployeeID;

            await _testMethodStandardRepository.UpdateTestMethodStandard(existingTestMethodStandard);
            _logger.LogInformation("TestMethodStandard '{TestMethodStandardName}' updated successfully.", model.Name);
        }

        public async Task RemoveTestMethodStandard(long id)
        {
            var existingTestMethodStandard = await _testMethodStandardRepository.GetTestMethodStandardById(id);
            if (existingTestMethodStandard == null)
                throw new InvalidOperationException("TestMethodStandard not found!");

            existingTestMethodStandard.IsActive = false;
            existingTestMethodStandard.ModifiedOn = DateTime.UtcNow;
            existingTestMethodStandard.ModifiedBy = loggedInUser.EmployeeID;

            await _testMethodStandardRepository.UpdateTestMethodStandard(existingTestMethodStandard);
            _logger.LogInformation("TestMethodStandard with ID '{TestMethodStandardId}' deleted successfully.", id);
        }

        public async Task<TestMethodStandard> GetTestMethodStandardDetails(long id)
        {
            var classification = await _testMethodStandardRepository.GetTestMethodStandardById(id);
            if (classification == null)
                throw new InvalidOperationException("TestMethodStandard not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchTestMethodStandardList(PageFilter filter)
        {
            return await _testMethodStandardRepository.GetAllTestMethodStandards(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodStandardDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testMethodStandardRepository.GetTestMethodStandardDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
