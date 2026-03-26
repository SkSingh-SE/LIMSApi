using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace LIMSApi.IntegrationTests.Infrastructure;

/// <summary>
/// Shared fixture that logs in once and provides an authenticated HttpClient for all tests.
/// Uses ICollectionFixture so xUnit creates only ONE instance for the entire test run.
/// </summary>
public class AuthFixture : IAsyncLifetime
{
    public HttpClient Client { get; private set; } = null!;
    public HttpClient UnauthenticatedClient { get; private set; } = null!;
    public string Token { get; private set; } = string.Empty;
    public string BaseUrl { get; private set; } = string.Empty;
    public long UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public bool IsAdmin { get; private set; }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task InitializeAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", optional: false)
            .Build();

        BaseUrl = config["ApiBaseUrl"]!;
        var email = config["TestCredentials:Email"]!;
        var password = config["TestCredentials:Password"]!;

        // SSL bypass for dev certs
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

        // Unauthenticated client for negative tests
        UnauthenticatedClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

        // Authenticated client
        Client = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Client.Timeout = TimeSpan.FromSeconds(30);

        // Login with retry (handles rate limiting)
        HttpResponseMessage? loginResponse = null;
        for (int attempt = 1; attempt <= 5; attempt++)
        {
            loginResponse = await Client.PostAsJsonAsync("/api/Auth/login",
                new { Email = email, Password = password });

            if (loginResponse.IsSuccessStatusCode)
                break;

            if (loginResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var retryAfter = loginResponse.Headers.RetryAfter?.Delta?.TotalSeconds ?? 30;
                await Task.Delay(TimeSpan.FromSeconds(Math.Max(retryAfter, 15)));
                continue;
            }

            // Non-rate-limit error — fail immediately
            break;
        }

        if (loginResponse == null || !loginResponse.IsSuccessStatusCode)
        {
            var errorBody = loginResponse != null
                ? await loginResponse.Content.ReadAsStringAsync()
                : "No response";
            throw new InvalidOperationException(
                $"Login failed with status {loginResponse?.StatusCode}. " +
                $"Ensure backend is running at {BaseUrl}. Response: {errorBody}");
        }

        var responseJson = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<JsonElement>(responseJson, JsonOptions);

        Token = loginResult.GetProperty("token").GetString()!;
        UserId = loginResult.GetProperty("userId").GetInt64();
        UserName = loginResult.GetProperty("userName").GetString() ?? "";
        IsAdmin = loginResult.GetProperty("isAdmin").GetBoolean();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
    }

    public Task DisposeAsync()
    {
        Client?.Dispose();
        UnauthenticatedClient?.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition(TestConstants.ApiTestCollection)]
public class ApiTestCollectionDefinition : ICollectionFixture<AuthFixture> { }
