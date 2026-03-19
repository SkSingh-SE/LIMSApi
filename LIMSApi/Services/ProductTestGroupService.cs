using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ProductTestGroupService : IProductTestGroupService
    {
        private readonly IProductTestGroupRepository _productTestGroupRepository;
        private readonly ILogger<ProductTestGroupService> _logger;

        public ProductTestGroupService(IProductTestGroupRepository productTestGroupRepo, ILogger<ProductTestGroupService> logger)
        {
            _productTestGroupRepository = productTestGroupRepo;
            _logger = logger;
        }

        public async Task CreateProductTestGroup(ProductTestGroup model)
        {
            if (model.ProductSpecificationID == 0)
                throw new ArgumentException("ProductSpecificationID should not be empty!");

            if (model.LaboratoryTestID == 0)
                throw new ArgumentException("LaboratoryTestID should not be empty!");

            await _productTestGroupRepository.AddProductTestGroup(model);
            _logger.LogInformation("ProductTestGroup created successfully for ProductSpecificationID: {ProductSpecificationID}.", model.ProductSpecificationID);
        }

        public async Task ModifyProductTestGroup(ProductTestGroup model)
        {
            if (model.ID == 0)
                throw new ArgumentException("ProductTestGroup ID should not be empty!");

            var existing = await _productTestGroupRepository.GetProductTestGroupById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("ProductTestGroup not found!");

            existing.ProductSpecificationID = model.ProductSpecificationID;
            existing.LaboratoryTestID = model.LaboratoryTestID;
            existing.TestMethodStandardID = model.TestMethodStandardID;
            existing.IsPerBatch = model.IsPerBatch;
            existing.Year = model.Year;
            existing.Remark = model.Remark;
            existing.ModifiedOn = DateTime.UtcNow;

            await _productTestGroupRepository.UpdateProductTestGroup(existing);
            _logger.LogInformation("ProductTestGroup '{ProductTestGroupId}' updated successfully.", model.ID);
        }

        public async Task RemoveProductTestGroup(long id)
        {
            var existing = await _productTestGroupRepository.GetProductTestGroupById(id);
            if (existing == null)
                throw new InvalidOperationException("ProductTestGroup not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;

            await _productTestGroupRepository.UpdateProductTestGroup(existing);
            _logger.LogInformation("ProductTestGroup with ID '{ProductTestGroupId}' deleted successfully.", id);
        }

        public async Task<ProductTestGroup> GetProductTestGroupDetails(long id)
        {
            var entity = await _productTestGroupRepository.GetProductTestGroupById(id);
            if (entity == null)
                throw new InvalidOperationException("ProductTestGroup not found!");

            return entity;
        }

        public async Task<PagedResponse<object>> FetchProductTestGroupList(PageFilter filter)
        {
            return await _productTestGroupRepository.GetAllProductTestGroups(filter);
        }

        public async Task<List<DropdwonSelector>> GetProductTestGroupDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _productTestGroupRepository.GetProductTestGroupDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<object>> GetByProductSpecificationId(long productSpecId)
        {
            return await _productTestGroupRepository.GetByProductSpecificationId(productSpecId);
        }
    }
}
