using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ProductFormService : IProductFormService
    {
        private readonly IProductFormRepository _repo;
        private readonly ILogger<ProductFormService> _logger;
        public ProductFormService(IProductFormRepository repo, ILogger<ProductFormService> logger) { _repo = repo; _logger = logger; }

        public async Task CreateProductForm(ProductFormMaster model) {
            if (string.IsNullOrWhiteSpace(model.Name)) throw new ArgumentException("ProductForm name should not be empty!");
            if (await _repo.ExistsByName(model.Name)) throw new InvalidOperationException("ProductForm already exists!");
            await _repo.AddProductForm(model);
            _logger.LogInformation("ProductForm '{Name}' created.", model.Name);
        }

        public async Task ModifyProductForm(ProductFormMaster model) {
            if (model.ID == 0) throw new ArgumentException("ProductForm ID should not be empty!");
            if (await _repo.ExistsByNameAndNotId(model.Name, model.ID)) throw new InvalidOperationException("Same ProductForm already exists!");
            var existing = await _repo.GetProductFormById(model.ID) ?? throw new InvalidOperationException("ProductForm not found!");
            existing.Name = model.Name;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateProductForm(existing);
        }

        public async Task RemoveProductForm(long id) {
            var existing = await _repo.GetProductFormById(id) ?? throw new InvalidOperationException("ProductForm not found!");
            existing.IsActive = false; existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdateProductForm(existing);
        }

        public async Task<ProductFormMaster> GetProductFormDetails(long id) => await _repo.GetProductFormById(id) ?? throw new InvalidOperationException("ProductForm not found!");
        public async Task<PagedResponse<object>> FetchProductFormList(PageFilter filter) => await _repo.GetAllProductForms(filter);
        public async Task<List<DropdwonSelector>> GetProductFormDropdown(string? searchTerm, int pageNo, int pageSize) => await _repo.GetProductFormDropdown(searchTerm, pageNo, pageSize);
    }
}
