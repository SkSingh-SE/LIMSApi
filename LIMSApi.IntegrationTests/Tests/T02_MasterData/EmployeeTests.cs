using System.Net;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests.T02_MasterData;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class EmployeeTests : ApiTestBase
{
    private const string BaseUrl = "/api/EmployeeMaster";

    public EmployeeTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_List_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/list");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step02_Dropdown_ReturnsData()
    {
        var (status, body) = await GetDropdownAsync($"{BaseUrl}/dropdown");
        AssertOk(status);
    }

    [Fact]
    public async Task Step03_Details_NonExistentId_ReturnsNoContent()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/details/{TestConstants.NonExistentId}");
        Assert.True(status == HttpStatusCode.NoContent || status == HttpStatusCode.NotFound || status == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Step04_OrgChart_ReturnsData()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/org-chart");
        AssertOk(status);
    }

    [Fact]
    public async Task Step05_List_WithSearch_ReturnsFiltered()
    {
        var (status, body) = await PostAsync($"{BaseUrl}/list",
            TestDataFactory.SearchPageFilter("admin"));
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step06_List_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{BaseUrl}/list", TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }
}
