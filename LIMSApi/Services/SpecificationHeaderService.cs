using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SpecificationHeaderService : ISpecificationHeaderService
    {
        private readonly ISpecificationHeaderRepository _uomRepository;
        private readonly ILogger<SpecificationHeaderService> _logger;

        public SpecificationHeaderService(ISpecificationHeaderRepository uomRepo, ILogger<SpecificationHeaderService> logger)
        {
            _uomRepository = uomRepo;
            _logger = logger;
        }

        public async Task CreateSpecificationHeader(SpecificationHeader model)
        {
            if (string.IsNullOrWhiteSpace(model.AliasName))
                throw new ArgumentException("SpecificationHeader name should not be empty!");

            bool exists = await _uomRepository.ExistsByName(model.AliasName);
            if (exists)
                throw new InvalidOperationException("SpecificationHeader already exists!");

            //if (model.SpecificationLines != null && model.SpecificationLines.Any())
            //{
            //    foreach(var line in model.SpecificationLines)
            //    {
            //        line.SpecificationHeaderID = model.ID;
            //        if (line.ProductConditions != null && line.ProductConditions.Any())
            //        {
            //            foreach (var condition in line.ProductConditions)
            //            {
            //                condition.SpecificationLineID = line.ID;
            //            }
            //        } 
            //        if(line.LaboratoryTests != null && line.LaboratoryTests.Any())
            //        {
            //            foreach( var test in line.LaboratoryTests)
            //            {
            //                test.SpecificationLineID = line.ID;
            //            }
            //        }
            //    }
            //}
            await _uomRepository.AddSpecificationHeader(model);

            _logger.LogInformation("SpecificationHeader '{SpecificationHeaderName}' created successfully.", model.AliasName);
        }

        public async Task ModifySpecificationHeader(SpecificationHeader model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SpecificationHeader ID should not be empty!");

            bool exists = await _uomRepository.ExistsByNameAndNotId(model.AliasName, model.ID);
            if (exists)
                throw new InvalidOperationException("Same SpecificationHeader already exists!");

            var existingSpecificationHeader = await _uomRepository.GetSpecificationHeaderById(model.ID);
            if (existingSpecificationHeader == null)
                throw new InvalidOperationException("SpecificationHeader not found!");

            // Update SpecificationHeader fields
            existingSpecificationHeader.SpecificationCode = model.SpecificationCode;
            existingSpecificationHeader.StandardOrganizationID = model.StandardOrganizationID;
            existingSpecificationHeader.Standard = model.Standard;
            existingSpecificationHeader.Part = model.Part;
            existingSpecificationHeader.StandardYear = model.StandardYear;
            existingSpecificationHeader.IsUNS = model.IsUNS;
            existingSpecificationHeader.AliasName = model.AliasName;
            existingSpecificationHeader.UNSSteelNumber = model.UNSSteelNumber;
            existingSpecificationHeader.MetalCalssificationID = model.MetalCalssificationID;
            existingSpecificationHeader.IsCustom = model.IsCustom;
            existingSpecificationHeader.Type = model.Type;
            existingSpecificationHeader.ModifiedOn = DateTime.UtcNow;

            // === Handle SpecificationLines ===
            var toRemoveLines = existingSpecificationHeader.SpecificationLines
                .Where(x => !model.SpecificationLines.Any(y => y.ID == x.ID)).ToList();

            foreach (var lineToRemove in toRemoveLines)
            {
                existingSpecificationHeader.SpecificationLines.Remove(lineToRemove);
            }

            foreach (var line in model.SpecificationLines)
            {
                var existingLine = existingSpecificationHeader.SpecificationLines
                    .FirstOrDefault(l => l.ID == line.ID);

                if (existingLine == null)
                {
                    // New Line
                    line.SpecificationHeaderID = model.ID;
                    existingSpecificationHeader.SpecificationLines.Add(line);
                }
                else
                {
                    // Update existing Line
                    existingLine.PropertyType = line.PropertyType;
                    existingLine.ManualSelection = line.ManualSelection;
                    existingLine.ParameterID = line.ParameterID;
                    existingLine.MinValue = line.MinValue;
                    existingLine.MaxValue = line.MaxValue;
                    existingLine.Notes = line.Notes;
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

                    // === Product Conditions ===
                    existingLine.ProductConditions?.Clear();

                    foreach (var condition in line.ProductConditions)
                    {
                        condition.SpecificationLineID = existingLine.ID;
                        existingLine.ProductConditions?.Add(condition);
                    }

                    // === Laboratory Tests ===
                    existingLine.LaboratoryTests?.Clear();
                    foreach (var test in line.LaboratoryTests)
                    {
                        test.SpecificationLineID = existingLine.ID;
                        existingLine.LaboratoryTests?.Add(test);
                    }
                }
            }

            await _uomRepository.UpdateSpecificationHeader(existingSpecificationHeader);
            _logger.LogInformation("SpecificationHeader '{SpecificationHeaderName}' updated successfully.", model.AliasName);
        }


        public async Task RemoveSpecificationHeader(long id)
        {
            var existingSpecificationHeader = await _uomRepository.GetSpecificationHeaderById(id);
            if (existingSpecificationHeader == null)
                throw new InvalidOperationException("SpecificationHeader not found!");

            existingSpecificationHeader.IsActive = false;
            existingSpecificationHeader.ModifiedOn = DateTime.UtcNow;

            await _uomRepository.UpdateSpecificationHeader(existingSpecificationHeader);
            _logger.LogInformation("SpecificationHeader with ID '{SpecificationHeaderId}' deleted successfully.", id);
        }

        public async Task<SpecificationHeader> GetSpecificationHeaderDetails(long id)
        {
            var classification = await _uomRepository.GetSpecificationHeaderById(id);
            if (classification == null)
                throw new InvalidOperationException("SpecificationHeader not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSpecificationHeaderList(PageFilter filter)
        {
            return await _uomRepository.GetAllSpecificationHeaders(filter);
        }

        public async Task<PagedResponse<object>> FetchCustomSpecificationHeaderList(PageFilter filter)
        {
            return await _uomRepository.GetAllCustomSpecificationHeaders(filter);
        }

        public async Task<List<DropdwonSelector>> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _uomRepository.GetSpecificationHeaderDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
