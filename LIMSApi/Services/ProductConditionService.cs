using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class ProductConditionService : IProductConditionService
    {
        private readonly IProductConditionRepository _parameterRepository;
        private readonly ILogger<ProductConditionService> _logger;
        private readonly LIMSContext _context;

        public ProductConditionService(IProductConditionRepository parameterRepo, ILogger<ProductConditionService> logger, LIMSContext context)
        {
            _parameterRepository = parameterRepo;
            _logger = logger;
            _context = context;
        }

        public async Task CreateProductCondition(ProductConditionMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("ProductCondition name should not be empty!");

            bool exists = await _parameterRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Product Condition Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _parameterRepository.ExistsByCode(model.Code);
                if (codeExists)
                    throw new InvalidOperationException("Product Condition Code already exists!");
            }

            await _parameterRepository.AddProductCondition(model);
            _logger.LogInformation("ProductCondition '{ProductConditionName}' created successfully.", model.Name);
        }

        public async Task ModifyProductCondition(ProductConditionMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("ProductCondition ID should not be empty!");

            bool exists = await _parameterRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Product Condition Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _parameterRepository.ExistsByCodeAndNotId(model.Code, model.ID);
                if (codeExists)
                    throw new InvalidOperationException("Product Condition Code already exists!");
            }

            var existingProductCondition = await _parameterRepository.GetProductConditionById(model.ID);
            if (existingProductCondition == null)
                throw new InvalidOperationException("ProductCondition not found!");

            existingProductCondition.Name = model.Name;
            existingProductCondition.Code = model.Code;
            existingProductCondition.ProductConditionCategoryID = model.ProductConditionCategoryID;
            existingProductCondition.LinkedHeatTreatmentID = model.LinkedHeatTreatmentID;
            existingProductCondition.CalibrationRequired = model.CalibrationRequired;
            existingProductCondition.IsDestructive = model.IsDestructive;

            // Update PropertiesCaptured junction
            existingProductCondition.PropertiesCaptured?.Clear();
            if (model.PropertiesCaptured != null)
                foreach (var prop in model.PropertiesCaptured)
                    existingProductCondition.PropertiesCaptured.Add(prop);

            existingProductCondition.ModifiedOn = DateTime.UtcNow;

            await _parameterRepository.UpdateProductCondition(existingProductCondition);
            _logger.LogInformation("ProductCondition '{ProductConditionName}' updated successfully.", model.Name);
        }

        public async Task RemoveProductCondition(long id)
        {
            var existingProductCondition = await _parameterRepository.GetProductConditionById(id);
            if (existingProductCondition == null)
                throw new InvalidOperationException("ProductCondition not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<ProductConditionMaster>(_context, id, "Product Condition", existingProductCondition.Name);

            existingProductCondition.IsActive = false;
            existingProductCondition.ModifiedOn = DateTime.UtcNow;

            await _parameterRepository.UpdateProductCondition(existingProductCondition);
            _logger.LogInformation("ProductCondition with ID '{ProductConditionId}' deleted successfully.", id);
        }

        public async Task<ProductConditionMaster> GetProductConditionDetails(long id)
        {
            var classification = await _parameterRepository.GetProductConditionById(id);
            if (classification == null)
                throw new InvalidOperationException("ProductCondition not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchProductConditionList(PageFilter filter)
        {
            return await _parameterRepository.GetAllProductConditions(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductConditionDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _parameterRepository.GetProductConditionDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
