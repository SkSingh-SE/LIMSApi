using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentService> _logger;
        private readonly LIMSContext _context;
        private readonly LoggedInUserDTO loggedInUser;

        public DepartmentService(IDepartmentRepository departmentrepo, ILogger<DepartmentService> logger, LIMSContext context)
        {
            _departmentRepository = departmentrepo;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateDepartment(DepartmentMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Department name should not be empty!");

            bool exists = await _departmentRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Department already exists!");

            await _departmentRepository.AddDepartment(model);
            _logger.LogInformation("Department '{DepartmentName}' created successfully.", model.Name);
        }

        public async Task ModifyDepartment(DepartmentMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Department ID should not be empty!");

            bool exists = await _departmentRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Department already exists!");

            var existingDepartment = await _departmentRepository.GetDepartmentById(model.ID);
            if (existingDepartment == null)
                throw new InvalidOperationException("Department not found!");

            existingDepartment.Name = model.Name;
            existingDepartment.Description = model.Description;
            existingDepartment.IsChemical = model.IsChemical;
            existingDepartment.ModifiedOn = DateTime.UtcNow;
            existingDepartment.ModifiedBy = loggedInUser?.EmployeeID ?? 0;

            await _departmentRepository.UpdateDepartment(existingDepartment);
            _logger.LogInformation("Department '{DepartmentName}' updated successfully.", model.Name);
        }

        public async Task RemoveDepartment(long id)
        {
            var existingDepartment = await _departmentRepository.GetDepartmentById(id);
            if (existingDepartment == null)
                throw new InvalidOperationException("Department not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<DepartmentMaster>(_context, id, "Department", existingDepartment.Name);

            existingDepartment.IsActive = false;
            existingDepartment.ModifiedOn = DateTime.UtcNow;
            existingDepartment.ModifiedBy = loggedInUser?.EmployeeID ?? 0;

            await _departmentRepository.UpdateDepartment(existingDepartment);
            _logger.LogInformation("Department with ID '{DepartmentId}' deleted successfully.", id);
        }

        public async Task<DepartmentMaster> GetDepartmentDetails(long id)
        {
            var classification = await _departmentRepository.GetDepartmentById(id);
            if (classification == null)
                throw new InvalidOperationException("Department not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchDepartmentList(PageFilter filter)
        {
            return await _departmentRepository.GetAllDepartments(filter);
        }

        public async Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _departmentRepository.GetDepartmentDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
