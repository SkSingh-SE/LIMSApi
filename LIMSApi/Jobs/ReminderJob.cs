
using Hangfire;
using LIMSApi.Data;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    public class ReminderJob
    {
        private readonly LIMSContext _dbContext;
        private readonly ILogger<ReminderJob> _logger;
        public ReminderJob(LIMSContext context, ILogger<ReminderJob> logger)
        {
            _dbContext = context;
            _logger = logger;
        }
        [AutomaticRetry(Attempts = 3)] // Hangfire retry policy
        public async Task Execute()
        {
            var log = new JobExecutionLog
            {
                JobName = nameof(ReminderJob),
                StartTime = DateTime.Now
            };

            try
            {
                _logger.LogInformation("ReminderJob started at {time}", DateTime.Now);

                // Example: fetch customers needing reminders
                var customersToRemind = await _dbContext.Customers
                    .Where(c => c.MonthlyBillingCustomer && c.IsActive).ToListAsync();

                foreach (var customer in customersToRemind)
                {
                    // Send reminder (email/SMS logic here)
                    // For example: await _notificationService.SendEmailAsync(customer.Email);

                    // Mark reminder sent
                    Console.WriteLine("Send Reminder");
                }

                await _dbContext.SaveChangesAsync();

                log.Status = "Success";
                _logger.LogInformation("ReminderJob completed successfully at {time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                log.Status = "Failed";
                log.ErrorMessage = ex.Message;
                _logger.LogError(ex, "ReminderJob failed at {time}", DateTime.Now);
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
