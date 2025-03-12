using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeRepository employeeRepo, ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepo;
            _logger = logger;
        }

        public async Task CreateEmployee(EmployeeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Employee name should not be empty!");

            bool exists = await _employeeRepository.ExistsByEmail(model.EmailId);
            if (exists)
                throw new InvalidOperationException("Employee already exists!");

            await _employeeRepository.AddEmployee(model);
            _logger.LogInformation("Employee '{EmployeeName}' created successfully.", model.Name);
        }

        public async Task ModifyEmployee(EmployeeMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Employee ID should not be empty!");

            bool exists = await _employeeRepository.ExistsByEmailAndNotId(model.EmailId, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Employee already exists!");

            var existingEmployee = await _employeeRepository.GetEmployeeById(model.ID);
            if (existingEmployee == null)
                throw new InvalidOperationException("Employee not found!");

            existingEmployee.Name = model.Name;
            existingEmployee.Gender = model.Gender;
            existingEmployee.BirthDate = model.BirthDate;
            existingEmployee.JoinDate = model.JoinDate;
            existingEmployee.MobileNo = model.MobileNo;
            existingEmployee.EmergencyMobileNo = model.EmergencyMobileNo;
            existingEmployee.EmergencyMobileNo = model.EmergencyMobileNo;
            existingEmployee.ResidentialAddress = model.ResidentialAddress;
            existingEmployee.PermanentResidentialAddress = model.PermanentResidentialAddress;
            existingEmployee.IsTeamHead = model.IsTeamHead;
            existingEmployee.DigitalSignature = model.DigitalSignature;
            existingEmployee.IsMarried = model.IsMarried;
            existingEmployee.SpouseName = model.SpouseName;
            existingEmployee.FatherName = model.FatherName;
            existingEmployee.MotherName = model.MotherName;
            existingEmployee.BloodGroup = model.BloodGroup;
            existingEmployee.ReportingTo = model.ReportingTo;
            existingEmployee.DepartmentID = model.DepartmentID;
            existingEmployee.DesignationID = model.DesignationID;
            existingEmployee.UserID = model.UserID;
            existingEmployee.TestTypeID = model.TestTypeID;


            existingEmployee.ModifiedOn = DateTime.UtcNow;

            await _employeeRepository.UpdateEmployee(existingEmployee);
            _logger.LogInformation("Employee '{EmployeeName}' updated successfully.", model.Name);
        }

        public async Task RemoveEmployee(long id)
        {
            var existingEmployee = await _employeeRepository.GetEmployeeById(id);
            if (existingEmployee == null)
                throw new InvalidOperationException("Employee not found!");

            existingEmployee.IsActive = false;
            existingEmployee.ModifiedOn = DateTime.UtcNow;

            await _employeeRepository.UpdateEmployee(existingEmployee);
            _logger.LogInformation("Employee with ID '{EmployeeId}' deleted successfully.", id);
        }

        public async Task<EmployeeMaster> GetEmployeeDetails(long id)
        {
            var classification = await _employeeRepository.GetEmployeeById(id);
            if (classification == null)
                throw new InvalidOperationException("Employee not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchEmployeeList(PageFilter filter)
        {
            return await _employeeRepository.GetAllEmployees(filter);
        }

        public async Task<List<DropdwonSelector>> GetEmployeeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _employeeRepository.GetEmployeeDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
