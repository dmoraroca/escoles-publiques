using System.Text.Json;

namespace Web.Services.Api;

public class StudentsApiClient : IStudentsApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public StudentsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ApiStudent>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/students");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/students");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<ApiStudent>>(s, _jsonOptions) ?? Enumerable.Empty<ApiStudent>();
    }
}
