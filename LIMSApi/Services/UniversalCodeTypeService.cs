using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class UniversalCodeTypeService : IUniversalCodeTypeService
    {
        private readonly IUniversalCodeTypeRepository _universalCodeTypeRepository;
        private readonly ILogger<UniversalCodeTypeService> _logger;

        public UniversalCodeTypeService(IUniversalCodeTypeRepository universalRepo, ILogger<UniversalCodeTypeService> logger)
        {
            _universalCodeTypeRepository = universalRepo;
            _logger = logger;
        }

        public async Task CreateUniversalCodeType(UniversalCodeTypeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("UniversalCodeType name should not be empty!");

            bool exists = await _universalCodeTypeRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("UniversalCodeType already exists!");

            await _universalCodeTypeRepository.AddUniversalCodeType(model);
            _logger.LogInformation("UniversalCodeType '{UniversalCodeTypeName}' created successfully.", model.Name);
        }

        public async Task ModifyUniversalCodeType(UniversalCodeTypeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("UniversalCodeType ID should not be empty!");

            bool exists = await _universalCodeTypeRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same UniversalCodeType already exists!");

            var existingUniversalCodeType = await _universalCodeTypeRepository.GetUniversalCodeTypeById(model.ID);
            if (existingUniversalCodeType == null)
                throw new InvalidOperationException("UniversalCodeType not found!");

            existingUniversalCodeType.Name = model.Name;
            existingUniversalCodeType.ModifiedOn = DateTime.UtcNow;

            await _universalCodeTypeRepository.UpdateUniversalCodeType(existingUniversalCodeType);
            _logger.LogInformation("UniversalCodeType '{UniversalCodeTypeName}' updated successfully.", model.Name);
        }

        public async Task RemoveUniversalCodeType(long id)
        {
            var existingUniversalCodeType = await _universalCodeTypeRepository.GetUniversalCodeTypeById(id);
            if (existingUniversalCodeType == null)
                throw new InvalidOperationException("UniversalCodeType not found!");

            existingUniversalCodeType.IsActive = false;
            existingUniversalCodeType.ModifiedOn = DateTime.UtcNow;

            await _universalCodeTypeRepository.UpdateUniversalCodeType(existingUniversalCodeType);
            _logger.LogInformation("UniversalCodeType with ID '{UniversalCodeTypeId}' deleted successfully.", id);
        }

        public async Task<UniversalCodeTypeMaster> GetUniversalCodeTypeDetails(long id)
        {
            var classification = await _universalCodeTypeRepository.GetUniversalCodeTypeById(id);
            if (classification == null)
                throw new InvalidOperationException("UniversalCodeType not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchUniversalCodeTypeList(PageFilter filter)
        {
            return await _universalCodeTypeRepository.GetAllUniversalCodeTypes(filter);
        }

        public async Task<List<DropdwonSelector>> GetUniversalCodeTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _universalCodeTypeRepository.GetUniversalCodeTypeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
