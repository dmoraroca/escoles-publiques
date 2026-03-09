using Domain.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Web.Services.Api;
/// <summary>
/// Encapsulates the functional responsibility of schools api client within the application architecture.
/// </summary>
public class SchoolsApiClient : ISchoolsApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    /// <summary>
    /// Initializes a new instance of the SchoolsApiClient class with its required dependencies.
    /// </summary>
    public SchoolsApiClient(HttpClient http)
    {
        _http = http;
    }
    /// <summary>
    /// Retrieves all async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<School>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/schools");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/schools");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<School>>(s, _jsonOptions) ?? Enumerable.Empty<School>();
    }
    /// <summary>
    /// Retrieves by id async and returns it to the caller.
    /// </summary>
    public async Task<School?> GetByIdAsync(long id)
    {
        var res = await _http.GetAsync($"api/schools/{id}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"GET api/schools/{id}");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<School>(s, _jsonOptions);
    }
    /// <summary>
    /// Creates async by applying the required business rules.
    /// </summary>
    public async Task<School> CreateAsync(School school)
    {
        var json = JsonSerializer.Serialize(school);
        var res = await _http.PostAsync("api/schools", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "POST api/schools");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<School>(s, _jsonOptions)!;
    }
    /// <summary>
    /// Updates async with the data received in the request.
    /// </summary>
    public async Task UpdateAsync(long id, School school)
    {
        var json = JsonSerializer.Serialize(school);
        var res = await _http.PutAsync($"api/schools/{id}", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"PUT api/schools/{id}");
    }
    /// <summary>
    /// Deletes async from the system in a controlled manner.
    /// </summary>
    public async Task DeleteAsync(long id)
    {
        var res = await _http.DeleteAsync($"api/schools/{id}");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"DELETE api/schools/{id}");
    }
}
