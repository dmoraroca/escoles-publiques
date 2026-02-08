using Domain.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Web.Services.Api;

public class SchoolsApiClient : ISchoolsApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SchoolsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<School>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/schools");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/schools");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<School>>(s, _jsonOptions) ?? Enumerable.Empty<School>();
    }

    public async Task<School?> GetByIdAsync(long id)
    {
        var res = await _http.GetAsync($"api/schools/{id}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"GET api/schools/{id}");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<School>(s, _jsonOptions);
    }

    public async Task<School> CreateAsync(School school)
    {
        var json = JsonSerializer.Serialize(school);
        var res = await _http.PostAsync("api/schools", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "POST api/schools");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<School>(s, _jsonOptions)!;
    }

    public async Task UpdateAsync(long id, School school)
    {
        var json = JsonSerializer.Serialize(school);
        var res = await _http.PutAsync($"api/schools/{id}", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"PUT api/schools/{id}");
    }

    public async Task DeleteAsync(long id)
    {
        var res = await _http.DeleteAsync($"api/schools/{id}");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"DELETE api/schools/{id}");
    }
}
