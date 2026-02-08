using System.Text.Json;

namespace Web.Services.Api;

public class EnrollmentsApiClient : IEnrollmentsApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public EnrollmentsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ApiEnrollment>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/enrollments");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/enrollments");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<ApiEnrollment>>(s, _jsonOptions) ?? Enumerable.Empty<ApiEnrollment>();
    }
}
