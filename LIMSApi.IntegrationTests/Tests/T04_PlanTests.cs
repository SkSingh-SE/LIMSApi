using System.Net;
using System.Text.Json;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class T04_PlanTests : ApiTestBase
{
    private const string InwardUrl = "/api/SampleInward";
    private const string PlanUrl = "/api/Plan";

    public T04_PlanTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_PlanList_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{InwardUrl}/plan-list");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step02_ReviewList_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{InwardUrl}/review-list");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step03_PlanList_WithSearch_ReturnsData()
    {
        var filter = new PageFilterBuilder()
            .Page(1, 10)
            .SortBy("ID", "desc")
            .Build();

        var (status, body) = await PostAsync($"{InwardUrl}/plan-list", filter);
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step04_PlanList_WithPagination_ReturnsData()
    {
        var (status1, body1) = await PostListAsync($"{InwardUrl}/plan-list", pageNumber: 1, pageSize: 5);
        AssertOk(status1);
        AssertPagedResponse(body1);
    }

    [Fact]
    public async Task Step05_ReviewList_WithSorting_ReturnsData()
    {
        var filter = new PageFilterBuilder()
            .Page(1, 10)
            .SortBy("ID", "asc")
            .Build();

        var (status, body) = await PostAsync($"{InwardUrl}/review-list", filter);
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step06_PlanList_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{InwardUrl}/plan-list", TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }
}
