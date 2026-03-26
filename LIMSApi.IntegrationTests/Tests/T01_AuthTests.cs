using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LIMSApi.IntegrationTests.Infrastructure;

namespace LIMSApi.IntegrationTests.Tests;

[Collection(TestConstants.ApiTestCollection)]
[TestCaseOrderer("LIMSApi.IntegrationTests.Infrastructure.AlphabeticalOrderer",
    "LIMSApi.IntegrationTests")]
public class T01_AuthTests : ApiTestBase
{
    public T01_AuthTests(AuthFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Step01_Login_ValidCredentials_ReturnsToken()
    {
        var (status, body) = await PostAsync("/api/Auth/login", new
        {
            email = "sksingh@gmail.com",
            password = "Sksingh@123"
        });

        AssertOk(status);
        Assert.True(body.TryGetProperty("token", out var token), "Response should contain 'token'");
        Assert.False(string.IsNullOrEmpty(token.GetString()), "Token should not be empty");
        Assert.True(body.TryGetProperty("userId", out _), "Response should contain 'userId'");
        Assert.True(body.TryGetProperty("userName", out _), "Response should contain 'userName'");
        Assert.True(body.TryGetProperty("email", out _), "Response should contain 'email'");
        Assert.True(body.TryGetProperty("role", out _), "Response should contain 'role'");
        Assert.True(body.TryGetProperty("expiresInSeconds", out var expires), "Response should contain 'expiresInSeconds'");
        Assert.True(expires.GetInt32() > 0, "Token expiry should be positive");
    }

    [Fact]
    public async Task Step02_Login_WrongPassword_Returns401()
    {
        var response = await UnauthClient.PostAsJsonAsync("/api/Auth/login", new
        {
            email = "sksingh@gmail.com",
            password = "WrongPassword@999"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.True(body.TryGetProperty("success", out var success));
        Assert.False(success.GetBoolean());
    }

    [Fact]
    public async Task Step03_Login_EmptyEmail_ReturnsError()
    {
        var response = await UnauthClient.PostAsJsonAsync("/api/Auth/login", new
        {
            email = "",
            password = "admin123"
        });

        // Should be 400 or 401 depending on validation
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected 400 or 401, got {response.StatusCode}");
    }

    [Fact]
    public async Task Step04_Login_EmptyPassword_ReturnsError()
    {
        var response = await UnauthClient.PostAsJsonAsync("/api/Auth/login", new
        {
            email = "admin@lims.com",
            password = ""
        });

        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected 400 or 401, got {response.StatusCode}");
    }

    [Fact]
    public async Task Step05_Login_NullBody_ReturnsBadRequest()
    {
        var response = await UnauthClient.PostAsync("/api/Auth/login",
            new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected 400 or 401 for empty body, got {response.StatusCode}");
    }

    [Fact]
    public async Task Step06_RefreshToken_WithValidAuth_ReturnsNewToken()
    {
        var (status, body) = await GetAsync("/api/Auth/refresh-token");

        AssertOk(status);
        // Refresh should return a token response
        Assert.True(
            body.TryGetProperty("token", out _) || body.ValueKind == JsonValueKind.String,
            "Refresh should return a token");
    }

    [Fact]
    public async Task Step07_RefreshToken_WithoutAuth_ReturnsNonSuccess()
    {
        var status = await GetUnauthAsync("/api/Auth/refresh-token");
        // AuthController is [AllowAnonymous], refresh without token returns 204/401/500
        Assert.NotEqual(HttpStatusCode.OK, status);
    }

    [Fact]
    public Task Step08_AuthFixture_HasValidToken()
    {
        Assert.False(string.IsNullOrEmpty(Auth.Token), "AuthFixture should have a valid token");
        Assert.True(Auth.UserId > 0, "AuthFixture should have a valid UserId");
        Assert.False(string.IsNullOrEmpty(Auth.UserName), "AuthFixture should have a UserName");
        return Task.CompletedTask;
    }
}
