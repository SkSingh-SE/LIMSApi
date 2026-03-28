using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class EquipmentReferenceMaterialService : IEquipmentReferenceMaterialService
    {
        private readonly IEquipmentReferenceMaterialRepository _repository;
        private readonly ILogger<EquipmentReferenceMaterialService> _logger;

        public EquipmentReferenceMaterialService(
            IEquipmentReferenceMaterialRepository repository,
            ILogger<EquipmentReferenceMaterialService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<EquipmentReferenceMaterial>> GetByEquipment(long equipmentMasterID)
        {
            return await _repository.GetByEquipmentId(equipmentMasterID);
        }

        public async Task Create(EquipmentReferenceMaterial model)
        {
            if (string.IsNullOrWhiteSpace(model.MaterialName))
                throw new ArgumentException("Material name is required.");

            if (string.IsNullOrWhiteSpace(model.Type))
                throw new ArgumentException("Material type is required.");

            await _repository.Add(model);
            _logger.LogInformation("Reference material '{Name}' added for equipment {EquipmentId}.", model.MaterialName, model.EquipmentMasterID);
        }

        public async Task Delete(long id)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                throw new KeyNotFoundException("Reference material not found.");

            await _repository.Delete(id);
            _logger.LogInformation("Reference material {Id} deleted.", id);
        }
    }
}
