using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SpecificationHeaderService : ISpecificationHeaderService
    {
        private readonly ISpecificationHeaderRepository _specificationRepo;
        private readonly ILogger<SpecificationHeaderService> _logger;

        public SpecificationHeaderService(ISpecificationHeaderRepository uomRepo, ILogger<SpecificationHeaderService> logger)
        {
            _specificationRepo = uomRepo;
            _logger = logger;
        }

        public async Task CreateSpecificationHeader(SpecificationHeader model)
        {
            if (string.IsNullOrWhiteSpace(model.AliasName))
                throw new ArgumentException("SpecificationHeader name should not be empty!");

            bool exists = await _specificationRepo.ExistsByName(model.AliasName);
            if (exists)
                throw new InvalidOperationException("SpecificationHeader already exists!");

            await _specificationRepo.AddSpecificationHeader(model);

            _logger.LogInformation("SpecificationHeader '{SpecificationHeaderName}' created successfully.", model.AliasName);
        }

        public async Task ModifySpecificationHeader(SpecificationHeader model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SpecificationHeader ID should not be empty!");

            bool exists = await _specificationRepo.ExistsByNameAndNotId(model.AliasName, model.ID);
            if (exists)
                throw new InvalidOperationException("Same SpecificationHeader already exists!");

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
            existingSpecificationHeader.ModifiedOn = DateTime.UtcNow;

            var toRemoveGrades = existingSpecificationHeader.Grades.Where(x => !model.Grades.Any(y =>  x.ID == x.ID)).ToList();
            foreach(var  grade in toRemoveGrades)
            {
                existingSpecificationHeader.Grades.Remove(grade);
            }

            foreach (var grade in model.Grades)
            {
                var existingGrade = existingSpecificationHeader.Grades.FirstOrDefault(x => x.ID == grade.ID);
                if (existingGrade == null)
                {
                    grade.SpecificationHeaderID = model.ID;
                    existingSpecificationHeader.Grades.Add(grade);
                    continue;
                }

                // Update existing grade
                existingGrade.Grade = grade.Grade;
                existingGrade.MetalClassificationID = grade.MetalClassificationID;
                existingGrade.IsUNS = grade.IsUNS;
                existingGrade.UNSSteelNumber = grade.UNSSteelNumber;

                // Remove missing lines
                var linesToRemove = existingGrade.SpecificationLines
                    .Where(x => !grade.SpecificationLines.Any(y => y.ID == x.ID)).ToList();

                foreach (var lineToRemove in linesToRemove)
                {
                    existingGrade.SpecificationLines.Remove(lineToRemove);
                }

                foreach (var line in grade.SpecificationLines)
                {
                    var existingLine = existingGrade.SpecificationLines
                        .FirstOrDefault(l => l.ID == line.ID);

                    if (existingLine == null)
                    {
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
                        existingLine.Notes = line.Notes;
                        existingLine.Equation = line.Equation;
                        existingLine.ParameterUnitID = line.ParameterUnitID;
                        existingLine.MinValueEquation = line.MinValueEquation;
                        existingLine.MaxValueEquation = line.MaxValueEquation;
                        existingLine.MinTolerance = line.MinTolerance;
                        existingLine.MaxTolerance = line.MaxTolerance;
                        existingLine.SpecimenOrientationID = line.SpecimenOrientationID;
                        existingLine.DimensionalFactorID = line.DimensionalFactorID;
                        existingLine.LowerLimitValue = line.LowerLimitValue;
                        existingLine.UpperLimitValue = line.UpperLimitValue;
                        existingLine.HeatTreatmentID = line.HeatTreatmentID;
                        existingLine.ProductConditionID1 = line.ProductConditionID1;
                        existingLine.ProductConditionID2 = line.ProductConditionID2;


                        // Laboratory Tests
                        existingLine.LaboratoryTests.Clear();
                        foreach (var test in line.LaboratoryTests)
                        {
                            if (test == null)
                                continue;
                            test.SpecificationLineID = existingLine.ID;
                            existingLine.LaboratoryTests.Add(test);
                        }
                    }
                }
            }
            await _specificationRepo.UpdateSpecificationHeader(existingSpecificationHeader);
            _logger.LogInformation("SpecificationHeader '{SpecificationHeaderName}' updated successfully.", model.AliasName);
        }


        public async Task RemoveSpecificationHeader(long id)
        {
            var existingSpecificationHeader = await _specificationRepo.GetSpecificationHeaderById(id);
            if (existingSpecificationHeader == null)
                throw new InvalidOperationException("SpecificationHeader not found!");

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
        public async Task<List<DropdwonSelector>> GetDefaultStandardForSpecification(long gradeId)
        {
            return await _specificationRepo.GetDefaultStandardForSpecification(gradeId);
        }
        public async Task<List<DropdwonSelector>> GetTestMethodsForSpecifications(long gradeId1, long gradeId2 = 0)
        {
            return await _specificationRepo.GetTestMethodsForSpecifications(gradeId1,gradeId2);
        }
    }
}
