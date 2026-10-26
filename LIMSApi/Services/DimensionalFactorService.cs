using System.Text.RegularExpressions;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using NCalc;

namespace LIMSApi.Services
{
    public class DimensionalFactorService : IDimensionalFactorService
    {
        private readonly IDimensionalFactorRepository _dimensionalRepository;
        private readonly ILogger<DimensionalFactorService> _logger;
        private readonly LIMSContext _context;

        public DimensionalFactorService(IDimensionalFactorRepository dimensionalRepo, ILogger<DimensionalFactorService> logger, LIMSContext context)
        {
            _dimensionalRepository = dimensionalRepo;
            _logger = logger;
            _context = context;
        }

        public async Task CreateDimensionalFactor(DimensionalFactorMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("DimensionalFactor name should not be empty!");

            bool exists = await _dimensionalRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Dimensional Factor Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _dimensionalRepository.ExistsByCode(model.Code);
                if (codeExists)
                    throw new InvalidOperationException("Dimensional Factor Code already exists!");
            }

            ValidateShapeFormulas(model);

            await _dimensionalRepository.AddDimensionalFactor(model);
            _logger.LogInformation("DimensionalFactor '{DimensionalFactorName}' created successfully.", model.Name);
        }

        public async Task ModifyDimensionalFactor(DimensionalFactorMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("DimensionalFactor ID should not be empty!");

            bool exists = await _dimensionalRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Dimensional Factor Name already exists!");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                bool codeExists = await _dimensionalRepository.ExistsByCodeAndNotId(model.Code, model.ID);
                if (codeExists)
                    throw new InvalidOperationException("Dimensional Factor Code already exists!");
            }

            ValidateShapeFormulas(model);

            var existingDimensionalFactor = await _dimensionalRepository.GetDimensionalFactorById(model.ID);
            if (existingDimensionalFactor == null)
                throw new InvalidOperationException("DimensionalFactor not found!");

            existingDimensionalFactor.Name = model.Name;
            existingDimensionalFactor.Code = model.Code;
            existingDimensionalFactor.ParameterUnitID = (model.ParameterUnitID == null || model.ParameterUnitID == 0) ? null : model.ParameterUnitID;
            existingDimensionalFactor.ParameterUnitEquivalentID = (model.ParameterUnitEquivalentID == null || model.ParameterUnitEquivalentID == 0) ? null : model.ParameterUnitEquivalentID;
            existingDimensionalFactor.Instrument = model.Instrument;
            existingDimensionalFactor.ToleranceType = model.ToleranceType;
            existingDimensionalFactor.DefaultTestMethodID = (model.DefaultTestMethodID == null || model.DefaultTestMethodID == 0) ? null : model.DefaultTestMethodID;

            existingDimensionalFactor.FactorType = string.IsNullOrWhiteSpace(model.FactorType) ? "Measured" : model.FactorType;
            existingDimensionalFactor.DefaultEquipmentTypeID = (model.DefaultEquipmentTypeID == null || model.DefaultEquipmentTypeID == 0) ? null : model.DefaultEquipmentTypeID;

            // Update ApplicableForms junction
            existingDimensionalFactor.ApplicableForms?.Clear();
            if (model.ApplicableForms != null)
                foreach (var form in model.ApplicableForms)
                    existingDimensionalFactor.ApplicableForms.Add(form);

            // Sync ShapeFormulas
            if (model.ShapeFormulas != null)
            {
                var incomingIds = model.ShapeFormulas.Where(sf => sf.ID > 0).Select(sf => sf.ID).ToHashSet();
                foreach (var existingFormula in existingDimensionalFactor.ShapeFormulas.Where(sf => sf.IsActive))
                {
                    if (!incomingIds.Contains(existingFormula.ID))
                    {
                        existingFormula.IsActive = false;
                        existingFormula.ModifiedOn = DateTime.UtcNow;
                    }
                }

                foreach (var incomingFormula in model.ShapeFormulas)
                {
                    if (incomingFormula.ID > 0)
                    {
                        var target = existingDimensionalFactor.ShapeFormulas.FirstOrDefault(sf => sf.ID == incomingFormula.ID);
                        if (target != null)
                        {
                            target.SpecimenTypeID = (incomingFormula.SpecimenTypeID == 0) ? null : incomingFormula.SpecimenTypeID;
                            target.SpecimenShape = incomingFormula.SpecimenShape;
                            target.FormulaExpression = incomingFormula.FormulaExpression;
                            target.Variables = incomingFormula.Variables;
                            target.IsActive = true;
                            target.ModifiedOn = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        incomingFormula.DimensionalFactorID = model.ID;
                        incomingFormula.IsActive = true;
                        incomingFormula.CreatedOn = DateTime.UtcNow;
                        existingDimensionalFactor.ShapeFormulas.Add(incomingFormula);
                    }
                }
            }

            existingDimensionalFactor.ModifiedOn = DateTime.UtcNow;

            await _dimensionalRepository.UpdateDimensionalFactor(existingDimensionalFactor);
            _logger.LogInformation("DimensionalFactor '{DimensionalFactorName}' updated successfully.", model.Name);
        }

