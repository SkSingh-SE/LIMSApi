using LIMSApi.Data;
using LIMSApi.Dtos;
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

        public DepartmentService(IDepartmentRepository departmentrepo, ILogger<DepartmentService> logger, LIMSContext context)
        {
            _departmentRepository = departmentrepo;
            _logger = logger;
            _context = context;
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
            existingDepartment.ModifiedOn = DateTime.UtcNow;

            await _departmentRepository.UpdateDepartment(existingDepartment);
            _logger.LogInformation("Department '{DepartmentName}' updated successfully.", model.Name);
        }

        public async Task RemoveDepartment(long id)
        {
            var existingDepartment = await _departmentRepository.GetDepartmentById(id);
            if (existingDepartment == null)
                throw new InvalidOperationException("Department not found!");

            bool hasEmployees = await _context.EmployeeMasters.AnyAsync(e => e.DepartmentID == id && e.IsActive);
            if (hasEmployees)
                throw new InvalidOperationException("Cannot delete: Department is linked to active employees.");

            existingDepartment.IsActive = false;
            existingDepartment.ModifiedOn = DateTime.UtcNow;

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
