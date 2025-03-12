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
            if (string.IsNullOrWhiteSpace(model.SpecificationCode))
                throw new ArgumentException("SpecificationHeader name should not be empty!");

            bool exists = await _uomRepository.ExistsByName(model.SpecificationCode);
            if (exists)
                throw new InvalidOperationException("SpecificationHeader already exists!");

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

            existingSpecificationHeader.AliasName = model.AliasName;
            existingSpecificationHeader.UNSSteelNumber = model.UNSSteelNumber;
            existingSpecificationHeader.Standard = model.Standard;
            existingSpecificationHeader.Part = model.Part;
            existingSpecificationHeader.SpecificationCode = model.SpecificationCode;
            existingSpecificationHeader.StandardOrganizationID = model.StandardOrganizationID;
            existingSpecificationHeader.StandardYear = model.StandardYear;
            existingSpecificationHeader.IsUNS = model.IsUNS;

            existingSpecificationHeader.ModifiedOn = DateTime.UtcNow;

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

        public async Task<List<DropdwonSelector>> GetSpecificationHeaderDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _uomRepository.GetSpecificationHeaderDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
