using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class SpecificationMasterService : ISpecificationMasterService
    {
        private static readonly Regex ValidCodeRegex = new(@"^[A-Z0-9_]+$", RegexOptions.Compiled);

        private readonly ISpecificationMasterRepository _repository;
        private readonly LIMSContext _context;
        private readonly ILogger<SpecificationMasterService> _logger;
        private LoggedInUserDTO loggedInUser;

        public SpecificationMasterService(ISpecificationMasterRepository repository, LIMSContext context, ILogger<SpecificationMasterService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        private string CurrentCompanyCode => !string.IsNullOrWhiteSpace(loggedInUser?.CompanyCode) ? loggedInUser.CompanyCode : "LIMS";
        private long CurrentEmployeeId => loggedInUser?.EmployeeID ?? 1;

        public async Task CreateSpecificationMaster(SpecificationMasterCreateDto dto)
        {
            var normalizedCode = NormalizeAndValidateCode(dto.Code);
            var normalizedName = ValidateName(dto.Name);

            var companyCode = CurrentCompanyCode;

            if (await _repository.ExistsByCode(normalizedCode, companyCode))
                throw new InvalidOperationException($"A specification with code '{normalizedCode}' already exists!");

            var entity = new SpecificationHeader
            {
                Code = normalizedCode,
                AliasName = normalizedName,
                Title = normalizedName,
                DisplayTitle = $"{normalizedCode} - {normalizedName}",
                SpecificationNo = normalizedCode,
                StandardReference = string.IsNullOrWhiteSpace(dto.StandardReference) ? null : dto.StandardReference.Trim(),
                Standard = string.IsNullOrWhiteSpace(dto.StandardReference) ? null : dto.StandardReference.Trim(),
                StandardOrganizationID = dto.StandardOrganizationID > 0 ? dto.StandardOrganizationID : null,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                IsActive = dto.IsActive,
                IsCustom = false,
                CompanyCode = companyCode,
                CreatedBy = CurrentEmployeeId,
                CreatedOn = DateTime.UtcNow
            };

            await _repository.AddSpecificationMaster(entity);
            _logger.LogInformation("Specification Master '{Name}' ({Code}) created successfully.", entity.AliasName, entity.Code);
        }

        public async Task ModifySpecificationMaster(SpecificationMasterUpdateDto dto)
        {
            if (dto.ID <= 0)
                throw new ArgumentException("Specification ID should not be empty!");

            var normalizedCode = NormalizeAndValidateCode(dto.Code);
            var normalizedName = ValidateName(dto.Name);

            var companyCode = CurrentCompanyCode;

            if (await _repository.ExistsByCodeAndNotId(normalizedCode, dto.ID, companyCode))
                throw new InvalidOperationException($"A specification with code '{normalizedCode}' already exists!");

            var existing = await _repository.GetSpecificationMasterById(dto.ID, companyCode);
            if (existing == null)
                throw new InvalidOperationException("Specification not found!");

            existing.Code = normalizedCode;
            existing.AliasName = normalizedName;
            existing.Title = normalizedName;
            existing.DisplayTitle = $"{normalizedCode} - {normalizedName}";
            existing.SpecificationNo = normalizedCode;
            existing.StandardReference = string.IsNullOrWhiteSpace(dto.StandardReference) ? null : dto.StandardReference.Trim();
            existing.Standard = string.IsNullOrWhiteSpace(dto.StandardReference) ? null : dto.StandardReference.Trim();
            existing.StandardOrganizationID = dto.StandardOrganizationID > 0 ? dto.StandardOrganizationID : null;
            existing.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            existing.IsActive = dto.IsActive;
            existing.ModifiedBy = CurrentEmployeeId;
            existing.ModifiedOn = DateTime.UtcNow;

            await _repository.UpdateSpecificationMaster(existing);
            _logger.LogInformation("Specification Master '{Name}' ({Code}) updated successfully.", existing.AliasName, existing.Code);
        }

        public async Task<bool> ToggleSpecificationMasterStatus(long id)
        {
            var companyCode = CurrentCompanyCode;
            var existing = await _repository.GetSpecificationMasterById(id, companyCode);
            if (existing == null)
                throw new InvalidOperationException("Specification not found!");

            if (!existing.IsActive && !string.IsNullOrWhiteSpace(existing.Code))
            {
                // Reactivation guard: verify permanent code collision
                if (await _repository.ExistsByCodeAndNotId(existing.Code, existing.ID, companyCode))
                    throw new InvalidOperationException($"Cannot reactivate: Specification code '{existing.Code}' is already in use by another specification.");
            }

            existing.IsActive = !existing.IsActive;
            existing.ModifiedBy = CurrentEmployeeId;
            existing.ModifiedOn = DateTime.UtcNow;

            await _repository.UpdateSpecificationMaster(existing);
            _logger.LogInformation("Specification Master '{Name}' ({Code}) status toggled to {Status}.", existing.AliasName, existing.Code, existing.IsActive ? "Active" : "Inactive");
            return existing.IsActive;
        }

        public async Task<SpecificationMasterDetailDto> GetSpecificationMasterDetails(long id)
        {
            var companyCode = CurrentCompanyCode;
            var entity = await _repository.GetSpecificationMasterById(id, companyCode);
            if (entity == null)
                throw new InvalidOperationException("Specification not found!");

            var versions = await _context.SpecificationVersions
                .AsNoTracking()
                .Where(v => v.SpecificationHeaderID == id)
                .ToListAsync();

            var activeVersion = versions.FirstOrDefault(v => v.Status == VersionStatus.Active && v.IsDefault)
                             ?? versions.FirstOrDefault(v => v.Status == VersionStatus.Active);

            int activeReqCount = 0;
            if (activeVersion != null)
            {
                activeReqCount = await _context.SpecificationLines
                    .AsNoTracking()
                    .CountAsync(l => l.SpecificationVersionID == activeVersion.ID);
            }

            return new SpecificationMasterDetailDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.AliasName,
                StandardReference = entity.StandardReference ?? entity.Standard,
                StandardOrganizationID = entity.StandardOrganizationID,
                StandardOrganizationName = entity.StandardOrganization?.Name,
                Description = entity.Description ?? entity.Title,
                IsActive = entity.IsActive,
                VersionCount = versions.Count,
                ActiveVersionName = activeVersion != null ? $"{activeVersion.Version}{(activeVersion.IsDefault ? " (Default)" : "")}" : null,
                ActiveVersionID = activeVersion?.ID,
                DraftVersionCount = versions.Count(v => v.Status == VersionStatus.Draft),
                SupersededVersionCount = versions.Count(v => v.Status == VersionStatus.Superseded),
                GradeCount = entity.Grades.Count,
                ActiveRequirementCount = activeReqCount,
                CreatedBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
                ModifiedBy = entity.ModifiedBy,
                ModifiedOn = entity.ModifiedOn,
                CompanyCode = entity.CompanyCode
            };
        }

        public async Task<PagedResponse<object>> FetchSpecificationMasterList(PageFilter filter)
        {
            return await _repository.GetAllSpecificationMasters(filter, CurrentCompanyCode);
        }

        public async Task<List<DropdwonSelector>> GetSpecificationMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetSpecificationMasterDropdown(searchTerm, pageNo, pageSize, CurrentCompanyCode);
        }

        public async Task<List<DropdwonSelector>> GetStandardOrganizationsDropdown()
        {
            return await _repository.GetStandardOrganizationsDropdown();
        }

        private static string NormalizeAndValidateCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Specification code should not be empty!");

            var trimmed = code.Trim();
            var normalized = Regex.Replace(trimmed, @"\s+", "_").ToUpperInvariant();

            if (!ValidCodeRegex.IsMatch(normalized))
                throw new ArgumentException("Specification code can only contain uppercase letters, numbers, and underscores (e.g. ASTM_A240).");

            if (normalized.Length > 100)
                throw new ArgumentException("Specification code cannot exceed 100 characters.");

            return normalized;
        }

        private static string ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Specification name should not be empty!");

            var trimmed = name.Trim();
            if (trimmed.Length > 100)
                throw new ArgumentException("Specification name cannot exceed 100 characters.");

            return trimmed;
        }
    }
}
