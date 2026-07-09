using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ChemicalSampleCategoryService : IChemicalSampleCategoryService
    {
        private readonly IChemicalSampleCategoryRepository _repository;
        private readonly ILogger<ChemicalSampleCategoryService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public ChemicalSampleCategoryService(IChemicalSampleCategoryRepository repository, ILogger<ChemicalSampleCategoryService> logger, LIMSContext context)
        {
            _repository = repository;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task Create(ChemicalSampleCategory model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Category name should not be empty!");

            model.Name = model.Name.Trim();

            bool exists = await _repository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("A category with the same name already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _repository.Add(model);
            _logger.LogInformation("Chemical sample category '{Name}' created successfully.", model.Name);
        }

        public async Task Modify(ChemicalSampleCategory model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Category ID should not be empty!");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Category name should not be empty!");

            model.Name = model.Name.Trim();

            bool exists = await _repository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("A category with the same name already exists!");

            var existing = await _repository.GetById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("Chemical sample category not found!");

            existing.Name = model.Name;
            existing.SortOrder = model.SortOrder;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.Update(existing);
            _logger.LogInformation("Chemical sample category '{Name}' updated successfully.", model.Name);
        }

        public async Task Remove(long id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
                throw new InvalidOperationException("Chemical sample category not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<ChemicalSampleCategory>(_context, id, "Chemical Sample Category");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.Delete(existing);
            _logger.LogInformation("Chemical sample category with ID '{Id}' deleted successfully.", id);
        }

        public async Task<ChemicalSampleCategory> GetDetails(long id)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
                throw new InvalidOperationException("Chemical sample category not found!");
            return entity;
        }

        public async Task<PagedResponse<object>> FetchList(PageFilter filter)
        {
            return await _repository.GetAll(filter);
        }

        public async Task<List<DropdwonSelector>> GetDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
