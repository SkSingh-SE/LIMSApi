using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class AnalysisTechniqueService : IAnalysisTechniqueService
    {
        private readonly IAnalysisTechniqueRepository _repository;
        private readonly ILogger<AnalysisTechniqueService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public AnalysisTechniqueService(IAnalysisTechniqueRepository repository, ILogger<AnalysisTechniqueService> logger, LIMSContext context)
        {
            _repository = repository;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateAnalysisTechnique(AnalysisTechniqueMaster model)
        {
            Validate(model);

            if (await _repository.ExistsByName(model.Name))
                throw new InvalidOperationException("An analysis technique with the same name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code) && await _repository.ExistsByCode(model.Code))
                throw new InvalidOperationException("An analysis technique with the same code already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _repository.AddAnalysisTechnique(model);
            _logger.LogInformation("Analysis technique '{Name}' created successfully.", model.Name);
        }

        public async Task ModifyAnalysisTechnique(AnalysisTechniqueMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Analysis technique ID should not be empty!");

            Validate(model);

            if (await _repository.ExistsByNameAndNotId(model.Name, model.ID))
                throw new InvalidOperationException("An analysis technique with the same name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code) && await _repository.ExistsByCodeAndNotId(model.Code, model.ID))
                throw new InvalidOperationException("An analysis technique with the same code already exists!");

            var existing = await _repository.GetAnalysisTechniqueById(model.ID);
            if (existing == null)
                throw new InvalidOperationException("Analysis technique not found!");

            existing.Name = model.Name;
            existing.Code = model.Code;
            existing.AliasNames = model.AliasNames;
            existing.IsSpectro = model.IsSpectro;
            existing.Description = model.Description;
            existing.SortOrder = model.SortOrder;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateAnalysisTechnique(existing);
            _logger.LogInformation("Analysis technique '{Name}' updated successfully.", model.Name);
        }

        public async Task RemoveAnalysisTechnique(long id)
        {
            var existing = await _repository.GetAnalysisTechniqueById(id);
            if (existing == null)
                throw new InvalidOperationException("Analysis technique not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<AnalysisTechniqueMaster>(_context, id, "Analysis Technique");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.DeleteAnalysisTechnique(existing);
            _logger.LogInformation("Analysis technique with ID '{Id}' deleted successfully.", id);
        }

        public async Task<AnalysisTechniqueMaster> GetAnalysisTechniqueDetails(long id)
        {
            var entity = await _repository.GetAnalysisTechniqueById(id);
            if (entity == null)
                throw new InvalidOperationException("Analysis technique not found!");

            return entity;
        }

        public async Task<PagedResponse<object>> FetchAnalysisTechniqueList(PageFilter filter)
        {
            return await _repository.GetAllAnalysisTechniques(filter);
        }

        public async Task<List<DropdwonSelector>> GetAnalysisTechniqueDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetAnalysisTechniqueDropdown(searchTerm, pageNo, pageSize);
        }

        private static void Validate(AnalysisTechniqueMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Name should not be empty!");

            model.Name = model.Name.Trim();
            model.Code = string.IsNullOrWhiteSpace(model.Code) ? null : model.Code.Trim();
            model.AliasNames = string.IsNullOrWhiteSpace(model.AliasNames) ? null : model.AliasNames.Trim();
            model.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        }
    }
}
