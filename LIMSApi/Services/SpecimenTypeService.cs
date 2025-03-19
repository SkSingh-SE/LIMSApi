using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SpecimenTypeService : ISpecimenTypeService
    {
        private readonly ISpecimenTypeRepository _specimenRepository;
        private readonly ILogger<SpecimenTypeService> _logger;

        public SpecimenTypeService(ISpecimenTypeRepository specimenRepository, ILogger<SpecimenTypeService> logger)
        {
            _specimenRepository = specimenRepository;
            _logger = logger;
        }

        public async Task CreateSpecimenType(SpecimenTypeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("SpecimenType name should not be empty!");

            bool exists = await _specimenRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("SpecimenType already exists!");

            await _specimenRepository.AddSpecimenType(model);
            _logger.LogInformation("SpecimenType '{SpecimenTypeName}' created successfully.", model.Name);
        }

        public async Task ModifySpecimenType(SpecimenTypeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SpecimenType ID should not be empty!");

            bool exists = await _specimenRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same SpecimenType already exists!");

            var existingSpecimenType = await _specimenRepository.GetSpecimenTypeById(model.ID);
            if (existingSpecimenType == null)
                throw new InvalidOperationException("SpecimenType not found!");

            existingSpecimenType.Name = model.Name;
            existingSpecimenType.ModifiedOn = DateTime.UtcNow;

            await _specimenRepository.UpdateSpecimenType(existingSpecimenType);
            _logger.LogInformation("SpecimenType '{SpecimenTypeName}' updated successfully.", model.Name);
        }

        public async Task<SpecimenTypeMaster> RemoveSpecimenType(long id)
        {
            var existingSpecimenType = await _specimenRepository.GetSpecimenTypeById(id);
            if (existingSpecimenType == null)
                throw new InvalidOperationException("SpecimenType not found!");

            existingSpecimenType.IsActive = false;
            existingSpecimenType.ModifiedOn = DateTime.UtcNow;

            await _specimenRepository.UpdateSpecimenType(existingSpecimenType);
            _logger.LogInformation("SpecimenType with ID '{SpecimenTypeId}' deleted successfully.", id);
            return existingSpecimenType;
        }

        public async Task<SpecimenTypeMaster> GetSpecimenTypeDetails(long id)
        {
            var classification = await _specimenRepository.GetSpecimenTypeById(id);
            if (classification == null)
                throw new InvalidOperationException("SpecimenType not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSpecimenTypeList(PageFilter filter)
        {
            return await _specimenRepository.GetAllSpecimenTypes(filter);
        }

        public async Task<List<DropdwonSelector>> GetSpecimenTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _specimenRepository.GetSpecimenTypeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
