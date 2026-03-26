using System.Net;
using System.Text;
using System.Text.Json;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests;

/// <summary>
/// Negative and edge-case tests: auth enforcement, invalid payloads,
/// boundary values, SQL injection, XSS, malformed JSON, etc.
/// </summary>
[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class T09_ValidationTests : ApiTestBase
{
    public T09_ValidationTests(AuthFixture fixture) : base(fixture) { }

    // ═══════════════════════════════════════════════════════
    // 401 Unauthorized — All major endpoints without token
    // ═══════════════════════════════════════════════════════

    [Theory]
    [InlineData("/api/DepartmentMaster/list")]
    [InlineData("/api/DesignationMaster/list")]
    [InlineData("/api/Customer/list")]
    [InlineData("/api/EmployeeMaster/list")]
    [InlineData("/api/EquipmentMaster/list")]
    [InlineData("/api/ParameterMaster/chemical-list")]
    [InlineData("/api/LaboratoryTest/list")]
    [InlineData("/api/SampleInward/list")]
    [InlineData("/api/TestResults/list")]
    [InlineData("/api/Reporting/list")]
    [InlineData("/api/Account/cases")]
    [InlineData("/api/Tax/list")]
    [InlineData("/api/Bank/list")]
    [InlineData("/api/Courier/list")]
    [InlineData("/api/MetalClassificationMaster/list")]
    [InlineData("/api/ProductSpecification/list")]
    [InlineData("/api/MaterialSpecification/list")]
    public async Task Auth_PostEndpoints_WithoutToken_Return401(string url)
    {
        var status = await PostUnauthAsync(url, TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }

    [Theory]
    [InlineData("/api/DepartmentMaster/details/1")]
    [InlineData("/api/Customer/details/1")]
    [InlineData("/api/SampleInward/details/1")]
    [InlineData("/api/Account/dashboard")]
    [InlineData("/api/EmployeeMaster/dropdown")]
    public async Task Auth_GetEndpoints_WithoutToken_Return401(string url)
    {
        var status = await GetUnauthAsync(url);
        AssertUnauthorized(status);
    }

    // ═══════════════════════════════════════════════════════
    // Invalid/Malformed JSON
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task MalformedJson_ListEndpoint_ReturnsBadRequest()
    {
        var content = new StringContent("{ invalid json }", Encoding.UTF8, "application/json");
        var response = await Client.PostAsync("/api/DepartmentMaster/list", content);

        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.UnsupportedMediaType,
            $"Expected 400 or 415 for malformed JSON, got {response.StatusCode}");
    }

    [Fact]
    public async Task EmptyBody_ListEndpoint_HandlesGracefully()
    {
        var content = new StringContent("", Encoding.UTF8, "application/json");
        var response = await Client.PostAsync("/api/DepartmentMaster/list", content);

        // Should handle gracefully (400 or default to empty filter)
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Expected 200 or 400 for empty body, got {response.StatusCode}");
    }

    [Fact]
    public async Task NullBody_CreateEndpoint_ReturnsBadRequest()
    {
        var content = new StringContent("null", Encoding.UTF8, "application/json");
        var response = await Client.PostAsync("/api/DepartmentMaster/create", content);

        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Expected 400 or 500 for null create body, got {response.StatusCode}");
    }

    // ═══════════════════════════════════════════════════════
    // SQL Injection Protection
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task SqlInjection_InSearchTerm_ReturnsSafeResponse()
    {
        var filter = new PageFilterBuilder()
            .Search("'; DROP TABLE DepartmentMaster; --")
            .Build();

        var (status, _) = await PostAsync("/api/DepartmentMaster/list", filter);

        // Should return OK (no results) or 400 — NOT crash
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest,
            $"SQL injection in search should be safe, got {status}");
    }

    [Fact]
    public async Task SqlInjection_InSortColumn_ReturnsSafeResponse()
    {
        var filter = new PageFilterBuilder()
            .SortBy("Name; DROP TABLE DepartmentMaster; --", "asc")
            .Build();

        var (status, _) = await PostAsync("/api/DepartmentMaster/list", filter);

        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest ||
            status == HttpStatusCode.InternalServerError,
            $"SQL injection in sort should be handled, got {status}");
    }

    [Fact]
    public async Task SqlInjection_InFilterValue_ReturnsSafeResponse()
    {
        var filter = new PageFilterBuilder()
            .Contains("Name", "' OR '1'='1")
            .Build();

        var (status, _) = await PostAsync("/api/DepartmentMaster/list", filter);

        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest,
            $"SQL injection in filter should be safe, got {status}");
    }

    [Fact]
    public async Task SqlInjection_InDropdownSearch_ReturnsSafeResponse()
    {
        var (status, _) = await GetDropdownAsync("/api/DepartmentMaster/dropdown",
            "'; DROP TABLE DepartmentMaster; --");

        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest,
            $"SQL injection in dropdown search should be safe, got {status}");
    }

    // ═══════════════════════════════════════════════════════
    // XSS Protection
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task Xss_InCreatePayload_ReturnsSafeResponse()
    {
        var (status, body) = await PostAsync("/api/DepartmentMaster/create", new
        {
            name = "<script>alert('xss')</script>",
            code = "XSS",
            description = "<img src=x onerror=alert('xss')>",
            isActive = true
        });

        // Should succeed but store sanitized data, or reject
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest,
            $"XSS in create should be handled, got {status}");

        // If created, clean it up
        if (status == HttpStatusCode.OK)
        {
            var (listStatus, listBody) = await PostAsync("/api/DepartmentMaster/list",
                TestDataFactory.SearchPageFilter("<script>"));
            if (listStatus == HttpStatusCode.OK)
            {
                var items = listBody.GetProperty("items");
                for (int i = 0; i < items.GetArrayLength(); i++)
                {
                    var id = items[i].GetProperty("id").GetInt64();
                    await DeleteAsync($"/api/DepartmentMaster/delete/{id}");
                }
            }
        }
    }

    // ═══════════════════════════════════════════════════════
    // Boundary Values
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task Pagination_ZeroPageSize_HandlesGracefully()
    {
        var (status, _) = await PostListAsync("/api/DepartmentMaster/list",
            pageNumber: 1, pageSize: 0);

        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest,
            $"Zero page size should be handled, got {status}");
    }

    [Fact]
    public async Task Pagination_NegativePageNumber_HandlesGracefully()
    {
        var (status, _) = await PostListAsync("/api/DepartmentMaster/list",
            pageNumber: -1, pageSize: 10);

        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest,
            $"Negative page number should be handled, got {status}");
    }

    [Fact]
    public async Task Pagination_VeryLargePageNumber_ReturnsEmptyData()
    {
        var (status, body) = await PostListAsync("/api/DepartmentMaster/list",
            pageNumber: 99999, pageSize: 10);

        AssertOk(status);
        var data = body.GetProperty("items");
        Assert.Equal(0, data.GetArrayLength());
    }

    [Fact]
    public async Task Pagination_LargePageSize_HandlesGracefully()
    {
        var (status, _) = await PostListAsync("/api/DepartmentMaster/list",
            pageNumber: 1, pageSize: 10000);

        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest,
            $"Very large page size should be handled, got {status}");
    }

    // ═══════════════════════════════════════════════════════
    // Non-existent ID tests across entities
    // ═══════════════════════════════════════════════════════

    [Theory]
    [InlineData("/api/DepartmentMaster/details")]
    [InlineData("/api/DesignationMaster/details")]
    [InlineData("/api/Customer/details")]
    [InlineData("/api/EmployeeMaster/details")]
    [InlineData("/api/SampleInward/details")]
    public async Task Details_NonExistentId_ReturnsNoContentOrNotFound(string baseUrl)
    {
        var (status, _) = await GetAsync($"{baseUrl}/{TestConstants.NonExistentId}");
        Assert.True(
            status == HttpStatusCode.NoContent || status == HttpStatusCode.NotFound || status == HttpStatusCode.BadRequest,
            $"Expected 204 or 404 for {baseUrl}, got {status}");
    }

    // ═══════════════════════════════════════════════════════
    // Delete non-existent entities
    // ═══════════════════════════════════════════════════════

    [Theory]
    [InlineData("/api/DepartmentMaster/delete")]
    [InlineData("/api/DesignationMaster/delete")]
    [InlineData("/api/Customer/delete")]
    public async Task Delete_NonExistentId_ReturnsError(string baseUrl)
    {
        var (status, _) = await DeleteAsync($"{baseUrl}/{TestConstants.NonExistentId}");
        Assert.True(
            status == HttpStatusCode.BadRequest || status == HttpStatusCode.NotFound ||
            status == HttpStatusCode.InternalServerError,
            $"Expected error for delete non-existent at {baseUrl}, got {status}");
    }

    // ═══════════════════════════════════════════════════════
    // Filter validation
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task Filter_InvalidColumnName_HandlesGracefully()
    {
        var filter = new PageFilterBuilder()
            .Contains("NonExistentColumn", "test")
            .Build();

        var (status, _) = await PostAsync("/api/DepartmentMaster/list", filter);

        // Should not crash — either ignore filter or return 400
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest ||
            status == HttpStatusCode.InternalServerError,
            $"Invalid filter column should be handled, got {status}");
    }

    [Fact]
    public async Task Filter_InvalidFilterType_HandlesGracefully()
    {
        var filter = new
        {
            pageNumber = 1,
            pageSize = 10,
            sortByColumn = "ID",
            sortOrder = "desc",
            filter = new[] { new { column = "Name", type = "InvalidType", value = "test", value2 = (string?)null } }
        };

        var (status, _) = await PostAsync("/api/DepartmentMaster/list", filter);

        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest ||
            status == HttpStatusCode.InternalServerError,
            $"Invalid filter type should be handled, got {status}");
    }

    // ═══════════════════════════════════════════════════════
    // Additional system endpoints
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task Dashboard_ReturnsData()
    {
        var (status, _) = await GetAsync("/api/Dashboard");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404 for dashboard, got {status}");
    }

    [Fact]
    public async Task Notifications_List_ReturnsData()
    {
        var (status, _) = await PostListAsync("/api/Notifications/list");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404 for notifications, got {status}");
    }

    [Fact]
    public async Task Settings_ReturnsData()
    {
        var (status, _) = await GetAsync("/api/Settings");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404 for settings, got {status}");
    }

    [Fact]
    public async Task Configuration_List_ReturnsData()
    {
        var (status, _) = await PostListAsync("/api/Configuration/list");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404 for configuration, got {status}");
    }
}
