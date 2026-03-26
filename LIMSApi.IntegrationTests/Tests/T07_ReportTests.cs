using System.Net;
using System.Text.Json;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class T07_ReportTests : ApiTestBase
{
    private const string BaseUrl = "/api/Reporting";
    private static long _reportId;

    public T07_ReportTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_List_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/list", sortByColumn: "ReportNo");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step02_List_WithSorting_ReturnsData()
    {
        var filter = new PageFilterBuilder()
            .Page(1, 10)
            .SortBy("ReportNo", "desc")
            .Build();

        var (status, body) = await PostAsync($"{BaseUrl}/list", filter);
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step03_List_GetFirstReport()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/list", pageSize: 1, sortByColumn: "ReportNo");
        AssertOk(status);

        var data = body.GetProperty("items");
        if (data.GetArrayLength() > 0)
        {
            var first = data[0];
            if (first.TryGetProperty("id", out var id))
                _reportId = id.GetInt64();
        }
    }

    [Fact]
    public async Task Step04_GetReport_ReturnsData()
    {
        if (_reportId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/{_reportId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NoContent,
            $"Expected 200 or 204, got {status}");
    }

    [Fact]
    public async Task Step05_PreviewData_ReturnsData()
    {
        if (_reportId == 0) return;

        var (status, body) = await GetAsync($"{BaseUrl}/{_reportId}/preview-data");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404, got {status}");
    }

    [Fact]
    public async Task Step06_ReportPreview_ReturnsData()
    {
        if (_reportId == 0) return;

        var (status, _) = await GetAsync($"{BaseUrl}/preview/{_reportId}");
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.NotFound,
            $"Expected 200 or 404 for preview, got {status}");
    }

    [Fact]
    public async Task Step07_Formats_ReturnsData()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/formats");
        AssertOk(status);
    }

    [Fact]
    public async Task Step08_GetReport_NonExistent_ReturnsError()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/{TestConstants.NonExistentId}");
        Assert.True(
            status == HttpStatusCode.NotFound || status == HttpStatusCode.NoContent ||
            status == HttpStatusCode.BadRequest,
            $"Expected 404/204/400, got {status}");
    }

    [Fact]
    public async Task Step09_List_WithPagination_ReturnsData()
    {
        var (status1, body1) = await PostListAsync($"{BaseUrl}/list", pageNumber: 1, pageSize: 3, sortByColumn: "ReportNo");
        AssertOk(status1);
        AssertPagedResponse(body1);
    }

    [Fact]
    public async Task Step10_List_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{BaseUrl}/list", TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }
}
