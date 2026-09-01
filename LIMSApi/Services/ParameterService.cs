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
            { "Chemical", "Reported", "Observed", "Mechanical", "Observation" };

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

        /// <summary>
        /// Validates a formula expression against existing parameter IDs.
        /// Returns (isValid, errorMessage, parameterIds[]).
        /// </summary>
        public async Task<(bool IsValid, string? Error, IEnumerable<long> ParamIds)> ValidateFormulaForApi(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return (false, "Formula cannot be empty.", Enumerable.Empty<long>());

            var referencedIds = _formulaEvaluator.ExtractParamIds(formula).ToList();
            if (!referencedIds.Any())
                return (false, "Formula must contain at least one parameter reference.", Enumerable.Empty<long>());

            // Validate all referenced IDs exist and are active
            var existingIds = await _context.ParameterMasters
                .Where(p => p.IsActive && referencedIds.Contains(p.ID))
                .Select(p => p.ID)
                .ToListAsync();

            var error = _formulaEvaluator.ValidateFormula(formula, existingIds);
            if (error != null)
                return (false, error, Enumerable.Empty<long>());

            return (true, null, referencedIds);
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

            var referencedIds = _formulaEvaluator.ExtractParamIds(formula).ToList();
            if (!referencedIds.Any())
                throw new ArgumentException("Formula must contain at least one parameter reference (e.g. {P12}).");

            var existingIds = await _context.ParameterMasters
                .Where(p => p.IsActive && referencedIds.Contains(p.ID))
                .Select(p => p.ID)
                .ToListAsync();

            var error = _formulaEvaluator.ValidateFormula(formula, existingIds);
            if (error != null)
                throw new ArgumentException(error);
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
