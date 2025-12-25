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

        public ParameterService(IParameterRepository parameterRepo, ILogger<ParameterService> logger)
        {
            _parameterRepository = parameterRepo;
            _logger = logger;
        }

        public async Task CreateParameter(ParameterMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Parameter name should not be empty!");

            bool exists = await _parameterRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Parameter already exists!");

            if (model.IsCalculated) ValidateFormula(model.Formula);
            model.Formula = model.IsCalculated ? model.Formula : null;

            await _parameterRepository.AddParameter(model);
            _logger.LogInformation("Parameter '{ParameterName}' created successfully.", model.Name);
        }

        public async Task ModifyParameter(ParameterMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Parameter ID should not be empty!");

            bool exists = await _parameterRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Parameter already exists!");

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

        public async Task<List<DropdwonSelector>> GetParameterDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _parameterRepository.GetParameterDropdown(searchTerm, pageNo, pageSize);
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

    //    public async Task SaveTestResultAsync(
    //long resultHeaderId,
    //List<SampleTestResultValue> manualValues)
    //    {
    //        // 1️⃣ Save Manual Values First
    //        _context.SampleTestResultValues.AddRange(manualValues);
    //        await _context.SaveChangesAsync();

    //        // 2️⃣ Build Parameter Value Map
    //        var valueMap = manualValues
    //            .Where(x => x.Value.HasValue)
    //            .ToDictionary(
    //                x => x.TestParameterID,
    //                x => x.Value!.Value
    //            );

    //        // 3️⃣ Fetch Calculated Parameters Used in This Test
    //        var calculatedParams = await _context.TestParameters
    //            .Where(x => x.IsCalculated && x.IsActive)
    //            .ToListAsync();

    //        foreach (var param in calculatedParams)
    //        {
    //            var formula = param.Formula;

    //            // 4️⃣ Evaluate Formula
    //            var calculatedValue = EvaluateFormula(formula, valueMap);

    //            // 5️⃣ Save Calculated Value
    //            var calcResult = new SampleTestResultValue
    //            {
    //                ResultHeaderID = resultHeaderId,
    //                TestParameterID = param.ID,
    //                Value = calculatedValue,
    //                IsPass = true // will be updated by spec check next
    //            };

    //            _context.SampleTestResultValues.Add(calcResult);

    //            // Update dictionary for dependent formulas
    //            valueMap[param.ID] = calculatedValue;
    //        }

    //        await _context.SaveChangesAsync();

    //        // 6️⃣ Apply Min / Max Specification
    //        await ApplySpecificationValidationAsync(resultHeaderId);
    //    }

    }
}
