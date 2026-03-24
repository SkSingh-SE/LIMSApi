using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class TaxService : ITaxService
    {
        private readonly ITaxRepository _taxRepository;
        private readonly ILogger<TaxService> _logger;
        private readonly LIMSContext _context;
        private LoggedInUserDTO loggedInUser;

        public TaxService(ITaxRepository TaxRepo, ILogger<TaxService> logger, LIMSContext context)
        {
            _taxRepository = TaxRepo;
            _logger = logger;
            _context = context;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateTax(TaxMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Tax name should not be empty!");

            bool exists = await _taxRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Tax already exists!");

            model.CreatedOn = DateTime.UtcNow;
            model.CreatedBy = loggedInUser.EmployeeID;
            model.CompanyCode = loggedInUser.CompanyCode;

            await _taxRepository.AddTax(model);
            _logger.LogInformation("Tax '{TaxName}' created successfully.", model.Name);
        }

        public async Task ModifyTax(TaxMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Tax ID should not be empty!");

            bool exists = await _taxRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Tax already exists!");

            var existingTax = await _taxRepository.GetTaxById(model.ID);
            if (existingTax == null)
                throw new InvalidOperationException("Tax not found!");


            existingTax.Name = model.Name;
            existingTax.Remark = model.Remark;
            existingTax.Date = model.Date;
            existingTax.Rate = model.Rate;
            existingTax.ModifiedOn = DateTime.UtcNow;
            existingTax.ModifiedBy = loggedInUser.EmployeeID;

            await _taxRepository.UpdateTax(existingTax);
            _logger.LogInformation("Tax '{TaxName}' updated successfully.", model.Name);
        }

        public async Task RemoveTax(long id)
        {
            var existingTax = await _taxRepository.GetTaxById(id);
            if (existingTax == null)
                throw new InvalidOperationException("Tax not found!");

            await DeleteValidationHelper.ValidateDeleteAsync<TaxMaster>(_context, id, "Tax");

            existingTax.IsActive = false;
            existingTax.ModifiedOn = DateTime.UtcNow;
            existingTax.ModifiedBy = loggedInUser.EmployeeID;

            await _taxRepository.DeleteTax(existingTax);
            _logger.LogInformation("Tax with ID '{TaxId}' deleted successfully.", id);
        }

        public async Task<TaxMaster> GetTaxDetails(long id)
        {
            var classification = await _taxRepository.GetTaxById(id);
            if (classification == null)
                throw new InvalidOperationException("Tax not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchTaxList(PageFilter filter)
        {
            return await _taxRepository.GetAllTaxs(filter);
        }

        public async Task<List<DropdwonSelector>> GetTaxDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _taxRepository.GetTaxDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
