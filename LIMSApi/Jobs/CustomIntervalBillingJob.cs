using LIMSApi.Data;
using LIMSApi.ServiceWORepo;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    /// <summary>
    /// Runs daily at 2 AM — generates invoices for customers with custom billing intervals (BillingEvery + BillingEveryDays).
    /// </summary>
    public class CustomIntervalBillingJob
    {
        private readonly LIMSContext _db;
        private readonly IAccountService _accountService;
        private readonly ILogger<CustomIntervalBillingJob> _logger;

        public CustomIntervalBillingJob(LIMSContext db, IAccountService accountService, ILogger<CustomIntervalBillingJob> logger)
        {
            _db = db;
            _accountService = accountService;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Custom interval billing job started at {Time}", DateTime.UtcNow);

            try
            {
                var customers = await _db.Customers
                    .Where(c => c.IsActive && c.BillingEvery && c.BillingEveryDays.HasValue && c.BillingEveryDays.Value > 0)
                    .Select(c => new { c.ID, c.BillingEveryDays, c.LastBillingDate })
                    .ToListAsync();

                var processedCount = 0;

                foreach (var customer in customers)
                {
                    var daysSinceLastBilling = customer.LastBillingDate.HasValue
                        ? (DateTime.UtcNow - customer.LastBillingDate.Value).Days
                        : int.MaxValue; // Never billed → due immediately

                    if (daysSinceLastBilling >= customer.BillingEveryDays!.Value)
                    {
                        await ProcessBillingForCustomer(customer.ID);

                        // Update LastBillingDate
                        var entity = await _db.Customers.FindAsync(customer.ID);
                        if (entity != null)
                        {
                            entity.LastBillingDate = DateTime.UtcNow;
                            await _db.SaveChangesAsync();
                        }

                        processedCount++;
                    }
                }

                _logger.LogInformation("Custom interval billing job completed. Processed {Count} of {Total} eligible customers.",
                    processedCount, customers.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Custom interval billing job failed.");
            }
        }

        private async Task ProcessBillingForCustomer(long customerId)
        {
            try
            {
                var pendingInwards = await _db.SampleInwards
                    .Where(i => i.CustomerID == customerId
                        && i.IsActive
                        && i.BillingStatus == "PRICE_SNAPSHOT"
                        && !i.IsInvoiceGenerated)
                    .Select(i => i.ID)
                    .ToListAsync();

                foreach (var inwardId in pendingInwards)
                {
                    try
                    {
                        await _accountService.GenerateInvoiceAsync(inwardId);
                        _logger.LogInformation("Custom billing: Generated invoice for inward {InwardId} (customer {CustomerId})", inwardId, customerId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Custom billing: Failed to generate invoice for inward {InwardId}", inwardId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Custom billing: Failed to process billing for customer {CustomerId}", customerId);
            }
        }
    }
}
