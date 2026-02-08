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
}
