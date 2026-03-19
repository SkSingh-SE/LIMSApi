using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class LaboratoryTestService : ILaboratoryTestService
    {
        private readonly ILaboratoryTestRepository _testMethodRepository;
        private readonly ILogger<LaboratoryTestService> _logger;
        private LoggedInUserDTO loggedInUser;

        public LaboratoryTestService(ILaboratoryTestRepository testMethodRepo, ILogger<LaboratoryTestService> logger)
        {
            _testMethodRepository = testMethodRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateTestMethod(LaboratoryTest model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestMethod name should not be empty!");

            bool exists = await _testMethodRepository.ExistsByName(model.SubGroup);
            if (exists)
                throw new InvalidOperationException($"SubGroup {model.SubGroup} already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _testMethodRepository.AddTestMethod(model);
            _logger.LogInformation("SubGroup '{SubGroup}' created successfully.", model.SubGroup);
        }

        public async Task ModifyTestMethod(LaboratoryTest model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestMethod ID should not be empty!");

            if (await _testMethodRepository.ExistsByNameAndNotId(model.SubGroup, model.ID))
                throw new InvalidOperationException($"Same Group {model.SubGroup} already exists!");

            var existingTestMethod = await _testMethodRepository.GetTestMethodById(model.ID);
            if (existingTestMethod == null)
                throw new InvalidOperationException("Laboratory Test not found!");

            existingTestMethod.Name = model.Name;
            existingTestMethod.LabDepartmentID = model.LabDepartmentID;
            existingTestMethod.SubGroup = model.SubGroup;
            existingTestMethod.Equation = model.Equation;
            existingTestMethod.TestCaption = model.TestCaption;
            existingTestMethod.InvoiceCaption = model.InvoiceCaption;
            existingTestMethod.TestDuration = model.TestDuration;
            existingTestMethod.ModifiedOn = DateTime.UtcNow;
            existingTestMethod.ModifiedBy = loggedInUser.EmployeeID;

            existingTestMethod.InvoiceCases.Clear();
            foreach (var invoiceCase in model.InvoiceCases)
            {
                invoiceCase.LabTestID = model.ID;
                existingTestMethod.InvoiceCases.Add(invoiceCase);
            }

            await _testMethodRepository.UpdateTestMethod(existingTestMethod);
            _logger.LogInformation("SubGroup '{SubGroup}' updated successfully.", model.SubGroup);
        }

        public async Task RemoveTestMethod(long id)
        {
            var existingTestMethod = await _testMethodRepository.GetTestMethodById(id);
            if (existingTestMethod == null)
                throw new InvalidOperationException("Laboratory Test not found!");

            existingTestMethod.IsActive = false;
            existingTestMethod.ModifiedOn = DateTime.UtcNow;
            existingTestMethod.ModifiedBy = loggedInUser.EmployeeID;

            await _testMethodRepository.UpdateTestMethod(existingTestMethod);
            _logger.LogInformation("Laboratory Test with ID '{TestMethodId}' deleted successfully.", id);
        }

        public async Task<LaboratoryTest> GetTestMethodDetails(long id)
        {
            var existingTestMethod = await _testMethodRepository.GetTestMethodById(id);
            if (existingTestMethod == null)
                throw new InvalidOperationException("Laboratory Test not found!");

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
        public async Task<List<DropdwonSelector>> GetChemicalTestMethodDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testMethodRepository.GetChemicalTestMethodDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<object>> GetTestCases(long labTestId)
        {
            return await _testMethodRepository.GetTestCases(labTestId);
        }

        public async Task<List<string>> GetDistinctTestNames(string? searchTerm, int pageSize)
        {
            return await _testMethodRepository.GetDistinctTestNames(searchTerm, pageSize);
        }
    }
}
