using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ParameterCategoryService : IParameterCategoryService
    {
        private readonly IParameterCategoryRepository _repo;
        private readonly ILogger<ParameterCategoryService> _logger;

        public ParameterCategoryService(IParameterCategoryRepository repo, ILogger<ParameterCategoryService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task CreateParameterCategory(ParameterCategoryMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("ParameterCategory name should not be empty!");
            bool exists = await _repo.ExistsByName(model.Name);
            if (exists) throw new InvalidOperationException("ParameterCategory already exists!");
            await _repo.AddParameterCategory(model);
        }

        public async Task ModifyParameterCategory(ParameterCategoryMaster model)
        {
            if (model.ID == 0) throw new ArgumentException("ParameterCategory ID should not be empty!");
            bool exists = await _repo.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists) throw new InvalidOperationException("Same ParameterCategory already exists!");
            var existing = await _repo.GetParameterCategoryById(model.ID);
            if (existing == null) throw new InvalidOperationException("ParameterCategory not found!");
            existing.Name = model.Name;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateParameterCategory(existing);
        }

        public async Task RemoveParameterCategory(long id)
        {
            var existing = await _repo.GetParameterCategoryById(id);
            if (existing == null) throw new InvalidOperationException("ParameterCategory not found!");
            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateParameterCategory(existing);
        }

        public async Task<ParameterCategoryMaster> GetParameterCategoryDetails(long id)
        {
            var item = await _repo.GetParameterCategoryById(id);
            if (item == null) throw new InvalidOperationException("ParameterCategory not found!");
            return item;
        }

        public async Task<PagedResponse<object>> FetchParameterCategoryList(PageFilter filter) => await _repo.GetAllParameterCategories(filter);
        public async Task<List<DropdwonSelector>> GetParameterCategoryDropdown(string? searchTerm, int pageNo, int pageSize) => await _repo.GetParameterCategoryDropdown(searchTerm, pageNo, pageSize);
    }
}
