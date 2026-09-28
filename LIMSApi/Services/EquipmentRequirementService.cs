using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class EquipmentRequirementService : IEquipmentRequirementService
    {
        private readonly IEquipmentRequirementRepository _repository;
        private readonly LIMSContext _context;
        private readonly IBranchContext _branchContext;
        private readonly ILogger<EquipmentRequirementService> _logger;
        private LoggedInUserDTO loggedInUser;

        public EquipmentRequirementService(
            IEquipmentRequirementRepository repository,
            LIMSContext context,
            IBranchContext branchContext,
            ILogger<EquipmentRequirementService> logger)
        {
            _repository = repository;
            _context = context;
            _branchContext = branchContext;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        private static string NormalizeCode(string code)
        {
            var normalized = code.Trim().ToUpperInvariant();
            normalized = Regex.Replace(normalized, @"\s+", "_");
            return normalized;
        }

        private async Task ValidateReferences(EquipmentRequirementCreateDto dto)
        {
            var test = await _context.LaboratoryTests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == dto.LaboratoryTestID && x.CompanyCode == loggedInUser.CompanyCode);
            if (test == null)
                throw new ArgumentException("Selected laboratory test is invalid or belongs to another company.");

            if (dto.TestMethodSpecificationID.HasValue)
            {
                var method = await _context.TestMethodSpecifications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.TestMethodSpecificationID.Value && x.CompanyCode == loggedInUser.CompanyCode);
                if (method == null)
                    throw new ArgumentException("Selected test method is invalid or belongs to another company.");

                if (dto.TestMethodSpecificationVersionID.HasValue)
                {
                    var version = await _context.TestMethodSpecificationVersions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ID == dto.TestMethodSpecificationVersionID.Value);
                    if (version == null)
                        throw new ArgumentException("Selected test method version is invalid.");
                    if (version.TestMethodSpecificationID != dto.TestMethodSpecificationID.Value)
                        throw new ArgumentException("Selected method version does not belong to the selected test method.");
                }
            }
            else if (dto.TestMethodSpecificationVersionID.HasValue)
            {
                throw new ArgumentException("Method version requires a test method to be selected.");
            }

            if (dto.ParameterID.HasValue)
            {
                var param = await _context.ParameterMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.ParameterID.Value && x.CompanyCode == loggedInUser.CompanyCode);
                if (param == null)
                    throw new ArgumentException("Selected parameter is invalid or belongs to another company.");
            }

            var type = await _context.EquipmentTypeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == dto.EquipmentTypeID && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
            if (type == null)
                throw new ArgumentException("Selected equipment type is invalid, inactive, or belongs to another company.");

            if (dto.EquipmentID.HasValue)
            {
                var equipment = await _context.EquipmentMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.EquipmentID.Value && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
                if (equipment == null)
                    throw new ArgumentException("Pinned equipment is invalid, inactive, or belongs to another company.");
            }

            if (dto.RangeUnitID.HasValue)
            {
                var unit = await _context.ParameterUnitMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == dto.RangeUnitID.Value && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);
                if (unit == null)
                    throw new ArgumentException("Selected range unit is invalid, inactive, or belongs to another company.");
            }

            if (dto.MinimumRange.HasValue && dto.MaximumRange.HasValue && dto.MinimumRange.Value > dto.MaximumRange.Value)
                throw new ArgumentException("Minimum range must be less than or equal to maximum range.");

            if ((dto.MinimumRange.HasValue || dto.MaximumRange.HasValue) && !dto.RangeUnitID.HasValue)
                throw new ArgumentException("Range unit is required when minimum or maximum range is specified.");
        }

        public async Task CreateRequirement(EquipmentRequirementCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Requirement code should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Requirement name should not be empty!");

            var code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Requirement code may contain only letters, numbers and underscore (_).");

            await ValidateReferences(dto);

            if (await _repository.ExistsByCode(code))
                throw new InvalidOperationException($"Requirement code '{code}' already exists!");

            if (await _repository.ExistsCombination(dto.LaboratoryTestID, dto.TestMethodSpecificationID, dto.ParameterID, dto.EquipmentTypeID))
                throw new InvalidOperationException("An active requirement already exists for this test, method, parameter and equipment type combination.");

            var model = new EquipmentRequirementMaster
            {
                Code = code,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                LaboratoryTestID = dto.LaboratoryTestID,
                TestMethodSpecificationID = dto.TestMethodSpecificationID,
                TestMethodSpecificationVersionID = dto.TestMethodSpecificationVersionID,
                ParameterID = dto.ParameterID,
                EquipmentTypeID = dto.EquipmentTypeID,
                EquipmentID = dto.EquipmentID,
                RequiredCapability = dto.RequiredCapability?.Trim(),
                MinimumRange = dto.MinimumRange,
                MaximumRange = dto.MaximumRange,
                RangeUnitID = dto.RangeUnitID,
                AccuracyRequirement = dto.AccuracyRequirement?.Trim(),
                ResolutionRequirement = dto.ResolutionRequirement?.Trim(),
                IsMandatory = dto.IsMandatory,
                DisplayOrder = dto.DisplayOrder,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = loggedInUser.EmployeeID,
                ModifiedOn = DateTime.UtcNow,
                ModifiedBy = loggedInUser.EmployeeID,
                CompanyCode = loggedInUser.CompanyCode,
                IsActive = true
            };

            await _repository.AddRequirement(model);
            _logger.LogInformation("Equipment requirement '{Name}' created successfully.", model.Name);
        }

        public async Task ModifyRequirement(EquipmentRequirementUpdateDto dto)
        {
            if (dto.ID == 0)
                throw new ArgumentException("Requirement ID should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Requirement code should not be empty!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Requirement name should not be empty!");

            var code = NormalizeCode(dto.Code);
            if (!Regex.IsMatch(code, @"^[A-Z0-9_]+$"))
                throw new ArgumentException("Requirement code may contain only letters, numbers and underscore (_).");

            await ValidateReferences(dto);

            if (await _repository.ExistsByCodeAndNotId(code, dto.ID))
                throw new InvalidOperationException($"Requirement code '{code}' already exists!");

            if (await _repository.ExistsCombinationAndNotId(dto.ID, dto.LaboratoryTestID, dto.TestMethodSpecificationID, dto.ParameterID, dto.EquipmentTypeID))
                throw new InvalidOperationException("An active requirement already exists for this test, method, parameter and equipment type combination.");

            var existing = await _context.EquipmentRequirementMasters
                .FirstOrDefaultAsync(x => x.ID == dto.ID && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Equipment requirement not found!");

            existing.Code = code;
            existing.Name = dto.Name.Trim();
            existing.Description = dto.Description?.Trim();
            existing.LaboratoryTestID = dto.LaboratoryTestID;
            existing.TestMethodSpecificationID = dto.TestMethodSpecificationID;
            existing.TestMethodSpecificationVersionID = dto.TestMethodSpecificationVersionID;
            existing.ParameterID = dto.ParameterID;
            existing.EquipmentTypeID = dto.EquipmentTypeID;
            existing.EquipmentID = dto.EquipmentID;
            existing.RequiredCapability = dto.RequiredCapability?.Trim();
            existing.MinimumRange = dto.MinimumRange;
            existing.MaximumRange = dto.MaximumRange;
            existing.RangeUnitID = dto.RangeUnitID;
            existing.AccuracyRequirement = dto.AccuracyRequirement?.Trim();
            existing.ResolutionRequirement = dto.ResolutionRequirement?.Trim();
            existing.IsMandatory = dto.IsMandatory;
            existing.DisplayOrder = dto.DisplayOrder;
            existing.IsActive = dto.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateRequirement(existing);
            _logger.LogInformation("Equipment requirement '{Name}' updated successfully.", existing.Name);
        }

        public async Task RemoveRequirement(long id)
        {
            var existing = await _context.EquipmentRequirementMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Equipment requirement not found!");

            existing.IsActive = false;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateRequirement(existing);
            _logger.LogInformation("Equipment requirement with ID '{Id}' deactivated successfully.", id);
        }

        public async Task<bool> ToggleRequirementStatus(long id)
        {
            var existing = await _context.EquipmentRequirementMasters
                .FirstOrDefaultAsync(x => x.ID == id && x.CompanyCode == loggedInUser.CompanyCode);

            if (existing == null)
                throw new InvalidOperationException("Equipment requirement not found!");

            if (!existing.IsActive && !string.IsNullOrWhiteSpace(existing.Code))
            {
                if (await _repository.ExistsByCodeAndNotId(existing.Code.Trim(), existing.ID))
                    throw new InvalidOperationException($"Cannot activate: requirement code '{existing.Code.Trim()}' is already in use by another active requirement.");
                if (await _repository.ExistsCombinationAndNotId(existing.ID, existing.LaboratoryTestID, existing.TestMethodSpecificationID, existing.ParameterID, existing.EquipmentTypeID))
                    throw new InvalidOperationException("Cannot activate: another active requirement already covers this test, method, parameter and equipment type combination.");
            }

            existing.IsActive = !existing.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateRequirement(existing);
            _logger.LogInformation("Equipment requirement '{Name}' status toggled to {Status}.", existing.Name, existing.IsActive ? "Active" : "Inactive");
            return existing.IsActive;
        }

        public async Task<EquipmentRequirementDetailDto> GetRequirementDetails(long id)
        {
            var entity = await _repository.GetRequirementById(id);
            if (entity == null)
                throw new InvalidOperationException("Equipment requirement not found!");

            var modName = await ResolveEmployeeName(entity.ModifiedBy, entity.CreatedBy);
            var creName = await ResolveEmployeeName(entity.CreatedBy, null);
            string? parameterUnit = null;
            if (entity.Parameter?.ParameterUnitID != null)
            {
                var unit = await _context.ParameterUnitMasters.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == entity.Parameter.ParameterUnitID.Value);
                parameterUnit = unit != null ? $"{unit.Name} ({unit.Symbol})" : null;
            }

            return new EquipmentRequirementDetailDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                LaboratoryTestID = entity.LaboratoryTestID,
                LaboratoryTestName = entity.LaboratoryTest?.Name ?? "-",
                TestMethodSpecificationID = entity.TestMethodSpecificationID,
                TestMethodName = entity.TestMethodSpecification?.Name,
                TestMethodSpecificationVersionID = entity.TestMethodSpecificationVersionID,
                TestMethodVersion = entity.TestMethodSpecificationVersion?.Version,
                ParameterID = entity.ParameterID,
                ParameterName = entity.Parameter?.Name,
                ParameterUnit = parameterUnit,
                EquipmentTypeID = entity.EquipmentTypeID,
                EquipmentTypeName = entity.EquipmentType?.Name ?? "-",
                EquipmentID = entity.EquipmentID,
                EquipmentName = entity.Equipment?.Name,
                RequiredCapability = entity.RequiredCapability,
                MinimumRange = entity.MinimumRange,
                MaximumRange = entity.MaximumRange,
                RangeUnitID = entity.RangeUnitID,
                RangeUnitName = entity.RangeUnit?.Name,
                AccuracyRequirement = entity.AccuracyRequirement,
                ResolutionRequirement = entity.ResolutionRequirement,
                IsMandatory = entity.IsMandatory,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                CompanyCode = entity.CompanyCode,
                CreatedByName = creName,
                CreatedOn = entity.CreatedOn,
                ModifiedByName = modName,
                ModifiedOn = entity.ModifiedOn
            };
        }

        private async Task<string> ResolveEmployeeName(long? employeeId, long? fallbackId)
        {
            var id = employeeId ?? fallbackId;
            if (!id.HasValue || id.Value == 0)
                return "-";
            var emp = await _context.EmployeeMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id.Value);
            return emp?.Name ?? "-";
        }

        public async Task<PagedResponse<object>> FetchRequirementList(EquipmentRequirementListRequest request)
        {
            return await _repository.GetPagedRequirements(request ?? new EquipmentRequirementListRequest());
        }

        public async Task<List<DropdwonSelector>> GetRequirementDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetRequirementDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<List<EligibleEquipmentDto>> GetEligibleEquipment(long requirementId, long branchId)
        {
            if (!_branchContext.IsAuthorizedForBranch(branchId, BranchAction.View))
                throw new UnauthorizedAccessException($"User is not authorized to view equipment in branch {branchId}.");

            var requirement = await _context.EquipmentRequirementMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == requirementId && x.IsActive && x.CompanyCode == loggedInUser.CompanyCode);

            if (requirement == null)
                throw new InvalidOperationException("Equipment requirement not found!");

            var now = DateTime.UtcNow;

            var equipment = await _context.EquipmentMasters
                .Include(e => e.Calibrations)
                .Include(e => e.Branch)
                .AsNoTracking()
                .Where(e => e.IsActive
                    && e.CompanyCode == loggedInUser.CompanyCode
                    && e.BranchID == branchId
                    && e.EquipmentTypeID == requirement.EquipmentTypeID)
                .ToListAsync();

            var typeName = (await _context.EquipmentTypeMasters.AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == requirement.EquipmentTypeID))?.Name ?? "-";

            var result = new List<EligibleEquipmentDto>();
            foreach (var eq in equipment)
            {
                var latestCal = eq.Calibrations?.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                var dueDate = latestCal?.CalibrationDueDate ?? eq.NextCalibrationDueDate;

                string calibrationStatus;
                bool selectable = true;
                string? blockReason = null;

                if (!eq.CalibrationRequired)
                {
                    calibrationStatus = "Not Required";
                }
                else if (!dueDate.HasValue)
                {
                    calibrationStatus = "Not Configured";
                    selectable = false;
                    blockReason = "Calibration validity is not configured for this equipment.";
                }
                else if (dueDate.Value < now)
                {
                    calibrationStatus = "Expired";
                    selectable = false;
                    blockReason = $"Calibration expired on {dueDate.Value:dd MMM yyyy}.";
                }
                else
                {
                    calibrationStatus = "Valid";
                }

                if (requirement.EquipmentID.HasValue && eq.ID != requirement.EquipmentID.Value)
                {
                    selectable = false;
                    blockReason = "Requirement pins a specific instrument; this equipment is not the pinned instrument.";
                }

                result.Add(new EligibleEquipmentDto
                {
                    EquipmentID = eq.ID,
                    Name = eq.Name,
                    EquipmentNo = eq.EquipmentNo,
                    ModelNo = eq.ModelNo,
                    BranchID = eq.BranchID,
                    BranchName = eq.Branch?.Name ?? "-",
                    EquipmentTypeID = eq.EquipmentTypeID,
                    EquipmentTypeName = typeName,
                    CalibrationRequired = eq.CalibrationRequired,
                    CalibrationStatus = calibrationStatus,
                    LastCalibrationDate = latestCal?.CalibrationDate ?? eq.LastCalibrationDate,
                    NextCalibrationDueDate = dueDate,
                    CalibrationCertificate = latestCal?.Certificate,
                    IsSelectable = selectable,
                    BlockReason = blockReason
                });
            }

            return result.OrderByDescending(x => x.IsSelectable).ThenBy(x => x.Name).ToList();
        }
    }
}
