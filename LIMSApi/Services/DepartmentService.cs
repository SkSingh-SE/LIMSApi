using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentService> _logger;
        private readonly LIMSContext _context;
        private readonly IBranchContext _branchContext;
        private readonly LoggedInUserDTO loggedInUser;

        public DepartmentService(
            IDepartmentRepository departmentRepo,
            ILogger<DepartmentService> logger,
            LIMSContext context,
            IBranchContext branchContext)
        {
            _departmentRepository = departmentRepo;
            _logger = logger;
            _context = context;
            _branchContext = branchContext;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateDepartment(DepartmentCreateDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.BranchID <= 0)
                throw new ArgumentException("Branch is required.");

            if (model.DisciplineID <= 0)
                throw new ArgumentException("Discipline is required.");

            if (string.IsNullOrWhiteSpace(model.Code))
                throw new ArgumentException("Department code is required.");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Department name is required.");

            // 1. Validate Branch exists and is active
            bool branchActive = await _departmentRepository.BranchExistsAndActive(model.BranchID);
            if (!branchActive)
                throw new InvalidOperationException("Selected branch does not exist or is inactive.");

            // 2. Validate Branch authorization
            if (!_branchContext.IsAuthorizedForBranch(model.BranchID, BranchAction.Create))
            {
                throw new UnauthorizedAccessException($"User is not authorized to create departments in branch {model.BranchID}.");
            }

            // 3. Validate Discipline exists and is active
            bool disciplineActive = await _departmentRepository.DisciplineExistsAndActive(model.DisciplineID);
            if (!disciplineActive)
                throw new InvalidOperationException("Selected discipline does not exist or is inactive.");

            // 4. Validate Branch supports Discipline (BranchDiscipline capability)
            bool branchSupportsDiscipline = await _departmentRepository.BranchSupportsDiscipline(model.BranchID, model.DisciplineID);
            if (!branchSupportsDiscipline)
                throw new InvalidOperationException("Selected branch does not support the selected discipline.");

            var trimmedCode = model.Code.Trim().ToUpperInvariant();
            var trimmedName = model.Name.Trim();

            // 5. Validate Department Code uniqueness within Branch
            bool codeExists = await _departmentRepository.ExistsByCodeInBranch(trimmedCode, model.BranchID);
            if (codeExists)
                throw new InvalidOperationException($"Department Code '{trimmedCode}' already exists in the selected branch.");

            // 6. Validate Department Name uniqueness within Branch
            bool nameExists = await _departmentRepository.ExistsByNameInBranch(trimmedName, model.BranchID);
            if (nameExists)
                throw new InvalidOperationException($"Department Name '{trimmedName}' already exists in the selected branch.");

            var entity = new DepartmentMaster
            {
                BranchID = model.BranchID,
                DisciplineID = model.DisciplineID,
                Code = trimmedCode,
                Name = trimmedName,
                Description = model.Description?.Trim(),
                IsChemical = model.IsChemical,
                IsActive = model.IsActive,
                CompanyCode = loggedInUser?.CompanyCode ?? "DEFAULT",
                CreatedBy = loggedInUser?.EmployeeID ?? 0,
                CreatedOn = DateTime.UtcNow
            };

            await _departmentRepository.AddDepartment(entity);
            _logger.LogInformation("Department '{DepartmentName}' ({Code}) created successfully for Branch {BranchID}.", entity.Name, entity.Code, entity.BranchID);
        }

        public async Task ModifyDepartment(DepartmentUpdateDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.ID <= 0)
                throw new ArgumentException("Department ID is required.");

            if (model.BranchID <= 0)
                throw new ArgumentException("Branch is required.");

            if (model.DisciplineID <= 0)
                throw new ArgumentException("Discipline is required.");

            if (string.IsNullOrWhiteSpace(model.Code))
                throw new ArgumentException("Department code is required.");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Department name is required.");

            var existingDepartment = await _departmentRepository.GetDepartmentById(model.ID);
            if (existingDepartment == null)
                throw new InvalidOperationException("Department not found!");

            // 1. Authorization checks for existing and new branch
            if (!_branchContext.IsAuthorizedForBranch(existingDepartment.BranchID, BranchAction.Edit))
            {
                throw new UnauthorizedAccessException($"User is not authorized to edit departments in branch {existingDepartment.BranchID}.");
            }

            if (existingDepartment.BranchID != model.BranchID && !_branchContext.IsAuthorizedForBranch(model.BranchID, BranchAction.Edit))
            {
                throw new UnauthorizedAccessException($"User is not authorized to move departments to branch {model.BranchID}.");
            }

            // 2. Validate Branch exists and is active
            bool branchActive = await _departmentRepository.BranchExistsAndActive(model.BranchID);
            if (!branchActive)
                throw new InvalidOperationException("Selected branch does not exist or is inactive.");

            // 3. Validate Discipline exists and is active
            bool disciplineActive = await _departmentRepository.DisciplineExistsAndActive(model.DisciplineID);
            if (!disciplineActive)
                throw new InvalidOperationException("Selected discipline does not exist or is inactive.");

            // 4. Validate Branch supports Discipline
            bool branchSupportsDiscipline = await _departmentRepository.BranchSupportsDiscipline(model.BranchID, model.DisciplineID);
            if (!branchSupportsDiscipline)
                throw new InvalidOperationException("Selected branch does not support the selected discipline.");

            var trimmedCode = model.Code.Trim().ToUpperInvariant();
            var trimmedName = model.Name.Trim();

            // 5. Validate Department Code uniqueness within Branch
            bool codeExists = await _departmentRepository.ExistsByCodeInBranchAndNotId(trimmedCode, model.BranchID, model.ID);
            if (codeExists)
                throw new InvalidOperationException($"Department Code '{trimmedCode}' already exists in the selected branch.");

            // 6. Validate Department Name uniqueness within Branch
            bool nameExists = await _departmentRepository.ExistsByNameInBranchAndNotId(trimmedName, model.BranchID, model.ID);
            if (nameExists)
                throw new InvalidOperationException($"Department Name '{trimmedName}' already exists in the selected branch.");

            existingDepartment.BranchID = model.BranchID;
            existingDepartment.DisciplineID = model.DisciplineID;
            existingDepartment.Code = trimmedCode;
            existingDepartment.Name = trimmedName;
            existingDepartment.Description = model.Description?.Trim();
            existingDepartment.IsChemical = model.IsChemical;
            existingDepartment.IsActive = model.IsActive;
            existingDepartment.ModifiedOn = DateTime.UtcNow;
            existingDepartment.ModifiedBy = loggedInUser?.EmployeeID ?? 0;

            await _departmentRepository.UpdateDepartment(existingDepartment);
            _logger.LogInformation("Department '{DepartmentName}' ({Code}) updated successfully.", existingDepartment.Name, existingDepartment.Code);
        }

        public async Task RemoveDepartment(long id)
        {
            var existingDepartment = await _departmentRepository.GetDepartmentById(id);
            if (existingDepartment == null)
                throw new InvalidOperationException("Department not found!");

            if (!_branchContext.IsAuthorizedForBranch(existingDepartment.BranchID, BranchAction.Delete))
            {
                throw new UnauthorizedAccessException($"User is not authorized to delete departments in branch {existingDepartment.BranchID}.");
            }

            await DeleteValidationHelper.ValidateDeleteAsync<DepartmentMaster>(_context, id, "Department", existingDepartment.Name);

            existingDepartment.IsActive = false;
            existingDepartment.ModifiedOn = DateTime.UtcNow;
            existingDepartment.ModifiedBy = loggedInUser?.EmployeeID ?? 0;

            await _departmentRepository.UpdateDepartment(existingDepartment);
            _logger.LogInformation("Department with ID '{DepartmentId}' deactivated successfully.", id);
        }

        public async Task<bool> ToggleDepartmentStatus(long id)
        {
            var existingDepartment = await _departmentRepository.GetDepartmentById(id);
            if (existingDepartment == null)
                throw new InvalidOperationException("Department not found!");

            if (!_branchContext.IsAuthorizedForBranch(existingDepartment.BranchID, BranchAction.Edit))
            {
                throw new UnauthorizedAccessException($"User is not authorized to modify departments in branch {existingDepartment.BranchID}.");
            }

            bool activating = !existingDepartment.IsActive;

            if (activating)
            {
                // Verify Branch is active
                bool branchActive = await _departmentRepository.BranchExistsAndActive(existingDepartment.BranchID);
                if (!branchActive)
                    throw new InvalidOperationException("Cannot activate department: Associated branch is inactive.");

                // If discipline is assigned, verify it is active and supported
                if (existingDepartment.DisciplineID.HasValue)
                {
                    bool disciplineActive = await _departmentRepository.DisciplineExistsAndActive(existingDepartment.DisciplineID.Value);
                    if (!disciplineActive)
                        throw new InvalidOperationException("Cannot activate department: Associated discipline is inactive.");

                    bool branchSupports = await _departmentRepository.BranchSupportsDiscipline(existingDepartment.BranchID, existingDepartment.DisciplineID.Value);
                    if (!branchSupports)
                        throw new InvalidOperationException("Cannot activate department: Branch does not support this discipline.");
                }

                // Verify Code uniqueness within Branch
                if (!string.IsNullOrWhiteSpace(existingDepartment.Code))
                {
                    bool codeExists = await _departmentRepository.ExistsByCodeInBranchAndNotId(existingDepartment.Code.Trim(), existingDepartment.BranchID, existingDepartment.ID);
                    if (codeExists)
                        throw new InvalidOperationException($"Cannot activate department: Department Code '{existingDepartment.Code.Trim()}' is already in use by an active department in this branch.");
                }

                // Verify Name uniqueness within Branch
                bool nameExists = await _departmentRepository.ExistsByNameInBranchAndNotId(existingDepartment.Name.Trim(), existingDepartment.BranchID, existingDepartment.ID);
                if (nameExists)
                    throw new InvalidOperationException($"Cannot activate department: Department Name '{existingDepartment.Name.Trim()}' is already in use by an active department in this branch.");
            }

            existingDepartment.IsActive = activating;
            existingDepartment.ModifiedOn = DateTime.UtcNow;
            existingDepartment.ModifiedBy = loggedInUser?.EmployeeID ?? 0;

            await _departmentRepository.UpdateDepartment(existingDepartment);
            _logger.LogInformation("Department '{DepartmentName}' status toggled to {Status}.", existingDepartment.Name, existingDepartment.IsActive ? "Active" : "Inactive");
            return existingDepartment.IsActive;
        }

        public async Task<DepartmentDetailDto> GetDepartmentDetails(long id)
        {
            var entity = await _departmentRepository.GetDepartmentById(id);
            if (entity == null)
                throw new InvalidOperationException("Department not found!");

            return new DepartmentDetailDto
            {
                ID = entity.ID,
                BranchID = entity.BranchID,
                BranchName = entity.Branch?.Name ?? "-",
                DisciplineID = entity.DisciplineID,
                DisciplineName = entity.Discipline?.Name ?? "-",
                DisciplineCode = entity.Discipline?.Code ?? "-",
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                IsChemical = entity.IsChemical,
                IsActive = entity.IsActive
            };
        }

        public async Task<PagedResponse<object>> FetchDepartmentList(PageFilter filter)
        {
            return await _departmentRepository.GetAllDepartments(filter);
        }

        public async Task<List<DropdwonSelector>> GetDepartmentDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _departmentRepository.GetDepartmentDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<object>> GetBranchDisciplines(long branchId)
        {
            return await _departmentRepository.GetBranchDisciplines(branchId);
        }

        public async Task<List<object>> GetAuthorizedBranches()
        {
            return await _departmentRepository.GetAuthorizedBranches();
        }
    }
}
