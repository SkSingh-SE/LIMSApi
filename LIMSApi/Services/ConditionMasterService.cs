using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class ConditionMasterService : IConditionMasterService
    {
        private static readonly Regex ValidCodeRegex = new(@"^[A-Z0-9_]+$", RegexOptions.Compiled);
        private static readonly HashSet<string> ValidValueTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Decimal", "Integer", "Text", "Boolean", "Date", "DateTime", "Selection"
        };

        private static readonly Dictionary<string, HashSet<string>> AllowedOperatorsByValueType = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Decimal", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=", ">", ">=", "<", "<=", "BETWEEN" } },
            { "Integer", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=", ">", ">=", "<", "<=", "BETWEEN" } },
            { "Date", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=", ">", ">=", "<", "<=", "BETWEEN" } },
            { "DateTime", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=", ">", ">=", "<", "<=", "BETWEEN" } },
            { "Text", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=", "CONTAINS" } },
            { "Boolean", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=" } },
            { "Selection", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=", "IN" } }
        };

        private readonly IConditionMasterRepository _repository;
        private readonly ILogger<ConditionMasterService> _logger;
        private LoggedInUserDTO loggedInUser;

        public ConditionMasterService(IConditionMasterRepository repository, ILogger<ConditionMasterService> logger)
        {
            _repository = repository;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateConditionMaster(ConditionMasterCreateDto dto)
        {
            var normalizedCode = NormalizeAndValidateCode(dto.Code);
            var normalizedName = ValidateName(dto.Name);
            var normalizedCategory = ValidateCategory(dto.Category);
            var normalizedValueType = ValidateValueType(dto.ValueType);
            var validatedOperators = ValidateAndNormalizeOperators(normalizedValueType, dto.AllowedOperators);
            var (allowedValuesJson, allowedValuesList) = ValidateAllowedValues(normalizedValueType, dto.AllowedValues);
            ValidateDefaultValue(normalizedValueType, dto.DefaultValue, allowedValuesList);

            if (await _repository.ExistsByCode(normalizedCode))
                throw new InvalidOperationException($"A condition master with code '{normalizedCode}' already exists!");

            var entity = new ConditionMaster
            {
                Code = normalizedCode,
                Name = normalizedName,
                Category = normalizedCategory,
                ValueType = normalizedValueType,
                ParameterUnitID = dto.ParameterUnitID,
                AllowedOperators = JsonSerializer.Serialize(validatedOperators),
                AllowedValuesJson = allowedValuesJson,
                DefaultValue = string.IsNullOrWhiteSpace(dto.DefaultValue) ? null : dto.DefaultValue.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = loggedInUser.EmployeeID,
                CompanyCode = loggedInUser.CompanyCode
            };

            await _repository.AddConditionMaster(entity);
            _logger.LogInformation("Condition master '{Name}' ({Code}) created successfully.", entity.Name, entity.Code);
        }

        public async Task ModifyConditionMaster(ConditionMasterUpdateDto dto)
        {
            if (dto.ID <= 0)
                throw new ArgumentException("Condition master ID must not be empty!");

            var normalizedCode = NormalizeAndValidateCode(dto.Code);
            var normalizedName = ValidateName(dto.Name);
            var normalizedCategory = ValidateCategory(dto.Category);
            var normalizedValueType = ValidateValueType(dto.ValueType);
            var validatedOperators = ValidateAndNormalizeOperators(normalizedValueType, dto.AllowedOperators);
            var (allowedValuesJson, allowedValuesList) = ValidateAllowedValues(normalizedValueType, dto.AllowedValues);
            ValidateDefaultValue(normalizedValueType, dto.DefaultValue, allowedValuesList);

            if (await _repository.ExistsByCodeAndNotId(normalizedCode, dto.ID))
                throw new InvalidOperationException($"A condition master with code '{normalizedCode}' already exists!");

            var existing = await _repository.GetConditionMasterById(dto.ID);
            if (existing == null)
                throw new KeyNotFoundException($"Condition master with ID {dto.ID} not found!");

            existing.Code = normalizedCode;
            existing.Name = normalizedName;
            existing.Category = normalizedCategory;
            existing.ValueType = normalizedValueType;
            existing.ParameterUnitID = dto.ParameterUnitID;
            existing.AllowedOperators = JsonSerializer.Serialize(validatedOperators);
            existing.AllowedValuesJson = allowedValuesJson;
            existing.DefaultValue = string.IsNullOrWhiteSpace(dto.DefaultValue) ? null : dto.DefaultValue.Trim();
            existing.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            existing.DisplayOrder = dto.DisplayOrder;
            existing.IsActive = dto.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateConditionMaster(existing);
            _logger.LogInformation("Condition master '{Name}' ({Code}) updated successfully.", existing.Name, existing.Code);
        }

        public async Task<bool> ToggleConditionMasterStatus(long id)
        {
            var existing = await _repository.GetConditionMasterById(id);
            if (existing == null)
                throw new KeyNotFoundException($"Condition master with ID {id} not found!");

            existing.IsActive = !existing.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = loggedInUser.EmployeeID;

            await _repository.UpdateConditionMaster(existing);
            _logger.LogInformation("Condition master ID {ID} status toggled to {IsActive}.", id, existing.IsActive);
            return existing.IsActive;
        }

        public async Task DeleteConditionMaster(long id)
        {
            var existing = await _repository.GetConditionMasterById(id);
            if (existing == null)
                throw new KeyNotFoundException($"Condition master with ID {id} not found!");

            var refCount = await _repository.GetReferenceCount(id);
            if (refCount > 0)
            {
                throw new InvalidOperationException(
                    $"Condition Master '{existing.Name}' ({existing.Code}) is referenced by {refCount} specification requirement line(s) and cannot be deleted. Please deactivate it instead.");
            }

            await _repository.DeleteConditionMaster(existing);
            _logger.LogInformation("Condition master '{Name}' ({Code}) deleted successfully.", existing.Name, existing.Code);
        }

        public async Task<ConditionMasterDto?> GetConditionMasterDetails(long id)
        {
            var entity = await _repository.GetConditionMasterById(id);
            if (entity == null) return null;

            List<string> ops = new();
            if (!string.IsNullOrWhiteSpace(entity.AllowedOperators))
            {
                try { ops = JsonSerializer.Deserialize<List<string>>(entity.AllowedOperators) ?? new(); }
                catch { ops = entity.AllowedOperators.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(); }
            }

            List<string>? vals = null;
            if (!string.IsNullOrWhiteSpace(entity.AllowedValuesJson))
            {
                try { vals = JsonSerializer.Deserialize<List<string>>(entity.AllowedValuesJson); }
                catch { }
            }

            var refCount = await _repository.GetReferenceCount(id);

            return new ConditionMasterDto
            {
                ID = entity.ID,
                Code = entity.Code,
                Name = entity.Name,
                Category = entity.Category,
                ValueType = entity.ValueType,
                ParameterUnitID = entity.ParameterUnitID,
                ParameterUnitName = entity.ParameterUnit?.Name,
                ParameterUnitSymbol = entity.ParameterUnit?.Symbol,
                AllowedOperators = ops,
                AllowedValues = vals,
                AllowedValuesJson = entity.AllowedValuesJson,
                DefaultValue = entity.DefaultValue,
                Description = entity.Description,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                CreatedBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
                ModifiedBy = entity.ModifiedBy,
                ModifiedOn = entity.ModifiedOn,
                CompanyCode = entity.CompanyCode,
                ReferenceCount = refCount
            };
        }

        public async Task<PagedResponse<object>> FetchConditionMasterList(PageFilter filter)
        {
            return await _repository.GetAllConditionMasters(filter);
        }

        public async Task<List<ConditionMasterDropdownDto>> GetConditionMasterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _repository.GetConditionMasterDropdown(searchTerm, pageNo, pageSize);
        }

        #region Helpers / Validation

        private static string NormalizeAndValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Condition Code is required.");

            var normalized = Regex.Replace(code.Trim().ToUpperInvariant(), @"[\s\-]+", "_");
            if (normalized.Length < 2 || normalized.Length > 50)
                throw new ArgumentException("Condition Code must be between 2 and 50 characters.");

            if (!ValidCodeRegex.IsMatch(normalized))
                throw new ArgumentException("Condition Code may only contain uppercase letters, numbers, and underscores (e.g. TEST_TEMP, SPEC_THICKNESS).");

            return normalized;
        }

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Condition Name is required.");

            var trimmed = name.Trim();
            if (trimmed.Length > 100)
                throw new ArgumentException("Condition Name cannot exceed 100 characters.");

            return trimmed;
        }

        private static string ValidateCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "General";

            var trimmed = category.Trim();
            if (trimmed.Length > 50)
                throw new ArgumentException("Category cannot exceed 50 characters.");

            return trimmed;
        }

        private static string ValidateValueType(string valueType)
        {
            if (string.IsNullOrWhiteSpace(valueType))
                throw new ArgumentException("Value Type is required.");

            var trimmed = valueType.Trim();
            if (!ValidValueTypes.Contains(trimmed))
                throw new ArgumentException($"Invalid Value Type '{trimmed}'. Supported types: {string.Join(", ", ValidValueTypes)}.");

            return trimmed;
        }

        private static List<string> ValidateAndNormalizeOperators(string valueType, List<string>? operators)
        {
            if (!AllowedOperatorsByValueType.TryGetValue(valueType, out var validForType))
            {
                validForType = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=" };
            }

            if (operators == null || !operators.Any())
            {
                // Default to all valid operators for this type
                return validForType.ToList();
            }

            var cleanList = new List<string>();
            foreach (var op in operators)
            {
                var trimmed = op.Trim().ToUpperInvariant();
                if (!validForType.Contains(trimmed))
                {
                    throw new ArgumentException($"Operator '{trimmed}' is not valid for Value Type '{valueType}'. Valid operators: {string.Join(", ", validForType)}.");
                }
                if (!cleanList.Contains(trimmed))
                {
                    cleanList.Add(trimmed);
                }
            }

            if (!cleanList.Any())
            {
                cleanList = validForType.ToList();
            }

            return cleanList;
        }

        private static (string? json, List<string>? list) ValidateAllowedValues(string valueType, List<string>? allowedValues)
        {
            if (valueType.Equals("Selection", StringComparison.OrdinalIgnoreCase))
            {
                if (allowedValues == null || !allowedValues.Any(v => !string.IsNullOrWhiteSpace(v)))
                {
                    throw new ArgumentException("At least one allowed value is required for Selection Value Type.");
                }

                var clean = allowedValues
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Select(v => v.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return (JsonSerializer.Serialize(clean), clean);
            }

            // For non-selection, allowed values must always be NULL to prevent invalid data
            return (null, null);
        }

        private static void ValidateDefaultValue(string valueType, string? defaultValue, List<string>? allowedValues)
        {
            if (string.IsNullOrWhiteSpace(defaultValue)) return;

            var val = defaultValue.Trim();
            switch (valueType)
            {
                case "Decimal":
                    if (!decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                        throw new ArgumentException($"Default value '{val}' is not a valid Decimal number.");
                    break;

                case "Integer":
                    if (!long.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                        throw new ArgumentException($"Default value '{val}' is not a valid Integer.");
                    break;

                case "Boolean":
                    if (!bool.TryParse(val, out _))
                        throw new ArgumentException($"Default value '{val}' is not a valid Boolean ('true' or 'false').");
                    break;

                case "Date":
                case "DateTime":
                    if (!DateTime.TryParse(val, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                        throw new ArgumentException($"Default value '{val}' is not a valid date/time.");
                    break;

                case "Selection":
                    if (allowedValues != null && allowedValues.Any() && !allowedValues.Contains(val, StringComparer.OrdinalIgnoreCase))
                        throw new ArgumentException($"Default value '{val}' must be one of the allowed values: {string.Join(", ", allowedValues)}.");
                    break;
            }
        }

        #endregion
    }
}
