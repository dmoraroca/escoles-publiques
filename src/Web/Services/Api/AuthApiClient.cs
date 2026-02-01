using System.Text;
using System.Text.Json;

namespace Web.Services.Api;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AuthApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> GetTokenAsync(string email, string password)
    {
        var json = JsonSerializer.Serialize(new { email, password });
        var res = await _http.PostAsync("api/auth/token", new StringContent(json, Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;

        var body = await res.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<TokenResponse>(body, _jsonOptions);
        return dto?.Token;
    }

    private sealed record TokenResponse(string Token);
}
