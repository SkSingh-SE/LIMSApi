using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class DisciplineService : IDisciplineService
    {
        private readonly IDisciplineRepository _disciplineRepository;
        private readonly ILogger<DisciplineService> _logger;
        private LoggedInUserDTO loggedInUser;

        public DisciplineService(IDisciplineRepository DisciplineRepo, ILogger<DisciplineService> logger)
        {
            _disciplineRepository = DisciplineRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateDiscipline(DisciplineMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Code))
                throw new ArgumentException("Discipline code should not be empty!");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Discipline name should not be empty!");

            bool codeExists = await _disciplineRepository.ExistsByCode(model.Code.Trim());
            if (codeExists)
                throw new InvalidOperationException($"Discipline code '{model.Code.Trim()}' already exists!");

            bool exists = await _disciplineRepository.ExistsByName(model.Name.Trim());
            if (exists)
                throw new InvalidOperationException("Discipline name already exists!");

            model.Name = model.Name.Trim();
            model.Code = model.Code.Trim().ToUpperInvariant();
            model.Description = model.Description?.Trim();
            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;
            model.IsActive = true;

            await _disciplineRepository.AddDiscipline(model);
            _logger.LogInformation("Discipline '{DisciplineName}' created successfully.", model.Name);
        }

        public async Task ModifyDiscipline(DisciplineMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Discipline ID should not be empty!");

            if (string.IsNullOrWhiteSpace(model.Code))
                throw new ArgumentException("Discipline code should not be empty!");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Discipline name should not be empty!");

            bool codeExists = await _disciplineRepository.ExistsByCodeAndNotId(model.Code.Trim(), model.ID);
            if (codeExists)
                throw new InvalidOperationException($"Discipline code '{model.Code.Trim()}' already exists!");

            bool exists = await _disciplineRepository.ExistsByNameAndNotId(model.Name.Trim(), model.ID);
            if (exists)
                throw new InvalidOperationException("Discipline name already exists!");

            var existingDiscipline = await _disciplineRepository.GetDisciplineById(model.ID);
            if (existingDiscipline == null)
                throw new InvalidOperationException("Discipline not found!");

            existingDiscipline.Name = model.Name.Trim();
            existingDiscipline.Code = model.Code.Trim().ToUpperInvariant();
            existingDiscipline.Description = model.Description?.Trim();
            existingDiscipline.SortOrder = model.SortOrder;
            existingDiscipline.IsActive = model.IsActive;
            existingDiscipline.ModifiedOn = DateTime.UtcNow;
            existingDiscipline.ModifiedBy = loggedInUser.EmployeeID;

            await _disciplineRepository.UpdateDiscipline(existingDiscipline);
            _logger.LogInformation("Discipline '{DisciplineName}' updated successfully.", model.Name);
        }

        public async Task RemoveDiscipline(long id)
        {
            var existingDiscipline = await _disciplineRepository.GetDisciplineById(id);
            if (existingDiscipline == null)
                throw new InvalidOperationException("Discipline not found!");

            existingDiscipline.IsActive = false;
            existingDiscipline.ModifiedOn = DateTime.UtcNow;
            existingDiscipline.ModifiedBy = loggedInUser.EmployeeID;

            await _disciplineRepository.UpdateDiscipline(existingDiscipline);
            _logger.LogInformation("Discipline with ID '{DisciplineId}' deactivated successfully.", id);
        }

        public async Task<bool> ToggleDisciplineStatus(long id)
        {
            var existingDiscipline = await _disciplineRepository.GetDisciplineById(id);
            if (existingDiscipline == null)
                throw new InvalidOperationException("Discipline not found!");

            if (!existingDiscipline.IsActive && !string.IsNullOrWhiteSpace(existingDiscipline.Code))
            {
                bool codeExists = await _disciplineRepository.ExistsByCodeAndNotId(existingDiscipline.Code.Trim(), existingDiscipline.ID);
                if (codeExists)
                    throw new InvalidOperationException($"Cannot activate: Discipline code '{existingDiscipline.Code.Trim()}' is already in use by another active discipline.");
            }

            existingDiscipline.IsActive = !existingDiscipline.IsActive;
            existingDiscipline.ModifiedOn = DateTime.UtcNow;
            existingDiscipline.ModifiedBy = loggedInUser.EmployeeID;

            await _disciplineRepository.UpdateDiscipline(existingDiscipline);
            _logger.LogInformation("Discipline '{DisciplineName}' status toggled to {Status}.", existingDiscipline.Name, existingDiscipline.IsActive ? "Active" : "Inactive");
            return existingDiscipline.IsActive;
        }

        public async Task<DisciplineMaster> GetDisciplineDetails(long id)
        {
            var classification = await _disciplineRepository.GetDisciplineById(id);
            if (classification == null)
                throw new InvalidOperationException("Discipline not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchDisciplineList(PageFilter filter)
        {
            return await _disciplineRepository.GetAllDisciplines(filter);
        }

        public async Task<List<DropdwonSelector>> GetDisciplineDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _disciplineRepository.GetDisciplineDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
