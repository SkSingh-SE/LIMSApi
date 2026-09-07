using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LIMSApi.Services
{
    public class SpecificationRequirementService : ISpecificationRequirementService
    {
        private readonly LIMSContext _context;
        private readonly FormulaEvaluator _formulaEvaluator;
        private readonly ILogger<SpecificationRequirementService> _logger;

        public SpecificationRequirementService(
            LIMSContext context,
            FormulaEvaluator formulaEvaluator,
            ILogger<SpecificationRequirementService> logger)
        {
            _context = context;
            _formulaEvaluator = formulaEvaluator;
            _logger = logger;
        }

        public async Task<SpecificationRequirementContextDto> GetContextAsync(long specId, long versionId, long gradeId, string companyCode)
        {
            var spec = await _context.SpecificationHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == specId);

            if (spec == null)
            {
                throw new KeyNotFoundException($"Specification with ID {specId} not found.");
            }

            var version = await _context.SpecificationVersions
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.ID == versionId && v.SpecificationHeaderID == specId);

            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {versionId} not found under Specification {specId}.");
            }

            var grade = await _context.SpecificationGrades
                .Include(g => g.MetalClassification)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.ID == gradeId && g.SpecificationHeaderID == specId);

            if (grade == null)
            {
                throw new KeyNotFoundException($"Applicability / Grade with ID {gradeId} not found under Specification {specId}.");
            }

            int count = await _context.SpecificationLines
                .CountAsync(sl => sl.SpecificationVersionID == versionId && sl.SpecificationGradeID == gradeId);

            return new SpecificationRequirementContextDto
            {
                SpecificationHeaderID = spec.ID,
                SpecificationCode = spec.Code ?? string.Empty,
                SpecificationName = spec.AliasName,
                StandardReference = spec.StandardReference,

                SpecificationVersionID = version.ID,
                VersionNumber = version.Version,
                Year = version.Year,
                VersionStatus = version.Status,
                IsDefaultVersion = version.IsDefault,
                EffectiveDate = version.EffectiveDate,

                SpecificationGradeID = grade.ID,
                GradeName = grade.Grade,
                MetalClassificationName = grade.MetalClassification?.Name,

                TotalRequirementsCount = count
            };
        }

        public async Task<List<SpecificationRequirementItemDto>> GetRequirementsAsync(long versionId, long gradeId, string companyCode)
        {
            var version = await _context.SpecificationVersions
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.ID == versionId);

            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {versionId} not found.");
            }

            bool isLocked = version.Status != VersionStatus.Draft;

            var lines = await _context.SpecificationLines
                .Include(sl => sl.Parameter)
                    .ThenInclude(p => p!.ParameterUnit)
                .Include(sl => sl.ParameterUnit)
                .Include(sl => sl.ProductSizeMaster)
                .Include(sl => sl.HeatTreatment)
                .Include(sl => sl.SpecimenOrientation)
                .Include(sl => sl.LaboratoryTest)
                .Include(sl => sl.Conditions)
                    .ThenInclude(c => c.ConditionMaster)
                        .ThenInclude(cm => cm.ParameterUnit)
                .Include(sl => sl.TestMethodMappings)
                    .ThenInclude(tm => tm.LaboratoryTest)
                .Include(sl => sl.TestMethodMappings)
                    .ThenInclude(tm => tm.TestMethodSpecification)
                .AsNoTracking()
                .Where(sl => sl.SpecificationVersionID == versionId && sl.SpecificationGradeID == gradeId)
                .OrderBy(sl => sl.ID)
                .ToListAsync();

            var result = new List<SpecificationRequirementItemDto>();

            foreach (var line in lines)
            {
                var param = line.Parameter;
                string paramName = param?.Name ?? "Unknown Parameter";
                string? unitName = line.ParameterUnit?.Name ?? param?.ParameterUnit?.Name;
                string? unitSymbol = line.ParameterUnit?.Symbol ?? param?.ParameterUnit?.Symbol;

                // Format limits
                string formattedLimits = FormatLimits(line);

                // Format conditions summary
                string conditionsSummary = FormatConditionsSummary(line.Conditions.ToList(), line.ProductSizeMaster?.DisplayName, line.HeatTreatment?.Name);

                // Format test methods summary
                string testMethodsSummary = FormatTestMethodsSummary(line.TestMethodMappings.ToList(), line.LaboratoryTest?.Name);

                result.Add(new SpecificationRequirementItemDto
                {
                    ID = line.ID,
                    SpecificationVersionID = line.SpecificationVersionID,
                    SpecificationGradeID = line.SpecificationGradeID ?? 0,

                    // Section 1: Definition
                    ParameterID = line.ParameterID ?? 0,
                    ParameterName = paramName,
                    ParameterCode = param?.Code,
                    ParameterSymbol = param?.Symbol,
                    ParameterUnitID = line.ParameterUnitID ?? param?.ParameterUnitID,
                    ParameterUnitName = unitName,
                    ParameterUnitSymbol = unitSymbol,
                    InputType = line.InputType ?? param?.InputType ?? string.Empty,
                    LegacyType = line.Type ?? "universal",

                    // Section 2: Limits & Tolerances
                    MinValue = line.MinValue,
                    MaxValue = line.MaxValue,
                    LowerLimitValue = line.LowerLimitValue,
                    UpperLimitValue = line.UpperLimitValue,
                    LowerLimitDecimalValue = line.LowerLimitDecimalValue,
                    UpperLimitDecimalValue = line.UpperLimitDecimalValue,
                    MinTolerance = line.MinTolerance,
                    MaxTolerance = line.MaxTolerance,
                    FormattedLimits = formattedLimits,

                    // Section 3: Equations
                    Equation = line.Equation,
                    MinEquation = line.MinEquation,
                    MaxEquation = line.MaxEquation,
                    ParameterFormula = param?.Formula,
                    ParameterFormulaDisplay = param?.FormulaDisplay,
                    ParameterIsCalculated = param?.IsCalculated ?? false,

                    // Section 4: Applicability & Conditions
                    ProductSizeMasterID = line.ProductSizeMasterID,
                    ProductSizeName = line.ProductSizeMaster?.DisplayName,
                    HeatTreatmentID = line.HeatTreatmentID,
                    HeatTreatmentName = line.HeatTreatment?.Name,
                    SpecimenOrientationID = line.SpecimenOrientationID,
                    SpecimenOrientationName = line.SpecimenOrientation?.Name,
                    DimensionalFactorID = line.DimensionalFactorID,
                    ProductConditionID1 = line.ProductConditionID1,
                    ProductConditionID2 = line.ProductConditionID2,
                    Conditions = line.Conditions.Select(c => new RequirementDimensionConditionDto
                    {
                        ID = c.ID,
                        ConditionMasterID = c.ConditionMasterID,
                        ConditionCode = c.ConditionMaster?.Code,
                        ConditionName = c.ConditionMaster?.Name,
                        Unit = c.ConditionMaster?.ParameterUnit?.Symbol ?? c.ConditionMaster?.ParameterUnit?.Name,
                        ValueType = c.ConditionMaster?.ValueType,
                        Operator = c.Operator,
                        Value1 = c.Value1,
                        Value2 = c.Value2
                    }).ToList(),
                    ConditionsSummary = conditionsSummary,

                    // Section 5: Test Method Applicability
                    LaboratoryTestID = line.LaboratoryTestID,
                    LaboratoryTestName = line.LaboratoryTest?.Name,
                    TestMethods = line.TestMethodMappings.Select(tm => new RequirementTestMethodMappingDto
                    {
                        ID = tm.ID,
                        LaboratoryTestID = tm.LaboratoryTestID,
                        LaboratoryTestName = tm.LaboratoryTest?.Name,
                        TestMethodSpecificationID = tm.TestMethodSpecificationID,
                        TestMethodSpecificationName = tm.TestMethodSpecification?.TestMethodStandard,
                        NumberOfTestSpecimen = tm.NumberOfTestSpecimen,
                        DisplayOrder = tm.DisplayOrder
                    }).OrderBy(tm => tm.DisplayOrder ?? 99).ToList(),
                    TestMethodsSummary = testMethodsSummary,

                    // Section 6: Instructions & Reporting
                    TestCondition = line.TestCondition,
                    TestNote = line.TestNote,

                    // Section 7: Status & Lock
                    IsLocked = isLocked
                });
            }

            return result;
        }

        public async Task<SpecificationRequirementItemDto?> GetRequirementByIdAsync(long id, string companyCode)
        {
            var line = await _context.SpecificationLines
                .Include(sl => sl.SpecificationVersion)
                .Include(sl => sl.Parameter)
                    .ThenInclude(p => p!.ParameterUnit)
                .Include(sl => sl.ParameterUnit)
                .Include(sl => sl.ProductSizeMaster)
                .Include(sl => sl.HeatTreatment)
                .Include(sl => sl.SpecimenOrientation)
                .Include(sl => sl.LaboratoryTest)
                .Include(sl => sl.Conditions)
                    .ThenInclude(c => c.ConditionMaster)
                        .ThenInclude(cm => cm.ParameterUnit)
                .Include(sl => sl.TestMethodMappings)
                    .ThenInclude(tm => tm.LaboratoryTest)
                .Include(sl => sl.TestMethodMappings)
                    .ThenInclude(tm => tm.TestMethodSpecification)
                .AsNoTracking()
                .FirstOrDefaultAsync(sl => sl.ID == id);

            if (line == null)
            {
                return null;
            }

            bool isLocked = line.SpecificationVersion != null && line.SpecificationVersion.Status != VersionStatus.Draft;
            var param = line.Parameter;

            return new SpecificationRequirementItemDto
            {
                ID = line.ID,
                SpecificationVersionID = line.SpecificationVersionID,
                SpecificationGradeID = line.SpecificationGradeID ?? 0,
                ParameterID = line.ParameterID ?? 0,
                ParameterName = param?.Name ?? "Unknown Parameter",
                ParameterCode = param?.Code,
                ParameterSymbol = param?.Symbol,
                ParameterUnitID = line.ParameterUnitID ?? param?.ParameterUnitID,
                ParameterUnitName = line.ParameterUnit?.Name ?? param?.ParameterUnit?.Name,
                ParameterUnitSymbol = line.ParameterUnit?.Symbol ?? param?.ParameterUnit?.Symbol,
                InputType = line.InputType ?? param?.InputType ?? string.Empty,
                LegacyType = line.Type ?? "universal",

                MinValue = line.MinValue,
                MaxValue = line.MaxValue,
                LowerLimitValue = line.LowerLimitValue,
                UpperLimitValue = line.UpperLimitValue,
                LowerLimitDecimalValue = line.LowerLimitDecimalValue,
                UpperLimitDecimalValue = line.UpperLimitDecimalValue,
                MinTolerance = line.MinTolerance,
                MaxTolerance = line.MaxTolerance,
                FormattedLimits = FormatLimits(line),

                // Section 3: Equations
                Equation = line.Equation,
                MinEquation = line.MinEquation,
                MaxEquation = line.MaxEquation,
                ParameterFormula = param?.Formula,
                ParameterFormulaDisplay = param?.FormulaDisplay,
                ParameterIsCalculated = param?.IsCalculated ?? false,

                ProductSizeMasterID = line.ProductSizeMasterID,
                ProductSizeName = line.ProductSizeMaster?.DisplayName,
                HeatTreatmentID = line.HeatTreatmentID,
                HeatTreatmentName = line.HeatTreatment?.Name,
                SpecimenOrientationID = line.SpecimenOrientationID,
                SpecimenOrientationName = line.SpecimenOrientation?.Name,
                DimensionalFactorID = line.DimensionalFactorID,
                ProductConditionID1 = line.ProductConditionID1,
                ProductConditionID2 = line.ProductConditionID2,
                Conditions = line.Conditions.Select(c => new RequirementDimensionConditionDto
                {
                    ID = c.ID,
                    ConditionMasterID = c.ConditionMasterID,
                    ConditionCode = c.ConditionMaster?.Code,
                    ConditionName = c.ConditionMaster?.Name,
                    Unit = c.ConditionMaster?.ParameterUnit?.Symbol ?? c.ConditionMaster?.ParameterUnit?.Name,
                    ValueType = c.ConditionMaster?.ValueType,
                    Operator = c.Operator,
                    Value1 = c.Value1,
                    Value2 = c.Value2
                }).ToList(),
                ConditionsSummary = FormatConditionsSummary(line.Conditions.ToList(), line.ProductSizeMaster?.DisplayName, line.HeatTreatment?.Name),

                LaboratoryTestID = line.LaboratoryTestID,
                LaboratoryTestName = line.LaboratoryTest?.Name,
                TestMethods = line.TestMethodMappings.Select(tm => new RequirementTestMethodMappingDto
                {
                    ID = tm.ID,
                    LaboratoryTestID = tm.LaboratoryTestID,
                    LaboratoryTestName = tm.LaboratoryTest?.Name,
                    TestMethodSpecificationID = tm.TestMethodSpecificationID,
                    TestMethodSpecificationName = tm.TestMethodSpecification?.TestMethodStandard,
                    NumberOfTestSpecimen = tm.NumberOfTestSpecimen,
                    DisplayOrder = tm.DisplayOrder
                }).OrderBy(tm => tm.DisplayOrder ?? 99).ToList(),
                TestMethodsSummary = FormatTestMethodsSummary(line.TestMethodMappings.ToList(), line.LaboratoryTest?.Name),

                TestCondition = line.TestCondition,
                TestNote = line.TestNote,
                IsLocked = isLocked
            };
        }

        public async Task<long> CreateRequirementAsync(SaveSpecificationRequirementDto dto, string companyCode, long userId)
        {
            var version = await _context.SpecificationVersions
                .FirstOrDefaultAsync(v => v.ID == dto.SpecificationVersionID);

            if (version == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {dto.SpecificationVersionID} not found.");
            }

            if (version.Status != VersionStatus.Draft)
            {
                throw new InvalidOperationException($"Specification Version '{version.Version}' is in '{version.Status}' status and is read-only. Requirements can only be added to Draft versions.");
            }

            var grade = await _context.SpecificationGrades
                .FirstOrDefaultAsync(g => g.ID == dto.SpecificationGradeID && g.SpecificationHeaderID == version.SpecificationHeaderID);

            if (grade == null)
            {
                throw new InvalidOperationException($"Applicability / Grade with ID {dto.SpecificationGradeID} does not belong to Specification Header {version.SpecificationHeaderID}.");
            }

            // 1. Parameter Master is the sole authority for InputType and ParameterUnit
            var param = await _context.ParameterMasters
                .Include(p => p.ParameterUnit)
                .FirstOrDefaultAsync(p => p.ID == dto.ParameterID);

            if (param == null)
            {
                throw new InvalidOperationException($"Parameter with ID {dto.ParameterID} was not found.");
            }

            // Reject if InputType is null, empty, or invalid
            if (string.IsNullOrWhiteSpace(param.InputType))
            {
                throw new InvalidOperationException("Selected parameter does not have a configured Input Type. Please configure the Parameter Master before using this parameter.");
            }

            var validTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Decimal", "Integer", "Boolean", "Dropdown", "MultiSelect", "Text" };
            if (!validTypes.Contains(param.InputType.Trim()))
            {
                throw new InvalidOperationException($"Selected parameter '{param.Name}' has an invalid Input Type '{param.InputType}'. Please configure the Parameter Master before using this parameter.");
            }

            var inheritedInputType = param.InputType.Trim();

            // Unit is strictly inherited from Parameter Master
            // If the selected parameter is expected to have a unit (Decimal or Integer) but ParameterUnitID is missing/invalid:
            if ((inheritedInputType.Equals("Decimal", StringComparison.OrdinalIgnoreCase) || inheritedInputType.Equals("Integer", StringComparison.OrdinalIgnoreCase))
                && (!param.ParameterUnitID.HasValue || param.ParameterUnitID.Value <= 0))
            {
                throw new InvalidOperationException($"Selected parameter '{param.Name}' requires a unit, but does not have a configured Parameter Unit in Parameter Master. Please configure the Parameter Master before using this parameter.");
            }

            var inheritedUnitId = param.ParameterUnitID;

            // Validate equation tokens and syntax
            await ValidateEquationTokensAsync(dto.Equation, "Result Calculation Override", dto.ParameterID);
            await ValidateEquationTokensAsync(dto.MinEquation, "Min Equation", dto.ParameterID);
            await ValidateEquationTokensAsync(dto.MaxEquation, "Max Equation", dto.ParameterID);

            var line = new SpecificationLine
            {
                SpecificationVersionID = dto.SpecificationVersionID,
                SpecificationGradeID = dto.SpecificationGradeID,
                ParameterID = dto.ParameterID,
                ParameterUnitID = inheritedUnitId,
                ParameterUnitEquivalentID = param.ParameterUnitEquivalentID,
                InputType = inheritedInputType,
                TextValue = inheritedInputType.Equals("Text", StringComparison.OrdinalIgnoreCase) ? dto.TextValue : null,
                Type = !string.IsNullOrWhiteSpace(param.ParameterType) ? param.ParameterType.Trim().ToLower() : "universal",

                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
                LowerLimitValue = dto.LowerLimitValue,
                UpperLimitValue = dto.UpperLimitValue,
                LowerLimitDecimalValue = dto.LowerLimitDecimalValue,
                UpperLimitDecimalValue = dto.UpperLimitDecimalValue,
                MinTolerance = dto.MinTolerance,
                MaxTolerance = dto.MaxTolerance,

                Equation = dto.Equation,
                MinEquation = dto.MinEquation,
                MaxEquation = dto.MaxEquation,

                ProductSizeMasterID = dto.ProductSizeMasterID,
                HeatTreatmentID = dto.HeatTreatmentID,
                SpecimenOrientationID = dto.SpecimenOrientationID,
                DimensionalFactorID = dto.DimensionalFactorID,
                ProductConditionID1 = dto.ProductConditionID1,
                ProductConditionID2 = dto.ProductConditionID2,

                LaboratoryTestID = dto.LaboratoryTestID,
                TestCondition = dto.TestCondition,
                TestNote = dto.TestNote
            };

            // Add conditions
            if (dto.Conditions != null && dto.Conditions.Any())
            {
                foreach (var cond in dto.Conditions)
                {
                    line.Conditions.Add(new SpecificationLineCondition
                    {
                        ConditionMasterID = cond.ConditionMasterID,
                        Operator = cond.Operator ?? "=",
                        Value1 = cond.Value1 ?? string.Empty,
                        Value2 = cond.Value2,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        CompanyCode = companyCode
                    });
                }
            }

            // Add test method mappings
            if (dto.TestMethods != null && dto.TestMethods.Any())
            {
                int order = 1;
                foreach (var tm in dto.TestMethods)
                {
                    line.TestMethodMappings.Add(new SpecificationLineTestMethod
                    {
                        LaboratoryTestID = tm.LaboratoryTestID ?? dto.LaboratoryTestID,
                        TestMethodSpecificationID = tm.TestMethodSpecificationID,
                        NumberOfTestSpecimen = tm.NumberOfTestSpecimen,
                        DisplayOrder = tm.DisplayOrder ?? order++
                    });
                }
            }

            _context.SpecificationLines.Add(line);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Requirement line ID {LineId} created for Version {VersionId} and Grade {GradeId}", line.ID, dto.SpecificationVersionID, dto.SpecificationGradeID);
            return line.ID;
        }

        public async Task UpdateRequirementAsync(long id, SaveSpecificationRequirementDto dto, string companyCode, long userId)
        {
            var line = await _context.SpecificationLines
                .Include(sl => sl.SpecificationVersion)
                .Include(sl => sl.Conditions)
                .Include(sl => sl.TestMethodMappings)
                .FirstOrDefaultAsync(sl => sl.ID == id);

            if (line == null)
            {
                throw new KeyNotFoundException($"Requirement line with ID {id} not found.");
            }

            if (line.SpecificationVersion == null || line.SpecificationVersion.Status != VersionStatus.Draft)
            {
                throw new InvalidOperationException($"This requirement line belongs to an Active or Superseded version and is read-only. Modifications are only permitted on Draft versions.");
            }

            // 1. Parameter Master is the sole authority for InputType and ParameterUnit
            var param = await _context.ParameterMasters
                .Include(p => p.ParameterUnit)
                .FirstOrDefaultAsync(p => p.ID == dto.ParameterID);

            if (param == null)
            {
                throw new InvalidOperationException($"Parameter with ID {dto.ParameterID} was not found.");
            }

            if (string.IsNullOrWhiteSpace(param.InputType))
            {
                throw new InvalidOperationException("Selected parameter does not have a configured Input Type. Please configure the Parameter Master before using this parameter.");
            }

            var validTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Decimal", "Integer", "Boolean", "Dropdown", "MultiSelect", "Text" };
            if (!validTypes.Contains(param.InputType.Trim()))
            {
                throw new InvalidOperationException($"Selected parameter '{param.Name}' has an invalid Input Type '{param.InputType}'. Please configure the Parameter Master before using this parameter.");
            }

            var inheritedInputType = param.InputType.Trim();

            if ((inheritedInputType.Equals("Decimal", StringComparison.OrdinalIgnoreCase) || inheritedInputType.Equals("Integer", StringComparison.OrdinalIgnoreCase))
                && (!param.ParameterUnitID.HasValue || param.ParameterUnitID.Value <= 0))
            {
                throw new InvalidOperationException($"Selected parameter '{param.Name}' requires a unit, but does not have a configured Parameter Unit in Parameter Master. Please configure the Parameter Master before using this parameter.");
            }

            // Validate equation tokens and syntax
            await ValidateEquationTokensAsync(dto.Equation, "Result Calculation Override", dto.ParameterID);
            await ValidateEquationTokensAsync(dto.MinEquation, "Min Equation", dto.ParameterID);
            await ValidateEquationTokensAsync(dto.MaxEquation, "Max Equation", dto.ParameterID);

            // Update core fields
            line.ParameterID = dto.ParameterID;
            line.ParameterUnitID = param.ParameterUnitID;
            line.ParameterUnitEquivalentID = param.ParameterUnitEquivalentID;
            line.InputType = inheritedInputType;
            line.TextValue = inheritedInputType.Equals("Text", StringComparison.OrdinalIgnoreCase) ? dto.TextValue : null;
            line.Type = !string.IsNullOrWhiteSpace(param.ParameterType) ? param.ParameterType.Trim().ToLower() : "universal";

            line.MinValue = dto.MinValue;
            line.MaxValue = dto.MaxValue;
            line.LowerLimitValue = dto.LowerLimitValue;
            line.UpperLimitValue = dto.UpperLimitValue;
            line.LowerLimitDecimalValue = dto.LowerLimitDecimalValue;
            line.UpperLimitDecimalValue = dto.UpperLimitDecimalValue;
            line.MinTolerance = dto.MinTolerance;
            line.MaxTolerance = dto.MaxTolerance;

            line.Equation = dto.Equation;
            line.MinEquation = dto.MinEquation;
            line.MaxEquation = dto.MaxEquation;

            line.ProductSizeMasterID = dto.ProductSizeMasterID;
            line.HeatTreatmentID = dto.HeatTreatmentID;
            line.SpecimenOrientationID = dto.SpecimenOrientationID;
            line.DimensionalFactorID = dto.DimensionalFactorID;
            line.ProductConditionID1 = dto.ProductConditionID1;
            line.ProductConditionID2 = dto.ProductConditionID2;

            line.LaboratoryTestID = dto.LaboratoryTestID;
            line.TestCondition = dto.TestCondition;
            line.TestNote = dto.TestNote;

            // Reconcile Conditions
            var incomingCondIds = (dto.Conditions ?? new List<RequirementDimensionConditionDto>())
                .Where(c => c.ID > 0)
                .Select(c => c.ID)
                .ToHashSet();

            var condsToRemove = line.Conditions.Where(c => !incomingCondIds.Contains(c.ID)).ToList();
            foreach (var cond in condsToRemove)
            {
                line.Conditions.Remove(cond);
            }

            if (dto.Conditions != null)
            {
                foreach (var condDto in dto.Conditions)
                {
                    if (condDto.ID > 0)
                    {
                        var existingCond = line.Conditions.FirstOrDefault(c => c.ID == condDto.ID);
                        if (existingCond != null)
                        {
                            existingCond.ConditionMasterID = condDto.ConditionMasterID;
                            existingCond.Operator = condDto.Operator ?? "=";
                            existingCond.Value1 = condDto.Value1 ?? string.Empty;
                            existingCond.Value2 = condDto.Value2;
                            existingCond.ModifiedBy = userId;
                            existingCond.ModifiedOn = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        line.Conditions.Add(new SpecificationLineCondition
                        {
                            ConditionMasterID = condDto.ConditionMasterID,
                            Operator = condDto.Operator ?? "=",
                            Value1 = condDto.Value1 ?? string.Empty,
                            Value2 = condDto.Value2,
                            CreatedBy = userId,
                            CreatedOn = DateTime.UtcNow,
                            CompanyCode = companyCode
                        });
                    }
                }
            }

            // Reconcile Test Method Mappings
            var incomingTmIds = (dto.TestMethods ?? new List<RequirementTestMethodMappingDto>())
                .Where(tm => tm.ID > 0)
                .Select(tm => tm.ID)
                .ToHashSet();

            var tmsToRemove = line.TestMethodMappings.Where(tm => !incomingTmIds.Contains(tm.ID)).ToList();
            foreach (var tm in tmsToRemove)
            {
                line.TestMethodMappings.Remove(tm);
            }

            if (dto.TestMethods != null)
            {
                int order = 1;
                foreach (var tmDto in dto.TestMethods)
                {
                    if (tmDto.ID > 0)
                    {
                        var existingTm = line.TestMethodMappings.FirstOrDefault(tm => tm.ID == tmDto.ID);
                        if (existingTm != null)
                        {
                            existingTm.LaboratoryTestID = tmDto.LaboratoryTestID ?? dto.LaboratoryTestID;
                            existingTm.TestMethodSpecificationID = tmDto.TestMethodSpecificationID;
                            existingTm.NumberOfTestSpecimen = tmDto.NumberOfTestSpecimen;
                            existingTm.DisplayOrder = tmDto.DisplayOrder ?? order++;
                        }
                    }
                    else
                    {
                        line.TestMethodMappings.Add(new SpecificationLineTestMethod
                        {
                            LaboratoryTestID = tmDto.LaboratoryTestID ?? dto.LaboratoryTestID,
                            TestMethodSpecificationID = tmDto.TestMethodSpecificationID,
                            NumberOfTestSpecimen = tmDto.NumberOfTestSpecimen,
                            DisplayOrder = tmDto.DisplayOrder ?? order++
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Requirement line ID {LineId} updated successfully", id);
        }

        public async Task DeleteRequirementAsync(long id, string companyCode)
        {
            var line = await _context.SpecificationLines
                .Include(sl => sl.SpecificationVersion)
                .Include(sl => sl.Conditions)
                .Include(sl => sl.TestMethodMappings)
                .FirstOrDefaultAsync(sl => sl.ID == id);

            if (line == null)
            {
                throw new KeyNotFoundException($"Requirement line with ID {id} not found.");
            }

            if (line.SpecificationVersion == null || line.SpecificationVersion.Status != VersionStatus.Draft)
            {
                throw new InvalidOperationException($"Cannot delete requirement line ID {id} because its specification version is in '{line.SpecificationVersion?.Status}' status. Requirements in Active or Superseded versions are immutable.");
            }

            // Dependency check: Is this line referenced in test execution results?
            bool isReferencedInChemical = await _context.ChemicalTestElements.AnyAsync(e => e.SpecificationLineID == id);
            if (isReferencedInChemical)
            {
                throw new InvalidOperationException($"Cannot delete requirement line ID {id} because it is referenced in test execution results (ChemicalTestElements).");
            }

            _context.SpecificationLineConditions.RemoveRange(line.Conditions);
            _context.SpecificationLineTestMethods.RemoveRange(line.TestMethodMappings);
            _context.SpecificationLines.Remove(line);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Requirement line ID {LineId} deleted successfully", id);
        }

        public async Task<int> CopyVersionRequirementsAsync(CopyVersionRequirementsDto dto, string companyCode, long userId)
        {
            var sourceVersion = await _context.SpecificationVersions
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.ID == dto.SourceVersionID);

            if (sourceVersion == null)
            {
                throw new KeyNotFoundException($"Source Specification Version with ID {dto.SourceVersionID} not found.");
            }

            var targetVersion = await _context.SpecificationVersions
                .FirstOrDefaultAsync(v => v.ID == dto.TargetVersionID);

            if (targetVersion == null)
            {
                throw new KeyNotFoundException($"Target Specification Version with ID {dto.TargetVersionID} not found.");
            }

            if (targetVersion.Status != VersionStatus.Draft)
            {
                throw new InvalidOperationException($"Target Specification Version '{targetVersion.Version}' is in '{targetVersion.Status}' status and is read-only. Cloning requirements is only allowed into Draft versions.");
            }

            var query = _context.SpecificationLines
                .Include(sl => sl.Conditions)
                .Include(sl => sl.TestMethodMappings)
                .AsNoTracking()
                .Where(sl => sl.SpecificationVersionID == dto.SourceVersionID);

            if (dto.SpecificationGradeID.HasValue && dto.SpecificationGradeID.Value > 0)
            {
                query = query.Where(sl => sl.SpecificationGradeID == dto.SpecificationGradeID.Value);
            }

            var sourceLines = await query.ToListAsync();

            if (!sourceLines.Any())
            {
                _logger.LogWarning("No requirement lines found in Source Version {SourceVersionID} to clone.", dto.SourceVersionID);
                return 0;
            }

            int clonedCount = 0;
            foreach (var src in sourceLines)
            {
                var clonedLine = new SpecificationLine
                {
                    SpecificationVersionID = dto.TargetVersionID,
                    SpecificationGradeID = src.SpecificationGradeID,
                    ManualSelection = src.ManualSelection,
                    ParameterID = src.ParameterID,
                    MinValue = src.MinValue,
                    MaxValue = src.MaxValue,
                    TextValue = src.TextValue,
                    InputType = src.InputType,
                    Notes = src.Notes,
                    Equation = src.Equation,
                    ParameterUnitID = src.ParameterUnitID,
                    ParameterUnitEquivalentID = src.ParameterUnitEquivalentID,
                    LaboratoryTestID = src.LaboratoryTestID,
                    MinEquation = src.MinEquation,
                    MaxEquation = src.MaxEquation,
                    MinTolerance = src.MinTolerance,
                    MaxTolerance = src.MaxTolerance,
                    SpecimenOrientationID = src.SpecimenOrientationID,
                    DimensionalFactorID = src.DimensionalFactorID,
                    LowerLimitValue = src.LowerLimitValue,
                    UpperLimitValue = src.UpperLimitValue,
                    LowerLimitDecimalValue = src.LowerLimitDecimalValue,
                    UpperLimitDecimalValue = src.UpperLimitDecimalValue,
                    HeatTreatmentID = src.HeatTreatmentID,
                    ProductConditionID1 = src.ProductConditionID1,
                    ProductConditionID2 = src.ProductConditionID2,
                    ProductSizeMasterID = src.ProductSizeMasterID,
                    TestCondition = src.TestCondition,
                    TestNote = src.TestNote,
                    Type = src.Type
                };

                // Deep copy conditions
                foreach (var cond in src.Conditions)
                {
                    clonedLine.Conditions.Add(new SpecificationLineCondition
                    {
                        ConditionMasterID = cond.ConditionMasterID,
                        Operator = cond.Operator,
                        Value1 = cond.Value1,
                        Value2 = cond.Value2,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        CompanyCode = companyCode
                    });
                }

                // Deep copy test methods
                foreach (var tm in src.TestMethodMappings)
                {
                    clonedLine.TestMethodMappings.Add(new SpecificationLineTestMethod
                    {
                        LaboratoryTestID = tm.LaboratoryTestID,
                        TestMethodSpecificationID = tm.TestMethodSpecificationID,
                        NumberOfTestSpecimen = tm.NumberOfTestSpecimen,
                        DisplayOrder = tm.DisplayOrder
                    });
                }

                _context.SpecificationLines.Add(clonedLine);
                clonedCount++;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Cloned {ClonedCount} requirements from Version {SourceVersionID} to Version {TargetVersionID}", clonedCount, dto.SourceVersionID, dto.TargetVersionID);
            return clonedCount;
        }

        public async Task<bool> ActivateSpecificationVersionAsync(long versionId, string companyCode, long userId)
        {
            var targetVersion = await _context.SpecificationVersions
                .Include(v => v.SpecificationHeader)
                .FirstOrDefaultAsync(v => v.ID == versionId);

            if (targetVersion == null)
            {
                throw new KeyNotFoundException($"Specification Version with ID {versionId} not found.");
            }

            if (targetVersion.SpecificationHeader == null || !targetVersion.SpecificationHeader.IsActive)
            {
                throw new InvalidOperationException("Cannot activate version because the parent Specification Master is inactive.");
            }

            if (targetVersion.Status == VersionStatus.Active)
            {
                return true; // Already active
            }

            // Validate all requirement equations before activation
            var versionLines = await _context.SpecificationLines
                .Include(sl => sl.Parameter)
                .Where(sl => sl.SpecificationVersionID == versionId)
                .ToListAsync();

            foreach (var l in versionLines)
            {
                if (!string.IsNullOrWhiteSpace(l.Equation))
                {
                    await ValidateEquationTokensAsync(l.Equation, $"Result Calculation Override for '{l.Parameter?.Name}'", l.ParameterID);
                }
                if (!string.IsNullOrWhiteSpace(l.MinEquation))
                {
                    await ValidateEquationTokensAsync(l.MinEquation, $"Min Equation for '{l.Parameter?.Name}'", l.ParameterID);
                }
                if (!string.IsNullOrWhiteSpace(l.MaxEquation))
                {
                    await ValidateEquationTokensAsync(l.MaxEquation, $"Max Equation for '{l.Parameter?.Name}'", l.ParameterID);
                }
            }

            // Transactional activation and superseding of existing active versions
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Supersede any currently active versions and unmark default
                var otherVersions = await _context.SpecificationVersions
                    .Where(v => v.SpecificationHeaderID == targetVersion.SpecificationHeaderID && v.ID != versionId && (v.Status == VersionStatus.Active || v.IsDefault))
                    .ToListAsync();

                foreach (var ver in otherVersions)
                {
                    if (ver.Status == VersionStatus.Active)
                    {
                        ver.Status = VersionStatus.Superseded;
                        ver.SupersededDate = DateTime.UtcNow;
                    }
                    ver.IsDefault = false;
                    ver.ModifiedBy = userId;
                    ver.ModifiedOn = DateTime.UtcNow;
                }

                // Step 1 Save: Clear existing defaults so unique index IX_SpecificationVersions_Header_Default is satisfied
                await _context.SaveChangesAsync();

                // 2. Activate target version and set as Default
                targetVersion.Status = VersionStatus.Active;
                targetVersion.IsDefault = true;
                targetVersion.EffectiveDate = targetVersion.EffectiveDate ?? DateTime.UtcNow.Date;
                targetVersion.ModifiedBy = userId;
                targetVersion.ModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Specification Version {VersionId} activated successfully. Superseded {Count} previous active versions.", versionId, otherVersions.Count);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to activate Specification Version {VersionId}", versionId);
                throw;
            }
        }

        private static string FormatLimits(SpecificationLine line)
        {
            if (!string.IsNullOrWhiteSpace(line.InputType) && line.InputType.Equals("Text", StringComparison.OrdinalIgnoreCase))
            {
                return !string.IsNullOrWhiteSpace(line.TextValue) ? line.TextValue : "-";
            }

            if (line.MinValue.HasValue && line.MaxValue.HasValue)
            {
                return $"{line.MinValue.Value:G29} - {line.MaxValue.Value:G29}";
            }

            if (line.MinValue.HasValue)
            {
                return $">= {line.MinValue.Value:G29}";
            }

            if (line.MaxValue.HasValue)
            {
                return $"<= {line.MaxValue.Value:G29}";
            }

            if (!string.IsNullOrWhiteSpace(line.LowerLimitValue) && line.LowerLimitDecimalValue.HasValue)
            {
                return $"{line.LowerLimitValue} {line.LowerLimitDecimalValue.Value:G29}";
            }

            if (!string.IsNullOrWhiteSpace(line.UpperLimitValue) && line.UpperLimitDecimalValue.HasValue)
            {
                return $"{line.UpperLimitValue} {line.UpperLimitDecimalValue.Value:G29}";
            }

            if (!string.IsNullOrWhiteSpace(line.MinEquation) || !string.IsNullOrWhiteSpace(line.MaxEquation))
            {
                return "Formula Driven";
            }

            if (!string.IsNullOrWhiteSpace(line.TextValue))
            {
                return line.TextValue;
            }

            return "-";
        }

        private static string FormatConditionsSummary(List<SpecificationLineCondition> conditions, string? productSize, string? heatTreatment)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(productSize))
            {
                parts.Add($"Size: {productSize}");
            }

            if (!string.IsNullOrWhiteSpace(heatTreatment))
            {
                parts.Add($"HT: {heatTreatment}");
            }

            foreach (var c in conditions)
            {
                string dimName = c.ConditionMaster?.Name ?? "Condition";
                string unit = !string.IsNullOrWhiteSpace(c.ConditionMaster?.ParameterUnit?.Symbol) ? $" {c.ConditionMaster.ParameterUnit.Symbol}" : "";
                if (!string.IsNullOrWhiteSpace(c.Value2))
                {
                    parts.Add($"{dimName} {c.Operator} {c.Value1} to {c.Value2}{unit}");
                }
                else
                {
                    parts.Add($"{dimName} {c.Operator} {c.Value1}{unit}");
                }
            }

            return parts.Any() ? string.Join(", ", parts) : "Standard (All Conditions)";
        }

        private static string FormatTestMethodsSummary(List<SpecificationLineTestMethod> testMethods, string? labTest)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(labTest))
            {
                parts.Add(labTest);
            }

            foreach (var tm in testMethods)
            {
                string name = tm.TestMethodSpecification?.TestMethodStandard ?? tm.LaboratoryTest?.Name ?? "";
                if (!string.IsNullOrWhiteSpace(name))
                {
                    parts.Add(name);
                }
            }

            return parts.Any() ? string.Join(", ", parts.Distinct()) : "-";
        }

        private async Task ValidateEquationTokensAsync(string? equation, string equationType, long? currentParamId)
        {
            if (string.IsNullOrWhiteSpace(equation)) return;

            var tokens = _formulaEvaluator.ExtractTokens(equation).ToList();
            if (!tokens.Any())
            {
                var legacyParamIds = _formulaEvaluator.ExtractParamIds(equation).ToList();
                if (!legacyParamIds.Any())
                {
                    return;
                }
                tokens = legacyParamIds.Select(id => $"P{id}").ToList();
            }

            var dummyVariables = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var token in tokens)
            {
                ParameterMaster? matchedParam = null;

                // 1. Try by Code
                matchedParam = await _context.ParameterMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Code == token);

                // 2. Try by Symbol
                if (matchedParam == null)
                {
                    matchedParam = await _context.ParameterMasters
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Symbol == token);
                }

                // 3. Try by legacy P{id} or PARAM_{id}
                if (matchedParam == null)
                {
                    long parsedId = 0;
                    if (token.StartsWith("P", StringComparison.OrdinalIgnoreCase) && long.TryParse(token.Substring(1), out parsedId))
                    {
                        matchedParam = await _context.ParameterMasters
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.ID == parsedId);
                    }
                    else if (token.StartsWith("PARAM_", StringComparison.OrdinalIgnoreCase) && long.TryParse(token.Substring(6), out parsedId))
                    {
                        matchedParam = await _context.ParameterMasters
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.ID == parsedId);
                    }
                }

                if (matchedParam == null)
                {
                    throw new InvalidOperationException($"Equation in {equationType} references unknown parameter token '{{{token}}}'. The referenced parameter does not exist.");
                }

                if (!matchedParam.IsActive)
                {
                    throw new InvalidOperationException($"Equation in {equationType} references parameter '{matchedParam.Name}' (token '{{{token}}}'), which is inactive. Inactive parameters cannot be referenced in requirement formulas.");
                }

                if (currentParamId.HasValue && matchedParam.ID == currentParamId.Value)
                {
                    throw new InvalidOperationException($"Equation in {equationType} cannot reference itself (token '{{{token}}}').");
                }

                dummyVariables[token] = 1.0;
                dummyVariables[$"P{matchedParam.ID}"] = 1.0;
            }

            try
            {
                var evalResult = _formulaEvaluator.Evaluate(equation, dummyVariables);
                if (!evalResult.HasValue)
                {
                    throw new InvalidOperationException($"Equation in {equationType} could not be evaluated. Please check formula syntax.");
                }
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Formula syntax error in {equationType}: {ex.Message}");
            }
        }
    }
}
