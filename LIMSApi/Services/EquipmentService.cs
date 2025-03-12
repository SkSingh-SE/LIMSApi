using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly ILogger<EquipmentService> _logger;

        public EquipmentService(IEquipmentRepository equipment, ILogger<EquipmentService> logger)
        {
            _equipmentRepository = equipment;
            _logger = logger;
        }

        public async Task CreateEquipment(EquipmentMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Equipment name should not be empty!");

            bool exists = await _equipmentRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Equipment already exists!");

            await _equipmentRepository.AddEquipment(model);
            _logger.LogInformation("Equipment '{EquipmentName}' created successfully.", model.Name);
        }

        public async Task ModifyEquipment(EquipmentMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Equipment ID should not be empty!");

            bool exists = await _equipmentRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Equipment already exists!");

            var existingEquipment = await _equipmentRepository.GetEquipmentById(model.ID);
            if (existingEquipment == null)
                throw new InvalidOperationException("Equipment not found!");

            existingEquipment.Name = model.Name;
            existingEquipment.ModifiedOn = DateTime.UtcNow;

            await _equipmentRepository.UpdateEquipment(existingEquipment);
            _logger.LogInformation("Equipment '{EquipmentName}' updated successfully.", model.Name);
        }

        public async Task RemoveEquipment(long id)
        {
            var existingEquipment = await _equipmentRepository.GetEquipmentById(id);
            if (existingEquipment == null)
                throw new InvalidOperationException("Equipment not found!");

            existingEquipment.IsActive = false;
            existingEquipment.ModifiedOn = DateTime.UtcNow;

            await _equipmentRepository.UpdateEquipment(existingEquipment);
            _logger.LogInformation("Equipment with ID '{EquipmentId}' deleted successfully.", id);
        }

        public async Task<EquipmentMaster> GetEquipmentDetails(long id)
        {
            var classification = await _equipmentRepository.GetEquipmentById(id);
            if (classification == null)
                throw new InvalidOperationException("Equipment not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchEquipmentList(PageFilter filter)
        {
            return await _equipmentRepository.GetAllEquipments(filter);
        }

        public async Task<List<DropdwonSelector>> GetEquipmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _equipmentRepository.GetEquipmentDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
