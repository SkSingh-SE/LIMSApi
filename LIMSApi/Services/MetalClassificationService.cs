using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class MetalClassificationService : IMetalClassificationService
    {
        private readonly IMetalClassificationRepository _MetalClassificationRepository;
        private readonly ILogger<MetalClassificationService> _logger;

        public MetalClassificationService(IMetalClassificationRepository MetalClassificationRepo, ILogger<MetalClassificationService> logger)
        {
            _MetalClassificationRepository = MetalClassificationRepo;
            _logger = logger;
        }

        public async Task CreateMetalClassification(MetalClassificationMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("MetalClassification name should not be empty!");

            bool exists = await _MetalClassificationRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("MetalClassification already exists!");

            await _MetalClassificationRepository.AddMetalClassification(model);
            _logger.LogInformation("MetalClassification '{MetalClassificationName}' created successfully.", model.Name);
        }

        public async Task ModifyMetalClassification(MetalClassificationMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("MetalClassification ID should not be empty!");

            bool exists = await _MetalClassificationRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same MetalClassification already exists!");

            var existingMetalClassification = await _MetalClassificationRepository.GetMetalClassificationById(model.ID);
            if (existingMetalClassification == null)
                throw new InvalidOperationException("MetalClassification not found!");

            existingMetalClassification.Name = model.Name;
            existingMetalClassification.Code = model.Code;
            existingMetalClassification.ParentID = model.ParentID;
            existingMetalClassification.HasChemicalParams = model.HasChemicalParams;
            existingMetalClassification.HasMechanicalParams = model.HasMechanicalParams;
            existingMetalClassification.SortOrder = model.SortOrder;
            existingMetalClassification.ModifiedOn = DateTime.UtcNow;

            existingMetalClassification?.Parameters?.Clear();
            existingMetalClassification.Parameters = model.Parameters;

            await _MetalClassificationRepository.UpdateMetalClassification(existingMetalClassification);
            _logger.LogInformation("MetalClassification '{MetalClassificationName}' updated successfully.", model.Name);
        }

        public async Task RemoveMetalClassification(long id)
        {
            var existingMetalClassification = await _MetalClassificationRepository.GetMetalClassificationById(id);
            if (existingMetalClassification == null)
                throw new InvalidOperationException("MetalClassification not found!");

            existingMetalClassification.IsActive = false;
            existingMetalClassification.ModifiedOn = DateTime.UtcNow;

            await _MetalClassificationRepository.UpdateMetalClassification(existingMetalClassification);
            _logger.LogInformation("MetalClassification with ID '{MetalClassificationId}' deleted successfully.", id);
        }

        public async Task<MetalClassificationMaster> GetMetalClassificationDetails(long id)
        {
            var classification = await _MetalClassificationRepository.GetMetalClassificationById(id);
            if (classification == null)
                throw new InvalidOperationException("MetalClassification not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchMetalClassificationList(PageFilter filter)
        {
            return await _MetalClassificationRepository.GetAllMetalClassifications(filter);
        }

        public async Task<List<DropdwonSelector>> GetMetalClassificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _MetalClassificationRepository.GetMetalClassificationDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<ParameterMaster>> GetParameterByMetalId(long id)
        {
            return await _MetalClassificationRepository.GetParameterByMetalId(id);
        }
    }
}
