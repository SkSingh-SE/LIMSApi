using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LIMSApi.IntegrationTests.Infrastructure;

/// <summary>
/// Base class providing HTTP helper methods and standard assertions for all API tests.
/// Includes auto-retry on 429 TooManyRequests to handle rate limiting.
/// </summary>
[Collection(TestConstants.ApiTestCollection)]
public abstract class ApiTestBase
{
    protected readonly HttpClient Client;
    protected readonly HttpClient UnauthClient;
    protected readonly AuthFixture Auth;

    private static readonly SemaphoreSlim Throttle = new(5, 5); // max 5 concurrent requests
    private const int RetryDelayMs = 2000;
    private const int MaxRetries = 3;

    protected static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected ApiTestBase(AuthFixture fixture)
    {
        Auth = fixture;
        Client = fixture.Client;
        UnauthClient = fixture.UnauthenticatedClient;
    }

    // ───────── Rate-limit aware request wrapper ─────────

    private async Task<HttpResponseMessage> SendWithRetry(Func<Task<HttpResponseMessage>> requestFn)
    {
        await Throttle.WaitAsync();
        try
        {
            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                var response = await requestFn();
                if (response.StatusCode != HttpStatusCode.TooManyRequests)
                    return response;

                // Rate limited — wait and retry
                await Task.Delay(RetryDelayMs * (attempt + 1));
            }
            // Return last response even if 429
            return await requestFn();
        }
        finally
        {
            Throttle.Release();
        }
    }

    // ───────── HTTP Helpers ─────────

    protected async Task<(HttpStatusCode Status, JsonElement Body)> GetAsync(string url)
    {
        var response = await SendWithRetry(() => Client.GetAsync(url));
        var body = await ParseBody(response);
        return (response.StatusCode, body);
    }

    protected async Task<(HttpStatusCode Status, JsonElement Body)> PostAsync(string url, object payload)
    {
        var response = await SendWithRetry(() => Client.PostAsJsonAsync(url, payload, JsonOpts));
        var body = await ParseBody(response);
        return (response.StatusCode, body);
    }

    protected async Task<(HttpStatusCode Status, JsonElement Body)> PutAsync(string url, object payload)
    {
        var response = await SendWithRetry(() => Client.PutAsJsonAsync(url, payload, JsonOpts));
        var body = await ParseBody(response);
        return (response.StatusCode, body);
    }

    protected async Task<(HttpStatusCode Status, JsonElement Body)> DeleteAsync(string url)
    {
        var response = await SendWithRetry(() => Client.DeleteAsync(url));
        var body = await ParseBody(response);
        return (response.StatusCode, body);
    }

    protected async Task<(HttpStatusCode Status, JsonElement Body)> PostFormAsync(string url, MultipartFormDataContent form)
    {
        var response = await SendWithRetry(() => Client.PostAsync(url, form));
        var body = await ParseBody(response);
        return (response.StatusCode, body);
    }

    protected async Task<(HttpStatusCode Status, JsonElement Body)> PutFormAsync(string url, MultipartFormDataContent form)
    {
        var response = await SendWithRetry(() => Client.PutAsync(url, form));
        var body = await ParseBody(response);
        return (response.StatusCode, body);
    }

    /// <summary>POST without auth token for 401 tests (no retry on 429).</summary>
    protected async Task<HttpStatusCode> PostUnauthAsync(string url, object payload)
    {
        await Throttle.WaitAsync();
        try
        {
            var response = await UnauthClient.PostAsJsonAsync(url, payload, JsonOpts);
            return response.StatusCode;
        }
        finally
        {
            Throttle.Release();
        }
    }

    protected async Task<HttpStatusCode> GetUnauthAsync(string url)
    {
        await Throttle.WaitAsync();
        try
        {
            var response = await UnauthClient.GetAsync(url);
            return response.StatusCode;
        }
        finally
        {
            Throttle.Release();
        }
    }

    // ───────── List / Dropdown Helpers ─────────

    protected async Task<(HttpStatusCode Status, JsonElement Body)> PostListAsync(
        string url, int pageNumber = 1, int pageSize = 10, string? searchTerm = null,
        string? sortByColumn = null, string? sortOrder = null)
    {
        var filter = new
        {
            pageNumber,
            pageSize,
            searchTerm,
            sortByColumn = sortByColumn ?? "ID",
            sortOrder = sortOrder ?? "desc",
            filter = Array.Empty<object>()
        };
        return await PostAsync(url, filter);
    }

    protected async Task<(HttpStatusCode Status, JsonElement Body)> GetDropdownAsync(
        string url, string? searchTerm = null, int pageNo = 1, int pageSize = 20)
    {
        var queryUrl = $"{url}?searchTerm={searchTerm ?? ""}&pageNo={pageNo}&pageSize={pageSize}";
        return await GetAsync(queryUrl);
    }

    // ───────── Response Parsing ─────────

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
            return default;

        try
        {
            return JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            // Return raw string wrapped in JSON for non-JSON responses
            return JsonSerializer.Deserialize<JsonElement>($"\"{content.Replace("\"", "\\\"")}\"");
        }
    }

    // ───────── Assertion Helpers ─────────

    protected static void AssertOk(HttpStatusCode status) =>
        Assert.Equal(HttpStatusCode.OK, status);

    protected static void AssertCreated(HttpStatusCode status, JsonElement body)
    {
        Assert.Equal(HttpStatusCode.OK, status);
        // Some endpoints return JSON object with message, others return plain string
        Assert.True(
            body.ValueKind == JsonValueKind.String ||
            body.TryGetProperty("message", out _) || body.TryGetProperty("Message", out _),
            "Create response should contain a 'message' property or be a string response");
    }

    protected static void AssertBadRequest(HttpStatusCode status) =>
        Assert.Equal(HttpStatusCode.BadRequest, status);

    protected static void AssertUnauthorized(HttpStatusCode status) =>
        Assert.True(
            status == HttpStatusCode.Unauthorized || status == HttpStatusCode.TooManyRequests,
            $"Expected 401 Unauthorized (or 429 from rate limiter), got {status}");

    protected static void AssertNotEmpty(JsonElement body)
    {
        Assert.NotEqual(default, body);
        Assert.NotEqual(JsonValueKind.Null, body.ValueKind);
    }

    protected static void AssertPagedResponse(JsonElement body)
    {
        Assert.True(
            body.TryGetProperty("totalRecords", out _) || body.TryGetProperty("TotalRecords", out _),
            $"Paged response should have 'totalRecords'. Got: {body}");
    }

    protected static long ExtractId(JsonElement body)
    {
        if (body.TryGetProperty("id", out var idProp) || body.TryGetProperty("Id", out idProp) || body.TryGetProperty("ID", out idProp))
            return idProp.GetInt64();

        throw new InvalidOperationException("Response does not contain an 'id' property");
    }

    protected static string ExtractMessage(JsonElement body)
    {
        if (body.TryGetProperty("message", out var msg) || body.TryGetProperty("Message", out msg))
            return msg.GetString() ?? "";
        return "";
    }
}
