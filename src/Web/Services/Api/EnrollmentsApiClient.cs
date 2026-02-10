using System.Text;
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

    public async Task<ApiEnrollment?> GetByIdAsync(long id)
    {
        var res = await _http.GetAsync($"api/enrollments/{id}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"GET api/enrollments/{id}");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiEnrollment>(s, _jsonOptions);
    }

    public async Task<ApiEnrollment> CreateAsync(ApiEnrollmentIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PostAsync("api/enrollments", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "POST api/enrollments");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiEnrollment>(s, _jsonOptions)!;
    }

    public async Task UpdateAsync(long id, ApiEnrollmentIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PutAsync($"api/enrollments/{id}", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"PUT api/enrollments/{id}");
    }

    public async Task DeleteAsync(long id)
    {
        var res = await _http.DeleteAsync($"api/enrollments/{id}");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"DELETE api/enrollments/{id}");
    }
}
