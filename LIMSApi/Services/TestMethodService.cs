using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TestMethodService : ITestMethodService
    {
        private readonly ITestMethodRepository _testMethodRepository;
        private readonly ILogger<TestMethodService> _logger;
        private LoggedInUserDTO loggedInUser;

        public TestMethodService(ITestMethodRepository testMethodRepo, ILogger<TestMethodService> logger)
        {
            _testMethodRepository = testMethodRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateTestMethod(TestMethodMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestMethod name should not be empty!");

            bool exists = await _testMethodRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("TestMethod already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _testMethodRepository.AddTestMethod(model);
            _logger.LogInformation("TestMethod '{TestMethodName}' created successfully.", model.Name);
        }

        public async Task ModifyTestMethod(TestMethodMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestMethod ID should not be empty!");

            bool exists = await _testMethodRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same TestMethod already exists!");

            var existingTestMethod = await _testMethodRepository.GetTestMethodById(model.ID);
            if (existingTestMethod == null)
                throw new InvalidOperationException("TestMethod not found!");

            existingTestMethod.Name = model.Name;
            existingTestMethod.Caption = model.Caption;
            existingTestMethod.LabDepartmentID = model.LabDepartmentID;
            existingTestMethod.ModifiedOn = DateTime.UtcNow;
            existingTestMethod.ModifiedBy = loggedInUser.EmployeeID;

            // remove unwanted mappings
            if (existingTestMethod.SubGroups.Any())
            {
                var subGroupToRemove = existingTestMethod.SubGroups.Where(sub => !model.SubGroups.Any(m => m.ID == sub.ID)).ToList();
                foreach (var subGroup in subGroupToRemove)
                {
                    existingTestMethod.SubGroups.Remove(subGroup);
                }
            }

            if (model.SubGroups != null && model.SubGroups.Any())
            {
                // Add or update mappings
                foreach (var subGroup in model.SubGroups)
                {
                    subGroup.TestMethodID = model.ID;

                    var existingSubGroups = existingTestMethod.SubGroups
                        .FirstOrDefault(m => m.ID == subGroup.ID);

                    if (existingSubGroups != null)
                    {
                        existingSubGroups.TestMethodID = model.ID;
                        existingSubGroups.Name = subGroup.Name;
                        existingSubGroups.InvoiceCase = subGroup.InvoiceCase;
                        existingSubGroups.FixedTimeDuration = subGroup.FixedTimeDuration;
                        existingSubGroups.SampleSize = subGroup.SampleSize;
                        existingSubGroups.TestCharge = subGroup.TestCharge;

                    }
                    else
                    {
                        existingTestMethod.SubGroups.Add(subGroup);
                    }
                }
            }

            await _testMethodRepository.UpdateTestMethod(existingTestMethod);
            _logger.LogInformation("TestMethod '{TestMethodName}' updated successfully.", model.Name);
        }

        public async Task RemoveTestMethod(long id)
        {
            var existingTestMethod = await _testMethodRepository.GetTestMethodById(id);
            if (existingTestMethod == null)
                throw new InvalidOperationException("TestMethod not found!");

            existingTestMethod.IsActive = false;
            existingTestMethod.ModifiedOn = DateTime.UtcNow;
            existingTestMethod.ModifiedBy = loggedInUser.EmployeeID;

            await _testMethodRepository.UpdateTestMethod(existingTestMethod);
            _logger.LogInformation("TestMethod with ID '{TestMethodId}' deleted successfully.", id);
        }

        public async Task<TestMethodMaster> GetTestMethodDetails(long id)
        {
            var existingTestMethod = await _testMethodRepository.GetTestMethodById(id);
            if (existingTestMethod == null)
                throw new InvalidOperationException("TestMethod not found!");

            return existingTestMethod;
        }

        public async Task<PagedResponse<object>> FetchTestMethodList(PageFilter filter)
        {
            return await _testMethodRepository.GetAllTestMethods(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testMethodRepository.GetTestMethodDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
