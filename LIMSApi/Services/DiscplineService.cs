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
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Discipline name should not be empty!");

            bool exists = await _disciplineRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Discipline already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _disciplineRepository.AddDiscipline(model);
            _logger.LogInformation("Discipline '{DisciplineName}' created successfully.", model.Name);
        }

        public async Task ModifyDiscipline(DisciplineMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Discipline ID should not be empty!");

            bool exists = await _disciplineRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Discipline already exists!");

            var existingDiscipline = await _disciplineRepository.GetDisciplineById(model.ID);
            if (existingDiscipline == null)
                throw new InvalidOperationException("Discipline not found!");


            existingDiscipline.Name = model.Name;
            existingDiscipline.Description = model.Description;
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

            await _disciplineRepository.DeleteDiscipline(existingDiscipline);
            _logger.LogInformation("Discipline with ID '{DisciplineId}' deleted successfully.", id);
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
