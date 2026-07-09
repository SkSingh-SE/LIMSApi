using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LIMSApi.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IParameterRepository _parameterRepository;
        private readonly ILogger<ParameterService> _logger;
        private readonly LIMSContext _context;

        public ParameterService(IParameterRepository parameterRepo, ILogger<ParameterService> logger, LIMSContext context)
        {
            _parameterRepository = parameterRepo;
            _logger = logger;
            _context = context;
        }

        public async Task CreateParameter(ParameterMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Parameter name should not be empty!");

            bool exists = await _parameterRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Parameter Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _parameterRepository.ExistsByCode(model.Code);
                if (codeExists)
                    throw new InvalidOperationException("Parameter Code already exists!");
            }

            if (model.IsCalculated) ValidateFormula(model.Formula);
            model.Formula = model.IsCalculated ? model.Formula : null;
            if (!string.IsNullOrWhiteSpace(model.ElementType))
                model.ElementType = model.ElementType.Trim();

            await _parameterRepository.AddParameter(model);
            _logger.LogInformation("Parameter '{ParameterName}' created successfully.", model.Name);
        }

        public async Task ModifyParameter(ParameterMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Parameter ID should not be empty!");

            bool exists = await _parameterRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Parameter Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _parameterRepository.ExistsByCodeAndNotId(model.Code, model.ID);
                if (codeExists)
                    throw new InvalidOperationException("Parameter Code already exists!");
            }

            var existingParameter = await _parameterRepository.GetParameterById(model.ID);
            if (existingParameter == null)
                throw new InvalidOperationException("Parameter not found!");

            existingParameter.Name = model.Name;
            existingParameter.ParameterType = model.ParameterType;
            existingParameter.ParameterUnitID = model.ParameterUnitID;
            existingParameter.Note = model.Note;
            existingParameter.AliasName = model.AliasName;
            existingParameter.IsCalculated = model.IsCalculated;
            //if(model.IsCalculated) await ValidateFormula(model.Formula);
            existingParameter.Formula = model.IsCalculated ? model.Formula : null;
            existingParameter.Code = model.Code;
            existingParameter.Symbol = model.Symbol;
            existingParameter.DecimalPrecision = model.DecimalPrecision;
            existingParameter.MinReportableLimit = model.MinReportableLimit;
            existingParameter.DefaultTestMethodID = model.DefaultTestMethodID;
            existingParameter.ParameterCategoryID = model.ParameterCategoryID;
            existingParameter.ElementType = !string.IsNullOrWhiteSpace(model.ElementType)
                ? model.ElementType.Trim().ToLower()
                : model.ElementType;

            // Update AllowedOrientations junction
            existingParameter.AllowedOrientations?.Clear();
            if (model.AllowedOrientations != null)
            {
                foreach (var orientation in model.AllowedOrientations)
                {
                    existingParameter.AllowedOrientations.Add(orientation);
                }
            }

            existingParameter.ModifiedOn = DateTime.UtcNow;

            await _parameterRepository.UpdateParameter(existingParameter);
            _logger.LogInformation("Parameter '{ParameterName}' updated successfully.", model.Name);
        }
        private async Task ValidateFormula(string? formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
                throw new Exception("Formula is required for calculated parameter.");

            
            var matches = Regex.Matches(formula, @"\{P(\d+)\}");

            if (!matches.Any())
                throw new Exception("Formula must contain at least one parameter.");

            foreach (Match match in matches)
            {
                long paramId = long.Parse(match.Groups[1].Value);

                bool exists = await _parameterRepository.GetParameterById(paramId) == null ? false : true;

                if (!exists)
                    throw new Exception($"Invalid parameter reference: P{paramId}");
            }
        }


        public async Task RemoveParameter(long id)
        {
            var existingParameter = await _parameterRepository.GetParameterById(id);
            if (existingParameter == null)
                throw new InvalidOperationException("Parameter not found!");

            bool hasSpecLines = await _context.SpecificationLines.AnyAsync(s => s.ParameterID == id);
            if (hasSpecLines)
                throw new InvalidOperationException("Cannot delete: Parameter is linked to Material Specifications.");

            bool hasLabScope = await _context.LabScopeSpecificationParameters.AnyAsync(s => s.ParameterID == id);
            if (hasLabScope)
                throw new InvalidOperationException("Cannot delete: Parameter is linked to Lab Scope.");

            existingParameter.IsActive = false;
            existingParameter.ModifiedOn = DateTime.UtcNow;

            await _parameterRepository.UpdateParameter(existingParameter);
            _logger.LogInformation("Parameter with ID '{ParameterId}' deleted successfully.", id);
        }

        public async Task<ParameterMaster> GetParameterDetails(long id)
        {
            var classification = await _parameterRepository.GetParameterById(id);
            if (classification == null)
                throw new InvalidOperationException("Parameter not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchChemicalParameterList(PageFilter filter)
        {
            return await _parameterRepository.GetAllChemicalParameters(filter);
        }
        public async Task<PagedResponse<object>> FetchMechanicalParameterList(PageFilter filter)
        {
            return await _parameterRepository.GetAllMechanicalParameters(filter);
        }

        public async Task<PagedResponse<object>> ParameterList(PageFilter filter)
        {
            return await _parameterRepository.ParameterList(filter);
        }


        public async Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize, string? elementTypes = null)
        {
            return await _parameterRepository.GetParameterDropdown(searchTerm, pageNo, pageSize, elementTypes);
        }
        public async Task<List<DropdwonSelector>> GetChemicalParameterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _parameterRepository.GetChemicalParameterDropdown(searchTerm, pageNo, pageSize);
        } 
        public async Task<List<DropdwonSelector>> GetMechanicalParameterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _parameterRepository.GetMechanicalParameterDropdown(searchTerm, pageNo, pageSize);
        }

        private decimal EvaluateFormula(
    string formula,
    Dictionary<long, decimal> paramValues)
        {
            foreach (var kv in paramValues)
            {
                formula = formula.Replace(
                    $"{{P{kv.Key}}}",
                    kv.Value.ToString(CultureInfo.InvariantCulture));
            }

            var table = new DataTable();
            var result = table.Compute(formula, "");

            return Convert.ToDecimal(result);
        }

    }
}
