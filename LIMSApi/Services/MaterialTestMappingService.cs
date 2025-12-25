using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class MaterialTestMappingService : IMaterialTestMappingService
    {
        private readonly IMaterialTestMappingRepository _repository;
        private readonly ILogger<MaterialTestMappingService> _logger;
        private readonly LoggedInUserDTO _loggedInUser;

        public MaterialTestMappingService(IMaterialTestMappingRepository repository, ILogger<MaterialTestMappingService> logger)
        {
            _repository = repository;
            _logger = logger;
            _loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task<PagedResponse<object>> GetAllMappings(PageFilter filter)
        {
            return await _repository.GetAllAsync(filter);
        }

        public async Task<MaterialTestMapping?> GetMapping(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateMapping(MaterialTestMapping dto)
        {
            var exists = await _repository.ExistsAsync(dto.MetalClassificationID, dto.ProductConditionID, dto.GradeID);
            if (exists)
                throw new InvalidOperationException("Mapping already exists for this Material/Grade/Condition combination.");


            await _repository.AddAsync(dto);
            _logger.LogInformation("New MaterialTestMapping created: {@Mapping}", dto);
        }

        public async Task UpdateMapping(MaterialTestMapping dto)
        {
            var mapping = await _repository.GetByIdAsync(dto.ID);
            if (mapping == null)
                throw new KeyNotFoundException("Mapping not found.");

            mapping.MetalClassificationID = dto.MetalClassificationID;
            mapping.ProductConditionID = dto.ProductConditionID;
            mapping.GradeID = dto.GradeID;
            mapping.IsDefault = dto.IsDefault;

            if (dto.LaboratoryTests.Any())
            {
                mapping.LaboratoryTests.Clear();
                foreach (var test in dto.LaboratoryTests)
                {
                    test.TestMappingID = mapping.ID;
                    mapping.LaboratoryTests.Add(test);
                }
            }

            await _repository.UpdateAsync(mapping);
            _logger.LogInformation("MaterialTestMapping updated: {@Mapping}", mapping);
        }

        public async Task<List<DropdwonSelector>> GetSuggestedTestsAsync(TestSuggestionRequest request)
        {
            return await _repository.GetSuggestedTestsAsync(request);
        }

        public async Task<List<DropdwonSelector>> GetSuggestedGradeAsync(GradeSuggestionRequest request)
        {
            return await _repository.GetSuggestedGradeAsync(request);
        }
    }
}
