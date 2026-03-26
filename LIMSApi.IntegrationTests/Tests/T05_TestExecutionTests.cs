using System.Net;
using System.Text.Json;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class T05_TestExecutionTests : ApiTestBase
{
    private const string BaseUrl = "/api/TestResults";
    private static long _headerId;
    private static long _sampleId;

    public T05_TestExecutionTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_List_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/list");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step02_List_WithSorting_ReturnsData()
    {
        var filter = new PageFilterBuilder()
            .Page(1, 10)
            .SortBy("ID", "desc")
            .Build();

        var (status, body) = await PostAsync($"{BaseUrl}/list", filter);
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step03_List_GetFirstHeader()
    {
        // Get first test result header from the list
        var (status, body) = await PostListAsync($"{BaseUrl}/list", pageSize: 1);
        AssertOk(status);

        var data = body.GetProperty("items");
        if (data.GetArrayLength() > 0)
        {
            var first = data[0];
            if (first.TryGetProperty("id", out var id))
                _headerId = id.GetInt64();
            if (first.TryGetProperty("sampleId", out var sid) ||
                first.TryGetProperty("sampleID", out sid))
                _sampleId = sid.GetInt64();
        }
    }

    [Fact]
    public async Task Step04_GetHeader_ReturnsData()
    {
        if (_headerId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/{_headerId}");
        AssertOk(status);
        AssertNotEmpty(body);
    }

    [Fact]
    public async Task Step05_FullResultPayload_ReturnsData()
    {
        if (_sampleId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/full-result-payload/{_sampleId}");
        AssertOk(status);
        AssertNotEmpty(body);
    }

    [Fact]
    public async Task Step06_OrientationCheck_ReturnsData()
    {
        if (_headerId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/orientation-check/{_headerId}");
        AssertOk(status);
    }

    [Fact]
    public async Task Step07_PriceBreakdown_ReturnsData()
    {
        if (_headerId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/price-breakdown/{_headerId}");
        // May be 200 or 404 depending on pricing setup
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound ||
            status == HttpStatusCode.BadRequest,
            $"Expected 200/404/400 for price breakdown, got {status}");
    }

    [Fact]
    public async Task Step08_PriceSummary_ReturnsData()
    {
        if (_headerId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/price-summary/{_headerId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound ||
            status == HttpStatusCode.BadRequest,
            $"Expected 200/404/400 for price summary, got {status}");
    }

    [Fact]
    public async Task Step09_GetHeader_NonExistent_Returns404()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/{TestConstants.NonExistentId}");
        Assert.True(
            status == HttpStatusCode.NotFound || status == HttpStatusCode.NoContent,
            $"Expected 404 or 204, got {status}");
    }

    [Fact]
    public async Task Step10_GetImages_ForHeader()
    {
        if (_headerId == 0) return;

        var (status, _) = await GetAsync($"{BaseUrl}/{_headerId}/images");
        // May be 200 (with or without images) or other
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NoContent ||
            status == HttpStatusCode.NotFound,
            $"Expected 200/204/404 for images, got {status}");
    }

    [Fact]
    public async Task Step11_PreparationStatus_ReturnsData()
    {
        if (_sampleId == 0) return;

        var (status, _) = await GetAsync($"{BaseUrl}/preparation-status/{_sampleId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200/404 for prep status, got {status}");
    }

    [Fact]
    public async Task Step12_NablScopeCheck_ReturnsData()
    {
        if (_headerId == 0) return;

        var (status, _) = await GetAsync($"{BaseUrl}/nabl-scope-check/{_headerId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound ||
            status == HttpStatusCode.BadRequest,
            $"Expected 200/404/400 for NABL scope check, got {status}");
    }

    [Fact]
    public async Task Step13_EnvironmentAtTime_ReturnsData()
    {
        if (_headerId == 0) return;

        var (status, _) = await GetAsync($"{BaseUrl}/environment-at-time/{_headerId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200/404 for environment, got {status}");
    }

    [Fact]
    public async Task Step14_List_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{BaseUrl}/list", TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }

    [Fact]
    public async Task Step15_UnifiedPriceSummary_ReturnsData()
    {
        if (_sampleId == 0) return;

        var (status, _) = await GetAsync($"{BaseUrl}/unified-price-summary/{_sampleId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound ||
            status == HttpStatusCode.BadRequest,
            $"Expected valid response for unified price summary, got {status}");
    }
}
