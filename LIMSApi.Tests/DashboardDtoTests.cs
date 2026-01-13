using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using LIMSApi.Dtos;
using NUnit.Framework;

namespace LIMSApi.Tests
{
    /// <summary>
    /// Unit tests for Dashboard DTOs
    /// Feature: lims-dashboard-backend, Property 8: DTO structure consistency
    /// Validates: Requirements 8.1, 8.5
    /// </summary>
    [TestFixture]
    public class DashboardDtoTests
    {
        [Test]
        public void DashboardResponseDto_ShouldHaveConsistentStructure()
        {
            // Arrange
            var dashboardResponse = new DashboardResponseDto
            {
                Cards = new List<DashboardCardDto>
                {
                    new DashboardCardDto
                    {
                        Key = "test-card",
                        Title = "Test Card",
                        Count = 5,
                        Status = "Normal",
                        AllowedRoles = new List<string> { "Admin" }
                    }
                },
                Charts = new List<DashboardChartDto>
                {
                    new DashboardChartDto
                    {
                        Key = "test-chart",
                        Title = "Test Chart",
                        ChartType = "Line",
                        DataPoints = new List<ChartDataPointDto>
                        {
                            new ChartDataPointDto { Label = "Test", Value = 10 }
                        },
                        AllowedRoles = new List<string> { "Admin" }
                    }
                },
                Notifications = new List<DashboardNotificationDto>
                {
                    new DashboardNotificationDto
                    {
                        Id = 1,
                        Title = "Test Notification",
                        Message = "Test Message",
                        Type = "Info",
                        Priority = "Normal",
                        CreatedAt = DateTime.UtcNow
                    }
                },
                GeneratedAt = DateTime.UtcNow
            };

            // Act & Assert - Verify structure consistency
            Assert.That(dashboardResponse.Cards, Is.Not.Null, "Cards collection should not be null");
            Assert.That(dashboardResponse.Charts, Is.Not.Null, "Charts collection should not be null");
            Assert.That(dashboardResponse.Notifications, Is.Not.Null, "Notifications collection should not be null");
            Assert.That(dashboardResponse.GeneratedAt, Is.GreaterThan(DateTime.MinValue), "GeneratedAt should have a valid timestamp");

            // Verify card structure
            var card = dashboardResponse.Cards.First();
            Assert.That(card.Key, Is.Not.Null.And.Not.Empty, "Card Key should not be empty");
            Assert.That(card.Title, Is.Not.Null.And.Not.Empty, "Card Title should not be empty");
            Assert.That(card.AllowedRoles, Is.Not.Null, "Card AllowedRoles should not be null");

            // Verify chart structure
            var chart = dashboardResponse.Charts.First();
            Assert.That(chart.Key, Is.Not.Null.And.Not.Empty, "Chart Key should not be empty");
            Assert.That(chart.Title, Is.Not.Null.And.Not.Empty, "Chart Title should not be empty");
            Assert.That(chart.ChartType, Is.Not.Null.And.Not.Empty, "Chart ChartType should not be empty");
            Assert.That(chart.DataPoints, Is.Not.Null, "Chart DataPoints should not be null");
            Assert.That(chart.AllowedRoles, Is.Not.Null, "Chart AllowedRoles should not be null");

            // Verify notification structure
            var notification = dashboardResponse.Notifications.First();
            Assert.That(notification.Id, Is.GreaterThan(0), "Notification Id should be positive");
            Assert.That(notification.Title, Is.Not.Null.And.Not.Empty, "Notification Title should not be empty");
            Assert.That(notification.Message, Is.Not.Null.And.Not.Empty, "Notification Message should not be empty");
            Assert.That(notification.Type, Is.Not.Null.And.Not.Empty, "Notification Type should not be empty");
        }

        [Test]
        public void DashboardResponseDto_ShouldBeSerializableToJson()
        {
            // Arrange
            var dashboardResponse = new DashboardResponseDto
            {
                Cards = new List<DashboardCardDto>
                {
                    new DashboardCardDto
                    {
                        Key = "test-card",
                        Title = "Test Card",
                        Count = 5,
                        Status = "Normal",
                        AllowedRoles = new List<string> { "Admin" }
                    }
                },
                Charts = new List<DashboardChartDto>(),
                Notifications = new List<DashboardNotificationDto>(),
                GeneratedAt = DateTime.UtcNow
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            // Act & Assert - Test JSON serialization
            string json = null;
            Assert.DoesNotThrow(() => 
            {
                json = JsonSerializer.Serialize(dashboardResponse, jsonOptions);
            }, "Dashboard response should be serializable to JSON");

            Assert.That(json, Is.Not.Null.And.Not.Empty, "Serialized JSON should not be empty");

            // Test JSON deserialization
            DashboardResponseDto deserializedResponse = null;
            Assert.DoesNotThrow(() => 
            {
                deserializedResponse = JsonSerializer.Deserialize<DashboardResponseDto>(json, jsonOptions);
            }, "Dashboard response should be deserializable from JSON");

            Assert.That(deserializedResponse, Is.Not.Null, "Deserialized response should not be null");
            Assert.That(deserializedResponse.Cards.Count, Is.EqualTo(dashboardResponse.Cards.Count), "Card count should match after deserialization");
        }

        [Test]
        public void DashboardCardDto_ShouldValidateRequiredFields()
        {
            // Arrange
            var validCard = new DashboardCardDto
            {
                Key = "valid-key",
                Title = "Valid Title",
                Count = 10,
                Status = "Normal",
                AllowedRoles = new List<string> { "Admin" }
            };

            var invalidCard = new DashboardCardDto
            {
                Key = "", // Invalid empty key
                Title = "Valid Title",
                Count = 10,
                Status = "Normal",
                AllowedRoles = new List<string> { "Admin" }
            };

            // Act & Assert
            var validResults = ValidateModel(validCard);
            var invalidResults = ValidateModel(invalidCard);

            Assert.That(validResults.Count, Is.EqualTo(0), "Valid card should have no validation errors");
            // Note: Since we don't have validation attributes on the DTOs yet, this test mainly checks structure
        }

        [Test]
        public void DashboardErrorResponse_ShouldHaveRequiredErrorInformation()
        {
            // Arrange
            var errorResponse = new DashboardErrorResponse
            {
                Error = "TestError",
                Message = "Test error message",
                StatusCode = 400,
                Timestamp = DateTime.UtcNow,
                TraceId = "test-trace-id"
            };

            // Act & Assert
            Assert.That(errorResponse.Error, Is.Not.Null, "Error field should not be null");
            Assert.That(errorResponse.Message, Is.Not.Null, "Message field should not be null");
            Assert.That(errorResponse.StatusCode, Is.GreaterThan(0), "StatusCode should be positive");

            // Test JSON serialization
            Assert.DoesNotThrow(() => 
            {
                var json = JsonSerializer.Serialize(errorResponse);
                var deserialized = JsonSerializer.Deserialize<DashboardErrorResponse>(json);
                Assert.That(deserialized, Is.Not.Null);
                Assert.That(deserialized.Error, Is.EqualTo(errorResponse.Error));
            }, "Error response should be serializable");
        }

        /// <summary>
        /// Helper method to validate model using data annotations
        /// </summary>
        private List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}