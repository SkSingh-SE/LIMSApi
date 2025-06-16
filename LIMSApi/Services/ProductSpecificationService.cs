using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ProductSpecificationService : IProductSpecificationService
    {
        private readonly IProductSpecificationRepository _ProductSpecificationRepository;
        private readonly ILogger<ProductSpecificationService> _logger;

        public ProductSpecificationService(IProductSpecificationRepository ProductSpecificationRepo, ILogger<ProductSpecificationService> logger)
        {
            _ProductSpecificationRepository = ProductSpecificationRepo;
            _logger = logger;
        }

        public async Task CreateProductSpecification(ProductSpecification model)
        {
            if (string.IsNullOrWhiteSpace(model.SpecificationName))
                throw new ArgumentException("ProductSpecification name should not be empty!");

            bool exists = await _ProductSpecificationRepository.ExistsByName(model.SpecificationName);
            if (exists)
                throw new InvalidOperationException("ProductSpecification already exists!");

            await _ProductSpecificationRepository.AddProductSpecification(model);
            _logger.LogInformation("ProductSpecification '{ProductSpecificationName}' created successfully.", model.SpecificationName);
        }

        public async Task ModifyProductSpecification(ProductSpecification model)
        {
            if (model.ID == 0)
                throw new ArgumentException("ProductSpecification ID should not be empty!");

            bool exists = await _ProductSpecificationRepository.ExistsByNameAndNotId(model.SpecificationName, model.ID);
            if (exists)
                throw new InvalidOperationException("Same ProductSpecification already exists!");

            var existingProductSpecification = await _ProductSpecificationRepository.GetProductSpecificationById(model.ID);
            if (existingProductSpecification == null)
                throw new InvalidOperationException("ProductSpecification not found!");

            existingProductSpecification.SpecificationName = model.SpecificationName;
            existingProductSpecification.AliasName = model.AliasName;
            existingProductSpecification.SpecificationCode = model.SpecificationCode;
            existingProductSpecification.GradeID = model.GradeID;
            existingProductSpecification.LaboratoryTestID = model.LaboratoryTestID;
            existingProductSpecification.MetalClassificationID = model.MetalClassificationID;
            existingProductSpecification.TestMethodSpecificationID = model.TestMethodSpecificationID;
            existingProductSpecification.IsCustom = model.IsCustom;
            existingProductSpecification.ModifiedOn = DateTime.UtcNow;

            await _ProductSpecificationRepository.UpdateProductSpecification(existingProductSpecification);
            _logger.LogInformation("ProductSpecification '{ProductSpecificationName}' updated successfully.", model.SpecificationName);
        }

        public async Task RemoveProductSpecification(long id)
        {
            var existingProductSpecification = await _ProductSpecificationRepository.GetProductSpecificationById(id);
            if (existingProductSpecification == null)
                throw new InvalidOperationException("ProductSpecification not found!");

            existingProductSpecification.IsActive = false;
            existingProductSpecification.ModifiedOn = DateTime.UtcNow;

            await _ProductSpecificationRepository.UpdateProductSpecification(existingProductSpecification);
            _logger.LogInformation("ProductSpecification with ID '{ProductSpecificationId}' deleted successfully.", id);
        }

        public async Task<ProductSpecification> GetProductSpecificationDetails(long id)
        {
            var classification = await _ProductSpecificationRepository.GetProductSpecificationById(id);
            if (classification == null)
                throw new InvalidOperationException("ProductSpecification not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchProductSpecificationList(PageFilter filter)
        {
            return await _ProductSpecificationRepository.GetAllProductSpecifications(filter);
        }
        public async Task<PagedResponse<object>> FetchCustomProductSpecificationList(PageFilter filter)
        {
            return await _ProductSpecificationRepository.GetAllCustomProductSpecifications(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductSpecificationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _ProductSpecificationRepository.GetProductSpecificationDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
