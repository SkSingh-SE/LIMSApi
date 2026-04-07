using LIMSApi.Data;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    /// <summary>
    /// Runs daily — finds invoices overdue beyond customer's CreditLimitTime and notifies Accounts team.
    /// </summary>
    public class PaymentOverdueJob
    {
        private readonly LIMSContext _db;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PaymentOverdueJob> _logger;

        public PaymentOverdueJob(LIMSContext db, INotificationService notificationService, ILogger<PaymentOverdueJob> logger)
        {
            _db = db;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Payment overdue check started at {Time}", DateTime.UtcNow);

            try
            {
                // Find invoices that are overdue based on customer's credit limit time
                var overdueInvoices = await _db.TaxInvoices
                    .Include(i => i.Customer)
                    .Include(i => i.Inward)
                    .Where(i => i.IsActive
                        && i.Status != "Paid"
                        && i.Customer != null
                        && i.Customer.CreditLimitTime.HasValue
                        && i.Customer.CreditLimitTime.Value > 0)
                    .ToListAsync();

                var notifiedCount = 0;

                foreach (var invoice in overdueInvoices)
                {
                    var daysSinceInvoice = (DateTime.UtcNow - invoice.InvoiceDate).Days;
                    var creditDays = invoice.Customer!.CreditLimitTime!.Value;

                    if (daysSinceInvoice > creditDays)
                    {
                        try
                        {
                            var daysOverdue = daysSinceInvoice - creditDays;
                            await _notificationService.NotifyByRoleAsync("Accounts",
                                "Payment Overdue",
                                $"Invoice {invoice.InvoiceNo} for {invoice.Customer.Name} is {daysOverdue} days overdue. Amount: {invoice.GrandTotal:N2}",
                                "TaxInvoice", invoice.ID);
                            notifiedCount++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to send overdue notification for invoice {InvoiceNo}", invoice.InvoiceNo);
                        }
                    }
                }

                _logger.LogInformation("Payment overdue check completed. {Count} overdue invoices notified.", notifiedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment overdue job failed.");
            }
        }
    }
}
