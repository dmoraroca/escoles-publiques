using System.Text;
using System.Text.Json;

namespace Web.Services.Api;

public class AnnualFeesApiClient : IAnnualFeesApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AnnualFeesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ApiAnnualFee>> GetAllAsync()
    {
        var res = await _http.GetAsync("api/annualfees");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "GET api/annualfees");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<ApiAnnualFee>>(s, _jsonOptions) ?? Enumerable.Empty<ApiAnnualFee>();
    }

    public async Task<ApiAnnualFee?> GetByIdAsync(long id)
    {
        var res = await _http.GetAsync($"api/annualfees/{id}");
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"GET api/annualfees/{id}");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiAnnualFee>(s, _jsonOptions);
    }

    public async Task<ApiAnnualFee> CreateAsync(ApiAnnualFeeIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PostAsync("api/annualfees", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, "POST api/annualfees");
        var s = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiAnnualFee>(s, _jsonOptions)!;
    }

    public async Task UpdateAsync(long id, ApiAnnualFeeIn dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var res = await _http.PutAsync($"api/annualfees/{id}", new StringContent(json, Encoding.UTF8, "application/json"));
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"PUT api/annualfees/{id}");
    }

    public async Task DeleteAsync(long id)
    {
        var res = await _http.DeleteAsync($"api/annualfees/{id}");
        ApiResponseHelper.EnsureSuccessOrUnauthorized(res, $"DELETE api/annualfees/{id}");
    }
}
