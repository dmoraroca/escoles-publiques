using System.Text;
using System.Text.Json;

namespace Web.Services.Api;
/// <summary>
/// Encapsulates the functional responsibility of annual fees api client within the application architecture.
/// </summary>
public class AnnualFeesApiClient : IAnnualFeesApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
            /// <summary>
            /// Initializes a new instance of the AnnualFeesApiClient class with its required dependencies.
            /// </summary>
            public AnnualFeesApiClient(HttpClient http)
    {
        _http = http;
    }
            /// <summary>
            /// Retrieves all async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<ApiAnnualFee>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/annualfees");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/annualfees");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<ApiAnnualFee>>(s, _jsonOptions) ?? Enumerable.Empty<ApiAnnualFee>();
    }
            /// <summary>
            /// Retrieves by id async and returns it to the caller.
            /// </summary>
            public async Task<ApiAnnualFee?> GetByIdAsync(long id)
    {
        var res = await _http.GetAsync($"api/annualfees/{id}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"GET api/annualfees/{id}");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiAnnualFee>(s, _jsonOptions);
    }
            /// <summary>
            /// Creates async by applying the required business rules.
            /// </summary>
            public async Task<ApiAnnualFee> CreateAsync(ApiAnnualFeeIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PostAsync("api/annualfees", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "POST api/annualfees");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiAnnualFee>(s, _jsonOptions)!;
    }
            /// <summary>
            /// Updates async with the data received in the request.
            /// </summary>
            public async Task UpdateAsync(long id, ApiAnnualFeeIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PutAsync($"api/annualfees/{id}", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"PUT api/annualfees/{id}");
    }
            /// <summary>
            /// Deletes async from the system in a controlled manner.
            /// </summary>
            public async Task DeleteAsync(long id)
    {
        var res = await _http.DeleteAsync($"api/annualfees/{id}");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"DELETE api/annualfees/{id}");
    }
}
