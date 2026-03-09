using System.Text.Json;

namespace Web.Services.Api;
/// <summary>
/// Encapsulates the functional responsibility of scopes api client within the application architecture.
/// </summary>
public class ScopesApiClient : IScopesApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    /// <summary>
    /// Initializes a new instance of the ScopesApiClient class with its required dependencies.
    /// </summary>
    public ScopesApiClient(HttpClient http)
    {
        _http = http;
    }
    /// <summary>
    /// Retrieves all async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<ApiScope>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/scopes");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/scopes");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<ApiScope>>(s, _jsonOptions) ?? Enumerable.Empty<ApiScope>();
    }
}
