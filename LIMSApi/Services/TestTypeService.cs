using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TestTypeService : ITestTypeService
    {
        private readonly ITestTypeRepository _testTypeRepository;
        private readonly ILogger<TestTypeService> _logger;

        public TestTypeService(ITestTypeRepository testTypeRepo, ILogger<TestTypeService> logger)
        {
            _testTypeRepository = testTypeRepo;
            _logger = logger;
        }

        public async Task CreateTestType(TestTypeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("TestType name should not be empty!");

            bool exists = await _testTypeRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("TestType already exists!");

            await _testTypeRepository.AddTestType(model);
            _logger.LogInformation("TestType '{TestTypeName}' created successfully.", model.Name);
        }

        public async Task ModifyTestType(TestTypeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("TestType ID should not be empty!");

            bool exists = await _testTypeRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same TestType already exists!");

            var existingTestType = await _testTypeRepository.GetTestTypeById(model.ID);
            if (existingTestType == null)
                throw new InvalidOperationException("TestType not found!");

            existingTestType.Name = model.Name;;
            existingTestType.ModifiedOn = DateTime.UtcNow;

            await _testTypeRepository.UpdateTestType(existingTestType);
            _logger.LogInformation("TestType '{TestTypeName}' updated successfully.", model.Name);
        }

        public async Task RemoveTestType(long id)
        {
            var existingTestType = await _testTypeRepository.GetTestTypeById(id);
            if (existingTestType == null)
                throw new InvalidOperationException("TestType not found!");

            existingTestType.IsActive = false;
            existingTestType.ModifiedOn = DateTime.UtcNow;

            await _testTypeRepository.UpdateTestType(existingTestType);
            _logger.LogInformation("TestType with ID '{TestTypeId}' deleted successfully.", id);
        }

        public async Task<TestTypeMaster> GetTestTypeDetails(long id)
        {
            var classification = await _testTypeRepository.GetTestTypeById(id);
            if (classification == null)
                throw new InvalidOperationException("TestType not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchTestTypeList(PageFilter filter)
        {
            return await _testTypeRepository.GetAllTestTypes(filter);
        }

        public async Task<List<DropdwonSelector>> GetTestTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _testTypeRepository.GetTestTypeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
