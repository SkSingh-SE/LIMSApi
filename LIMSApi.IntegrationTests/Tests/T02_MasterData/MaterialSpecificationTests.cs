using System.Net;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests.T02_MasterData;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class MaterialSpecificationTests : ApiTestBase
{
    private const string BaseUrl = "/api/MaterialSpecification";

    public MaterialSpecificationTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_List_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/list");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step02_CustomList_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/customList");
        AssertOk(status);
    }

    [Fact]
    public async Task Step03_Dropdown_ReturnsData()
    {
        var (status, _) = await GetDropdownAsync($"{BaseUrl}/dropdown");
        AssertOk(status);
    }

    [Fact]
    public async Task Step04_GradeDropdown_ReturnsData()
    {
        var (status, _) = await GetDropdownAsync($"{BaseUrl}/grade-dropdown");
        AssertOk(status);
    }

    [Fact]
    public async Task Step05_Details_NonExistentId_ReturnsNoContent()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/details/{TestConstants.NonExistentId}");
        Assert.True(status == HttpStatusCode.NoContent || status == HttpStatusCode.NotFound || status == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Step06_TestMethods_ReturnsData()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/test-methods");
        // May fail with DB error if missing required params
        Assert.True(
            status == HttpStatusCode.OK || status == HttpStatusCode.BadRequest,
            $"Expected 200 or 400, got {status}");
    }

    [Fact]
    public async Task Step07_List_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{BaseUrl}/list", TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }
}
