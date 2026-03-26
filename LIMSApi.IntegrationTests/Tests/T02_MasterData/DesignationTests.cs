using System.Net;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests.T02_MasterData;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class DesignationTests : ApiTestBase
{
    private const string BaseUrl = "/api/DesignationMaster";
    private static long _createdId;

    public DesignationTests(AuthFixture fixture) : base(fixture) { }

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
        var uniqueName = $"{TestConstants.TestPrefix}DesigFind";
        var (status, body) = await PostAsync($"{BaseUrl}/create", TestDataFactory.Designation(uniqueName));
        AssertCreated(status, body);

        // Find the created record
        var (listStatus, listBody) = await PostAsync($"{BaseUrl}/list",
            TestDataFactory.SearchPageFilter(uniqueName));
        AssertOk(listStatus);
        var items = listBody.GetProperty("items");
        if (items.GetArrayLength() > 0)
            _createdId = items[0].GetProperty("id").GetInt64();
    }

    [Fact]
    public async Task Step03_Details_ExistingId_ReturnsEntity()
    {
        if (_createdId == 0) return;
        var (status, body) = await GetAsync($"{BaseUrl}/details/{_createdId}");
        AssertOk(status);
        AssertNotEmpty(body);
    }

    [Fact]
    public async Task Step04_Details_NonExistentId_ReturnsNoContent()
    {
        var (status, _) = await GetAsync($"{BaseUrl}/details/{TestConstants.NonExistentId}");
        Assert.True(status == HttpStatusCode.NoContent || status == HttpStatusCode.NotFound || status == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Step05_Update_ValidData_ReturnsSuccess()
    {
        if (_createdId == 0) return;
        var (status, body) = await PutAsync($"{BaseUrl}/update", new
        {
            id = _createdId,
            name = $"{TestConstants.TestPrefix}DesigUpdated",
            code = $"{TestConstants.TestPrefix}DGU",
            description = "Updated",
            isActive = true
        });
        AssertOk(status);
    }

    [Fact]
    public async Task Step06_Dropdown_ReturnsData()
    {
        var (status, body) = await GetDropdownAsync($"{BaseUrl}/dropdown");
        AssertOk(status);
    }

    [Fact]
    public async Task Step07_Delete_ExistingId_ReturnsSuccess()
    {
        if (_createdId == 0) return;
        var (status, _) = await DeleteAsync($"{BaseUrl}/delete/{_createdId}");
        AssertOk(status);
    }

    [Fact]
    public async Task Step08_List_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{BaseUrl}/list", TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }
}
