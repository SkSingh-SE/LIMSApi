namespace LIMSApi.IntegrationTests.Infrastructure;

public static class TestConstants
{
    // Collection names for xUnit test ordering
    public const string ApiTestCollection = "ApiTests";

    // Unique prefix to identify test-created data
    public static readonly string TestPrefix = $"_TEST_{DateTime.Now:HHmmss}_";

    // Default page filter values
    public const int DefaultPageSize = 10;
    public const int DefaultPageNumber = 1;

    // Non-existent ID for negative tests
    public const long NonExistentId = 999999999;
}
