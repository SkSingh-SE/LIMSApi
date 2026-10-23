using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<CurrencyService> _logger;
        public CurrencyService(ICurrencyRepository currencyRepo, ILogger<CurrencyService> logger)
        {
            _currencyRepository = currencyRepo;
            _logger = logger;
            
        }

        
        public async Task CreateCurrency(CurrencyMaster model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Currency name should not be empty!");

            bool exists = await _currencyRepository.ExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException("Currency already exists!");

            await _currencyRepository.AddCurrency(model);
            _logger.LogInformation("Currency '{CurrencyName}' created successfully.", model.Name);
        }

        public async Task ModifyCurrency(CurrencyMaster model)
        {
            if (model.ID == 0)
                throw new ArgumentException("Currency ID should not be empty!");

            bool exists = await _currencyRepository.ExistsByNameAndNotId(model.Name, model.ID);
            if (exists)
                throw new InvalidOperationException("Same Currency already exists!");

            var existingCurrency = await _currencyRepository.GetCurrencyById(model.ID);
            if (existingCurrency == null)
                throw new InvalidOperationException("Currency not found!");

            existingCurrency.Name = model.Name;
            existingCurrency.Code = model.Code;
            existingCurrency.ModifiedOn = DateTime.UtcNow;

            await _currencyRepository.UpdateCurrency(existingCurrency);
            _logger.LogInformation("Currency '{CurrencyName}' updated successfully.", model.Name);
        }

        public async Task RemoveCurrency(long id)
        {
            var existingCurrency = await _currencyRepository.GetCurrencyById(id);
            if (existingCurrency == null)
                throw new InvalidOperationException("Currency not found!");

            existingCurrency.IsActive = false;
            existingCurrency.ModifiedOn = DateTime.UtcNow;

            await _currencyRepository.UpdateCurrency(existingCurrency);
            _logger.LogInformation("Currency with ID '{CurrencyId}' deleted successfully.", id);
        }

        public async Task<CurrencyMaster> GetCurrencyDetails(long id)
        {
            var data = LoggedInUserProvider.CurrentUser;
            var currency = await _currencyRepository.GetCurrencyById(id);
            if (currency == null)
                throw new InvalidOperationException("Currency not found!");

            return currency;
        }

        public async Task<PagedResponse<CurrencyMaster>> FetchCurrencies(PageFilter filter)
        {
            return await _currencyRepository.GetAllCurrencys(filter);
        }

        public async Task<List<DropdwonSelector>> GetCurrencyDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _currencyRepository.GetCurrencyDropdown(searchTerm, pageNo, pageSize);
        }

        public async Task<CurrencyMaster?> GetDefaultCurrency()
        {
            return await _currencyRepository.GetDefaultCurrency();
        }

        public async Task SetDefaultCurrency(long id)
        {
            var currency = await _currencyRepository.GetCurrencyById(id);
            if (currency == null)
                throw new KeyNotFoundException("Currency not found.");

            await _currencyRepository.SetDefaultCurrency(id);
            _logger.LogInformation("Currency '{CurrencyName}' set as default.", currency.Name);
        }
    }
}

