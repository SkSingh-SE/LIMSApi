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
            existingEmployee.DateOfBirth = model.DateOfBirth;
            existingEmployee.DateOfJoin = model.DateOfJoin;
            existingEmployee.MobileNo = model.MobileNo;
            existingEmployee.EmergencyMobileNo = model.EmergencyMobileNo;
            existingEmployee.ResidentialAddressLine1 = model.ResidentialAddressLine1;
            existingEmployee.ResidentialAddressLine2 = model.ResidentialAddressLine2;
            existingEmployee.ResidentialPinCode = model.ResidentialPinCode;
            existingEmployee.ResidentialAreaID = model.ResidentialAreaID;
            existingEmployee.PermanentAddressLine1 = model.PermanentAddressLine1;
            existingEmployee.PermanentAddressLine2 = model.PermanentAddressLine2;
            existingEmployee.PermanentPinCode = model.PermanentPinCode;
            existingEmployee.PermanentAreaID = model.PermanentAreaID;
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
            existingEmployee.PANNumber = model.PANNumber;
            existingEmployee.BankName = model.BankName;
            existingEmployee.Branch = model.Branch;
            existingEmployee.AccountNumber = model.AccountNumber;
            existingEmployee.IFSCCode = model.IFSCCode;
            existingEmployee.AccountHolderName = model.AccountHolderName;

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
            var employee = await _employeeRepository.GetEmployeeById(id);
            if (employee == null)
                throw new InvalidOperationException("Employee not found!");

            return employee;
        }

        public async Task<PagedResponse<object>> FetchEmployeeList(PageFilter filter)
        {
            return await _employeeRepository.GetAllEmployees(filter);
        }

        public async Task<List<DropdwonSelector>> GetEmployeeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _employeeRepository.GetEmployeeDropdown(searchTerm, pageNo, pageSize);
        }

        // Employee Qualification Management
        public async Task AddEmployeeQualification(EmployeeQualification qualification)
        {
            _logger.LogInformation("Qualification '{Qualification}' created successfully.", qualification.Qualification);
            await _employeeRepository.AddEmployeeQualification(qualification);
        }

        public async Task UpdateEmployeeQualification(EmployeeQualification qualification)
        {
            _logger.LogInformation("Qualification '{Qualification}' updated successfully.", qualification.Qualification);
            await _employeeRepository.UpdateEmployeeQualification(qualification);
        }

        public async Task DeleteEmployeeQualification(long id)
        {
            await _employeeRepository.DeleteEmployeeQualification(id);
        }

        public async Task<List<EmployeeQualification>> GetEmployeeQualifications(long employeeId)
        {
            return await _employeeRepository.GetEmployeeQualifications(employeeId);
        }

        // Employee Document Management
        public async Task AddEmployeeDocument(EmployeeDocument document)
        {
            _logger.LogInformation("Document '{document}' created successfully.", document.DocumentType);
            await _employeeRepository.AddEmployeeDocument(document);
        }

        public async Task DeleteEmployeeDocument(long id)
        {
            await _employeeRepository.DeleteEmployeeDocument(id);
        }

        public async Task<EmployeeDocument?> GetEmployeeDocumentById(long id)
        {
            return await _employeeRepository.GetEmployeeDocumentById(id);
        }

        public async Task<List<EmployeeDocument>> GetEmployeeDocuments(long employeeId)
        {
            return await _employeeRepository.GetEmployeeDocuments(employeeId);
        }
    }
}
