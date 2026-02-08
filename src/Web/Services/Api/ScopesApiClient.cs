using System.Text.Json;

namespace Web.Services.Api;

public class ScopesApiClient : IScopesApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ScopesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ApiScope>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/scopes");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/scopes");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<ApiScope>>(s, _jsonOptions) ?? Enumerable.Empty<ApiScope>();
    }
}
