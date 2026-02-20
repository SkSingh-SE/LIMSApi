
using Hangfire;
using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using LIMSApi.ServiceWORepo;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Jobs
{
    public class ReminderJob
    {
        private readonly LIMSContext _dbContext;
        private readonly ILogger<ReminderJob> _logger;
        private readonly EmailService _emailService;
        private readonly TemplateService _templateService;

        public ReminderJob(LIMSContext context, ILogger<ReminderJob> logger, EmailService emailService,TemplateService templateService)
        {
            _dbContext = context;
            _logger = logger;
            _emailService = emailService;
            _templateService = templateService;
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

                // Send 12-hour reminders for cases with missing information (INWARD_REGISTERED status)
                var cutoffTime = DateTime.UtcNow.AddHours(-12);

var casesNeedingReminder = await _dbContext.SampleInwards
    .Include(i => i.Customer)
    .Include(i => i.Contacts)
    .Where(i => i.IsActive &&
                i.InwardStatus == InwardStatus.INWARD_REGISTERED.ToString() &&
                i.ModifiedOn.HasValue &&
                i.ModifiedOn.Value <= cutoffTime)
    .ToListAsync();

                int reminderCount = 0;
                foreach (var inward in casesNeedingReminder)
                {
                    var modal = new
                    {
                        CaseNo = inward.CaseNo,
                        CustomerName = inward.Customer?.Name
                    };
                    var contact = inward.Contacts?.FirstOrDefault(c => !string.IsNullOrEmpty(c.EmailId));
                    if (contact != null)
                    {
                       var emailBody = await _templateService.GetTemplateAsync(MessageTemplateKey.SAMPLE_INWARD_INCOMPLETE_REMINDER, NotificationType.Email, modal);
                        
                        await _emailService.SendEmailAsync(
                            contact.EmailId,
                            $"Missing Information Reminder - Case {inward.CaseNo}",
                            emailBody);
                        
                        reminderCount++;
                        _logger.LogInformation("Sent reminder email for Case {CaseNo}", inward.CaseNo);
                    }
                }

                // Also handle monthly billing customers (existing logic)
                var customersToRemind = await _dbContext.Customers
                    .Where(c => c.MonthlyBillingCustomer && c.IsActive).ToListAsync();

                foreach (var customer in customersToRemind)
                {
                    // Send reminder (email/SMS logic here)
                    // For example: await _notificationService.SendEmailAsync(customer.Email);
                    Console.WriteLine("Send Reminder");
                }

                await _dbContext.SaveChangesAsync();

                log.Status = "Success";
                _logger.LogInformation("ReminderJob completed successfully. Sent {Count} reminders at {time}", reminderCount, DateTime.Now);
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
