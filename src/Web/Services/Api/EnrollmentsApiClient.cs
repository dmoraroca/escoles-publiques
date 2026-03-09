using System.Text;
using System.Text.Json;

namespace Web.Services.Api;
/// <summary>
/// Encapsulates the functional responsibility of enrollments api client within the application architecture.
/// </summary>
public class EnrollmentsApiClient : IEnrollmentsApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
            /// <summary>
            /// Initializes a new instance of the EnrollmentsApiClient class with its required dependencies.
            /// </summary>
            public EnrollmentsApiClient(HttpClient http)
    {
        _http = http;
    }
            /// <summary>
            /// Retrieves all async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<ApiEnrollment>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/enrollments");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/enrollments");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<ApiEnrollment>>(s, _jsonOptions) ?? Enumerable.Empty<ApiEnrollment>();
    }
            /// <summary>
            /// Retrieves by id async and returns it to the caller.
            /// </summary>
            public async Task<ApiEnrollment?> GetByIdAsync(long id)
    {
        var res = await _http.GetAsync($"api/enrollments/{id}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"GET api/enrollments/{id}");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiEnrollment>(s, _jsonOptions);
    }
            /// <summary>
            /// Creates async by applying the required business rules.
            /// </summary>
            public async Task<ApiEnrollment> CreateAsync(ApiEnrollmentIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PostAsync("api/enrollments", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "POST api/enrollments");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiEnrollment>(s, _jsonOptions)!;
    }
            /// <summary>
            /// Updates async with the data received in the request.
            /// </summary>
            public async Task UpdateAsync(long id, ApiEnrollmentIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PutAsync($"api/enrollments/{id}", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"PUT api/enrollments/{id}");
    }
            /// <summary>
            /// Deletes async from the system in a controlled manner.
            /// </summary>
            public async Task DeleteAsync(long id)
    {
        var res = await _http.DeleteAsync($"api/enrollments/{id}");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"DELETE api/enrollments/{id}");
    }
}
