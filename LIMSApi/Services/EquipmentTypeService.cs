using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class EquipmentTypeService : IEquipmentTypeService
    {
        private readonly IEquipmentTypeRepository _equipmentTypeRepository;
        private readonly ILogger<EquipmentTypeService> _logger;

        public EquipmentTypeService(IEquipmentTypeRepository equipmentRepo, ILogger<EquipmentTypeService> logger)
        {
            _equipmentTypeRepository = equipmentRepo;
            _logger = logger;
        }

        public async Task CreateEquipmentType(EquipmentTypeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("EquipmentType name should not be empty!");

            bool exists = await _equipmentTypeRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("EquipmentType already exists!");

            await _equipmentTypeRepository.AddEquipmentType(model);
            _logger.LogInformation("EquipmentType '{EquipmentTypeName}' created successfully.", model.Name);
        }

        public async Task ModifyEquipmentType(EquipmentTypeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("EquipmentType ID should not be empty!");

            bool exists = await _equipmentTypeRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same EquipmentType already exists!");

            var existingEquipmentType = await _equipmentTypeRepository.GetEquipmentTypeById(model.ID);
            if (existingEquipmentType == null)
                throw new InvalidOperationException("EquipmentType not found!");

            existingEquipmentType.Name = model.Name;
            existingEquipmentType.Description = model.Description;
            existingEquipmentType.ModifiedOn = DateTime.UtcNow;

            await _equipmentTypeRepository.UpdateEquipmentType(existingEquipmentType);
            _logger.LogInformation("EquipmentType '{EquipmentTypeName}' updated successfully.", model.Name);
        }

        public async Task RemoveEquipmentType(long id)
        {
            var existingEquipmentType = await _equipmentTypeRepository.GetEquipmentTypeById(id);
            if (existingEquipmentType == null)
                throw new InvalidOperationException("EquipmentType not found!");

            existingEquipmentType.IsActive = false;
            existingEquipmentType.ModifiedOn = DateTime.UtcNow;

            await _equipmentTypeRepository.UpdateEquipmentType(existingEquipmentType);
            _logger.LogInformation("EquipmentType with ID '{EquipmentTypeId}' deleted successfully.", id);
        }

        public async Task<EquipmentTypeMaster> GetEquipmentTypeDetails(long id)
        {
            var classification = await _equipmentTypeRepository.GetEquipmentTypeById(id);
            if (classification == null)
                throw new InvalidOperationException("EquipmentType not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchEquipmentTypeList(PageFilter filter)
        {
            return await _equipmentTypeRepository.GetAllEquipmentTypes(filter);
        }

        public async Task<List<DropdwonSelector>> GetEquipmentTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _equipmentTypeRepository.GetEquipmentTypeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
