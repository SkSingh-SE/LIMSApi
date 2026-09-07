using System.Text.RegularExpressions;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class AnalysisTechniqueService : IAnalysisTechniqueService
    {
        private static readonly Regex ValidCodeRegex = new(@"^[A-Z0-9_]+$", RegexOptions.Compiled);

        private readonly IAnalysisTechniqueRepository _repository;
        private readonly ILogger<AnalysisTechniqueService> _logger;
        private LoggedInUserDTO loggedInUser;

        public AnalysisTechniqueService(IAnalysisTechniqueRepository repository, ILogger<AnalysisTechniqueService> logger)
        {
            _repository = repository;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateAnalysisTechnique(AnalysisTechniqueCreateDto dto)
        {
            var normalizedCode = NormalizeAndValidateCode(dto.Code);
            var normalizedName = ValidateName(dto.Name);

            if (await _repository.ExistsByCode(normalizedCode))
                throw new InvalidOperationException($"An analysis technique with code '{normalizedCode}' already exists!");

            var entity = new AnalysisTechniqueMaster
            {
                Code = normalizedCode,
                Name = normalizedName,
                AliasNames = string.IsNullOrWhiteSpace(dto.AliasNames) ? null : dto.AliasNames.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                IsActive = dto.IsActive,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = loggedInUser.EmployeeID,
                CompanyCode = loggedInUser.CompanyCode
            };

            await _repository.AddAnalysisTechnique(entity);
            _logger.LogInformation("Analysis technique '{Name}' ({Code}) created successfully.", entity.Name, entity.Code);
        }

        public async Task ModifyAnalysisTechnique(AnalysisTechniqueUpdateDto dto)
        {
            if (dto.ID <= 0)
                throw new ArgumentException("Analysis technique ID should not be empty!");

            var normalizedCode = NormalizeAndValidateCode(dto.Code);
            var normalizedName = ValidateName(dto.Name);

            if (await _repository.ExistsByCodeAndNotId(normalizedCode, dto.ID))
                throw new InvalidOperationException($"An analysis technique with code '{normalizedCode}' already exists!");

            var existing = await _repository.GetAnalysisTechniqueById(dto.ID);
            if (existing == null)
                throw new InvalidOperationException("Analysis technique not found!");

            existing.Code = normalizedCode;
            existing.Name = normalizedName;
            existing.AliasNames = string.IsNullOrWhiteSpace(dto.AliasNames) ? null : dto.AliasNames.Trim();
            existing.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            existing.IsActive = dto.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateAnalysisTechnique(existing);
            _logger.LogInformation("Analysis technique '{Name}' ({Code}) updated successfully.", existing.Name, existing.Code);
        }

        public async Task<bool> ToggleAnalysisTechniqueStatus(long id)
        {
            var existing = await _repository.GetAnalysisTechniqueById(id);
            if (existing == null)
                throw new InvalidOperationException("Analysis technique not found!");

            if (!existing.IsActive)
            {
                // Reactivation guard: verify code collision across organization
                if (await _repository.ExistsByCodeAndNotId(existing.Code, existing.ID))
                    throw new InvalidOperationException($"Cannot reactivate: Technique code '{existing.Code}' is already in use by another technique.");
            }

            existing.IsActive = !existing.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateAnalysisTechnique(existing);
            _logger.LogInformation("Analysis technique '{Name}' ({Code}) status toggled to {Status}.", existing.Name, existing.Code, existing.IsActive ? "Active" : "Inactive");
            return existing.IsActive;
        }

        public async Task<AnalysisTechniqueDetailDto> GetAnalysisTechniqueDetails(long id)
        {
            var entity = await _repository.GetAnalysisTechniqueById(id);
            if (entity == null)
                throw new InvalidOperationException("Analysis technique not found!");

            return new AnalysisTechniqueDetailDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                AliasNames = entity.AliasNames,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedOn = entity.CreatedOn,
                ModifiedOn = entity.ModifiedOn
            };
        }

        public async Task<PagedResponse<object>> FetchAnalysisTechniqueList(PageFilter filter)
        {
            return await _repository.GetAllAnalysisTechniques(filter);
        }

        public async Task<List<DropdwonSelector>> GetAnalysisTechniqueDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetAnalysisTechniqueDropdown(searchTerm, pageNo, pageSize);
        }

        private static string NormalizeAndValidateCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Technique code should not be empty!");

            // Trim, convert whitespace to single underscore, uppercase
            var trimmed = code.Trim();
            var normalized = Regex.Replace(trimmed, @"\s+", "_").ToUpperInvariant();

            if (!ValidCodeRegex.IsMatch(normalized))
                throw new ArgumentException("Technique code can only contain uppercase letters, numbers, and underscores (e.g. GRAV_METRIC).");

            if (normalized.Length > 50)
                throw new ArgumentException("Technique code cannot exceed 50 characters.");

            return normalized;
        }

        private static string ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Technique name should not be empty!");

            var trimmed = name.Trim();
            if (trimmed.Length > 100)
                throw new ArgumentException("Technique name cannot exceed 100 characters.");

            return trimmed;
        }
    }
}
