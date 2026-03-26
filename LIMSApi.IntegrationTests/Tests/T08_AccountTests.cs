using System.Net;
using System.Text.Json;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class T08_AccountTests : ApiTestBase
{
    private const string BaseUrl = "/api/Account";
    private static long _inwardId;
    private static long _customerId;

    public T08_AccountTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_Dashboard_ReturnsData()
    {
        var (status, body) = await GetAsync($"{BaseUrl}/dashboard");
        AssertOk(status);
    }

    [Fact]
    public async Task Step02_Cases_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/cases");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step03_Cases_GetFirstCase()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/cases", pageSize: 1);
        AssertOk(status);

        var data = body.GetProperty("items");
        if (data.GetArrayLength() > 0)
        {
            var first = data[0];
            if (first.TryGetProperty("id", out var id) ||
                first.TryGetProperty("inwardId", out id) ||
                first.TryGetProperty("inwardID", out id))
                _inwardId = id.GetInt64();
            if (first.TryGetProperty("customerId", out var cid) ||
                first.TryGetProperty("customerID", out cid))
                _customerId = cid.GetInt64();
        }
    }

    [Fact]
    public async Task Step04_CaseSummary_ReturnsData()
    {
        if (_inwardId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/cases/{_inwardId}/summary");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404, got {status}");
    }

    [Fact]
    public async Task Step05_CasePayments_ReturnsData()
    {
        if (_inwardId == 0) return;

        var (status, body) = await PostListAsync($"{BaseUrl}/cases/{_inwardId}/payments");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404, got {status}");
    }

    [Fact]
    public async Task Step06_ValidatePricing_ReturnsData()
    {
        if (_inwardId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/cases/{_inwardId}/validate-pricing");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest ||
            status == HttpStatusCode.NotFound,
            $"Expected 200/400/404 for validate pricing, got {status}");
    }

    [Fact]
    public async Task Step07_PaymentGate_ReturnsData()
    {
        if (_inwardId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/payment-gate/{_inwardId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404 for payment gate, got {status}");
    }

    [Fact]
    public async Task Step08_CreditCheck_ReturnsData()
    {
        if (_customerId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/credit-check/{_customerId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404 for credit check, got {status}");
    }

    [Fact]
    public async Task Step09_LedgerSummary_ReturnsData()
    {
        if (_customerId == 0) return;

        var (status, body) = await GetAsync(
            $"{BaseUrl}/ledger-summary/{_customerId}?periodStart=2025-01-01&periodEnd=2025-12-31");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404 for ledger summary, got {status}");
    }

    [Fact]
    public async Task Step10_Cases_WithSorting_ReturnsData()
    {
        var filter = new PageFilterBuilder()
            .Page(1, 10)
            .SortBy("ID", "desc")
            .Build();

        var (status, body) = await PostAsync($"{BaseUrl}/cases", filter);
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step11_CaseSummary_NonExistent_ReturnsError()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/cases/{TestConstants.NonExistentId}/summary");
        Assert.True(
            status == HttpStatusCode.NotFound || status == HttpStatusCode.BadRequest ||
            status == HttpStatusCode.NoContent || status == HttpStatusCode.InternalServerError,
            $"Expected 404/400/204/500, got {status}");
    }

    [Fact]
    public async Task Step12_Dashboard_WithoutAuth_Returns401()
    {
        var status = await GetUnauthAsync($"{BaseUrl}/dashboard");
        AssertUnauthorized(status);
    }

    [Fact]
    public async Task Step13_Cases_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{BaseUrl}/cases", TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }
}
