using System.Text;
using System.Text.Json;

namespace Web.Services.Api;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AuthApiClient(HttpClient http, ILogger<AuthApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync(string email, string password)
    {
        var json = JsonSerializer.Serialize(new { email, password });
        var res = await _http.PostAsync("api/auth/token", new StringContent(json, Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("Auth token request failed with status {StatusCode}", (int)res.StatusCode);
            return null;
        }

        var body = await res.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<TokenResponse>(body, _jsonOptions);
        if (!string.IsNullOrWhiteSpace(dto?.Token)) return dto.Token;

        // Fallback: read token property directly to avoid deserialization mismatches.
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("token", out var tokenProp))
            {
                var token = tokenProp.GetString();
                if (!string.IsNullOrWhiteSpace(token)) return token;
            }
            if (doc.RootElement.TryGetProperty("Token", out var tokenPropAlt))
            {
                var token = tokenPropAlt.GetString();
                if (!string.IsNullOrWhiteSpace(token)) return token;
            }
        }
        catch (JsonException)
        {
            // Ignore parsing errors; we'll return null below.
        }

        _logger.LogWarning("Auth token missing in response. ContentType={ContentType}, Length={Length}",
            res.Content.Headers.ContentType?.ToString() ?? "unknown",
            body.Length);
        return null;
    }

    private sealed record TokenResponse(string Token);
}
