using System.Net;
using LIMSApi.IntegrationTests.Helpers;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class T06_VerificationTests : ApiTestBase
{
    private const string BaseUrl = "/api/TestResults";

    public T06_VerificationTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_VerificationList_ReturnsPagedResponse()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/verification-list");
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step02_VerificationList_WithSorting_ReturnsData()
    {
        var filter = new PageFilterBuilder()
            .Page(1, 10)
            .SortBy("ID", "desc")
            .Build();

        var (status, body) = await PostAsync($"{BaseUrl}/verification-list", filter);
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step03_VerificationList_WithPagination_ReturnsData()
    {
        var (status, body) = await PostListAsync($"{BaseUrl}/verification-list",
            pageNumber: 1, pageSize: 5);
        AssertOk(status);
        AssertPagedResponse(body);
    }

    [Fact]
    public async Task Step04_VerificationList_WithoutAuth_Returns401()
    {
        var status = await PostUnauthAsync($"{BaseUrl}/verification-list",
            TestDataFactory.DefaultPageFilter());
        AssertUnauthorized(status);
    }
}
