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
        private readonly IFileUploadService _uploadService;
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public EmployeeService(IEmployeeRepository employeeRepo, ILogger<EmployeeService> logger, IFileUploadService uploadService, IAuthService authService, IUserRepository userRepository)
        {
            _employeeRepository = employeeRepo;
            _logger = logger;
            _uploadService = uploadService;
            _authService = authService;
            _userRepository = userRepository;
        }

        public async Task CreateEmployee(EmployeeMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Employee name should not be empty!");

            bool exists = await _employeeRepository.ExistsByEmail(model.EmailId);
            if (exists)
                throw new InvalidOperationException("Employee already exists!");

            var createdEmployee = await _employeeRepository.AddEmployee(model);
            _logger.LogInformation("Employee '{EmployeeName}' created successfully.", model.Name);

            UserMaster newUser = new UserMaster
            {
                EmployeeID = createdEmployee.ID,
                UserName = model.Name,
                Password = _authService.GetHashedPassword(model.Password),
                EmailId = model.EmailId,
                RoleID = model.RoleID,
                RoleName = "User",
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = model.CreatedBy,
                CompanyCode = model.CompanyCode
            };
            await _authService.RegisterUser(newUser);

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
            existingEmployee.MaritalStatus = model.MaritalStatus;
            existingEmployee.SpouseName = model.SpouseName;
            existingEmployee.FatherName = model.FatherName;
            existingEmployee.MotherName = model.MotherName;
            existingEmployee.BloodGroup = model.BloodGroup;
            existingEmployee.ReportingManagerID = model.ReportingManagerID;
            existingEmployee.DepartmentID = model.DepartmentID;
            existingEmployee.DesignationID = model.DesignationID;
            existingEmployee.UserID = model.UserID;
            existingEmployee.PANNumber = model.PANNumber;
            existingEmployee.BankName = model.BankName;
            existingEmployee.Branch = model.Branch;
            existingEmployee.AccountNumber = model.AccountNumber;
            existingEmployee.IFSCCode = model.IFSCCode;
            existingEmployee.AccountHolderName = model.AccountHolderName;
            existingEmployee.EmailId = model.EmailId;
            existingEmployee.Password = model.Password;
            existingEmployee.RoleID = model.RoleID;

            existingEmployee.ModifiedOn = DateTime.UtcNow;

            await _employeeRepository.UpdateEmployee(existingEmployee);
            _logger.LogInformation("Employee '{EmployeeName}' updated successfully.", model.Name);

            var hashedPassword = _authService.GetHashedPassword(model.Password);
            // Update user details if UserID is provided
            var user = await _userRepository.GetUserByEmail(model.EmailId);
            if (user != null)
            {
                user.UserName = model.Name;
                user.EmailId = model.EmailId;
                user.RoleID = model.RoleID;
                user.Password = hashedPassword == user.Password ? user.Password : hashedPassword;
                user.ModifiedOn = DateTime.UtcNow;
                await _userRepository.UpdateUser(user);
                _logger.LogInformation("User '{UserName}' updated successfully.", user.UserName);
            }

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

        public async Task CreateDocuments(List<EmployeeDocument> model)
        {
            foreach (var document in model)
            {
                if (document.file != null)
                {
                    var fileUploadResponse = await _uploadService.UploadFileAsync(document.file, FileType.Employee, null, document.FileName);
                    if (fileUploadResponse == null)
                        throw new InvalidOperationException("File upload failed!");
                    document.FilePath = fileUploadResponse.FilePath;
                    document.FileName = fileUploadResponse.OriginalFileName;
                    document.UploadReferenceID = fileUploadResponse.ID;
                    document.UploadedOn = DateTime.UtcNow;

                    await _employeeRepository.AddEmployeeDocument(document);
                }
            }
            _logger.LogInformation("Documents created successfully.");
        }

        public async Task CreateQualifications(List<EmployeeQualification> model)
        {
            foreach (var qualification in model)
            {
                await _employeeRepository.AddEmployeeQualification(qualification);
            }
        }

        public async Task ModifyDocuments(List<EmployeeDocument> model)
        {
            var employeeId = model.FirstOrDefault()?.EmployeeID ?? 0;
            if (employeeId == 0) return;

            // Fetch existing documents
            var existingDocs = await _employeeRepository.GetEmployeeDocuments(employeeId);
            var incomingIds = model.Where(d => d.ID != 0).Select(d => d.ID).ToList();

            // 1. Delete documents that were removed
            var toDelete = existingDocs.Where(d => !incomingIds.Contains(d.ID)).ToList();
            foreach (var doc in toDelete)
            {
                await _employeeRepository.DeleteEmployeeDocument(doc.ID);
            }

            // 2. Update or add
            foreach (var document in model)
            {
                if (document.ID != 0)
                {
                    var existingDoc = await _employeeRepository.GetEmployeeDocumentById(document.ID);
                    if (existingDoc != null)
                    {
                        // Replace file if a new one is uploaded
                        if (document.file != null)
                        {
                            var fileUploadResponse = await _uploadService.UploadFileAsync(document.file, FileType.Employee, null, document.DocumentType);
                            if (fileUploadResponse == null)
                                throw new InvalidOperationException("File upload failed!");

                            existingDoc.FilePath = fileUploadResponse.FilePath;
                            existingDoc.FileName = fileUploadResponse.OriginalFileName;
                            existingDoc.UploadReferenceID = fileUploadResponse.ID;
                            existingDoc.UploadedOn = DateTime.UtcNow;
                        }
                        else
                        {
                            existingDoc.FileName = document.FileName;
                            existingDoc.FilePath = document.FilePath;
                        }
                        existingDoc.DocumentType = document.DocumentType;
                        existingDoc.ModifiedOn = DateTime.UtcNow;

                        await _employeeRepository.UpdateEmployeeDocument(existingDoc);
                    }
                }
                else
                {
                    // New document
                    if (document.file != null)
                    {
                        var fileUploadResponse = await _uploadService.UploadFileAsync(document.file, FileType.Employee, null, document.DocumentType);
                        if (fileUploadResponse == null)
                            throw new InvalidOperationException("File upload failed!");

                        document.FilePath = fileUploadResponse.FilePath;
                        document.FileName = fileUploadResponse.OriginalFileName;
                        document.UploadReferenceID = fileUploadResponse.ID;
                        document.UploadedOn = DateTime.UtcNow;

                        await _employeeRepository.AddEmployeeDocument(document);
                    }
                }
            }

            _logger.LogInformation("Documents modified successfully.");
        }



        public async Task ModifyQualifications(List<EmployeeQualification> model)
        {
            var employeeId = model.FirstOrDefault()?.EmployeeID ?? 0;
            if (employeeId == 0) return;

            var existingQualifications = await _employeeRepository.GetEmployeeQualifications(employeeId);
            var incomingIds = model.Where(x => x.ID > 0).Select(x => x.ID).ToList();

            // 1. Delete qualifications that are not in the incoming list
            var toDelete = existingQualifications.Where(x => !incomingIds.Contains(x.ID)).ToList();
            foreach (var item in toDelete)
            {
                await _employeeRepository.DeleteEmployeeQualification(item.ID);
            }

            // 2. Update existing or add new
            foreach (var qualification in model)
            {
                if (qualification.ID > 0)
                {
                    var existingQualification = await _employeeRepository.GetEmployeeQualificationById(qualification.ID);
                    if (existingQualification != null)
                    {
                        existingQualification.Qualification = qualification.Qualification;
                        existingQualification.SchoolOrUniversity = qualification.SchoolOrUniversity;
                        existingQualification.PassingYear = qualification.PassingYear;
                        existingQualification.ModifiedOn = DateTime.UtcNow;

                        await _employeeRepository.UpdateEmployeeQualification(existingQualification);
                    }
                }
                else
                {
                    qualification.CreatedOn = DateTime.UtcNow;
                    await _employeeRepository.AddEmployeeQualification(qualification);
                }
            }
        }


    }
}
