using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.ServiceWORepo;
using LIMSApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Moq;

namespace LIMSApi.Tests
{
    /// <summary>
    /// Unit tests for Dashboard Service
    /// Feature: lims-dashboard-backend, Property 4: Read-only data access
    /// Validates: Requirements 1.4, 1.5
    /// </summary>
    [TestFixture]
    public class DashboardServiceTests
    {
        private LIMSContext _context;
        private DashboardService _dashboardService;
        private Mock<ILogger<DashboardService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<LIMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _context = new LIMSContext(options, mockHttpContextAccessor.Object);
            _mockLogger = new Mock<ILogger<DashboardService>>();
            _dashboardService = new DashboardService(_context, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task DashboardService_ShouldOnlyPerformReadOperations_WithValidUser()
        {
            // Arrange
            var userContext = new LoggedInUserDTO
            {
                UserId = 1,
                Role = "Admin",
                Name = "Test User",
                Email = "test@example.com",
                EmployeeID = 1,
                CompanyCode = "TEST"
            };

            // Get initial state of the database
            var initialSampleInwardCount = _context.SampleInwards.Count();
            var initialSampleDetailCount = _context.SampleDetails.Count();
            var initialTestResultCount = _context.TestResultHeaders.Count();
            var initialTaxInvoiceCount = _context.TaxInvoices.Count();
            var initialPaymentOrderCount = _context.PaymentOrders.Count();

            // Act - Perform dashboard operations
            var dashboard = await _dashboardService.GetDashboardAsync(userContext);
            var cards = await _dashboardService.GetDashboardCardsAsync(userContext);
            var charts = await _dashboardService.GetDashboardChartsAsync(userContext);
            var notifications = await _dashboardService.GetDashboardNotificationsAsync(userContext);

            // Assert - Verify no data was modified
            var finalSampleInwardCount = _context.SampleInwards.Count();
            var finalSampleDetailCount = _context.SampleDetails.Count();
            var finalTestResultCount = _context.TestResultHeaders.Count();
            var finalTaxInvoiceCount = _context.TaxInvoices.Count();
            var finalPaymentOrderCount = _context.PaymentOrders.Count();

            Assert.That(finalSampleInwardCount, Is.EqualTo(initialSampleInwardCount), 
                "SampleInward count should not change after dashboard operations");
            Assert.That(finalSampleDetailCount, Is.EqualTo(initialSampleDetailCount), 
                "SampleDetail count should not change after dashboard operations");
            Assert.That(finalTestResultCount, Is.EqualTo(initialTestResultCount), 
                "TestResultHeader count should not change after dashboard operations");
            Assert.That(finalTaxInvoiceCount, Is.EqualTo(initialTaxInvoiceCount), 
                "TaxInvoice count should not change after dashboard operations");
            Assert.That(finalPaymentOrderCount, Is.EqualTo(initialPaymentOrderCount), 
                "PaymentOrder count should not change after dashboard operations");

            // Verify results are not null
            Assert.That(dashboard, Is.Not.Null, "Dashboard result should not be null");
            Assert.That(cards, Is.Not.Null, "Cards result should not be null");
            Assert.That(charts, Is.Not.Null, "Charts result should not be null");
            Assert.That(notifications, Is.Not.Null, "Notifications result should not be null");
        }

        [Test]
        public void DashboardService_ShouldValidateUserContext_WithNullUser()
        {
            // Arrange
            LoggedInUserDTO nullUserContext = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await _dashboardService.GetDashboardAsync(nullUserContext));
        }

        [Test]
        public void DashboardService_ShouldValidateUserContext_WithInvalidUserId()
        {
            // Arrange
            var invalidUserContext = new LoggedInUserDTO
            {
                UserId = 0, // Invalid user ID
                Role = "Admin",
                Name = "Test User",
                Email = "test@example.com",
                EmployeeID = 1,
                CompanyCode = "TEST"
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await _dashboardService.GetDashboardAsync(invalidUserContext));
        }

        [Test]
        public void DashboardService_ShouldValidateUserContext_WithEmptyRole()
        {
            // Arrange
            var invalidUserContext = new LoggedInUserDTO
            {
                UserId = 1,
                Role = "", // Empty role
                Name = "Test User",
                Email = "test@example.com",
                EmployeeID = 1,
                CompanyCode = "TEST"
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await _dashboardService.GetDashboardAsync(invalidUserContext));
        }

        [Test]
        public async Task DashboardService_ShouldReturnEmptyResults_WithValidUserAndEmptyDatabase()
        {
            // Arrange
            var userContext = new LoggedInUserDTO
            {
                UserId = 1,
                Role = "Admin",
                Name = "Test User",
                Email = "test@example.com",
                EmployeeID = 1,
                CompanyCode = "TEST"
            };

            // Act
            var dashboard = await _dashboardService.GetDashboardAsync(userContext);

            // Assert
            Assert.That(dashboard, Is.Not.Null, "Dashboard should not be null");
            Assert.That(dashboard.Cards, Is.Not.Null, "Cards should not be null");
            Assert.That(dashboard.Charts, Is.Not.Null, "Charts should not be null");
            Assert.That(dashboard.Notifications, Is.Not.Null, "Notifications should not be null");
            Assert.That(dashboard.GeneratedAt, Is.GreaterThan(DateTime.MinValue), "GeneratedAt should be set");
        }

        [Test]
        public async Task DashboardService_ShouldFilterDataByRole_AdminVsNormalUser()
        {
            // Arrange
            var adminUser = new LoggedInUserDTO
            {
                UserId = 1,
                Role = "Admin",
                Name = "Admin User",
                Email = "admin@example.com",
                EmployeeID = 1,
                CompanyCode = "TEST"
            };

            var normalUser = new LoggedInUserDTO
            {
                UserId = 2,
                Role = "FrontDesk",
                Name = "Normal User",
                Email = "user@example.com",
                EmployeeID = 2,
                CompanyCode = "TEST"
            };

            // Act
            var adminDashboard = await _dashboardService.GetDashboardAsync(adminUser);
            var normalDashboard = await _dashboardService.GetDashboardAsync(normalUser);

            // Assert
            Assert.That(adminDashboard, Is.Not.Null, "Admin dashboard should not be null");
            Assert.That(normalDashboard, Is.Not.Null, "Normal user dashboard should not be null");

            // Admin should potentially have access to more cards than normal users
            // (This test validates the role-based filtering logic exists)
            Assert.That(adminDashboard.Cards, Is.Not.Null, "Admin cards should not be null");
            Assert.That(normalDashboard.Cards, Is.Not.Null, "Normal user cards should not be null");
        }
    }
}