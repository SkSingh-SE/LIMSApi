using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ProductSpecificationGradeService : IProductSpecificationGradeService
    {
        private readonly IProductSpecificationGradeRepository _productSpecificationGradeRepository;
        private readonly ILogger<ProductSpecificationGradeService> _logger;

        public ProductSpecificationGradeService(IProductSpecificationGradeRepository productSpecificationGradeRepo, ILogger<ProductSpecificationGradeService> logger)
        {
            _productSpecificationGradeRepository = productSpecificationGradeRepo;
            _logger = logger;
        }

        public async Task CreateProductSpecificationGrade(ProductSpecificationGrade model)
        {
            if (model.ProductSpecificationID == 0)
                throw new ArgumentException("ProductSpecificationID should not be empty!");

            if (model.SpecificationGradeID == 0)
                throw new ArgumentException("SpecificationGradeID should not be empty!");

            await _productSpecificationGradeRepository.AddProductSpecificationGrade(model);
            _logger.LogInformation("ProductSpecificationGrade created successfully for ProductSpecificationID: {ProductSpecificationID}.", model.ProductSpecificationID);
        }

        public async Task ModifyProductSpecificationGrade(ProductSpecificationGrade model)
        {
            if (model.ID == 0)
                throw new ArgumentException("ProductSpecificationGrade ID should not be empty!");

            var existing = await _productSpecificationGradeRepository.GetProductSpecificationGradeById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("ProductSpecificationGrade not found!");

            existing.ProductSpecificationID = model.ProductSpecificationID;
            existing.SpecificationGradeID = model.SpecificationGradeID;
            existing.AliasName = model.AliasName;
            existing.ModifiedOn = DateTime.UtcNow;

            await _productSpecificationGradeRepository.UpdateProductSpecificationGrade(existing);
            _logger.LogInformation("ProductSpecificationGrade '{ProductSpecificationGradeId}' updated successfully.", model.ID);
        }

        public async Task RemoveProductSpecificationGrade(long id)
        {
            var existing = await _productSpecificationGradeRepository.GetProductSpecificationGradeById(id);
            if (existing == null)
                throw new InvalidOperationException("ProductSpecificationGrade not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;

            await _productSpecificationGradeRepository.UpdateProductSpecificationGrade(existing);
            _logger.LogInformation("ProductSpecificationGrade with ID '{ProductSpecificationGradeId}' deleted successfully.", id);
        }

        public async Task<ProductSpecificationGrade> GetProductSpecificationGradeDetails(long id)
        {
            var entity = await _productSpecificationGradeRepository.GetProductSpecificationGradeById(id);
            if (entity == null)
                throw new InvalidOperationException("ProductSpecificationGrade not found!");

            return entity;
        }

        public async Task<PagedResponse<object>> FetchProductSpecificationGradeList(PageFilter filter)
        {
            return await _productSpecificationGradeRepository.GetAllProductSpecificationGrades(filter);
        }

        public async Task<List<object>> GetByProductSpecificationId(long productSpecId)
        {
            return await _productSpecificationGradeRepository.GetByProductSpecificationId(productSpecId);
        }
    }
}