        public async Task RemoveDimensionalFactor(long id)
        {
            var existingDimensionalFactor = await _dimensionalRepository.GetDimensionalFactorById(id);
            if (existingDimensionalFactor == null)
                throw new InvalidOperationException("DimensionalFactor not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<DimensionalFactorMaster>(_context, id, "Dimensional Factor", existingDimensionalFactor.Name);

            existingDimensionalFactor.IsActive = false;
            existingDimensionalFactor.ModifiedOn = DateTime.UtcNow;

            await _dimensionalRepository.UpdateDimensionalFactor(existingDimensionalFactor);
            _logger.LogInformation("DimensionalFactor with ID '{DimensionalFactorId}' deleted successfully.", id);
        }

        public async Task<DimensionalFactorMaster> GetDimensionalFactorDetails(long id)
        {
            var classification = await _dimensionalRepository.GetDimensionalFactorById(id);
            if (classification == null)
                throw new InvalidOperationException("DimensionalFactor not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchDimensionalFactorList(PageFilter filter)
        {
            return await _dimensionalRepository.GetAllDimensionalFactors(filter);
        }

        public async Task<List<DropdwonSelector>> GetDimensionalFactorDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _dimensionalRepository.GetDimensionalFactorDropdown(searchTerm, pageNo, pageSize);
        }

        private static void ValidateShapeFormulas(DimensionalFactorMaster model)
        {
            if (string.Equals(model.FactorType, "Calculated", StringComparison.OrdinalIgnoreCase) && model.ShapeFormulas != null)
            {
                foreach (var sf in model.ShapeFormulas.Where(f => f.IsActive))
                {
                    if (string.IsNullOrWhiteSpace(sf.SpecimenShape))
                        throw new ArgumentException("Specimen Shape name cannot be empty in shape formula expression.");

                    if (string.IsNullOrWhiteSpace(sf.FormulaExpression))
                        throw new ArgumentException($"Formula expression cannot be empty for shape '{sf.SpecimenShape}'.");

                    // Check balanced parentheses
                    int depth = 0;
                    foreach (char c in sf.FormulaExpression)
                    {
                        if (c == '(') depth++;
                        else if (c == ')')
                        {
                            depth--;
                            if (depth < 0)
                                throw new ArgumentException($"Unmatched closing parenthesis in shape formula for '{sf.SpecimenShape}': {sf.FormulaExpression}");
                        }
                    }
                    if (depth > 0)
                        throw new ArgumentException($"Unclosed opening parenthesis in shape formula for '{sf.SpecimenShape}': {sf.FormulaExpression}");

                    // Dry-run evaluate syntax with NCalc
                    try
                    {
                        string ncalcExpr = sf.FormulaExpression
                            .Replace("PI", "3.141592653589793", StringComparison.OrdinalIgnoreCase)
                            .Replace("SQRT", "Sqrt", StringComparison.OrdinalIgnoreCase)
                            .Replace("POW", "Pow", StringComparison.OrdinalIgnoreCase);

                        ncalcExpr = Regex.Replace(ncalcExpr, @"([a-zA-Z0-9_.]+)\s*\^\s*([0-9.]+)", "Pow($1, $2)");

                        var exp = new Expression(ncalcExpr, EvaluateOptions.IgnoreCase);
                        var matches = Regex.Matches(sf.FormulaExpression, @"[a-zA-Z_][a-zA-Z0-9_]*");
                        foreach (Match m in matches)
                        {
                            string token = m.Value;
                            if (!string.Equals(token, "PI", StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(token, "SQRT", StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(token, "POW", StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(token, "ABS", StringComparison.OrdinalIgnoreCase))
                            {
                                exp.Parameters[token] = 10.0;
                            }
                        }
                        exp.Evaluate();
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"Invalid formula syntax for shape '{sf.SpecimenShape}' ({sf.FormulaExpression}): {ex.Message}");
                    }
                }
            }
        }
    }
}
