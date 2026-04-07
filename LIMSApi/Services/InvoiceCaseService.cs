using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;

namespace LIMSApi.Services
{
    public class InvoiceCaseService : IInvoiceCaseService
    {
        private readonly IInvoiceCaseRepository _InvoiceCaseRepository;
        private readonly ILogger<InvoiceCaseService> _logger;

        public InvoiceCaseService(IInvoiceCaseRepository InvoiceCaseRepo, ILogger<InvoiceCaseService> logger)
        {
            _InvoiceCaseRepository = InvoiceCaseRepo;
            _logger = logger;
        }

        public async Task CreateInvoiceCase(InvoiceCase model)
        {
            // Check duplicate: same Financial Year + same Sub Group Test
            if (await _InvoiceCaseRepository.ExistsByFinancialYearAndTest(model.FinancialYearId, model.LaboratoryTestID))
                throw new InvalidOperationException($"Invoice Case for this Sub Group Test already exists for this Financial Year!");

            foreach (var price in model.InvoiceCasePrices)
            {
                if (price != null)
                {
                    if (await _InvoiceCaseRepository.ExistsByFinancialAndName(model.FinancialYearId, price.Name))
                    {
                        throw new InvalidOperationException($"Same Invoice case {price.Name} already exists!");
                    }
                }
            }
            await _InvoiceCaseRepository.AddInvoiceCase(model);
            _logger.LogInformation("InvoiceCase '{InvoiceCaseName}' created successfully.", model.LaboratoryTestID);
        }

        public async Task ModifyInvoiceCase(InvoiceCase model)
        {
            if (model.ID == 0)
                throw new ArgumentException("InvoiceCase ID should not be empty!");

            var existingInvoiceCase = await _InvoiceCaseRepository.GetInvoiceCaseById(model.ID);
            if (existingInvoiceCase == null)
                throw new InvalidOperationException("InvoiceCase not found!");

            // Check duplicate on update: same Financial Year + same Sub Group Test (excluding self)
            if (await _InvoiceCaseRepository.ExistsByFinancialYearAndTestNotId(model.FinancialYearId, model.LaboratoryTestID, model.ID))
                throw new InvalidOperationException($"Invoice Case for this Sub Group Test already exists for this Financial Year!");

            existingInvoiceCase.LaboratoryTestID = model.LaboratoryTestID;
            existingInvoiceCase.FinancialYearId = model.FinancialYearId;
            existingInvoiceCase.ModifiedOn = DateTime.UtcNow;

            // --- Remove missing Invoice Prices ---
            var toRemovePrice = existingInvoiceCase.InvoiceCasePrices
                .Where(x => !model.InvoiceCasePrices.Any(y => y.ID == x.ID))
                .ToList();

            foreach (var IP in toRemovePrice)
            {
                existingInvoiceCase.InvoiceCasePrices.Remove(IP);
            }

            // --- Add or Update Invoice Prices ---
            foreach (var incomingInvoices in model.InvoiceCasePrices)
            {
                var existingInvoicePrices = existingInvoiceCase.InvoiceCasePrices
                    .FirstOrDefault(x => x.ID == incomingInvoices.ID);

                if (existingInvoicePrices == null)
                {
                    existingInvoiceCase.InvoiceCasePrices.Add(incomingInvoices);
                }
                else
                {
                    // Update existing specification
                    existingInvoicePrices.InvoiceCaseConfigID = incomingInvoices.InvoiceCaseConfigID;
                    existingInvoicePrices.Name = incomingInvoices.Name;
                    existingInvoicePrices.AliasName = incomingInvoices.AliasName;
                    existingInvoicePrices.Price = incomingInvoices.Price;
                    existingInvoicePrices.InvoiceCaseConfigID = incomingInvoices.InvoiceCaseConfigID;

                }
            }

            await _InvoiceCaseRepository.UpdateInvoiceCase(existingInvoiceCase);
            _logger.LogInformation("InvoiceCase '{InvoiceCaseName}' updated successfully.", model.LaboratoryTestID);
        }


        public async Task RemoveInvoiceCase(long id)
        {
            var existingInvoiceCase = await _InvoiceCaseRepository.GetInvoiceCaseById(id);
            if (existingInvoiceCase == null)
                throw new InvalidOperationException("InvoiceCase not found!");

            existingInvoiceCase.IsActive = false;
            existingInvoiceCase.ModifiedOn = DateTime.UtcNow;

            await _InvoiceCaseRepository.UpdateInvoiceCase(existingInvoiceCase);
            _logger.LogInformation("InvoiceCase with ID '{InvoiceCaseId}' deleted successfully.", id);
        }

        public async Task<InvoiceCase> GetInvoiceCaseDetails(long id)
        {
            var classification = await _InvoiceCaseRepository.GetInvoiceCaseById(id);
            if (classification == null)
                throw new InvalidOperationException("InvoiceCase not found!");

            return classification;
        }

        public async Task<PagedResponse<object>> FetchInvoiceCaseList(PageFilter filter)
        {
            return await _InvoiceCaseRepository.GetAllInvoiceCases(filter);
        }

    }
}
