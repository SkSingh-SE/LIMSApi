using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ProductSizeMasterService : IProductSizeMasterService
    {
        private readonly IProductSizeMasterRepository _repository;
        private readonly ILogger<ProductSizeMasterService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public ProductSizeMasterService(IProductSizeMasterRepository repository, ILogger<ProductSizeMasterService> logger, LIMSContext context)
        {
            _repository = repository;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateProductSize(ProductSizeMaster model)
        {
            Validate(model);

            bool exists = await _repository.ExistsByName(model.DisplayName);
            if (exists)
                throw new InvalidOperationException("A product size with the same display name already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _repository.AddProductSize(model);
            _logger.LogInformation("Product size '{DisplayName}' created successfully.", model.DisplayName);
        }

        public async Task ModifyProductSize(ProductSizeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Product size ID should not be empty!");

            Validate(model);

            bool exists = await _repository.ExistsByNameAndNotId(model.DisplayName, model.ID);
            if (exists)
                throw new InvalidOperationException("A product size with the same display name already exists!");

            var existing = await _repository.GetProductSizeById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("Product size not found!");

            existing.SizeType = model.SizeType;
            existing.MinValue = model.MinValue;
            existing.MaxValue = model.MaxValue;
            existing.ParameterUnitID = model.ParameterUnitID;
            existing.ParameterUnitEquivalentID = model.ParameterUnitEquivalentID;
            existing.DisplayName = model.DisplayName;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateProductSize(existing);
            _logger.LogInformation("Product size '{DisplayName}' updated successfully.", model.DisplayName);
        }

        public async Task RemoveProductSize(long id)
        {
            var existing = await _repository.GetProductSizeById(id);
            if (existing == null)
                throw new InvalidOperationException("Product size not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<ProductSizeMaster>(_context, id, "Product Size");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.DeleteProductSize(existing);
            _logger.LogInformation("Product size with ID '{Id}' deleted successfully.", id);
        }

        public async Task<ProductSizeMaster> GetProductSizeDetails(long id)
        {
            var entity = await _repository.GetProductSizeById(id);
            if (entity == null)
                throw new InvalidOperationException("Product size not found!");

            return entity;
        }

        public async Task<PagedResponse<object>> FetchProductSizeList(PageFilter filter)
        {
            return await _repository.GetAllProductSizes(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductSizeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetProductSizeDropdown(searchTerm, pageNo, pageSize);
        }

        private static void Validate(ProductSizeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.DisplayName))
                throw new ArgumentException("Display name should not be empty!");
            if (string.IsNullOrWhiteSpace(model.SizeType))
                throw new ArgumentException("Size type should not be empty!");

            model.DisplayName = model.DisplayName.Trim();
            model.SizeType = model.SizeType.Trim();

            if (model.MinValue.HasValue && model.MaxValue.HasValue && model.MinValue > model.MaxValue)
                throw new ArgumentException("Min value cannot be greater than max value.");
        }
    }
}
