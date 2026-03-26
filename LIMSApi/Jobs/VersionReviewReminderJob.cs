using Hangfire;
using LIMSApi.Data;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    public class VersionReviewReminderJob
    {
        private readonly LIMSContext _dbContext;
        private readonly ILogger<VersionReviewReminderJob> _logger;

        public VersionReviewReminderJob(LIMSContext context, ILogger<VersionReviewReminderJob> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task Execute()
        {
            var log = new JobExecutionLog
            {
                JobName = nameof(VersionReviewReminderJob),
                StartTime = DateTime.Now
            };

            try
            {
                _logger.LogInformation("VersionReviewReminderJob started at {time}", DateTime.Now);

                var cutoffDate = DateTime.UtcNow.AddDays(30);

                // Query versions with their parent spec's CompanyCode for multi-tenancy
                var versionsDue = await _dbContext.TestMethodSpecificationVersions
                    .Include(v => v.TestMethodSpecification)
                    .Where(v => v.Status == VersionStatus.Active
                        && v.TestMethodSpecification != null
                        && v.TestMethodSpecification.IsActive
                        && v.ReviewDate != null
                        && v.ReviewDate <= cutoffDate
                        && v.ReviewDate >= DateTime.UtcNow)
                    .ToListAsync();

                if (versionsDue.Any())
                {
                    _logger.LogInformation("Found {Count} versions due for review.", versionsDue.Count);

                    // Group versions by CompanyCode for multi-tenancy
                    var versionsByCompany = versionsDue.GroupBy(v => v.TestMethodSpecification!.CompanyCode);

                    int totalNotifications = 0;

                    foreach (var companyGroup in versionsByCompany)
                    {
                        var companyCode = companyGroup.Key;

                        // Get managers for this company only
                        var labManagers = await _dbContext.UserMasters
                            .Include(u => u.Role)
                            .Where(u => u.IsActive
                                && u.CompanyCode == companyCode
                                && (u.Role.Name == "Admin" || u.Role.Name == "LabManager"))
                            .ToListAsync();

                        foreach (var version in companyGroup)
                        {
                            var specName = version.TestMethodSpecification?.Name ?? "Unknown";
                            var message = $"Test Method Specification '{specName}' version '{version.Version}' is due for review on {version.ReviewDate:dd MMM yyyy}.";

                            foreach (var user in labManagers)
                            {
                                _dbContext.Notifications.Add(new Notification
                                {
                                    UserID = user.ID,
                                    Title = "Version Review Due",
                                    Message = message,
                                    Type = NotificationType.System,
                                    IsRead = false,
                                    CreatedOn = DateTime.UtcNow,
                                    CompanyCode = companyCode
                                });
                                totalNotifications++;
                            }
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    log.Status = "Completed";
                    log.ErrorMessage = $"Sent {totalNotifications} review reminder(s) for {versionsDue.Count} version(s).";
                }
                else
                {
                    log.Status = "Completed";
                    log.ErrorMessage = "No versions due for review.";
                }

                _logger.LogInformation("VersionReviewReminderJob completed at {time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VersionReviewReminderJob failed.");
                log.Status = "Failed";
                log.ErrorMessage = ex.Message;
            }
            finally
            {
                log.EndTime = DateTime.Now;
                _dbContext.JobExecutionLogs.Add(log);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
