using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class SpecificationHeaderService : ISpecificationHeaderService
    {
        private readonly ISpecificationHeaderRepository _specificationRepo;
        private readonly ILogger<SpecificationHeaderService> _logger;
        private readonly LIMSContext _context;

        public SpecificationHeaderService(ISpecificationHeaderRepository uomRepo, ILogger<SpecificationHeaderService> logger, LIMSContext context)
        {
            _specificationRepo = uomRepo;
            _logger = logger;
            _context = context;
        }

        private void ValidateSpecificationLines(SpecificationHeader model)
        {
            foreach (var grade in model.Grades)
            {
                foreach (var line in grade.SpecificationLines)
                {
                    if (line.ParameterID == null || line.ParameterID == 0)
                        throw new ArgumentException($"Parameter is required for all specification lines in grade '{grade.Grade}'.");
                    if (line.MinValue.HasValue && line.MaxValue.HasValue && line.MinValue > line.MaxValue)
                        throw new ArgumentException($"Min Value cannot be greater than Max Value in grade '{grade.Grade}'.");
                }
            }
        }

        /// <summary>
        /// Rounds all numeric fields of every specification line to its linked parameter's DecimalPrecision.
        /// Defensive: prevents frontend tampering from saving more precision than the parameter allows,
        /// and ensures DB values match what the UI showed on save.
        /// </summary>
        private async Task RoundLinesToParameterPrecisionAsync(SpecificationHeader model)
        {
            var paramIds = model.Grades
                .SelectMany(g => g.SpecificationLines)
                .Where(l => l.ParameterID.HasValue && l.ParameterID.Value > 0)
                .Select(l => l.ParameterID!.Value)
                .Distinct()
                .ToList();

            if (paramIds.Count == 0) return;

            var precisionMap = await _context.ParameterMasters
                .Where(p => paramIds.Contains(p.ID))
                .Select(p => new { p.ID, p.DecimalPrecision })
                .ToDictionaryAsync(x => x.ID, x => x.DecimalPrecision);

            foreach (var grade in model.Grades)
            {
                foreach (var line in grade.SpecificationLines)
                {
                    if (!line.ParameterID.HasValue || !precisionMap.TryGetValue(line.ParameterID.Value, out var precision))
                        continue;
                    // Clamp precision to column scale (decimal(18,6))
                    if (precision < 0) precision = 0;
                    if (precision > 6) precision = 6;

                    line.MinValue = Round(line.MinValue, precision);
                    line.MaxValue = Round(line.MaxValue, precision);
                    line.MinValueEquation = Round(line.MinValueEquation, precision);
                    line.MaxValueEquation = Round(line.MaxValueEquation, precision);
                    line.MinTolerance = Round(line.MinTolerance, precision);
                    line.MaxTolerance = Round(line.MaxTolerance, precision);
                }
            }
        }

        private static decimal? Round(decimal? value, int precision)
        {
            if (!value.HasValue) return null;
            return Math.Round(value.Value, precision, MidpointRounding.AwayFromZero);
        }

        public async Task CreateSpecificationHeader(SpecificationHeader model)
        {
            if (string.IsNullOrWhiteSpace(model.AliasName))
                throw new ArgumentException("SpecificationHeader name should not be empty!");

            ValidateSpecificationLines(model);
            await RoundLinesToParameterPrecisionAsync(model);

            bool exists = await _specificationRepo.ExistsByNameAndVersion(model.AliasName, model.Version);
            if (exists)
                throw new InvalidOperationException("A specification with the same name and version already exists!");

            await _specificationRepo.AddSpecificationHeader(model);

            _logger.LogInformation("SpecificationHeader '{SpecificationHeaderName}' created successfully.", model.AliasName);
        }

        public async Task ModifySpecificationHeader(SpecificationHeader model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SpecificationHeader ID should not be empty!");

            ValidateSpecificationLines(model);
            await RoundLinesToParameterPrecisionAsync(model);

            bool exists = await _specificationRepo.ExistsByNameAndVersion(model.AliasName, model.Version, model.ID);
            if (exists)
                throw new InvalidOperationException("A specification with the same name and version already exists!");

            var existingSpecificationHeader = await _specificationRepo.GetSpecificationHeaderById(model.ID);
            if (existingSpecificationHeader == null)
                throw new InvalidOperationException("SpecificationHeader not found!");

            // Update SpecificationHeader fields
            existingSpecificationHeader.AliasName = model.AliasName;
            existingSpecificationHeader.StandardOrganizationID = model.StandardOrganizationID;
            existingSpecificationHeader.Standard = model.Standard;
            existingSpecificationHeader.Part = model.Part;
            existingSpecificationHeader.StandardYear = model.StandardYear;
            existingSpecificationHeader.IsCustom = model.IsCustom;
            existingSpecificationHeader.SpecificationNo = model.SpecificationNo;
            existingSpecificationHeader.Version = model.Version;
            existingSpecificationHeader.DisplayTitle = model.DisplayTitle;
            existingSpecificationHeader.Title = model.Title;
            existingSpecificationHeader.IdentifierConfigJson = model.IdentifierConfigJson;
            existingSpecificationHeader.ModifiedOn = DateTime.UtcNow;

            // Header parameter template: clear + repopulate (copied into grade tabs on Add Grade, client-side)
            existingSpecificationHeader.HeaderParameters.Clear();
            foreach (var hp in model.HeaderParameters)
            {
                existingSpecificationHeader.HeaderParameters.Add(new SpecificationHeaderParameter
                {
                    ParameterID = hp.ParameterID,
                    ParameterUnitID = hp.ParameterUnitID,
                    ParameterUnitEquivalentID = hp.ParameterUnitEquivalentID,
                    Type = hp.Type,
                    DisplayOrder = hp.DisplayOrder
                });
            }

            var incomingGradeIds = model.Grades.Where(y => y.ID > 0).Select(y => y.ID).ToHashSet();
            var toRemoveGrades = existingSpecificationHeader.Grades.Where(x => !incomingGradeIds.Contains(x.ID)).ToList();
            foreach(var  grade in toRemoveGrades)
            {
                existingSpecificationHeader.Grades.Remove(grade);
            }

            foreach (var grade in model.Grades)
            {
                var existingGrade = grade.ID > 0
                    ? existingSpecificationHeader.Grades.FirstOrDefault(x => x.ID == grade.ID)
                    : null;
                if (existingGrade == null)
                {
                    grade.ID = 0; // Ensure EF treats as new insert
                    grade.SpecificationHeaderID = model.ID;
                    existingSpecificationHeader.Grades.Add(grade);
                    continue;
                }

                // Update existing grade
                existingGrade.Grade = grade.Grade;
                existingGrade.IsUNS = grade.IsUNS;
                existingGrade.UNSSteelNumber = grade.UNSSteelNumber;
                existingGrade.MetalClassificationID = grade.MetalClassificationID;
                existingGrade.IdentifierValuesJson = grade.IdentifierValuesJson;

                // Remove missing lines (only check lines with real IDs from payload, ignore new lines with ID=0)
                var incomingLineIds = grade.SpecificationLines.Where(y => y.ID > 0).Select(y => y.ID).ToHashSet();
                var linesToRemove = existingGrade.SpecificationLines
                    .Where(x => !incomingLineIds.Contains(x.ID)).ToList();

                foreach (var lineToRemove in linesToRemove)
                {
                    existingGrade.SpecificationLines.Remove(lineToRemove);
                }

                foreach (var line in grade.SpecificationLines)
                {
                    var existingLine = line.ID > 0
                        ? existingGrade.SpecificationLines.FirstOrDefault(l => l.ID == line.ID)
                        : null;

                    if (existingLine == null)
                    {
                        line.ID = 0; // Ensure EF treats as new insert
                        line.SpecificationGradeID = existingGrade.ID;
                        existingGrade.SpecificationLines.Add(line);
                    }
                    else
                    {
                        // Update existing line
                        existingLine.Type = line.Type;
                        existingLine.ManualSelection = line.ManualSelection;
                        existingLine.ParameterID = line.ParameterID;
                        existingLine.MinValue = line.MinValue;
                        existingLine.MaxValue = line.MaxValue;
                        existingLine.TextValue = line.TextValue;
                        existingLine.InputType = line.InputType;
                        existingLine.Notes = line.Notes;
                        existingLine.Equation = line.Equation;
                        existingLine.ParameterUnitID = line.ParameterUnitID;
                        existingLine.ParameterUnitEquivalentID = line.ParameterUnitEquivalentID;
                        existingLine.LaboratoryTestID = line.LaboratoryTestID;
                        existingLine.MinEquation = line.MinEquation;
                        existingLine.MaxEquation = line.MaxEquation;
                        existingLine.MinValueEquation = line.MinValueEquation;
                        existingLine.MaxValueEquation = line.MaxValueEquation;
                        existingLine.MinTolerance = line.MinTolerance;
                        existingLine.MaxTolerance = line.MaxTolerance;
                        existingLine.SpecimenOrientationID = line.SpecimenOrientationID;
                        existingLine.DimensionalFactorID = line.DimensionalFactorID;
                        existingLine.LowerLimitValue = line.LowerLimitValue;
                        existingLine.UpperLimitValue = line.UpperLimitValue;
                        existingLine.LowerLimitDecimalValue = line.LowerLimitDecimalValue;
                        existingLine.UpperLimitDecimalValue = line.UpperLimitDecimalValue;
                        existingLine.HeatTreatmentID = line.HeatTreatmentID;
                        existingLine.ProductConditionID1 = line.ProductConditionID1;
                        existingLine.ProductConditionID2 = line.ProductConditionID2;
                        existingLine.ProductSizeMasterID = line.ProductSizeMasterID;
                        existingLine.TestCondition = line.TestCondition;
                        existingLine.TestNote = line.TestNote;

                        // MS-E: replace test-method mappings (clear + repopulate)
                        existingLine.TestMethodMappings.Clear();
                        if (line.TestMethodMappings != null)
                        {
                            foreach (var m in line.TestMethodMappings)
                            {
                                existingLine.TestMethodMappings.Add(new SpecificationLineTestMethod
                                {
                                    LaboratoryTestID = m.LaboratoryTestID,
                                    TestMethodSpecificationID = m.TestMethodSpecificationID,
                                    NumberOfTestSpecimen = m.NumberOfTestSpecimen,
                                    DisplayOrder = m.DisplayOrder
                                });
                            }
                        }
                    }
                }
            }
            await _specificationRepo.UpdateSpecificationHeader(existingSpecificationHeader);
            _logger.LogInformation("SpecificationHeader '{SpecificationHeaderName}' updated successfully.", model.AliasName);
        }


        // Returns a detached deep-copy of an existing spec with all IDs zeroed and Version cleared,
        // so the frontend can pre-fill the create form for a new version (clone-previous).
        public async Task<SpecificationHeader> GetCloneTemplate(long id)
        {
            var src = await _specificationRepo.GetSpecificationHeaderById(id);
            if (src == null)
                throw new InvalidOperationException("SpecificationHeader not found!");

            return new SpecificationHeader
            {
                ID = 0,
                AliasName = src.AliasName,
                StandardOrganizationID = src.StandardOrganizationID,
                Standard = src.Standard,
                Part = src.Part,
                StandardYear = src.StandardYear,
                IsCustom = src.IsCustom,
                SpecificationNo = src.SpecificationNo,
                Version = null, // user enters the new version
                DisplayTitle = src.DisplayTitle,
                Title = src.Title,
                IdentifierConfigJson = src.IdentifierConfigJson,
                HeaderParameters = src.HeaderParameters.Select(hp => new SpecificationHeaderParameter
                {
                    ID = 0,
                    ParameterID = hp.ParameterID,
                    ParameterUnitID = hp.ParameterUnitID,
                    ParameterUnitEquivalentID = hp.ParameterUnitEquivalentID,
                    Type = hp.Type,
                    DisplayOrder = hp.DisplayOrder
                }).ToList(),
                Grades = src.Grades.Select(g => new SpecificationGrade
                {
                    ID = 0,
                    Grade = g.Grade,
                    IsUNS = g.IsUNS,
                    UNSSteelNumber = g.UNSSteelNumber,
                    MetalClassificationID = g.MetalClassificationID,
                    IdentifierValuesJson = g.IdentifierValuesJson,
                    SpecificationLines = g.SpecificationLines.Select(l => new SpecificationLine
                    {
                        ID = 0,
                        ManualSelection = l.ManualSelection,
                        ParameterID = l.ParameterID,
                        MinValue = l.MinValue,
                        MaxValue = l.MaxValue,
                        Notes = l.Notes,
                        Equation = l.Equation,
                        MinEquation = l.MinEquation,
                        MaxEquation = l.MaxEquation,
                        ParameterUnitID = l.ParameterUnitID,
                        ParameterUnitEquivalentID = l.ParameterUnitEquivalentID,
                        LaboratoryTestID = l.LaboratoryTestID,
                        MinValueEquation = l.MinValueEquation,
                        MaxValueEquation = l.MaxValueEquation,
                        MinTolerance = l.MinTolerance,
                        MaxTolerance = l.MaxTolerance,
                        SpecimenOrientationID = l.SpecimenOrientationID,
                        DimensionalFactorID = l.DimensionalFactorID,
                        LowerLimitValue = l.LowerLimitValue,
                        UpperLimitValue = l.UpperLimitValue,
                        LowerLimitDecimalValue = l.LowerLimitDecimalValue,
                        UpperLimitDecimalValue = l.UpperLimitDecimalValue,
                        HeatTreatmentID = l.HeatTreatmentID,
                        ProductConditionID1 = l.ProductConditionID1,
                        ProductConditionID2 = l.ProductConditionID2,
                        ProductSizeMasterID = l.ProductSizeMasterID,
                        TestCondition = l.TestCondition,
                        TestNote = l.TestNote,
                        Type = l.Type,
                        TestMethodMappings = l.TestMethodMappings.Select(m => new SpecificationLineTestMethod
                        {
                            ID = 0,
                            LaboratoryTestID = m.LaboratoryTestID,
                            TestMethodSpecificationID = m.TestMethodSpecificationID,
                            NumberOfTestSpecimen = m.NumberOfTestSpecimen,
                            DisplayOrder = m.DisplayOrder
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }

        public async Task RemoveSpecificationHeader(long id)
        {
            var existingSpecificationHeader = await _specificationRepo.GetSpecificationHeaderById(id);
            if (existingSpecificationHeader == null)
                throw new InvalidOperationException("SpecificationHeader not found!");

            bool hasTolerances = await _context.ToleranceMasters.AnyAsync(t => t.SpecificationHeaderID == id && t.IsActive);
            if (hasTolerances)
                throw new InvalidOperationException("Cannot delete: Material Specification is linked to Tolerance records.");

            existingSpecificationHeader.IsActive = false;
            existingSpecificationHeader.ModifiedOn = DateTime.UtcNow;

            await _specificationRepo.UpdateSpecificationHeader(existingSpecificationHeader);
            _logger.LogInformation("SpecificationHeader with ID '{SpecificationHeaderId}' deleted successfully.", id);
        }

        public async Task<SpecificationHeader> GetSpecificationHeaderDetails(long id)
        {
            var classification = await _specificationRepo.GetSpecificationHeaderById(id);
            if (classification == null)
                throw new InvalidOperationException("SpecificationHeader not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSpecificationHeaderList(PageFilter filter)
        {
            return await _specificationRepo.GetAllSpecificationHeaders(filter);
        }

        public async Task<PagedResponse<object>> FetchCustomSpecificationHeaderList(PageFilter filter)
        {
            return await _specificationRepo.GetAllCustomSpecificationHeaders(filter);
        }

        public async Task<List<DropdwonSelector>> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _specificationRepo.GetSpecificationHeaderDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> GetGradeDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _specificationRepo.GetGradeDropdown(searchTerm, pageNo, pageSize);
        }
        public async Task<List<DropdwonSelector>> GetGradeDropdownMetalWise(string? searchTerm, int pageNo, int pageSize, long metalId)
        {
            return await _specificationRepo.GetGradeDropdownMetalWise(searchTerm, pageNo, pageSize, metalId);
        }
        public async Task<List<DropdwonSelector>> GetDefaultStandardForSpecification(long gradeId)
        {
            return await _specificationRepo.GetDefaultStandardForSpecification(gradeId);
        }
        public async Task<List<DropdwonSelector>> GetTestMethodsForSpecifications(long gradeId1, long gradeId2 = 0)
        {
            return await _specificationRepo.GetTestMethodsForSpecifications(gradeId1,gradeId2);
        }
        public async Task<List<ChemicalElementDto>> GetChemicalElementsBySpecificationsAsync(long gradeId1, long gradeId2 = 0)
        {
            return await _specificationRepo.GetChemicalElementsBySpecificationsAsync(gradeId1,gradeId2);
        }
    }
}
