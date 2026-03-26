using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class InvoiceCaseConfigurationService : IInvoiceCaseConfigurationService
    {
        private readonly IInvoiceCaseConfigurationRepository _InvoiceCaseConfigurationRepository;
        private readonly ILogger<InvoiceCaseConfigurationService> _logger;
        private LoggedInUserDTO loggedInUser;

        public InvoiceCaseConfigurationService(IInvoiceCaseConfigurationRepository InvoiceCaseConfigurationRepo, ILogger<InvoiceCaseConfigurationService> logger)
        {
            _InvoiceCaseConfigurationRepository = InvoiceCaseConfigurationRepo;
            _logger = logger;
            loggedInUser = LoggedInUserProvider.CurrentUser;
        }

        public async Task CreateInvoiceCaseConfiguration(List<InvoiceCaseConfiguration> configurations)
        {
            foreach (var model in configurations)
            {

                if (string.IsNullOrWhiteSpace(model.Name))
                    throw new ArgumentException("InvoiceCaseConfiguration name should not be empty!");

                if (await _InvoiceCaseConfigurationRepository.ExistsByName(model.Name))
                    throw new InvalidOperationException("Same InvoiceCaseConfiguration already exists!");

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = loggedInUser.EmployeeID;
                model.CompanyCode = loggedInUser.CompanyCode;

                // Sync comma-separated AliasName string from normalized AliasNames collection
                SyncAliasNameString(model);

                await _InvoiceCaseConfigurationRepository.AddInvoiceCaseConfiguration(model);
                _logger.LogInformation("InvoiceCaseConfiguration '{InvoiceCaseConfigurationName}' created successfully.", model.Name);
            }
        }

        public async Task ModifyInvoiceCaseConfiguration(List<InvoiceCaseConfiguration> configurations)
        {
            foreach (var model in configurations)
            {

                if (model.ID == 0)
                    throw new ArgumentException("InvoiceCaseConfiguration ID should not be empty!");

                if (await _InvoiceCaseConfigurationRepository.ExistsByNameAndNotId(model.Name, model.ID))
                    throw new InvalidOperationException("Same InvoiceCaseConfiguration already exists!");

                var existingInvoiceCaseConfiguration = await _InvoiceCaseConfigurationRepository.GetInvoiceCaseConfigurationById(model.ID);

                if (existingInvoiceCaseConfiguration == null)
                    throw new InvalidOperationException("InvoiceCaseConfiguration not found!");

                existingInvoiceCaseConfiguration.Name = model.Name;
                existingInvoiceCaseConfiguration.SelectionType = model.SelectionType;
                existingInvoiceCaseConfiguration.AliasName = model.AliasName;
                existingInvoiceCaseConfiguration.Value = model.Value;
                existingInvoiceCaseConfiguration.Start = model.Start;
                existingInvoiceCaseConfiguration.End = model.End;
                existingInvoiceCaseConfiguration.Unit = model.Unit;

                existingInvoiceCaseConfiguration.ModifiedOn = DateTime.UtcNow;
                existingInvoiceCaseConfiguration.ModifiedBy = loggedInUser.EmployeeID;

                var toRemoveAlias = existingInvoiceCaseConfiguration.AliasNames.Where(x => !model.AliasNames.Any(y => y.ID == x.ID)).ToList();

                foreach (var alias in toRemoveAlias)
                {
                    existingInvoiceCaseConfiguration.AliasNames.Remove(alias);
                }

                foreach (var alias in model.AliasNames)
                {
                    var existingAlias = existingInvoiceCaseConfiguration.AliasNames.FirstOrDefault(x => x.ID == alias.ID && alias.ID != 0);
                    if (existingAlias != null)
                    {
                        existingAlias.Name = alias.Name;
                        existingAlias.InvoiceConfigurationID = alias.InvoiceConfigurationID;
                    }
                    else
                    {
                        existingInvoiceCaseConfiguration.AliasNames.Add(alias);
                    }
                }

                // Sync comma-separated AliasName string from normalized AliasNames collection
                SyncAliasNameString(existingInvoiceCaseConfiguration);

                await _InvoiceCaseConfigurationRepository.UpdateInvoiceCaseConfiguration(existingInvoiceCaseConfiguration);
                _logger.LogInformation("InvoiceCaseConfiguration '{InvoiceCaseConfigurationName}' updated successfully.", model.Name);
            }
        }

        /// <summary>
        /// Sync the comma-separated AliasName string field from the normalized AliasNames collection.
        /// Keeps both in sync — normalized table is primary, string field is backward-compat.
        /// </summary>
        private static void SyncAliasNameString(InvoiceCaseConfiguration config)
        {
            if (config.AliasNames != null && config.AliasNames.Any())
            {
                config.AliasName = string.Join(", ", config.AliasNames
                    .Where(a => !string.IsNullOrWhiteSpace(a.Name))
                    .Select(a => a.Name.Trim()));
            }
        }

        public async Task RemoveInvoiceCaseConfiguration(long id)
        {
            var existingInvoiceCaseConfiguration = await _InvoiceCaseConfigurationRepository.GetInvoiceCaseConfigurationById(id);
            if (existingInvoiceCaseConfiguration == null)
                throw new InvalidOperationException("InvoiceCaseConfiguration not found!");

            existingInvoiceCaseConfiguration.IsActive = false;
            existingInvoiceCaseConfiguration.ModifiedOn = DateTime.UtcNow;
            existingInvoiceCaseConfiguration.ModifiedBy = loggedInUser.EmployeeID;

            await _InvoiceCaseConfigurationRepository.UpdateInvoiceCaseConfiguration(existingInvoiceCaseConfiguration);
            _logger.LogInformation("InvoiceCaseConfiguration with ID '{InvoiceCaseConfigurationId}' deleted successfully.", id);
        }

        public async Task<InvoiceCaseConfiguration> GetInvoiceCaseConfigurationDetails(long id)
        {
            var existingInvoiceCaseConfiguration = await _InvoiceCaseConfigurationRepository.GetInvoiceCaseConfigurationById(id);
            if (existingInvoiceCaseConfiguration == null)
                throw new InvalidOperationException("InvoiceCaseConfiguration not found!");

            return existingInvoiceCaseConfiguration;
        }

        public async Task<PagedResponse<object>> FetchInvoiceCaseConfigurationList(PageFilter filter)
        {
            return await _InvoiceCaseConfigurationRepository.GetAllInvoiceCaseConfigurations(filter);
        }

        public async Task<List<DropdwonSelector>> GetInvoiceCaseConfigurationDropdown(string? searchTerm, int pageNo, int pageSize)
        {
            return await _InvoiceCaseConfigurationRepository.GetInvoiceCaseConfigurationDropdown(searchTerm, pageNo, pageSize);
        }
    }
}
