using System.Text;
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

    public async Task<ApiStudent?> GetByIdAsync(long id)
    {
        var res = await _http.GetAsync($"api/students/{id}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"GET api/students/{id}");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiStudent>(s, _jsonOptions);
    }

    public async Task<ApiStudent> CreateAsync(ApiStudentIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PostAsync("api/students", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "POST api/students");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiStudent>(s, _jsonOptions)!;
    }

    public async Task UpdateAsync(long id, ApiStudentIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PutAsync($"api/students/{id}", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"PUT api/students/{id}");
    }

    public async Task DeleteAsync(long id)
    {
        var res = await _http.DeleteAsync($"api/students/{id}");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"DELETE api/students/{id}");
    }
}
