using LIMSApi.Data;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    /// <summary>
    /// Runs daily — notifies Accounts team about POs expiring within 7 days.
    /// </summary>
    public class POExpiryJob
    {
        private readonly LIMSContext _db;
        private readonly INotificationService _notificationService;
        private readonly ILogger<POExpiryJob> _logger;

        public POExpiryJob(LIMSContext db, INotificationService notificationService, ILogger<POExpiryJob> logger)
        {
            _db = db;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("PO expiry check started at {Time}", DateTime.UtcNow);

            try
            {
                var sevenDaysFromNow = DateTime.UtcNow.AddDays(7);

                var expiringPOs = await _db.CustomerPurchaseOrders
                    .Include(p => p.Customer)
                    .Where(p => p.IsActive
                        && p.Status == "Active"
                        && p.ValidUntil.HasValue
                        && p.ValidUntil.Value <= sevenDaysFromNow
                        && p.ValidUntil.Value >= DateTime.UtcNow)
                    .ToListAsync();

                foreach (var po in expiringPOs)
                {
                    try
                    {
                        var daysUntilExpiry = (po.ValidUntil!.Value - DateTime.UtcNow).Days;
                        await _notificationService.NotifyByRoleAsync("Accounts",
                            "PO Expiring Soon",
                            $"PO {po.PONumber} for {po.Customer?.Name} expires in {daysUntilExpiry} day(s). Remaining: {po.RemainingAmount:N2}",
                            "CustomerPurchaseOrder", po.ID);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send expiry notification for PO {PONumber}", po.PONumber);
                    }
                }

                _logger.LogInformation("PO expiry check completed. {Count} expiring POs notified.", expiringPOs.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PO expiry job failed.");
            }
        }
    }
}
