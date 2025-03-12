using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ClassificationService : IClassificationService
    {
        private readonly IClassificationRepository _classificationRepository;
        private readonly ILogger<ClassificationService> _logger;

        public ClassificationService(IClassificationRepository classificationRepo, ILogger<ClassificationService> logger)
        {
            _classificationRepository = classificationRepo;
            _logger = logger;
        }

        public async Task CreateClassification(ClassificationMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Classification name should not be empty!");

            bool exists = await _classificationRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Classification already exists!");

            await _classificationRepository.AddClassification(model);
            _logger.LogInformation("Classification '{ClassificationName}' created successfully.", model.Name);
        }

        public async Task ModifyClassification(ClassificationMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Classification ID should not be empty!");

            bool exists = await _classificationRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Classification already exists!");

            var existingClassification = await _classificationRepository.GetClassificationById(model.ID);
            if (existingClassification == null)
                throw new InvalidOperationException("Classification not found!");

            existingClassification.Name = model.Name;
            existingClassification.Description = model.Description;
            existingClassification.ModifiedOn = DateTime.UtcNow;

            await _classificationRepository.UpdateClassification(existingClassification);
            _logger.LogInformation("Classification '{ClassificationName}' updated successfully.", model.Name);
        }

        public async Task RemoveClassification(long id)
        {
            var existingClassification = await _classificationRepository.GetClassificationById(id);
            if (existingClassification == null)
                throw new InvalidOperationException("Classification not found!");

            existingClassification.IsActive = false;
            existingClassification.ModifiedOn = DateTime.UtcNow;

            await _classificationRepository.UpdateClassification(existingClassification);
            _logger.LogInformation("Classification with ID '{ClassificationId}' deleted successfully.", id);
        }

        public async Task<ClassificationMaster> GetClassificationDetails(long id)
        {
            var classification = await _classificationRepository.GetClassificationById(id);
            if (classification == null)
                throw new InvalidOperationException("Classification not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchClassificationList(PageFilter filter)
        {
            return await _classificationRepository.GetAllClassifications(filter);
        }

        public async Task<List<DropdwonSelector>> GetClassificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _classificationRepository.GetClassificationDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
