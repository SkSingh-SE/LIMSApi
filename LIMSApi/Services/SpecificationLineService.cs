using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SpecificationLineService : ISpecificationLineService
    {
        private readonly ISpecificationLineRepository _specificationLineRepository;
        private readonly ILogger<SpecificationLineService> _logger;

        public SpecificationLineService(ISpecificationLineRepository specificationLineRepository, ILogger<SpecificationLineService> logger)
        {
            _specificationLineRepository = specificationLineRepository;
            _logger = logger;
        }

        public async Task CreateSpecificationLine(SpecificationLine model)
        {
            await _specificationLineRepository.AddSpecificationLine(model);
            _logger.LogInformation("SpecificationLine created against '{SpecificationHeaderId}' successfully.", model.SpecificationHeaderID);
        }

        public async Task ModifySpecificationLine(SpecificationLine model)
        {
            
            var existingSpecificationLine = await _specificationLineRepository.GetSpecificationLineById(model.ID);
            if (existingSpecificationLine == null)
                throw new InvalidOperationException("SpecificationLine not found!");

            existingSpecificationLine.SpecificationHeaderID = model.SpecificationHeaderID;
            existingSpecificationLine.HeatTreatmentID = model.HeatTreatmentID;
            existingSpecificationLine.ParameterID  = model.ParameterID;
            existingSpecificationLine.DimensionalFactorID  = model.DimensionalFactorID;
            existingSpecificationLine.SpecimenOrientationID  = model.SpecimenOrientationID;
            existingSpecificationLine.MinValue = model.MinValue;
            existingSpecificationLine.MaxValue = model.MaxValue;
            existingSpecificationLine.PropertyType = model.PropertyType;
            existingSpecificationLine.Notes = model.Notes;
            existingSpecificationLine.ManualSelection = model.ManualSelection;
            existingSpecificationLine.LowerLimitValue = model.LowerLimitValue;
            existingSpecificationLine.UpperLimitValue = model.UpperLimitValue;

            existingSpecificationLine.ModifiedOn = DateTime.UtcNow;

            await _specificationLineRepository.UpdateSpecificationLine(existingSpecificationLine);
            _logger.LogInformation("SpecificationLine updated against '{SpecificationHeaderId}' successfully.", model.SpecificationHeaderID);
        }

        public async Task RemoveSpecificationLine(long id)
        {
            var existingSpecificationLine = await _specificationLineRepository.GetSpecificationLineById(id);
            if (existingSpecificationLine == null)
                throw new InvalidOperationException("SpecificationLine not found!");

            existingSpecificationLine.IsActive = false;
            existingSpecificationLine.ModifiedOn = DateTime.UtcNow;

            await _specificationLineRepository.UpdateSpecificationLine(existingSpecificationLine);
            _logger.LogInformation("SpecificationLine with ID '{SpecificationLineId}' deleted successfully.", id);
        }

        public async Task<SpecificationLine> GetSpecificationLineDetails(long id)
        {
            var classification = await _specificationLineRepository.GetSpecificationLineById(id);
            if (classification == null)
                throw new InvalidOperationException("SpecificationLine not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSpecificationLineList(PageFilter filter)
        {
            return await _specificationLineRepository.GetAllSpecificationLines(filter);
        }

        public async Task<List<DropdwonSelector>> GetSpecificationLineDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _specificationLineRepository.GetSpecificationLineDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
