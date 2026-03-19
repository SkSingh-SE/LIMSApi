using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class HeatTreatmentCategoryService : IHeatTreatmentCategoryService
    {
        private readonly IHeatTreatmentCategoryRepository _heatTreatmentCategoryRepository;
        private readonly ILogger<HeatTreatmentCategoryService> _logger;

        public HeatTreatmentCategoryService(IHeatTreatmentCategoryRepository heatTreatmentCategoryRepo, ILogger<HeatTreatmentCategoryService> logger)
        {
            _heatTreatmentCategoryRepository = heatTreatmentCategoryRepo;
            _logger = logger;
        }

        public async Task CreateHeatTreatmentCategory(HeatTreatmentCategoryMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("HeatTreatmentCategory name should not be empty!");

            bool exists = await _heatTreatmentCategoryRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("HeatTreatmentCategory already exists!");

            await _heatTreatmentCategoryRepository.AddHeatTreatmentCategory(model);
            _logger.LogInformation("HeatTreatmentCategory '{HeatTreatmentCategoryName}' created successfully.", model.Name);
        }

        public async Task ModifyHeatTreatmentCategory(HeatTreatmentCategoryMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("HeatTreatmentCategory ID should not be empty!");

            bool exists = await _heatTreatmentCategoryRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same HeatTreatmentCategory already exists!");

            var existingHeatTreatmentCategory = await _heatTreatmentCategoryRepository.GetHeatTreatmentCategoryById(model.ID);
            if (existingHeatTreatmentCategory == null)
                throw new InvalidOperationException("HeatTreatmentCategory not found!");

            existingHeatTreatmentCategory.Name = model.Name;
            existingHeatTreatmentCategory.ModifiedOn = DateTime.UtcNow;

            await _heatTreatmentCategoryRepository.UpdateHeatTreatmentCategory(existingHeatTreatmentCategory);
            _logger.LogInformation("HeatTreatmentCategory '{HeatTreatmentCategoryName}' updated successfully.", model.Name);
        }

        public async Task RemoveHeatTreatmentCategory(long id)
        {
            var existingHeatTreatmentCategory = await _heatTreatmentCategoryRepository.GetHeatTreatmentCategoryById(id);
            if (existingHeatTreatmentCategory == null)
                throw new InvalidOperationException("HeatTreatmentCategory not found!");

            existingHeatTreatmentCategory.IsActive = false;
            existingHeatTreatmentCategory.ModifiedOn = DateTime.UtcNow;

            await _heatTreatmentCategoryRepository.UpdateHeatTreatmentCategory(existingHeatTreatmentCategory);
            _logger.LogInformation("HeatTreatmentCategory with ID '{HeatTreatmentCategoryId}' deleted successfully.", id);
        }

        public async Task<HeatTreatmentCategoryMaster> GetHeatTreatmentCategoryDetails(long id)
        {
            var classification = await _heatTreatmentCategoryRepository.GetHeatTreatmentCategoryById(id);
            if (classification == null)
                throw new InvalidOperationException("HeatTreatmentCategory not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchHeatTreatmentCategoryList(PageFilter filter)
        {
            return await _heatTreatmentCategoryRepository.GetAllHeatTreatmentCategories(filter);
        }

        public async Task<List<DropdwonSelector>> GetHeatTreatmentCategoryDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _heatTreatmentCategoryRepository.GetHeatTreatmentCategoryDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
