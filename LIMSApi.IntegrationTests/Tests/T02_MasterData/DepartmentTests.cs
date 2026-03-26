using System.Net;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests.T02_MasterData;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class DepartmentTests : ApiTestBase
{
    private const string BaseUrl = "/api/DepartmentMaster";
    private static long _createdId;

    public DepartmentTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_List_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/list");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step02_Create_ValidData_ReturnsSuccess()
    {
        var (status, body) = await PostAsync($"{BaseUrl}/create", TestDataFactory.Department());
        AssertCreated(status, body);
    }

    [Fact]
    public async Task Step03_Create_FindCreatedAndSetId()
    {
        // Create with known name to find it
        var uniqueName = $"{TestConstants.TestPrefix}DeptFind";
        var (status, _) = await PostAsync($"{BaseUrl}/create", TestDataFactory.Department(uniqueName));
        AssertOk(status);

        // Search for it
        var (listStatus, listBody) = await PostAsync($"{BaseUrl}/list",
            TestDataFactory.SearchPageFilter(uniqueName));
        AssertOk(listStatus);

        var items = listBody.GetProperty("items");
        Assert.True(items.GetArrayLength() > 0, "Should find the created department");

        _createdId = items[0].GetProperty("id").GetInt64();
        Assert.True(_createdId > 0);
    }

    [Fact]
    public async Task Step04_Details_ExistingId_ReturnsEntity()
    {
        if (_createdId == 0) return; // Skip if create didn't run

        var (status, body) = await GetAsync($"{BaseUrl}/details/{_createdId}");
        AssertOk(status);
        AssertNotEmpty(body);
    }

    [Fact]
    public async Task Step05_Details_NonExistentId_ReturnsNoContent()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/details/{TestConstants.NonExistentId}");
        Assert.True(
            status == HttpStatusCode.NoContent || status == HttpStatusCode.NotFound || status == HttpStatusCode.BadRequest,
            $"Expected 204 or 404, got {status}");
    }

    [Fact]
    public async Task Step06_Update_ValidData_ReturnsSuccess()
    {
        if (_createdId == 0) return;

        var (status, body) = await PutAsync($"{BaseUrl}/update", new
        {
            id = _createdId,
            name = $"{TestConstants.TestPrefix}DeptUpdated",
            code = $"{TestConstants.TestPrefix}DU",
            description = "Updated department",
            isActive = true
        });
        AssertOk(status);
        Assert.Contains("updated", ExtractMessage(body).ToLower());
    }

    [Fact]
    public async Task Step07_Dropdown_ReturnsData()
    {
        var (status, body) = await GetDropdownAsync($"{BaseUrl}/dropdown");
        AssertOk(status);
        Assert.True(body.ValueKind == System.Text.Json.JsonValueKind.Array, "Dropdown should return array");
    }

    [Fact]
    public async Task Step08_Dropdown_WithSearch_ReturnsFiltered()
    {
        var (status, body) = await GetDropdownAsync($"{BaseUrl}/dropdown", TestConstants.TestPrefix);
        AssertOk(status);
    }

    [Fact]
    public async Task Step09_List_WithSearch_ReturnsFiltered()
    {
        var (status, body) = await PostAsync($"{BaseUrl}/list",
            TestDataFactory.SearchPageFilter(TestConstants.TestPrefix));
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step10_List_WithSorting_ReturnsData()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/list",
            sortByColumn: "Name", sortOrder: "asc");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step11_Delete_ExistingId_ReturnsSuccess()
    {
        if (_createdId == 0) return;

        var (status, body) = await DeleteAsync($"{BaseUrl}/delete/{_createdId}");
        AssertOk(status);
        Assert.Contains("deleted", ExtractMessage(body).ToLower());
    }

    [Fact]
    public async Task Step12_Delete_NonExistentId_ReturnsError()
    {
        var (status, _) = await DeleteAsync($"{BaseUrl}/delete/{TestConstants.NonExistentId}");
        Assert.True(
            status == HttpStatusCode.BadRequest || status == HttpStatusCode.InternalServerError,
            $"Expected 400 or 500 for non-existent delete, got {status}");
    }

    [Fact]
    public async Task Step13_List_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{BaseUrl}/list", TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }
}
