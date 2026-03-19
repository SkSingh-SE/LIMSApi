using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SpecimenOrientationCategoryService : ISpecimenOrientationCategoryService
    {
        private readonly ISpecimenOrientationCategoryRepository _repo;
        private readonly ILogger<SpecimenOrientationCategoryService> _logger;
        public SpecimenOrientationCategoryService(ISpecimenOrientationCategoryRepository repo, ILogger<SpecimenOrientationCategoryService> logger) { _repo = repo; _logger = logger; }

        public async Task CreateSpecimenOrientationCategory(SpecimenOrientationCategoryMaster model) {
            if (string.IsNullOrWhiteSpace(model.Name)) throw new ArgumentException("SpecimenOrientationCategory name should not be empty!");
            if (await _repo.ExistsByName(model.Name)) throw new InvalidOperationException("SpecimenOrientationCategory already exists!");
            await _repo.AddSpecimenOrientationCategory(model);
            _logger.LogInformation("SpecimenOrientationCategory '{Name}' created.", model.Name);
        }

        public async Task ModifySpecimenOrientationCategory(SpecimenOrientationCategoryMaster model) {
            if (model.ID == 0) throw new ArgumentException("SpecimenOrientationCategory ID should not be empty!");
            if (await _repo.ExistsByNameAndNotId(model.Name, model.ID)) throw new InvalidOperationException("Same SpecimenOrientationCategory already exists!");
            var existing = await _repo.GetSpecimenOrientationCategoryById(model.ID) ?? throw new InvalidOperationException("SpecimenOrientationCategory not found!");
            existing.Name = model.Name;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateSpecimenOrientationCategory(existing);
        }

        public async Task RemoveSpecimenOrientationCategory(long id) {
            var existing = await _repo.GetSpecimenOrientationCategoryById(id) ?? throw new InvalidOperationException("SpecimenOrientationCategory not found!");
            existing.IsActive = false; existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateSpecimenOrientationCategory(existing);
        }

        public async Task<SpecimenOrientationCategoryMaster> GetSpecimenOrientationCategoryDetails(long id) => await _repo.GetSpecimenOrientationCategoryById(id) ?? throw new InvalidOperationException("SpecimenOrientationCategory not found!");
        public async Task<PagedResponse<object>> FetchSpecimenOrientationCategoryList(PageFilter filter) => await _repo.GetAllSpecimenOrientationCategorys(filter);
        public async Task<List<DropdwonSelector>> GetSpecimenOrientationCategoryDropdown(string? searchTerm, int pageNo, int pageSize) => await _repo.GetSpecimenOrientationCategoryDropdown(searchTerm, pageNo, pageSize);
    }
}
