using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Helpers;
using LIMSApi.Helpers.Enums;
using LIMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    public class DashboardService : IDashboardService
    {
        private readonly LIMSContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(LIMSContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardResponseDto> GetDashboardAsync(LoggedInUserDTO userContext)
        {
            try
            {
                // Validate user context first
                ValidateUserContext(userContext);

                _logger.LogInformation("Getting dashboard data for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var cards = await GetDashboardCardsAsync(userContext);
                var charts = await GetDashboardChartsAsync(userContext);
                var notifications = await GetDashboardNotificationsAsync(userContext);

                return new DashboardResponseDto
                {
                    Cards = cards,
                    Charts = charts,
                    Notifications = notifications,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user context for dashboard request: {UserId}", userContext?.UserId);
                throw new ArgumentException("Invalid user context provided", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized dashboard access attempt by user {UserId}", userContext?.UserId);
                throw;
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                _logger.LogError(ex, "Database connectivity error while getting dashboard data for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Database connectivity issue. Please try again later.", ex);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while getting dashboard data for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Database operation failed. Please try again later.", ex);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Database timeout while getting dashboard data for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Request timed out. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting dashboard data for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("An unexpected error occurred while retrieving dashboard data.", ex);
            }
        }

        public async Task<List<DashboardCardDto>> GetDashboardCardsAsync(LoggedInUserDTO userContext)
        {
            try
            {
                // Validate user context first
                ValidateUserContext(userContext);

                _logger.LogInformation("Getting dashboard cards for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var cards = new List<DashboardCardDto>();

                // Add operational cards for all roles
                cards.AddRange(await GetOperationalCardsAsync(userContext));

                // Add billing cards only for Admin and Accounts roles
                if (CanAccessBillingData(userContext.Role))
                {
                    cards.AddRange(await GetBillingCardsAsync(userContext));
                }

                // Ensure billing data is excluded for Normal users (double-check security)
                cards = EnsureBillingDataExclusion(cards, userContext.Role);

                _logger.LogInformation("Returning {CardCount} dashboard cards for user {UserId} with role {Role}", 
                    cards.Count, userContext.UserId, userContext.Role);

                return cards;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user context for dashboard cards request: {UserId}", userContext?.UserId);
                throw new ArgumentException("Invalid user context provided", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized dashboard cards access attempt by user {UserId}", userContext?.UserId);
                throw;
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                _logger.LogError(ex, "Database connectivity error while getting dashboard cards for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Database connectivity issue. Please try again later.", ex);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while getting dashboard cards for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Database operation failed. Please try again later.", ex);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Database timeout while getting dashboard cards for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Request timed out. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting dashboard cards for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("An unexpected error occurred while retrieving dashboard cards.", ex);
            }
        }

        public async Task<List<DashboardChartDto>> GetDashboardChartsAsync(LoggedInUserDTO userContext)
        {
            try
            {
                // Validate user context first
                ValidateUserContext(userContext);

                _logger.LogInformation("Getting dashboard charts for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var charts = new List<DashboardChartDto>();

                // Add operational charts for all roles
                charts.AddRange(await GetOperationalChartsAsync(userContext));

                // Add billing charts only for Admin and Accounts roles
                if (CanAccessBillingData(userContext.Role))
                {
                    charts.AddRange(await GetBillingChartsAsync(userContext));
                }

                // Ensure billing data is excluded for Normal users (double-check security)
                charts = EnsureBillingChartsExclusion(charts, userContext.Role);

                _logger.LogInformation("Returning {ChartCount} dashboard charts for user {UserId} with role {Role}", 
                    charts.Count, userContext.UserId, userContext.Role);

                return charts;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user context for dashboard charts request: {UserId}", userContext?.UserId);
                throw new ArgumentException("Invalid user context provided", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized dashboard charts access attempt by user {UserId}", userContext?.UserId);
                throw;
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                _logger.LogError(ex, "Database connectivity error while getting dashboard charts for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Database connectivity issue. Please try again later.", ex);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while getting dashboard charts for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Database operation failed. Please try again later.", ex);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Database timeout while getting dashboard charts for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Request timed out. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting dashboard charts for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("An unexpected error occurred while retrieving dashboard charts.", ex);
            }
        }

        public async Task<List<DashboardNotificationDto>> GetDashboardNotificationsAsync(LoggedInUserDTO userContext)
        {
            try
            {
                // Validate user context first
                ValidateUserContext(userContext);

                _logger.LogInformation("Getting dashboard notifications for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var notifications = new List<DashboardNotificationDto>();

                // Add system notifications based on role
                notifications.AddRange(await GetSystemNotificationsAsync(userContext));

                return notifications;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user context for dashboard notifications request: {UserId}", userContext?.UserId);
                throw new ArgumentException("Invalid user context provided", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized dashboard notifications access attempt by user {UserId}", userContext?.UserId);
                throw;
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                _logger.LogError(ex, "Database connectivity error while getting dashboard notifications for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Database connectivity issue. Please try again later.", ex);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while getting dashboard notifications for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Database operation failed. Please try again later.", ex);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Database timeout while getting dashboard notifications for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("Request timed out. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting dashboard notifications for user {UserId}", userContext?.UserId);
                throw new InvalidOperationException("An unexpected error occurred while retrieving dashboard notifications.", ex);
            }
        }

        private async Task<int> GetPendingSampleInwardCountAsync()
        {
            try
            {
                var count = await _context.SampleInwards
                    .Where(s => s.InwardStatus == "Sample Received")
                    .CountAsync();
                
                _logger.LogDebug("Found {Count} pending sample inward records", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending sample inward count");
                return 0;
            }
        }

        private async Task<int> GetTodaysSamplesCountAsync()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                
                var count = await _context.SampleInwards
                    .Where(s => s.CollectionTime >= today && s.CollectionTime < tomorrow)
                    .CountAsync();
                
                _logger.LogDebug("Found {Count} samples collected today", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting today's samples count");
                return 0;
            }
        }

        private async Task<int> GetOverdueSamplesCountAsync()
        {
            try
            {
                var today = DateTime.Today;
                
                // Consider samples overdue if they are not completed and were collected more than 7 days ago
                // This is a business rule that may need adjustment based on actual requirements
                var overdueDate = today.AddDays(-7);
                
                var count = await _context.SampleDetails
                    .Include(sd => sd.SampleInward)
                    .Where(sd => sd.SampleStatus != "Completed" && 
                                sd.IsTestingCompleted == false &&
                                sd.SampleInward!.CollectionTime < overdueDate)
                    .CountAsync();
                
                _logger.LogDebug("Found {Count} overdue samples", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue samples count");
                return 0;
            }
        }

        private async Task<int> GetPendingPlanApprovalCountAsync()
        {
            try
            {
                // Count samples that have test plans but are not yet approved
                // This assumes test plans need approval before testing can begin
                var count = await _context.TestPlans
                    .Include(stp => stp.SampleDetail)
                    .Where(stp => stp.SampleDetail!.SampleStatus == "Plan Pending" || 
                                 stp.SampleDetail!.SampleStatus == "Pending Approval")
                    .CountAsync();
                
                _logger.LogDebug("Found {Count} samples with pending plan approval", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending plan approval count");
                return 0;
            }
        }

        private async Task<int> GetSamplesUnderTestingCountAsync()
        {
            try
            {
                var count = await _context.TestResultHeaders
                    .Where(trh => trh.Status == "In Progress" || trh.Status == "Started")
                    .Select(trh => trh.SampleID)
                    .Distinct()
                    .CountAsync();
                
                _logger.LogDebug("Found {Count} samples under testing", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting samples under testing count");
                return 0;
            }
        }

        private async Task<int> GetResultsPendingReviewCountAsync()
        {
            try
            {
                var count = await _context.TestResultHeaders
                    .Where(trh => trh.Status == "Completed" && trh.CompletedAt != null)
                    .Select(trh => trh.SampleID)
                    .Distinct()
                    .CountAsync();
                
                _logger.LogDebug("Found {Count} samples with results pending review", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting results pending review count");
                return 0;
            }
        }

        private async Task<int> GetReportsPendingDispatchCountAsync()
        {
            try
            {
                var count = await _context.ReportHeaders
                    .Where(rh => rh.Status == "Approved" || rh.Status == "Final")
                    .Include(rh => rh.Reports)
                    .Where(rh => !rh.Reports.Any(r => r.Status == "Dispatched"))
                    .CountAsync();
                
                _logger.LogDebug("Found {Count} reports pending dispatch", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reports pending dispatch count");
                return 0;
            }
        }

        private async Task<int> GetStandardsDueForReviewCountAsync()
        {
            try
            {
                var cutoff = DateTime.UtcNow.AddDays(30);
                var count = await _context.TestMethodSpecificationVersions
                    .Where(v => v.Status == VersionStatus.Active
                        && v.ReviewDate != null
                        && v.ReviewDate <= cutoff
                        && v.ReviewDate >= DateTime.UtcNow)
                    .CountAsync();

                _logger.LogDebug("Found {Count} standards due for review", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting standards due for review count");
                return 0;
            }
        }

        private async Task<UrgentSampleInfo> GetUrgentSampleInfoAsync()
        {
            try
            {
                var urgentInfo = new UrgentSampleInfo();
                
                // Check for urgent samples in pending inward
                urgentInfo.HasUrgentPendingInward = await _context.SampleInwards
                    .AnyAsync(s => s.InwardStatus == "Sample Received" && s.Urgent);
                
                // Check for urgent samples collected today
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                urgentInfo.HasUrgentTodaysSamples = await _context.SampleInwards
                    .AnyAsync(s => s.CollectionTime >= today && s.CollectionTime < tomorrow && s.Urgent);
                
                // Check for urgent samples with pending plan approval
                urgentInfo.HasUrgentPendingApproval = await _context.TestPlans
                    .Include(stp => stp.SampleDetail)
                    .ThenInclude(sd => sd!.SampleInward)
                    .AnyAsync(stp => (stp.SampleDetail!.SampleStatus == "Plan Pending" || 
                                     stp.SampleDetail!.SampleStatus == "Pending Approval") &&
                                    stp.SampleDetail!.SampleInward!.Urgent);
                
                // Check for urgent samples under testing
                urgentInfo.HasUrgentUnderTesting = await _context.TestResultHeaders
                    .Include(trh => trh.Sample)
                    .ThenInclude(s => s!.SampleInward)
                    .AnyAsync(trh => (trh.Status == "In Progress" || trh.Status == "Started") &&
                                    trh.Sample!.SampleInward!.Urgent);
                
                // Check for urgent samples with results pending review
                urgentInfo.HasUrgentPendingReview = await _context.TestResultHeaders
                    .Include(trh => trh.Sample)
                    .ThenInclude(s => s!.SampleInward)
                    .AnyAsync(trh => trh.Status == "Completed" && trh.CompletedAt != null &&
                                    trh.Sample!.SampleInward!.Urgent);
                
                // Check for urgent reports pending dispatch
                urgentInfo.HasUrgentPendingDispatch = await _context.ReportHeaders
                    .Include(rh => rh.Sample)
                    .ThenInclude(s => s!.SampleInward)
                    .Include(rh => rh.Reports)
                    .AnyAsync(rh => (rh.Status == "Approved" || rh.Status == "Final") &&
                                   !rh.Reports.Any(r => r.Status == "Dispatched") &&
                                   rh.Sample!.SampleInward!.Urgent);
                
                _logger.LogDebug("Urgent sample info: PendingInward={HasUrgentPendingInward}, " +
                               "TodaysSamples={HasUrgentTodaysSamples}, PendingApproval={HasUrgentPendingApproval}, " +
                               "UnderTesting={HasUrgentUnderTesting}, PendingReview={HasUrgentPendingReview}, " +
                               "PendingDispatch={HasUrgentPendingDispatch}",
                               urgentInfo.HasUrgentPendingInward, urgentInfo.HasUrgentTodaysSamples,
                               urgentInfo.HasUrgentPendingApproval, urgentInfo.HasUrgentUnderTesting,
                               urgentInfo.HasUrgentPendingReview, urgentInfo.HasUrgentPendingDispatch);
                
                return urgentInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting urgent sample information");
                return new UrgentSampleInfo();
            }
        }

        private DashboardCardDto CreateOperationalCardWithData(string key, string title, int count, 
            bool hasUrgent, string[] allowedRoles)
        {
            var status = hasUrgent ? "Critical" : (count > 0 ? "Normal" : "Normal");
            
            return new DashboardCardDto
            {
                Key = key,
                Title = title,
                Count = count,
                Status = status,
                AllowedRoles = allowedRoles.ToList(),
                Description = $"Operational metric: {title}",
                Metadata = new Dictionary<string, object>
                {
                    { "CardType", "Operational" },
                    { "LastUpdated", DateTime.UtcNow },
                    { "HasUrgent", hasUrgent },
                    { "UrgentIndicator", hasUrgent ? "⚠️" : "" }
                }
            };
        }

        private class UrgentSampleInfo
        {
            public bool HasUrgentPendingInward { get; set; }
            public bool HasUrgentTodaysSamples { get; set; }
            public bool HasUrgentPendingApproval { get; set; }
            public bool HasUrgentUnderTesting { get; set; }
            public bool HasUrgentPendingReview { get; set; }
            public bool HasUrgentPendingDispatch { get; set; }
        }

        private void ValidateUserContext(LoggedInUserDTO userContext)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException(nameof(userContext), "User context cannot be null");
            }

            if (userContext.UserId <= 0)
            {
                throw new ArgumentException("Invalid user ID", nameof(userContext));
            }

            if (string.IsNullOrWhiteSpace(userContext.Role))
            {
                throw new ArgumentException("User role cannot be null or empty", nameof(userContext));
            }

            if (string.IsNullOrWhiteSpace(userContext.Name))
            {
                throw new ArgumentException("User name cannot be null or empty", nameof(userContext));
            }

            if (string.IsNullOrWhiteSpace(userContext.Email))
            {
                throw new ArgumentException("User email cannot be null or empty", nameof(userContext));
            }

            // Validate role is a recognized role
            //if (!IsValidRole(userContext.Role))
            //{
            //    throw new ArgumentException($"Invalid user role: {userContext.Role}", nameof(userContext));
            //}

            // Additional validation for email format
            if (!IsValidEmail(userContext.Email))
            {
                throw new ArgumentException("Invalid email format", nameof(userContext));
            }

            // Validate user name length
            if (userContext.Name.Length > 100)
            {
                throw new ArgumentException("User name exceeds maximum length", nameof(userContext));
            }

            _logger.LogDebug("User context validated for user {UserId} with role {Role}", 
                userContext.UserId, userContext.Role);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            // Check against known roles in the system
            var validRoles = new[]
            {
                "Admin", "SystemAdmin", "Accounts", 
                "FrontDesk", "Technical", "Lab"
            };

            return validRoles.Any(validRole => 
                role.Equals(validRole, StringComparison.OrdinalIgnoreCase) ||
                role.Contains(validRole, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsAdminRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            return role.Contains("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAccountsRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            return role.Equals("Accounts", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsNormalUserRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            var normalUserRoles = new[] { "FrontDesk", "Technical", "Lab" };
            return normalUserRoles.Any(normalRole => 
                role.Equals(normalRole, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsAdminOrAccountsRole(string? role)
        {
            return IsAdminRole(role) || IsAccountsRole(role);
        }

        private bool CanAccessBillingData(string? role)
        {
            return IsAdminRole(role) || IsAccountsRole(role);
        }

        private async Task<List<DashboardCardDto>> GetOperationalCardsAsync(LoggedInUserDTO userContext)
        {
            _logger.LogDebug("Getting operational cards for user {UserId}", userContext.UserId);
            
            var cards = new List<DashboardCardDto>();

            // Get actual counts from database
            var pendingSampleInwardCount = await GetPendingSampleInwardCountAsync();
            var todaysSamplesCount = await GetTodaysSamplesCountAsync();
            var overdueSamplesCount = await GetOverdueSamplesCountAsync();
            var pendingPlanApprovalCount = await GetPendingPlanApprovalCountAsync();
            var samplesUnderTestingCount = await GetSamplesUnderTestingCountAsync();
            var resultsPendingReviewCount = await GetResultsPendingReviewCountAsync();
            var reportsPendingDispatchCount = await GetReportsPendingDispatchCountAsync();
            var standardsDueForReviewCount = await GetStandardsDueForReviewCountAsync();

            // Check for urgent samples
            var urgentSampleInfo = await GetUrgentSampleInfoAsync();

            // Create operational cards with actual data
            var operationalCards = new List<DashboardCardDto>
            {
                CreateOperationalCardWithData("pending-sample-inward", "Pending Sample Inward", 
                    pendingSampleInwardCount, urgentSampleInfo.HasUrgentPendingInward,
                    new[] { "Admin", "FrontDesk", "Technical", "Lab" }),
                CreateOperationalCardWithData("todays-samples", "Today's Samples", 
                    todaysSamplesCount, urgentSampleInfo.HasUrgentTodaysSamples,
                    new[] { "Admin", "FrontDesk", "Technical", "Lab" }),
                CreateOperationalCardWithData("overdue-samples", "Overdue Samples", 
                    overdueSamplesCount, overdueSamplesCount > 0,
                    new[] { "Admin", "Technical", "Lab" }),
                CreateOperationalCardWithData("pending-plan-approval", "Pending Plan Approval", 
                    pendingPlanApprovalCount, urgentSampleInfo.HasUrgentPendingApproval,
                    new[] { "Admin", "Technical" }),
                CreateOperationalCardWithData("samples-under-testing", "Samples Under Testing", 
                    samplesUnderTestingCount, urgentSampleInfo.HasUrgentUnderTesting,
                    new[] { "Admin", "Technical", "Lab" }),
                CreateOperationalCardWithData("results-pending-review", "Results Pending Review", 
                    resultsPendingReviewCount, urgentSampleInfo.HasUrgentPendingReview,
                    new[] { "Admin", "Technical" }),
                CreateOperationalCardWithData("reports-pending-dispatch", "Reports Pending Dispatch",
                    reportsPendingDispatchCount, urgentSampleInfo.HasUrgentPendingDispatch,
                    new[] { "Admin", "FrontDesk" }),
                CreateOperationalCardWithData("standards-due-review", "Standards Due for Review",
                    standardsDueForReviewCount, standardsDueForReviewCount > 0,
                    new[] { "Admin", "Technical", "Lab" })
            };

            // Filter cards based on user role
            cards.AddRange(FilterCardsByRole(operationalCards, userContext.Role));

            return cards;
        }

        private async Task<List<DashboardCardDto>> GetBillingCardsAsync(LoggedInUserDTO userContext)
        {
            _logger.LogDebug("Getting billing cards for user {UserId}", userContext.UserId);
            
            var cards = new List<DashboardCardDto>();

            // Only Admin and Accounts roles can see billing cards
            if (!CanAccessBillingData(userContext.Role))
            {
                _logger.LogWarning("User {UserId} with role {Role} attempted to access billing cards", 
                    userContext.UserId, userContext.Role);
                return cards;
            }

            // Get actual billing data from database
            var pendingInvoicesData = await GetPendingInvoicesDataAsync();
            var paidInvoicesData = await GetPaidInvoicesDataAsync();
            var paymentSummaryData = await GetPaymentSummaryDataAsync();

            var billingCards = new List<DashboardCardDto>
            {
                CreateBillingCardWithData("pending-invoices", "Pending Invoices", 
                    pendingInvoicesData.Count, pendingInvoicesData.TotalAmount,
                    new[] { "Admin", "Accounts" }),
                CreateBillingCardWithData("paid-invoices", "Paid Invoices", 
                    paidInvoicesData.Count, paidInvoicesData.TotalAmount,
                    new[] { "Admin", "Accounts" }),
                CreateBillingCardWithData("payment-summary", "Payment Summary", 
                    paymentSummaryData.Count, paymentSummaryData.TotalAmount,
                    new[] { "Admin", "Accounts" })
            };

            // Filter cards based on user role
            cards.AddRange(FilterCardsByRole(billingCards, userContext.Role));

            return cards;
        }

        private DashboardCardDto CreateOperationalCard(string key, string title, int count, string[] allowedRoles)
        {
            return new DashboardCardDto
            {
                Key = key,
                Title = title,
                Count = count,
                Status = "Normal",
                AllowedRoles = allowedRoles.ToList(),
                Description = $"Operational metric: {title}",
                Metadata = new Dictionary<string, object>
                {
                    { "CardType", "Operational" },
                    { "LastUpdated", DateTime.UtcNow }
                }
            };
        }

        private DashboardCardDto CreateBillingCard(string key, string title, int count, string[] allowedRoles)
        {
            return new DashboardCardDto
            {
                Key = key,
                Title = title,
                Count = count,
                Status = "Normal",
                AllowedRoles = allowedRoles.ToList(),
                Description = $"Billing metric: {title}",
                Metadata = new Dictionary<string, object>
                {
                    { "CardType", "Billing" },
                    { "LastUpdated", DateTime.UtcNow },
                    { "RequiresBillingAccess", true }
                }
            };
        }

        private DashboardCardDto CreateBillingCardWithData(string key, string title, int count, 
            decimal totalAmount, string[] allowedRoles)
        {
            return new DashboardCardDto
            {
                Key = key,
                Title = title,
                Count = count,
                Status = "Normal",
                AllowedRoles = allowedRoles.ToList(),
                Description = $"Billing metric: {title}",
                Metadata = new Dictionary<string, object>
                {
                    { "CardType", "Billing" },
                    { "LastUpdated", DateTime.UtcNow },
                    { "RequiresBillingAccess", true },
                    { "TotalAmount", totalAmount },
                    { "Currency", "INR" }
                }
            };
        }

        private async Task<BillingCardData> GetPendingInvoicesDataAsync()
        {
            try
            {
                var pendingInvoices = await _context.TaxInvoices
                    .Where(ti => ti.Status == "Generated")
                    .Select(ti => new { ti.ID, ti.GrandTotal })
                    .ToListAsync();

                var count = pendingInvoices.Count;
                var totalAmount = pendingInvoices.Sum(pi => pi.GrandTotal);

                _logger.LogDebug("Found {Count} pending invoices with total amount {TotalAmount}", 
                    count, totalAmount);

                return new BillingCardData { Count = count, TotalAmount = totalAmount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending invoices data");
                return new BillingCardData { Count = 0, TotalAmount = 0 };
            }
        }

        private async Task<BillingCardData> GetPaidInvoicesDataAsync()
        {
            try
            {
                var paidPayments = await _context.PaymentOrders
                    .Where(po => po.Status == PaymentStatus.Paid)
                    .Select(po => new { po.ID, po.Amount })
                    .ToListAsync();

                var count = paidPayments.Count;
                var totalAmount = paidPayments.Sum(pp => pp.Amount);

                _logger.LogDebug("Found {Count} paid invoices with total amount {TotalAmount}", 
                    count, totalAmount);

                return new BillingCardData { Count = count, TotalAmount = totalAmount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paid invoices data");
                return new BillingCardData { Count = 0, TotalAmount = 0 };
            }
        }

        private async Task<BillingCardData> GetPaymentSummaryDataAsync()
        {
            try
            {
                // Payment summary includes all payment orders regardless of status
                var allPayments = await _context.PaymentOrders
                    .Select(po => new { po.ID, po.Amount, po.Status })
                    .ToListAsync();

                var count = allPayments.Count;
                var totalAmount = allPayments.Sum(ap => ap.Amount);

                _logger.LogDebug("Found {Count} total payment orders with total amount {TotalAmount}", 
                    count, totalAmount);

                return new BillingCardData { Count = count, TotalAmount = totalAmount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment summary data");
                return new BillingCardData { Count = 0, TotalAmount = 0 };
            }
        }

        private class BillingCardData
        {
            public int Count { get; set; }
            public decimal TotalAmount { get; set; }
        }

        private List<DashboardCardDto> FilterCardsByRole(List<DashboardCardDto> cards, string? userRole)
        {
            if (string.IsNullOrWhiteSpace(userRole))
            {
                _logger.LogWarning("Cannot filter cards: user role is null or empty");
                return new List<DashboardCardDto>();
            }

            var filteredCards = cards.Where(card => IsCardAllowedForRole(card, userRole)).ToList();
            
            _logger.LogDebug("Filtered {TotalCards} cards to {FilteredCards} for role {Role}", 
                cards.Count, filteredCards.Count, userRole);

            return filteredCards;
        }

        private bool IsCardAllowedForRole(DashboardCardDto card, string userRole)
        {
            if (card.AllowedRoles == null || !card.AllowedRoles.Any())
            {
                // If no roles specified, allow all authenticated users
                return true;
            }

            // Check if user role matches any allowed role (case-insensitive)
            return card.AllowedRoles.Any(allowedRole => 
                userRole.Equals(allowedRole, StringComparison.OrdinalIgnoreCase) ||
                userRole.Contains(allowedRole, StringComparison.OrdinalIgnoreCase));
        }

        private List<DashboardCardDto> EnsureBillingDataExclusion(List<DashboardCardDto> cards, string? userRole)
        {
            // For Normal users, ensure no billing data is included
            if (IsNormalUserRole(userRole))
            {
                var filteredCards = cards.Where(card => 
                {
                    // Check if card has billing metadata
                    if (card.Metadata?.ContainsKey("RequiresBillingAccess") == true)
                    {
                        return false;
                    }

                    // Check if card type is billing
                    if (card.Metadata?.ContainsKey("CardType") == true && 
                        card.Metadata["CardType"].ToString() == "Billing")
                    {
                        return false;
                    }

                    return true;
                }).ToList();

                if (filteredCards.Count != cards.Count)
                {
                    _logger.LogInformation("Excluded {ExcludedCount} billing cards for Normal user role {Role}", 
                        cards.Count - filteredCards.Count, userRole);
                }

                return filteredCards;
            }

            return cards;
        }

        private List<DashboardChartDto> EnsureBillingChartsExclusion(List<DashboardChartDto> charts, string? userRole)
        {
            // For Normal users, ensure no billing charts are included
            if (IsNormalUserRole(userRole))
            {
                var filteredCharts = charts.Where(chart => 
                {
                    // Check if chart has billing metadata
                    if (chart.Options?.ContainsKey("RequiresBillingAccess") == true)
                    {
                        return false;
                    }

                    // Check if chart type is billing
                    if (chart.Options?.ContainsKey("ChartType") == true && 
                        chart.Options["ChartType"].ToString() == "Billing")
                    {
                        return false;
                    }

                    return true;
                }).ToList();

                if (filteredCharts.Count != charts.Count)
                {
                    _logger.LogInformation("Excluded {ExcludedCount} billing charts for Normal user role {Role}", 
                        charts.Count - filteredCharts.Count, userRole);
                }

                return filteredCharts;
            }

            return charts;
        }

        private async Task<List<DashboardChartDto>> GetOperationalChartsAsync(LoggedInUserDTO userContext)
        {
            _logger.LogDebug("Getting operational charts for user {UserId}", userContext.UserId);
            
            var charts = new List<DashboardChartDto>();

            // Get actual chart data
            var dailySampleTrendChart = await GetDailySampleInwardTrendChartAsync();
            var testingCompletionChart = await GetTestingCompletionStatusChartAsync();

            // All roles can see operational charts, but with different levels of detail
            var operationalCharts = new List<DashboardChartDto>
            {
                dailySampleTrendChart,
                testingCompletionChart
            };

            // Filter charts based on user role
            charts.AddRange(FilterChartsByRole(operationalCharts, userContext.Role));

            return charts;
        }

        private async Task<List<DashboardChartDto>> GetBillingChartsAsync(LoggedInUserDTO userContext)
        {
            _logger.LogDebug("Getting billing charts for user {UserId}", userContext.UserId);
            
            var charts = new List<DashboardChartDto>();

            // Only Admin and Accounts roles can see billing charts
            if (!CanAccessBillingData(userContext.Role))
            {
                _logger.LogWarning("User {UserId} with role {Role} attempted to access billing charts", 
                    userContext.UserId, userContext.Role);
                return charts;
            }

            // Get actual billing chart data
            var billingSummaryChart = await GetBillingSummaryChartAsync();
            var paymentTrendsChart = await GetPaymentTrendsChartAsync();

            var billingCharts = new List<DashboardChartDto>
            {
                billingSummaryChart,
                paymentTrendsChart
            };

            // Filter charts based on user role
            charts.AddRange(FilterChartsByRole(billingCharts, userContext.Role));

            return charts;
        }

        private DashboardChartDto CreateOperationalChart(string key, string title, string[] allowedRoles)
        {
            return new DashboardChartDto
            {
                Key = key,
                Title = title,
                ChartType = "Line",
                DataPoints = new List<ChartDataPointDto>(),
                AllowedRoles = allowedRoles.ToList(),
                Options = new Dictionary<string, object>
                {
                    { "ChartType", "Operational" },
                    { "LastUpdated", DateTime.UtcNow }
                }
            };
        }

        private DashboardChartDto CreateBillingChart(string key, string title, string[] allowedRoles)
        {
            return new DashboardChartDto
            {
                Key = key,
                Title = title,
                ChartType = "Bar",
                DataPoints = new List<ChartDataPointDto>(),
                AllowedRoles = allowedRoles.ToList(),
                Options = new Dictionary<string, object>
                {
                    { "ChartType", "Billing" },
                    { "LastUpdated", DateTime.UtcNow },
                    { "RequiresBillingAccess", true }
                }
            };
        }

        private List<DashboardChartDto> FilterChartsByRole(List<DashboardChartDto> charts, string? userRole)
        {
            if (string.IsNullOrWhiteSpace(userRole))
            {
                _logger.LogWarning("Cannot filter charts: user role is null or empty");
                return new List<DashboardChartDto>();
            }

            var filteredCharts = charts.Where(chart => IsChartAllowedForRole(chart, userRole)).ToList();
            
            _logger.LogDebug("Filtered {TotalCharts} charts to {FilteredCharts} for role {Role}", 
                charts.Count, filteredCharts.Count, userRole);

            return filteredCharts;
        }

        private bool IsChartAllowedForRole(DashboardChartDto chart, string userRole)
        {
            if (chart.AllowedRoles == null || !chart.AllowedRoles.Any())
            {
                // If no roles specified, allow all authenticated users
                return true;
            }

            // Check if user role matches any allowed role (case-insensitive)
            return chart.AllowedRoles.Any(allowedRole => 
                userRole.Equals(allowedRole, StringComparison.OrdinalIgnoreCase) ||
                userRole.Contains(allowedRole, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<List<DashboardNotificationDto>> GetSystemNotificationsAsync(LoggedInUserDTO userContext)
        {
            _logger.LogDebug("Getting system notifications for user {UserId}", userContext.UserId);
            
            var notifications = new List<DashboardNotificationDto>();

            // Get system notifications from audit logs
            notifications.AddRange(await GetAuditLogNotificationsAsync(userContext));

            // Get urgent notifications
            notifications.AddRange(await GetUrgentNotificationsAsync(userContext));

            // Apply role-based filtering
            notifications = ApplyRoleBasedNotificationFiltering(notifications, userContext);

            // Sort by priority and creation date (most recent first)
            notifications = notifications
                .OrderBy(n => GetPriorityOrder(n.Priority))
                .ThenByDescending(n => n.CreatedAt)
                .ToList();

            _logger.LogDebug("Returning {NotificationCount} notifications for user {UserId}", 
                notifications.Count, userContext.UserId);

            return notifications;
        }

        private async Task<List<DashboardNotificationDto>> GetAuditLogNotificationsAsync(LoggedInUserDTO userContext)
        {
            try
            {
                _logger.LogDebug("Getting audit log notifications for user {UserId}", userContext.UserId);

                var notifications = new List<DashboardNotificationDto>();

                // Get recent audit activities (last 7 days) for data export and sensitive access events
                var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
                
                var auditActivities = await _context.SiteActivities
                    .Where(sa => sa.ModifiedOn >= sevenDaysAgo &&
                                (sa.Action!.Contains("Export", StringComparison.OrdinalIgnoreCase) ||
                                 sa.Action!.Contains("Download", StringComparison.OrdinalIgnoreCase) ||
                                 sa.Action!.Contains("Delete", StringComparison.OrdinalIgnoreCase) ||
                                 sa.ModuleName!.Contains("Admin", StringComparison.OrdinalIgnoreCase) ||
                                 sa.ModuleName!.Contains("User", StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(sa => sa.ModifiedOn)
                    .Take(20) // Limit to recent 20 activities
                    .ToListAsync();

                foreach (var activity in auditActivities)
                {
                    var notification = new DashboardNotificationDto
                    {
                        Id = activity.ID,
                        Title = GetAuditNotificationTitle(activity.Action, activity.ModuleName),
                        Message = GetAuditNotificationMessage(activity),
                        Type = GetAuditNotificationType(activity.Action),
                        Priority = GetAuditNotificationPriority(activity.Action),
                        CreatedAt = activity.ModifiedOn ?? DateTime.UtcNow,
                        IsRead = false,
                        ActionUrl = null, // Could be set to relevant module URL
                        Metadata = new Dictionary<string, object>
                        {
                            { "Source", "AuditLog" },
                            { "ModuleName", activity.ModuleName ?? "" },
                            { "Action", activity.Action ?? "" },
                            { "UserAgent", activity.Browser ?? "" },
                            { "IpAddress", activity.Ipaddress ?? "" },
                            { "TraceId", activity.TraceId ?? "" }
                        }
                    };

                    notifications.Add(notification);
                }

                _logger.LogDebug("Generated {Count} audit log notifications", notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit log notifications for user {UserId}", userContext.UserId);
                return new List<DashboardNotificationDto>();
            }
        }

        private string GetAuditNotificationTitle(string? action, string? moduleName)
        {
            if (string.IsNullOrWhiteSpace(action))
                return "System Activity";

            return action.ToLowerInvariant() switch
            {
                var a when a.Contains("export") => "Data Export Activity",
                var a when a.Contains("download") => "Data Download Activity",
                var a when a.Contains("delete") => "Data Deletion Activity",
                var a when a.Contains("admin") => "Administrative Activity",
                var a when a.Contains("user") => "User Management Activity",
                _ => $"{moduleName} Activity"
            };
        }

        private string GetAuditNotificationMessage(SiteActivity activity)
        {
            var user = !string.IsNullOrWhiteSpace(activity.ModifiedBy) ? activity.ModifiedBy : "Unknown User";
            var action = !string.IsNullOrWhiteSpace(activity.Action) ? activity.Action : "Unknown Action";
            var module = !string.IsNullOrWhiteSpace(activity.ModuleName) ? activity.ModuleName : "System";
            
            var message = $"{user} performed {action} in {module}";
            
            if (!string.IsNullOrWhiteSpace(activity.Description))
            {
                message += $": {activity.Description}";
            }

            return message;
        }

        private string GetAuditNotificationType(string? action)
        {
            if (string.IsNullOrWhiteSpace(action))
                return "Info";

            return action.ToLowerInvariant() switch
            {
                var a when a.Contains("delete") => "Warning",
                var a when a.Contains("export") => "Info",
                var a when a.Contains("download") => "Info",
                var a when a.Contains("admin") => "Warning",
                _ => "Info"
            };
        }

        private string GetAuditNotificationPriority(string? action)
        {
            if (string.IsNullOrWhiteSpace(action))
                return "Normal";

            return action.ToLowerInvariant() switch
            {
                var a when a.Contains("delete") => "High",
                var a when a.Contains("admin") => "High",
                var a when a.Contains("export") => "Normal",
                var a when a.Contains("download") => "Normal",
                _ => "Normal"
            };
        }

        private async Task<List<DashboardNotificationDto>> GetUrgentNotificationsAsync(LoggedInUserDTO userContext)
        {
            try
            {
                _logger.LogDebug("Getting urgent notifications for user {UserId}", userContext.UserId);

                var notifications = new List<DashboardNotificationDto>();

                // Get urgent samples that need immediate attention
                await AddUrgentSampleNotificationsAsync(notifications);

                // Get SLA breach notifications
                await AddSlaBreachNotificationsAsync(notifications);

                // Get existing system notifications marked as urgent
                await AddSystemUrgentNotificationsAsync(notifications);

                _logger.LogDebug("Generated {Count} urgent notifications", notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting urgent notifications for user {UserId}", userContext.UserId);
                return new List<DashboardNotificationDto>();
            }
        }

        private async Task AddUrgentSampleNotificationsAsync(List<DashboardNotificationDto> notifications)
        {
            try
            {
                // Get urgent samples that are pending inward
                var urgentPendingInward = await _context.SampleInwards
                    .Where(s => s.Urgent && s.InwardStatus == "Sample Received")
                    .Include(s => s.Customer)
                    .OrderByDescending(s => s.CollectionTime)
                    .Take(10)
                    .ToListAsync();

                foreach (var sample in urgentPendingInward)
                {
                    notifications.Add(new DashboardNotificationDto
                    {
                        Id = sample.ID + 100000, // Offset to avoid ID conflicts
                        Title = "Urgent Sample Pending Inward",
                        Message = $"Urgent sample {sample.CaseNo} from {sample.Customer?.Name ?? "Unknown Customer"} is pending inward processing since {sample.CollectionTime:MMM dd, yyyy}",
                        Type = "Warning",
                        Priority = "Critical",
                        CreatedAt = sample.CollectionTime,
                        IsRead = false,
                        ActionUrl = $"/samples/inward/{sample.ID}",
                        Metadata = new Dictionary<string, object>
                        {
                            { "Source", "UrgentSample" },
                            { "SampleId", sample.ID },
                            { "CaseNo", sample.CaseNo },
                            { "CustomerId", sample.CustomerID },
                            { "Status", sample.InwardStatus },
                            { "UrgentFlag", true }
                        }
                    });
                }

                // Get urgent samples under testing that are taking too long
                var urgentUnderTesting = await _context.TestResultHeaders
                    .Include(trh => trh.Sample)
                    .ThenInclude(s => s!.SampleInward)
                    .ThenInclude(si => si!.Customer)
                    .Where(trh => trh.Sample!.SampleInward!.Urgent &&
                                 (trh.Status == "In Progress" || trh.Status == "Started") &&
                                 trh.CreatedOn < DateTime.UtcNow.AddDays(-2)) // More than 2 days in testing
                    .OrderByDescending(trh => trh.CreatedOn)
                    .Take(10)
                    .ToListAsync();

                foreach (var testResult in urgentUnderTesting)
                {
                    var sample = testResult.Sample;
                    var sampleInward = sample?.SampleInward;
                    var customer = sampleInward?.Customer;

                    notifications.Add(new DashboardNotificationDto
                    {
                        Id = testResult.ID + 200000, // Offset to avoid ID conflicts
                        Title = "Urgent Sample Testing Delayed",
                        Message = $"Urgent sample {sampleInward?.CaseNo ?? "Unknown"} from {customer?.Name ?? "Unknown Customer"} has been under testing for {(DateTime.UtcNow - testResult.CreatedOn).Days} days",
                        Type = "Error",
                        Priority = "Critical",
                        CreatedAt = testResult.CreatedOn,
                        IsRead = false,
                        ActionUrl = $"/testing/results/{testResult.ID}",
                        Metadata = new Dictionary<string, object>
                        {
                            { "Source", "UrgentTesting" },
                            { "TestResultId", testResult.ID },
                            { "SampleId", sample?.ID ?? 0 },
                            { "CaseNo", sampleInward?.CaseNo ?? "" },
                            { "Status", testResult.Status ?? "" },
                            { "DaysInTesting", (DateTime.UtcNow - testResult.CreatedOn).Days },
                            { "UrgentFlag", true }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding urgent sample notifications");
            }
        }

        private async Task AddSlaBreachNotificationsAsync(List<DashboardNotificationDto> notifications)
        {
            try
            {
                // Define SLA thresholds (these could be configurable)
                var standardSlaThreshold = DateTime.UtcNow.AddDays(-7); // 7 days for standard samples
                var urgentSlaThreshold = DateTime.UtcNow.AddDays(-3);   // 3 days for urgent samples

                // Get samples that have breached SLA
                var slaBreachSamples = await _context.SampleInwards
                    .Where(s => (s.Urgent && s.CollectionTime < urgentSlaThreshold && s.InwardStatus != "Completed") ||
                               (!s.Urgent && s.CollectionTime < standardSlaThreshold && s.InwardStatus != "Completed"))
                    .Include(s => s.Customer)
                    .OrderBy(s => s.CollectionTime) // Oldest first
                    .Take(15)
                    .ToListAsync();

                foreach (var sample in slaBreachSamples)
                {
                    var daysOverdue = (DateTime.UtcNow - sample.CollectionTime).Days;
                    var slaType = sample.Urgent ? "urgent" : "standard";
                    var expectedDays = sample.Urgent ? 3 : 7;

                    notifications.Add(new DashboardNotificationDto
                    {
                        Id = sample.ID + 300000, // Offset to avoid ID conflicts
                        Title = "SLA Breach Alert",
                        Message = $"Sample {sample.CaseNo} from {sample.Customer?.Name ?? "Unknown Customer"} has breached {slaType} SLA by {daysOverdue - expectedDays} days",
                        Type = "Error",
                        Priority = "High",
                        CreatedAt = sample.CollectionTime,
                        IsRead = false,
                        ActionUrl = $"/samples/{sample.ID}",
                        Metadata = new Dictionary<string, object>
                        {
                            { "Source", "SLABreach" },
                            { "SampleId", sample.ID },
                            { "CaseNo", sample.CaseNo },
                            { "CustomerId", sample.CustomerID },
                            { "DaysOverdue", daysOverdue },
                            { "ExpectedDays", expectedDays },
                            { "SlaType", slaType },
                            { "UrgentFlag", sample.Urgent }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding SLA breach notifications");
            }
        }

        private async Task AddSystemUrgentNotificationsAsync(List<DashboardNotificationDto> notifications)
        {
            try
            {
                // Get recent urgent system notifications from the Notification table
                var systemNotifications = await _context.Notifications
                    .Where(n => n.Type == NotificationType.System &&
                               n.CreatedOn >= DateTime.UtcNow.AddDays(-7) &&
                               !n.IsRead)
                    .OrderByDescending(n => n.CreatedOn)
                    .Take(10)
                    .ToListAsync();

                foreach (var sysNotification in systemNotifications)
                {
                    // Determine priority based on title/message content
                    var priority = DeterminePriorityFromContent(sysNotification.Title, sysNotification.Message);
                    var type = DetermineTypeFromContent(sysNotification.Title, sysNotification.Message);

                    notifications.Add(new DashboardNotificationDto
                    {
                        Id = sysNotification.ID + 400000, // Offset to avoid ID conflicts
                        Title = sysNotification.Title,
                        Message = sysNotification.Message,
                        Type = type,
                        Priority = priority,
                        CreatedAt = sysNotification.CreatedOn,
                        IsRead = sysNotification.IsRead,
                        ActionUrl = GetActionUrlFromNotification(sysNotification),
                        Metadata = new Dictionary<string, object>
                        {
                            { "Source", "SystemNotification" },
                            { "OriginalId", sysNotification.ID },
                            { "UserId", sysNotification.UserID ?? 0 },
                            { "EntityId", sysNotification.EntityID ?? 0 },
                            { "EntityType", sysNotification.EntityType ?? "" },
                            { "WorkflowId", sysNotification.WorkflowID ?? 0 },
                            { "NotificationType", sysNotification.Type.ToString() }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding system urgent notifications");
            }
        }

        private string DeterminePriorityFromContent(string title, string message)
        {
            var content = $"{title} {message}".ToLowerInvariant();

            if (content.Contains("urgent") || content.Contains("critical") || content.Contains("error") || content.Contains("failed"))
                return "Critical";
            
            if (content.Contains("warning") || content.Contains("overdue") || content.Contains("breach"))
                return "High";
            
            if (content.Contains("reminder") || content.Contains("pending"))
                return "Normal";

            return "Normal";
        }

        private string DetermineTypeFromContent(string title, string message)
        {
            var content = $"{title} {message}".ToLowerInvariant();

            if (content.Contains("error") || content.Contains("failed") || content.Contains("breach"))
                return "Error";
            
            if (content.Contains("warning") || content.Contains("overdue") || content.Contains("urgent"))
                return "Warning";
            
            if (content.Contains("success") || content.Contains("completed") || content.Contains("approved"))
                return "Success";

            return "Info";
        }

        private string? GetActionUrlFromNotification(Notification notification)
        {
            if (notification.EntityType == null || notification.EntityID == null)
                return null;

            return notification.EntityType.ToLowerInvariant() switch
            {
                "sample" => $"/samples/{notification.EntityID}",
                "test" => $"/testing/{notification.EntityID}",
                "invoice" => $"/billing/invoices/{notification.EntityID}",
                "payment" => $"/billing/payments/{notification.EntityID}",
                "workflow" => $"/workflow/{notification.WorkflowID}",
                _ => null
            };
        }

        private List<DashboardNotificationDto> ApplyRoleBasedNotificationFiltering(
            List<DashboardNotificationDto> notifications, LoggedInUserDTO userContext)
        {
            try
            {
                _logger.LogDebug("Applying role-based notification filtering for user {UserId} with role {Role}", 
                    userContext.UserId, userContext.Role);

                var filteredNotifications = new List<DashboardNotificationDto>();

                foreach (var notification in notifications)
                {
                    if (IsNotificationAllowedForRole(notification, userContext.Role))
                    {
                        filteredNotifications.Add(notification);
                    }
                }

                var excludedCount = notifications.Count - filteredNotifications.Count;
                if (excludedCount > 0)
                {
                    _logger.LogDebug("Excluded {ExcludedCount} notifications for role {Role}", 
                        excludedCount, userContext.Role);
                }

                return filteredNotifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying role-based notification filtering for user {UserId}", 
                    userContext.UserId);
                return notifications; // Return unfiltered on error
            }
        }

        private bool IsNotificationAllowedForRole(DashboardNotificationDto notification, string? userRole)
        {
            if (string.IsNullOrWhiteSpace(userRole))
                return false;

            // Get notification source from metadata
            var source = notification.Metadata?.ContainsKey("Source") == true 
                ? notification.Metadata["Source"].ToString() 
                : "";

            return source switch
            {
                "AuditLog" => IsAuditLogNotificationAllowedForRole(notification, userRole),
                "UrgentSample" => IsUrgentSampleNotificationAllowedForRole(notification, userRole),
                "UrgentTesting" => IsUrgentTestingNotificationAllowedForRole(notification, userRole),
                "SLABreach" => IsSlaBreachNotificationAllowedForRole(notification, userRole),
                "SystemNotification" => IsSystemNotificationAllowedForRole(notification, userRole),
                _ => true // Allow unknown notification types by default
            };
        }

        private bool IsAuditLogNotificationAllowedForRole(DashboardNotificationDto notification, string userRole)
        {
            // Only Admin users should see audit log notifications
            if (IsAdminRole(userRole))
                return true;

            // Accounts users can see billing-related audit activities
            if (IsAccountsRole(userRole))
            {
                var action = notification.Metadata?.ContainsKey("Action") == true 
                    ? notification.Metadata["Action"].ToString()?.ToLowerInvariant() ?? ""
                    : "";
                
                var moduleName = notification.Metadata?.ContainsKey("ModuleName") == true 
                    ? notification.Metadata["ModuleName"].ToString()?.ToLowerInvariant() ?? ""
                    : "";

                return action.Contains("invoice") || action.Contains("payment") || 
                       moduleName.Contains("billing") || moduleName.Contains("account");
            }

            // Normal users should not see audit log notifications
            return false;
        }

        private bool IsUrgentSampleNotificationAllowedForRole(DashboardNotificationDto notification, string userRole)
        {
            // All operational roles can see urgent sample notifications
            return IsAdminRole(userRole) || 
                   userRole.Equals("FrontDesk", StringComparison.OrdinalIgnoreCase) ||
                   userRole.Equals("Technical", StringComparison.OrdinalIgnoreCase) ||
                   userRole.Equals("Lab", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsUrgentTestingNotificationAllowedForRole(DashboardNotificationDto notification, string userRole)
        {
            // Technical and Lab roles can see urgent testing notifications
            return IsAdminRole(userRole) || 
                   userRole.Equals("Technical", StringComparison.OrdinalIgnoreCase) ||
                   userRole.Equals("Lab", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsSlaBreachNotificationAllowedForRole(DashboardNotificationDto notification, string userRole)
        {
            // All roles can see SLA breach notifications as they affect everyone
            return IsValidRole(userRole);
        }

        private bool IsSystemNotificationAllowedForRole(DashboardNotificationDto notification, string userRole)
        {
            // Check if the notification is user-specific
            var userId = notification.Metadata?.ContainsKey("UserId") == true 
                ? notification.Metadata["UserId"].ToString() 
                : "";

            // If notification is for a specific user, only show to that user or admins
            if (!string.IsNullOrWhiteSpace(userId) && userId != "0")
            {
                // For now, we don't have the current user ID in the role check
                // So we'll allow admins to see all notifications and others to see general ones
                return IsAdminRole(userRole);
            }

            // Check entity type restrictions
            var entityType = notification.Metadata?.ContainsKey("EntityType") == true 
                ? notification.Metadata["EntityType"].ToString()?.ToLowerInvariant() ?? ""
                : "";

            return entityType switch
            {
                "invoice" or "payment" => CanAccessBillingData(userRole),
                "sample" => IsAdminRole(userRole) || IsNormalUserRole(userRole),
                "test" => IsAdminRole(userRole) || 
                         userRole.Equals("Technical", StringComparison.OrdinalIgnoreCase) ||
                         userRole.Equals("Lab", StringComparison.OrdinalIgnoreCase),
                "workflow" => IsValidRole(userRole), // All valid roles can see workflow notifications
                _ => true // Allow general system notifications for all roles
            };
        }

        private int GetPriorityOrder(string priority)
        {
            return priority switch
            {
                "Critical" => 1,
                "High" => 2,
                "Normal" => 3,
                "Low" => 4,
                _ => 5
            };
        }

        private async Task<DashboardChartDto> GetDailySampleInwardTrendChartAsync()
        {
            try
            {
                _logger.LogDebug("Getting daily sample inward trend chart data");

                // Get data for the last 30 days
                var endDate = DateTime.Today.AddDays(1); // Include today
                var startDate = endDate.AddDays(-30);

                var dailyData = await _context.SampleInwards
                    .Where(s => s.CollectionTime >= startDate && s.CollectionTime < endDate)
                    .GroupBy(s => s.CollectionTime.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                // Create data points for all days in the range (including days with 0 samples)
                var dataPoints = new List<ChartDataPointDto>();
                for (var date = startDate.Date; date < endDate.Date; date = date.AddDays(1))
                {
                    var dayData = dailyData.FirstOrDefault(d => d.Date == date);
                    var count = dayData?.Count ?? 0;

                    dataPoints.Add(new ChartDataPointDto
                    {
                        Label = date.ToString("MMM dd"),
                        Value = count,
                        Date = date,
                        Metadata = new Dictionary<string, object>
                        {
                            { "FullDate", date.ToString("yyyy-MM-dd") },
                            { "DayOfWeek", date.DayOfWeek.ToString() }
                        }
                    });
                }

                _logger.LogDebug("Generated {DataPointCount} data points for daily sample trend chart", dataPoints.Count);

                return new DashboardChartDto
                {
                    Key = "daily-sample-trend",
                    Title = "Daily Sample Inward Trend (Last 30 Days)",
                    ChartType = "Line",
                    DataPoints = dataPoints,
                    AllowedRoles = new List<string> { "Admin", "FrontDesk", "Technical", "Lab" },
                    Options = new Dictionary<string, object>
                    {
                        { "ChartType", "Operational" },
                        { "LastUpdated", DateTime.UtcNow },
                        { "TimeRange", "30 days" },
                        { "XAxisLabel", "Date" },
                        { "YAxisLabel", "Sample Count" },
                        { "ShowDataPoints", true },
                        { "ShowTrendLine", true }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily sample inward trend chart data");
                
                // Return empty chart on error
                return new DashboardChartDto
                {
                    Key = "daily-sample-trend",
                    Title = "Daily Sample Inward Trend (Last 30 Days)",
                    ChartType = "Line",
                    DataPoints = new List<ChartDataPointDto>(),
                    AllowedRoles = new List<string> { "Admin", "FrontDesk", "Technical", "Lab" },
                    Options = new Dictionary<string, object>
                    {
                        { "ChartType", "Operational" },
                        { "LastUpdated", DateTime.UtcNow },
                    }
                };
            }
        }

        private async Task<DashboardChartDto> GetTestingCompletionStatusChartAsync()
        {
            try
            {
                _logger.LogDebug("Getting testing completion status chart data");

                // Aggregate TestResultHeader data by completion status
                var statusData = await _context.TestResultHeaders
                    .GroupBy(trh => trh.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Status)
                    .ToListAsync();

                // Create data points for each status
                var dataPoints = new List<ChartDataPointDto>();
                var statusColors = new Dictionary<string, string>
                {
                    { "Pending", "#FFA500" },      // Orange
                    { "In Progress", "#1E90FF" },  // DodgerBlue
                    { "Started", "#32CD32" },      // LimeGreen
                    { "Completed", "#228B22" },    // ForestGreen
                    { "On Hold", "#DC143C" },      // Crimson
                    { "Cancelled", "#696969" }     // DimGray
                };

                foreach (var statusItem in statusData)
                {
                    var color = statusColors.ContainsKey(statusItem.Status) 
                        ? statusColors[statusItem.Status] 
                        : "#808080"; // Default gray

                    dataPoints.Add(new ChartDataPointDto
                    {
                        Label = statusItem.Status,
                        Value = statusItem.Count,
                        Metadata = new Dictionary<string, object>
                        {
                            { "Color", color },
                            { "Percentage", 0 }, // Will be calculated on frontend
                            { "StatusCategory", GetStatusCategory(statusItem.Status) }
                        }
                    });
                }

                // Calculate percentages
                var totalCount = dataPoints.Sum(dp => dp.Value);
                if (totalCount > 0)
                {
                    foreach (var dataPoint in dataPoints)
                    {
                        var percentage = Math.Round((dataPoint.Value / totalCount) * 100, 1);
                        dataPoint.Metadata!["Percentage"] = percentage;
                    }
                }

                _logger.LogDebug("Generated {DataPointCount} data points for testing completion status chart", dataPoints.Count);

                return new DashboardChartDto
                {
                    Key = "testing-completion-status",
                    Title = "Testing Completion Status",
                    ChartType = "Pie",
                    DataPoints = dataPoints,
                    AllowedRoles = new List<string> { "Admin", "Technical", "Lab" },
                    Options = new Dictionary<string, object>
                    {
                        { "ChartType", "Operational" },
                        { "LastUpdated", DateTime.UtcNow },
                        { "ShowLegend", true },
                        { "ShowPercentages", true },
                        { "TotalTests", totalCount },
                        { "Description", "Distribution of test completion statuses across all active tests" }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting testing completion status chart data");
                
                // Return empty chart on error
                return new DashboardChartDto
                {
                    Key = "testing-completion-status",
                    Title = "Testing Completion Status",
                    ChartType = "Pie",
                    DataPoints = new List<ChartDataPointDto>(),
                    AllowedRoles = new List<string> { "Admin", "Technical", "Lab" },
                    Options = new Dictionary<string, object>
                    {
                        { "ChartType", "Operational" },
                        { "LastUpdated", DateTime.UtcNow },
                        { "Error", "Failed to load chart data" }
                    }
                };
            }
        }

        private string GetStatusCategory(string status)
        {
            return status switch
            {
                "Pending" => "Not Started",
                "In Progress" or "Started" => "Active",
                "Completed" => "Finished",
                "On Hold" or "Cancelled" => "Inactive",
                _ => "Other"
            };
        }

        private async Task<DashboardChartDto> GetBillingSummaryChartAsync()
        {
            try
            {
                _logger.LogDebug("Getting billing summary chart data");

                // Aggregate TaxInvoice data by status
                var invoiceStatusData = await _context.TaxInvoices
                    .GroupBy(ti => ti.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(ti => ti.GrandTotal)
                    })
                    .OrderBy(x => x.Status)
                    .ToListAsync();

                // Create data points for each invoice status
                var dataPoints = new List<ChartDataPointDto>();
                var statusColors = new Dictionary<string, string>
                {
                    { "Generated", "#FFA500" },    // Orange
                    { "Sent", "#1E90FF" },         // DodgerBlue
                    { "Paid", "#228B22" },         // ForestGreen
                    { "Overdue", "#DC143C" },      // Crimson
                    { "Cancelled", "#696969" }     // DimGray
                };

                foreach (var statusItem in invoiceStatusData)
                {
                    var color = statusColors.ContainsKey(statusItem.Status) 
                        ? statusColors[statusItem.Status] 
                        : "#808080"; // Default gray

                    dataPoints.Add(new ChartDataPointDto
                    {
                        Label = $"{statusItem.Status} Invoices",
                        Value = statusItem.TotalAmount,
                        Metadata = new Dictionary<string, object>
                        {
                            { "Color", color },
                            { "Count", statusItem.Count },
                            { "AverageAmount", statusItem.Count > 0 ? Math.Round(statusItem.TotalAmount / statusItem.Count, 2) : 0 },
                            { "Status", statusItem.Status },
                            { "Currency", "INR" }
                        }
                    });
                }

                // Calculate percentages
                var totalAmount = dataPoints.Sum(dp => dp.Value);
                if (totalAmount > 0)
                {
                    foreach (var dataPoint in dataPoints)
                    {
                        var percentage = Math.Round((dataPoint.Value / totalAmount) * 100, 1);
                        dataPoint.Metadata!["Percentage"] = percentage;
                    }
                }

                _logger.LogDebug("Generated {DataPointCount} data points for billing summary chart", dataPoints.Count);

                return new DashboardChartDto
                {
                    Key = "billing-summary",
                    Title = "Billing Summary by Status",
                    ChartType = "Doughnut",
                    DataPoints = dataPoints,
                    AllowedRoles = new List<string> { "Admin", "Accounts" },
                    Options = new Dictionary<string, object>
                    {
                        { "ChartType", "Billing" },
                        { "LastUpdated", DateTime.UtcNow },
                        { "RequiresBillingAccess", true },
                        { "ShowLegend", true },
                        { "ShowPercentages", true },
                        { "TotalAmount", totalAmount },
                        { "Currency", "INR" },
                        { "Description", "Distribution of invoice amounts by status" }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting billing summary chart data");
                
                // Return empty chart on error
                return new DashboardChartDto
                {
                    Key = "billing-summary",
                    Title = "Billing Summary by Status",
                    ChartType = "Doughnut",
                    DataPoints = new List<ChartDataPointDto>(),
                    AllowedRoles = new List<string> { "Admin", "Accounts" },
                    Options = new Dictionary<string, object>
                    {
                        { "ChartType", "Billing" },
                        { "LastUpdated", DateTime.UtcNow },
                        { "RequiresBillingAccess", true },
                        { "Error", "Failed to load chart data" }
                    }
                };
            }
        }

        private async Task<DashboardChartDto> GetPaymentTrendsChartAsync()
        {
            try
            {
                _logger.LogDebug("Getting payment trends chart data");

                // Get payment data for the last 30 days
                var endDate = DateTime.Today.AddDays(1); // Include today
                var startDate = endDate.AddDays(-30);

                var dailyPaymentData = await _context.PaymentOrders
                    .Where(po => po.PaidOn >= startDate && po.PaidOn < endDate && po.Status == PaymentStatus.Paid)
                    .GroupBy(po => po.PaidOn!.Value.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(po => po.Amount)
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                // Create data points for all days in the range (including days with 0 payments)
                var dataPoints = new List<ChartDataPointDto>();
                for (var date = startDate.Date; date < endDate.Date; date = date.AddDays(1))
                {
                    var dayData = dailyPaymentData.FirstOrDefault(d => d.Date == date);
                    var amount = dayData?.TotalAmount ?? 0;
                    var count = dayData?.Count ?? 0;

                    dataPoints.Add(new ChartDataPointDto
                    {
                        Label = date.ToString("MMM dd"),
                        Value = amount,
                        Date = date,
                        Metadata = new Dictionary<string, object>
                        {
                            { "FullDate", date.ToString("yyyy-MM-dd") },
                            { "DayOfWeek", date.DayOfWeek.ToString() },
                            { "PaymentCount", count },
                            { "AveragePayment", count > 0 ? Math.Round(amount / count, 2) : 0 },
                            { "Currency", "INR" }
                        }
                    });
                }

                var totalAmount = dataPoints.Sum(dp => dp.Value);
                var totalPayments = dataPoints.Sum(dp => (int)dp.Metadata!["PaymentCount"]);

                _logger.LogDebug("Generated {DataPointCount} data points for payment trends chart", dataPoints.Count);

                return new DashboardChartDto
                {
                    Key = "payment-trends",
                    Title = "Payment Trends (Last 30 Days)",
                    ChartType = "Bar",
                    DataPoints = dataPoints,
                    AllowedRoles = new List<string> { "Admin", "Accounts" },
                    Options = new Dictionary<string, object>
                    {
                        { "ChartType", "Billing" },
                        { "LastUpdated", DateTime.UtcNow },
                        { "RequiresBillingAccess", true },
                        { "TimeRange", "30 days" },
                        { "XAxisLabel", "Date" },
                        { "YAxisLabel", "Payment Amount (INR)" },
                        { "TotalAmount", totalAmount },
                        { "TotalPayments", totalPayments },
                        { "Currency", "INR" },
                        { "ShowDataLabels", false },
                        { "Description", "Daily payment amounts received over the last 30 days" }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment trends chart data");
                
                // Return empty chart on error
                return new DashboardChartDto
                {
                    Key = "payment-trends",
                    Title = "Payment Trends (Last 30 Days)",
                    ChartType = "Bar",
                    DataPoints = new List<ChartDataPointDto>(),
                    AllowedRoles = new List<string> { "Admin", "Accounts" },
                    Options = new Dictionary<string, object>
                    {
                        { "ChartType", "Billing" },
                        { "LastUpdated", DateTime.UtcNow },
                        { "RequiresBillingAccess", true },
                        { "Error", "Failed to load chart data" }
                    }
                };
            }
        }
    }
}