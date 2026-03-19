using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class PropertyTypeService : IPropertyTypeService
    {
        private readonly IPropertyTypeRepository _repo;
        private readonly ILogger<PropertyTypeService> _logger;
        public PropertyTypeService(IPropertyTypeRepository repo, ILogger<PropertyTypeService> logger) { _repo = repo; _logger = logger; }

        public async Task CreatePropertyType(PropertyTypeMaster model) {
            if (string.IsNullOrWhiteSpace(model.Name)) throw new ArgumentException("PropertyType name should not be empty!");
            if (await _repo.ExistsByName(model.Name)) throw new InvalidOperationException("PropertyType already exists!");
            await _repo.AddPropertyType(model);
            _logger.LogInformation("PropertyType '{Name}' created.", model.Name);
        }

        public async Task ModifyPropertyType(PropertyTypeMaster model) {
            if (model.ID == 0) throw new ArgumentException("PropertyType ID should not be empty!");
            if (await _repo.ExistsByNameAndNotId(model.Name, model.ID)) throw new InvalidOperationException("Same PropertyType already exists!");
            var existing = await _repo.GetPropertyTypeById(model.ID) ?? throw new InvalidOperationException("PropertyType not found!");
            existing.Name = model.Name;
            existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdatePropertyType(existing);
        }

        public async Task RemovePropertyType(long id) {
            var existing = await _repo.GetPropertyTypeById(id) ?? throw new InvalidOperationException("PropertyType not found!");
            existing.IsActive = false; existing.ModifiedOn = DateTime.UtcNow;
            await _repo.UpdatePropertyType(existing);
        }

        public async Task<PropertyTypeMaster> GetPropertyTypeDetails(long id) => await _repo.GetPropertyTypeById(id) ?? throw new InvalidOperationException("PropertyType not found!");
        public async Task<PagedResponse<object>> FetchPropertyTypeList(PageFilter filter) => await _repo.GetAllPropertyTypes(filter);
        public async Task<List<DropdwonSelector>> GetPropertyTypeDropdown(string? searchTerm, int pageNo, int pageSize) => await _repo.GetPropertyTypeDropdown(searchTerm, pageNo, pageSize);
    }
}
