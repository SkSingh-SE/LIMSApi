using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class SubContractorService : ISubContractorService
    {
        private readonly ISubContractorRepository _subContractorRepository;
        private readonly ILogger<SubContractorService> _logger;
        private LoggedInUserDTO loggedInUser;

        public SubContractorService(ISubContractorRepository SubContractorRepo, ILogger<SubContractorService> logger)
        {
            _subContractorRepository = SubContractorRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateSubContractor(SubContractorMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("SubContractor name should not be empty!");

            bool exists = await _subContractorRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("SubContractor already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _subContractorRepository.AddSubContractor(model);
            _logger.LogInformation("SubContractor '{SubContractorName}' created successfully.", model.Name);
        }

        public async Task ModifySubContractor(SubContractorMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("SubContractor ID should not be empty!");

            bool exists = await _subContractorRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same SubContractor already exists!");

            var existingSubContractor = await _subContractorRepository.GetSubContractorById(model.ID);
            if (existingSubContractor == null)
                throw new InvalidOperationException("SubContractor not found!");


            existingSubContractor.Name = model.Name;
            existingSubContractor.Alias = model.Alias;
            existingSubContractor.EmailID = model.EmailID;
            existingSubContractor.PhoneNo = model.PhoneNo;
            existingSubContractor.MobileNo = model.MobileNo;
            existingSubContractor.GSTNo = model.GSTNo;
            existingSubContractor.Address = model.Address;

            existingSubContractor.ModifiedOn = DateTime.UtcNow;
            existingSubContractor.ModifiedBy = loggedInUser.EmployeeID;

            await _subContractorRepository.UpdateSubContractor(existingSubContractor);
            _logger.LogInformation("SubContractor '{SubContractorName}' updated successfully.", model.Name);
        }

        public async Task RemoveSubContractor(long id)
        {
            var existingSubContractor = await _subContractorRepository.GetSubContractorById(id);
            if (existingSubContractor == null)
                throw new InvalidOperationException("SubContractor not found!");

            existingSubContractor.IsActive = false;
            existingSubContractor.ModifiedOn = DateTime.UtcNow;
            existingSubContractor.ModifiedBy = loggedInUser.EmployeeID;

            await _subContractorRepository.DeleteSubContractor(existingSubContractor);
            _logger.LogInformation("SubContractor with ID '{SubContractorId}' deleted successfully.", id);
        }

        public async Task<SubContractorMaster> GetSubContractorDetails(long id)
        {
            var classification = await _subContractorRepository.GetSubContractorById(id);
            if (classification == null)
                throw new InvalidOperationException("SubContractor not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchSubContractorList(PageFilter filter)
        {
            return await _subContractorRepository.GetAllSubContractors(filter);
        }

        public async Task<List<DropdwonSelector>> GetSubContractorDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _subContractorRepository.GetSubContractorDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
