using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ProductConditionCategoryService : IProductConditionCategoryService
    {
        private readonly IProductConditionCategoryRepository _repo;
        private readonly ILogger<ProductConditionCategoryService> _logger;
        public ProductConditionCategoryService(IProductConditionCategoryRepository repo, ILogger<ProductConditionCategoryService> logger) { _repo = repo; _logger = logger; }

        public async Task CreateProductConditionCategory(ProductConditionCategoryMaster model) {
            if (string.IsNullOrWhiteSpace(model.Name)) throw new ArgumentException("ProductConditionCategory name should not be empty!");
            if (await _repo.ExistsByName(model.Name)) throw new InvalidOperationException("ProductConditionCategory already exists!");
            await _repo.AddProductConditionCategory(model);
            _logger.LogInformation("ProductConditionCategory '{Name}' created.", model.Name);
        }

        public async Task ModifyProductConditionCategory(ProductConditionCategoryMaster model) {
            if (model.ID == 0) throw new ArgumentException("ProductConditionCategory ID should not be empty!");
            if (await _repo.ExistsByNameAndNotId(model.Name, model.ID)) throw new InvalidOperationException("Same ProductConditionCategory already exists!");
            var existing = await _repo.GetProductConditionCategoryById(model.ID) ?? throw new InvalidOperationException("ProductConditionCategory not found!");
            existing.Name = model.Name;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateProductConditionCategory(existing);
        }

        public async Task RemoveProductConditionCategory(long id) {
            var existing = await _repo.GetProductConditionCategoryById(id) ?? throw new InvalidOperationException("ProductConditionCategory not found!");
            existing.IsActive = false; existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateProductConditionCategory(existing);
        }

        public async Task<ProductConditionCategoryMaster> GetProductConditionCategoryDetails(long id) => await _repo.GetProductConditionCategoryById(id) ?? throw new InvalidOperationException("ProductConditionCategory not found!");
        public async Task<PagedResponse<object>> FetchProductConditionCategoryList(PageFilter filter) => await _repo.GetAllProductConditionCategorys(filter);
        public async Task<List<DropdwonSelector>> GetProductConditionCategoryDropdown(string? searchTerm, int pageNo, int pageSize) => await _repo.GetProductConditionCategoryDropdown(searchTerm, pageNo, pageSize);
    }
}
