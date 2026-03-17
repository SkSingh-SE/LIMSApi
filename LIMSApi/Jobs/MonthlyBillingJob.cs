using LIMSApi.Data;
using LIMSApi.ServiceWORepo;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    public class MonthlyBillingJob
    {
        private readonly LIMSContext _db;
        private readonly IAccountService _accountService;
        private readonly ILogger<MonthlyBillingJob> _logger;

        public MonthlyBillingJob(LIMSContext db, IAccountService accountService, ILogger<MonthlyBillingJob> logger)
        {
            _db = db;
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// Runs on 1st of every month — generates consolidated invoices for monthly billing customers.
        /// </summary>
        public async Task ExecuteMonthly()
        {
            _logger.LogInformation("Monthly billing job started at {Time}", DateTime.UtcNow);

            try
            {
                var monthlyCustomers = await _db.Customers
                    .Where(c => c.IsActive && c.MonthlyBillingCustomer)
                    .Select(c => c.ID)
                    .ToListAsync();

                foreach (var customerId in monthlyCustomers)
                {
                    await ProcessBillingForCustomer(customerId);
                }

                _logger.LogInformation("Monthly billing job completed. Processed {Count} customers.", monthlyCustomers.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Monthly billing job failed.");
            }
        }

        /// <summary>
        /// Runs every Monday — generates consolidated invoices for weekly billing customers.
        /// </summary>
        public async Task ExecuteWeekly()
        {
            _logger.LogInformation("Weekly billing job started at {Time}", DateTime.UtcNow);

            try
            {
                var weeklyCustomers = await _db.Customers
                    .Where(c => c.IsActive && c.WeeklyBillingCustomer)
                    .Select(c => c.ID)
                    .ToListAsync();

                foreach (var customerId in weeklyCustomers)
                {
                    await ProcessBillingForCustomer(customerId);
                }

                _logger.LogInformation("Weekly billing job completed. Processed {Count} customers.", weeklyCustomers.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Weekly billing job failed.");
            }
        }

        private async Task ProcessBillingForCustomer(long customerId)
        {
            try
            {
                // Find all inwards for this customer with PRICE_SNAPSHOT status that don't have an invoice yet
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
                        _logger.LogInformation("Generated invoice for inward {InwardId} (customer {CustomerId})", inwardId, customerId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to generate invoice for inward {InwardId}", inwardId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process billing for customer {CustomerId}", customerId);
            }
        }
    }
}
