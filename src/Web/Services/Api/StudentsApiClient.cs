using System.Text;
using System.Text.Json;

namespace Web.Services.Api;
/// <summary>
/// Encapsulates the functional responsibility of students api client within the application architecture.
/// </summary>
public class StudentsApiClient : IStudentsApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
            /// <summary>
            /// Initializes a new instance of the StudentsApiClient class with its required dependencies.
            /// </summary>
            public StudentsApiClient(HttpClient http)
    {
        _http = http;
    }
            /// <summary>
            /// Retrieves all async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<ApiStudent>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/students");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/students");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<ApiStudent>>(s, _jsonOptions) ?? Enumerable.Empty<ApiStudent>();
    }
            /// <summary>
            /// Retrieves by id async and returns it to the caller.
            /// </summary>
            public async Task<ApiStudent?> GetByIdAsync(long id)
    {
        var res = await _http.GetAsync($"api/students/{id}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"GET api/students/{id}");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiStudent>(s, _jsonOptions);
    }
            /// <summary>
            /// Creates async by applying the required business rules.
            /// </summary>
            public async Task<ApiStudent> CreateAsync(ApiStudentIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PostAsync("api/students", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "POST api/students");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiStudent>(s, _jsonOptions)!;
    }
            /// <summary>
            /// Updates async with the data received in the request.
            /// </summary>
            public async Task UpdateAsync(long id, ApiStudentIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PutAsync($"api/students/{id}", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"PUT api/students/{id}");
    }
            /// <summary>
            /// Deletes async from the system in a controlled manner.
            /// </summary>
            public async Task DeleteAsync(long id)
    {
        var res = await _http.DeleteAsync($"api/students/{id}");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"DELETE api/students/{id}");
    }
}
