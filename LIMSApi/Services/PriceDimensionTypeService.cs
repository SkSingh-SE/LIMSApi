using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class PriceDimensionTypeService : IPriceDimensionTypeService
    {
        private readonly IPriceDimensionTypeRepository _repository;
        private readonly ILogger<PriceDimensionTypeService> _logger;

        public PriceDimensionTypeService(IPriceDimensionTypeRepository repository, ILogger<PriceDimensionTypeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task CreatePriceDimensionType(PriceDimensionType model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Price Dimension Type name should not be empty!");

            bool exists = await _repository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Price Dimension Type already exists!");

            await _repository.AddPriceDimensionType(model);
            _logger.LogInformation("PriceDimensionType '{Name}' created successfully.", model.Name);
        }

        public async Task ModifyPriceDimensionType(PriceDimensionType model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Price Dimension Type ID should not be empty!");

            bool exists = await _repository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Price Dimension Type already exists!");

            var existing = await _repository.GetPriceDimensionTypeById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("Price Dimension Type not found!");

            existing.Name = model.Name;
            existing.Unit = model.Unit;
            existing.IsRange = model.IsRange;
            existing.Description = model.Description;
            existing.SortOrder = model.SortOrder;
            existing.ModifiedOn = DateTime.UtcNow;

            await _repository.UpdatePriceDimensionType(existing);
            _logger.LogInformation("PriceDimensionType '{Name}' updated successfully.", model.Name);
        }

        public async Task<PriceDimensionType> RemovePriceDimensionType(long id)
        {
            var existing = await _repository.GetPriceDimensionTypeById(id);
            if (existing == null)
                throw new InvalidOperationException("Price Dimension Type not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;

            await _repository.UpdatePriceDimensionType(existing);
            _logger.LogInformation("PriceDimensionType with ID '{Id}' deleted successfully.", id);
            return existing;
        }

        public async Task<PriceDimensionType> GetPriceDimensionTypeDetails(long id)
        {
            var item = await _repository.GetPriceDimensionTypeById(id);
            if (item == null)
                throw new InvalidOperationException("Price Dimension Type not found!");
            return item;
        }

        public async Task<PagedResponse<object>> FetchPriceDimensionTypeList(PageFilter filter)
        {
            return await _repository.GetAllPriceDimensionTypes(filter);
        }

        public async Task<List<DropdwonSelector>> GetPriceDimensionTypeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetPriceDimensionTypeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
