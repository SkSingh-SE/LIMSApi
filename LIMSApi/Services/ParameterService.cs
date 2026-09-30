using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IParameterRepository _parameterRepository;
        private readonly ILogger<ParameterService> _logger;
        private readonly LIMSContext _context;
        private readonly FormulaEvaluator _formulaEvaluator;

        // Valid InputType values
        private static readonly HashSet<string> NumericInputTypes = new(StringComparer.OrdinalIgnoreCase)
            { "Decimal", "Integer" };
        private static readonly HashSet<string> DropdownInputTypes = new(StringComparer.OrdinalIgnoreCase)
            { "Dropdown", "MultiSelect" };
        private static readonly HashSet<string> AllInputTypes = new(StringComparer.OrdinalIgnoreCase)
            { "Decimal", "Integer", "Boolean", "Dropdown", "MultiSelect", "Text" };
        private static readonly HashSet<string> ValidParameterTypes = new(StringComparer.OrdinalIgnoreCase)
            { "Quantitative", "Qualitative", "Reported", "Observed", "Derived" };

        public ParameterService(IParameterRepository parameterRepo, ILogger<ParameterService> logger,
            LIMSContext context, FormulaEvaluator formulaEvaluator)
        {
            _parameterRepository = parameterRepo;
            _logger = logger;
            _context = context;
            _formulaEvaluator = formulaEvaluator;
        }

        public async Task CreateParameter(ParameterMaster model)
        {
            ValidateModel(model);

            bool exists = await _parameterRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Parameter Name already exists!");

            // Defaults
            model.InputType = string.IsNullOrWhiteSpace(model.InputType) ? "Decimal" : model.InputType;
            if (!string.IsNullOrWhiteSpace(model.ElementType))
                model.ElementType = model.ElementType.Trim().ToLower();

            // Formula: only keep if numeric type + isCalculated
            if (!model.IsCalculated || !NumericInputTypes.Contains(model.InputType))
            {
                model.IsCalculated = false;
                model.Formula = null;
                model.FormulaDisplay = null;
            }
            else
            {
                await ValidateFormulaExpression(model.Formula);
            }

            // Dropdown options: only for Dropdown/MultiSelect
            if (!DropdownInputTypes.Contains(model.InputType))
                model.DropdownOptions.Clear();

            await _parameterRepository.AddParameter(model);
            _logger.LogInformation("Parameter '{ParameterName}' created successfully.", model.Name);
        }

        public async Task ModifyParameter(ParameterMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Parameter ID should not be empty!");

            ValidateModel(model);

            bool exists = await _parameterRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Parameter Name already exists!");

            var existingParameter = await _parameterRepository.GetParameterById(model.ID);
            if (existingParameter == null)
                throw new InvalidOperationException("Parameter not found!");

            // Normalize InputType
            model.InputType = string.IsNullOrWhiteSpace(model.InputType) ? "Decimal" : model.InputType;

            // Update scalar fields
            existingParameter.Name         = model.Name;
            existingParameter.Symbol       = model.Symbol;
            existingParameter.ParameterType = model.ParameterType;
            existingParameter.InputType    = model.InputType;
            existingParameter.ParameterUnitID = NumericInputTypes.Contains(model.InputType) ? model.ParameterUnitID : null;
            existingParameter.ParameterUnitEquivalentID = NumericInputTypes.Contains(model.InputType) ? model.ParameterUnitEquivalentID : null;
            existingParameter.UnitConversionFactor = NumericInputTypes.Contains(model.InputType) ? model.UnitConversionFactor : null;
            existingParameter.DecimalPrecision = model.DecimalPrecision;
            existingParameter.Note         = model.Note;
            existingParameter.ElementType  = !string.IsNullOrWhiteSpace(model.ElementType)
                ? model.ElementType.Trim().ToLower()
                : model.ElementType;

            // Formula: only for numeric + isCalculated
            if (model.IsCalculated && NumericInputTypes.Contains(model.InputType))
            {
                await ValidateFormulaExpression(model.Formula);
                existingParameter.IsCalculated   = true;
                existingParameter.Formula        = model.Formula;
                existingParameter.FormulaDisplay = model.FormulaDisplay;
            }
            else
            {
                existingParameter.IsCalculated   = false;
                existingParameter.Formula        = null;
                existingParameter.FormulaDisplay = null;
            }

            // Dropdown Options sync (add / update / deactivate)
            if (DropdownInputTypes.Contains(model.InputType))
            {
                await SyncDropdownOptions(existingParameter, model.DropdownOptions?.ToList());
            }
            else
            {
                // Deactivate all options if switching away from dropdown
                foreach (var opt in existingParameter.DropdownOptions)
                    opt.IsActive = false;
            }

            existingParameter.ModifiedOn = DateTime.UtcNow;
            await _parameterRepository.UpdateParameter(existingParameter);
            _logger.LogInformation("Parameter '{ParameterName}' updated successfully.", model.Name);
        }

        public async Task RemoveParameter(long id)
        {
            var existingParameter = await _parameterRepository.GetParameterById(id);
            if (existingParameter == null)
                throw new InvalidOperationException("Parameter not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<ParameterMaster>(_context, id, "Parameter", existingParameter.Name);

            existingParameter.IsActive = false;
            existingParameter.ModifiedOn = DateTime.UtcNow;

            // Also deactivate child dropdown options
            if (existingParameter.DropdownOptions != null)
            {
                foreach (var opt in existingParameter.DropdownOptions)
                {
                    opt.IsActive = false;
                }
            }

            await _parameterRepository.UpdateParameter(existingParameter);
            _logger.LogInformation("Parameter '{ParameterName}' (ID: {ParameterId}) deleted successfully.", existingParameter.Name, id);
        }

        public async Task<ParameterMaster> GetParameterDetails(long id)
        {
            var parameter = await _parameterRepository.GetParameterById(id);
            if (parameter == null)
                throw new InvalidOperationException("Parameter not found!");
            return parameter;
        }

        public async Task<PagedResponse<object>> FetchChemicalParameterList(PageFilter filter)
            => await _parameterRepository.GetAllChemicalParameters(filter);

        public async Task<PagedResponse<object>> FetchMechanicalParameterList(PageFilter filter)
            => await _parameterRepository.GetAllMechanicalParameters(filter);

        public async Task<PagedResponse<object>> ParameterList(PageFilter filter)
            => await _parameterRepository.ParameterList(filter);

        public async Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize, string? elementTypes = null)
            => await _parameterRepository.GetParameterDropdown(searchTerm, pageNo, pageSize, elementTypes);

        public async Task<List<DropdwonSelector>> GetChemicalParameterDropdown(string? searchTerm, int pageNo, int pageSize)
            => await _parameterRepository.GetChemicalParameterDropdown(searchTerm, pageNo, pageSize);

        public async Task<List<DropdwonSelector>> GetMechanicalParameterDropdown(string? searchTerm, int pageNo, int pageSize)
            => await _parameterRepository.GetMechanicalParameterDropdown(searchTerm, pageNo, pageSize);

        // ─── Unified Universal Parameter Implementation ───────────────────────

        public async Task<PagedResponse<ParameterListItemDto>> GetAllParametersUnified(PageFilter filter)
            => await _parameterRepository.GetAllParametersUnified(filter);

        public async Task<ParameterDetailDto> GetParameterDetailById(long id)
        {
            var detail = await _parameterRepository.GetParameterDetailById(id);
            if (detail == null)
                throw new InvalidOperationException($"Parameter with ID {id} not found.");
            return detail;
        }

        public async Task<long> CreateParameterUnified(ParameterCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var normalizedCode = NormalizeCode(dto.Code);
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Parameter Name is required.");

            // Global Code uniqueness across entire table (active and inactive)
            bool codeExists = await _parameterRepository.ExistsByCode(normalizedCode);
            if (codeExists)
                throw new InvalidOperationException($"Parameter code '{normalizedCode}' already exists.");

            // Active Name uniqueness
            bool nameExists = await _parameterRepository.ExistsByName(dto.Name.Trim());
            if (nameExists)
                throw new InvalidOperationException($"Parameter with name '{dto.Name.Trim()}' already exists.");

            // Unit validation
            if (dto.ParameterUnitID.HasValue && dto.ParameterUnitID.Value > 0)
            {
                bool unitValid = await _context.ParameterUnitMasters.AnyAsync(u => u.ID == dto.ParameterUnitID.Value && u.IsActive);
                if (!unitValid)
                    throw new ArgumentException("Selected parameter unit is invalid or inactive.");
            }

            // Decimal precision validation
            if (dto.DecimalPrecision < 0 || dto.DecimalPrecision > 6)
                throw new ArgumentException("Decimal precision must be between 0 and 6.");

            // Input type & calculation role validation
            var inputType = string.IsNullOrWhiteSpace(dto.InputType) ? "Decimal" : dto.InputType.Trim();
            if (!UniversalInputTypes.Contains(inputType))
                throw new ArgumentException($"Invalid InputType '{inputType}'. Allowed values: Decimal, Integer, Boolean, Dropdown, MultiSelect, Text, Date.");

            var calcRole = string.IsNullOrWhiteSpace(dto.CalculationRole) ? "Input" : dto.CalculationRole.Trim();
            if (!UniversalCalculationRoles.Contains(calcRole))
                throw new ArgumentException($"Invalid CalculationRole '{calcRole}'. Allowed values: Input, Calculated, Derived, CurvePeak.");

            bool isCalculated = calcRole.Equals("Calculated", StringComparison.OrdinalIgnoreCase) ||
                                calcRole.Equals("Derived", StringComparison.OrdinalIgnoreCase) ||
                                dto.IsCalculated;

            string? formula = null;
            string? formulaDisplay = null;
            if (isCalculated)
            {
                await ValidateFormulaUniversal(dto.Formula, normalizedCode, 0);
                formula = dto.Formula?.Trim();
                formulaDisplay = string.IsNullOrWhiteSpace(dto.FormulaDisplay) ? formula : dto.FormulaDisplay.Trim();
            }

            var entity = new ParameterMaster
            {
                Code = normalizedCode,
                Name = dto.Name.Trim(),
                Symbol = dto.Symbol?.Trim(),
                ParameterType = NormalizeParameterType(dto.ParameterType),
                InputType = inputType,
                ParameterUnitID = dto.ParameterUnitID,
                ParameterUnitEquivalentID = dto.ParameterUnitEquivalentID,
                UnitConversionFactor = dto.UnitConversionFactor,
                DecimalPrecision = dto.DecimalPrecision,
                CalculationRole = calcRole,
                IsCalculated = isCalculated,
                Formula = formula,
                FormulaDisplay = formulaDisplay,
                Sequence = dto.Sequence,
                Note = dto.Description?.Trim(),
                ElementType = !string.IsNullOrWhiteSpace(dto.ElementType) ? dto.ElementType.Trim().ToLower() : "normal",
                IsActive = dto.IsActive
            };

            // Add dropdown options if Dropdown / MultiSelect
            if (DropdownInputTypes.Contains(inputType) && dto.DropdownOptions != null && dto.DropdownOptions.Any())
            {
                foreach (var opt in dto.DropdownOptions)
                {
                    entity.DropdownOptions.Add(new ParameterDropdownOption
                    {
                        DisplayText = opt.DisplayText?.Trim() ?? string.Empty,
                        Value = opt.Value?.Trim() ?? string.Empty,
                        DisplayOrder = opt.DisplayOrder,
                        IsDefault = opt.IsDefault,
                        IsActive = true
                    });
                }
            }

            await _parameterRepository.AddParameter(entity);
            _logger.LogInformation("Parameter '{Code}' ({Name}) created successfully with ID {ID}.", entity.Code, entity.Name, entity.ID);
            return entity.ID;
        }

        public async Task ModifyParameterUnified(ParameterUpdateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.ID <= 0) throw new ArgumentException("Parameter ID is required.");

            var existing = await _parameterRepository.GetParameterById(dto.ID);
            if (existing == null)
                throw new InvalidOperationException($"Parameter with ID {dto.ID} not found.");

            var normalizedCode = NormalizeCode(dto.Code);
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Parameter Name is required.");

            // Global Code uniqueness across entire table (active and inactive)
            bool codeExists = await _parameterRepository.ExistsByCodeAndNotId(normalizedCode, dto.ID);
            if (codeExists)
                throw new InvalidOperationException($"Parameter code '{normalizedCode}' already exists.");

            // Active Name uniqueness
            bool nameExists = await _parameterRepository.ExistsByNameAndNotId(dto.Name.Trim(), dto.ID);
            if (nameExists)
                throw new InvalidOperationException($"Parameter with name '{dto.Name.Trim()}' already exists.");

            // Unit validation
            if (dto.ParameterUnitID.HasValue && dto.ParameterUnitID.Value > 0)
            {
                bool unitValid = await _context.ParameterUnitMasters.AnyAsync(u => u.ID == dto.ParameterUnitID.Value && u.IsActive);
                if (!unitValid)
                    throw new ArgumentException("Selected parameter unit is invalid or inactive.");
            }

            // Decimal precision validation
            if (dto.DecimalPrecision < 0 || dto.DecimalPrecision > 6)
                throw new ArgumentException("Decimal precision must be between 0 and 6.");

            // Input type & calculation role validation
            var inputType = string.IsNullOrWhiteSpace(dto.InputType) ? "Decimal" : dto.InputType.Trim();
            if (!UniversalInputTypes.Contains(inputType))
                throw new ArgumentException($"Invalid InputType '{inputType}'. Allowed values: Decimal, Integer, Boolean, Dropdown, MultiSelect, Text, Date.");

            var calcRole = string.IsNullOrWhiteSpace(dto.CalculationRole) ? "Input" : dto.CalculationRole.Trim();
            if (!UniversalCalculationRoles.Contains(calcRole))
                throw new ArgumentException($"Invalid CalculationRole '{calcRole}'. Allowed values: Input, Calculated, Derived, CurvePeak.");

            bool isCalculated = calcRole.Equals("Calculated", StringComparison.OrdinalIgnoreCase) ||
                                calcRole.Equals("Derived", StringComparison.OrdinalIgnoreCase) ||
                                dto.IsCalculated;

            string? formula = null;
            string? formulaDisplay = null;
            if (isCalculated)
            {
                await ValidateFormulaUniversal(dto.Formula, normalizedCode, dto.ID);
                formula = dto.Formula?.Trim();
                formulaDisplay = string.IsNullOrWhiteSpace(dto.FormulaDisplay) ? formula : dto.FormulaDisplay.Trim();
            }

            // Update entity
            existing.Code = normalizedCode;
            existing.Name = dto.Name.Trim();
            existing.Symbol = dto.Symbol?.Trim();
            existing.ParameterType = NormalizeParameterType(dto.ParameterType);
            existing.InputType = inputType;
            existing.ParameterUnitID = dto.ParameterUnitID;
            existing.ParameterUnitEquivalentID = dto.ParameterUnitEquivalentID;
            existing.UnitConversionFactor = dto.UnitConversionFactor;
            existing.DecimalPrecision = dto.DecimalPrecision;
            existing.CalculationRole = calcRole;
            existing.IsCalculated = isCalculated;
            existing.Formula = formula;
            existing.FormulaDisplay = formulaDisplay;
            existing.Sequence = dto.Sequence;
            existing.Note = dto.Description?.Trim();
            existing.IsActive = dto.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.ElementType))
                existing.ElementType = dto.ElementType.Trim().ToLower();

            // Sync dropdown options
            if (DropdownInputTypes.Contains(inputType))
            {
                var incomingModels = dto.DropdownOptions?.Select(o => new ParameterDropdownOption
                {
                    ID = o.ID,
                    ParameterID = existing.ID,
                    DisplayText = o.DisplayText,
                    Value = o.Value,
                    DisplayOrder = o.DisplayOrder,
                    IsDefault = o.IsDefault,
                    IsActive = o.IsActive
                }).ToList();

                await SyncDropdownOptions(existing, incomingModels);
            }
            else
            {
                foreach (var opt in existing.DropdownOptions)
                    opt.IsActive = false;
            }

            await _parameterRepository.UpdateParameter(existing);
            _logger.LogInformation("Parameter '{Code}' ({Name}) updated successfully.", existing.Code, existing.Name);
        }

        public async Task<bool> ToggleParameterStatus(long id)
        {
            var parameter = await _parameterRepository.GetParameterById(id);
            if (parameter == null)
                throw new InvalidOperationException($"Parameter with ID {id} not found.");

            if (parameter.IsActive)
            {
                // Soft deactivation - code remains permanently preserved
                parameter.IsActive = false;
                _logger.LogInformation("Parameter '{Code}' ({Name}) deactivated.", parameter.Code, parameter.Name);
            }
            else
            {
                // Collision guard on reactivation: check if another active record has the same name
                bool nameExists = await _parameterRepository.ExistsByNameAndNotId(parameter.Name, parameter.ID);
                if (nameExists)
                    throw new InvalidOperationException($"Cannot activate parameter because an active parameter with name '{parameter.Name}' already exists.");

                parameter.IsActive = true;
                _logger.LogInformation("Parameter '{Code}' ({Name}) reactivated.", parameter.Code, parameter.Name);
            }

            parameter.ModifiedOn = DateTime.UtcNow;
            await _parameterRepository.UpdateParameter(parameter);
            return parameter.IsActive;
        }

        public async Task<object> GetParameterTypesMetadata()
        {
            var units = await _context.ParameterUnitMasters
                .Where(u => u.IsActive)
                .OrderBy(u => u.Name)
                .Select(u => new
                {
                    u.ID,
                    u.Code,
                    u.Name,
                    u.Symbol,
                    u.QuantityType
                })
                .ToListAsync();

            return await Task.FromResult(new
            {
                ParameterTypes = UniversalParameterTypes.ToList(),
                InputTypes = UniversalInputTypes.ToList(),
                CalculationRoles = UniversalCalculationRoles.ToList(),
                Units = units
            });
        }

        private static readonly HashSet<string> UniversalInputTypes = new(StringComparer.OrdinalIgnoreCase)
            { "Decimal", "Integer", "Boolean", "Dropdown", "MultiSelect", "Text", "Date" };

        private static readonly HashSet<string> UniversalParameterTypes = new(StringComparer.OrdinalIgnoreCase)
            { "Quantitative", "Qualitative", "Reported", "Observed", "Derived" };

        private static readonly HashSet<string> UniversalCalculationRoles = new(StringComparer.OrdinalIgnoreCase)
            { "Input", "Calculated", "Derived", "CurvePeak" };

        private static string NormalizeCode(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode))
                throw new ArgumentException("Parameter Code is required.");

            var code = rawCode.Trim().ToUpper();
            if (code.Contains(' '))
                throw new ArgumentException("Parameter Code cannot contain spaces. Use underscores instead.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(code, "^[A-Z0-9_]+$"))
                throw new ArgumentException("Parameter Code must contain only uppercase letters, numbers, and underscores.");

            return code;
        }

        private async Task ValidateFormulaUniversal(string? formula, string currentCode, long currentId)
        {
            if (string.IsNullOrWhiteSpace(formula))
                throw new ArgumentException("Formula is required for calculated parameter.");

            var tokens = _formulaEvaluator.ExtractTokens(formula).ToList();
            if (!tokens.Any())
                throw new ArgumentException("Formula must contain at least one parameter reference (e.g. {SOIL_LL} or {P12}).");

            // Self-reference check
            foreach (var token in tokens)
            {
                if (string.Equals(token, currentCode, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(token, $"P{currentId}", StringComparison.OrdinalIgnoreCase) ||
                    (currentId > 0 && long.TryParse(token, out long tid) && tid == currentId))
                {
                    throw new ArgumentException($"Self-referencing formula is not allowed: parameter cannot reference '{token}'.");
                }
            }

            // Verify tokens exist as active codes or valid IDs
            var upperTokens = tokens.Select(t => t.ToUpper()).ToList();
            var validCodes = await _context.ParameterMasters
                .Where(p => p.IsActive && p.Code != null && upperTokens.Contains(p.Code.ToUpper()))
                .Select(p => p.Code!.ToUpper())
                .ToListAsync();

            var legacyIds = _formulaEvaluator.ExtractParamIds(formula).ToList();
            var validIds = await _context.ParameterMasters
                .Where(p => p.IsActive && legacyIds.Contains(p.ID))
                .Select(p => p.ID)
                .ToListAsync();

            // Validate that every token is either a valid code or a valid P{id}
            foreach (var token in tokens)
            {
                bool isCode = validCodes.Contains(token.ToUpper());
                bool isLegacyId = false;
                if (token.StartsWith("P", StringComparison.OrdinalIgnoreCase) && long.TryParse(token.Substring(1), out long pid))
                {
                    isLegacyId = validIds.Contains(pid);
                }

                if (!isCode && !isLegacyId)
                {
                    throw new ArgumentException($"Invalid parameter reference '{token}' in formula: parameter does not exist or is inactive.");
                }
            }

            // Test syntax with dummy values
            var dummyValues = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in tokens)
            {
                dummyValues[t] = 1.0;
                if (t.StartsWith("P", StringComparison.OrdinalIgnoreCase))
                    dummyValues[t.Substring(1)] = 1.0;
            }

            var testResult = _formulaEvaluator.Evaluate(formula, dummyValues);
            if (!testResult.HasValue)
            {
                throw new ArgumentException("Formula expression has a syntax or evaluation error.");
            }
        }

        /// <summary>
        /// Validates a formula expression against existing parameter IDs.
        /// Returns (isValid, errorMessage, parameterIds[]).
        /// </summary>
        public async Task<(bool IsValid, string? Error, IEnumerable<long> ParamIds)> ValidateFormulaForApi(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return (false, "Formula cannot be empty.", Enumerable.Empty<long>());

            var tokens = _formulaEvaluator.ExtractTokens(formula).ToList();
            if (!tokens.Any())
                return (false, "Formula must contain at least one parameter reference (e.g. {SOIL_LL} or {P12}).", Enumerable.Empty<long>());

            var paramIds = new List<long>();
            var upperTokens = tokens.Select(t => t.ToUpper()).ToList();

            // Find valid active parameter codes
            var parametersByCode = await _context.ParameterMasters
                .Where(p => p.IsActive && p.Code != null && upperTokens.Contains(p.Code.ToUpper()))
                .ToDictionaryAsync(p => p.Code!.ToUpper(), p => p.ID);

            // Find legacy IDs
            var legacyIds = _formulaEvaluator.ExtractParamIds(formula).ToList();
            var parametersById = await _context.ParameterMasters
                .Where(p => p.IsActive && legacyIds.Contains(p.ID))
                .ToDictionaryAsync(p => p.ID, p => p.ID);

            foreach (var token in tokens)
            {
                if (parametersByCode.TryGetValue(token.ToUpper(), out long idFromCode))
                {
                    paramIds.Add(idFromCode);
                }
                else if (token.StartsWith("P", StringComparison.OrdinalIgnoreCase) &&
                         long.TryParse(token.Substring(1), out long pid) &&
                         parametersById.ContainsKey(pid))
                {
                    paramIds.Add(pid);
                }
                else
                {
                    return (false, $"Invalid parameter reference '{token}' in formula: parameter does not exist or is inactive.", Enumerable.Empty<long>());
                }
            }

            // Syntax evaluation check with dummy variables
            var dummyDict = tokens.Distinct().ToDictionary(t => t, _ => 1.0, StringComparer.OrdinalIgnoreCase);
            var evalResult = _formulaEvaluator.Evaluate(formula, dummyDict);
            if (evalResult == null)
            {
                return (false, "Formula expression syntax error or invalid mathematical operations.", Enumerable.Empty<long>());
            }

            return (true, null, paramIds.Distinct());
        }

        // ──────────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────────

        private static string NormalizeParameterType(string? pType)
        {
            if (string.IsNullOrWhiteSpace(pType)) return "Reported";
            if (pType.Equals("Mechanical", StringComparison.OrdinalIgnoreCase)) return "Reported";
            if (pType.Equals("Observation", StringComparison.OrdinalIgnoreCase)) return "Observed";
            if (pType.Equals("Chemical", StringComparison.OrdinalIgnoreCase)) return "Chemical";
            if (pType.Equals("Reported", StringComparison.OrdinalIgnoreCase)) return "Reported";
            if (pType.Equals("Observed", StringComparison.OrdinalIgnoreCase)) return "Observed";
            return pType;
        }

        private static void ValidateModel(ParameterMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Parameter name should not be empty!");

            model.ParameterType = NormalizeParameterType(model.ParameterType);

            if (!string.IsNullOrWhiteSpace(model.InputType) && !AllInputTypes.Contains(model.InputType))
                throw new ArgumentException($"Invalid InputType '{model.InputType}'. Must be one of: Decimal, Integer, Boolean, Dropdown, MultiSelect, Text.");

            if (!string.IsNullOrWhiteSpace(model.ParameterType) && !ValidParameterTypes.Contains(model.ParameterType))
                throw new ArgumentException($"Invalid ParameterType '{model.ParameterType}'. Must be: Chemical, Reported, or Observed.");
        }

        private async Task ValidateFormulaExpression(string? formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
                throw new ArgumentException("Formula is required for calculated parameter.");

            var (isValid, error, _) = await ValidateFormulaForApi(formula);
            if (!isValid)
                throw new ArgumentException(error ?? "Invalid formula expression.");
        }

        private async Task SyncDropdownOptions(ParameterMaster existing, List<ParameterDropdownOption>? incomingOptions)
        {
            if (incomingOptions == null || !incomingOptions.Any())
                throw new ArgumentException("At least one dropdown option is required for Dropdown/MultiSelect parameter.");

            // Deactivate options not in incoming
            var incomingIds = incomingOptions.Where(o => o.ID > 0).Select(o => o.ID).ToHashSet();
            foreach (var opt in existing.DropdownOptions)
            {
                if (!incomingIds.Contains(opt.ID))
                    opt.IsActive = false;
            }

            foreach (var incoming in incomingOptions)
            {
                if (incoming.ID > 0)
                {
                    // Update existing
                    var existingOpt = existing.DropdownOptions.FirstOrDefault(o => o.ID == incoming.ID);
                    if (existingOpt != null)
                    {
                        existingOpt.DisplayText  = incoming.DisplayText;
                        existingOpt.Value        = incoming.Value;
                        existingOpt.DisplayOrder = incoming.DisplayOrder;
                        existingOpt.IsDefault    = incoming.IsDefault;
                        existingOpt.IsActive     = true;
                    }
                }
                else
                {
                    // Add new
                    existing.DropdownOptions.Add(new ParameterDropdownOption
                    {
                        ParameterID  = existing.ID,
                        DisplayText  = incoming.DisplayText,
                        Value        = incoming.Value,
                        DisplayOrder = incoming.DisplayOrder,
                        IsDefault    = incoming.IsDefault,
                        IsActive     = true
                    });
                }
            }

            await Task.CompletedTask;
        }
    }
}
