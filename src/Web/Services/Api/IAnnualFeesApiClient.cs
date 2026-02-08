namespace Web.Services.Api;

public interface IAnnualFeesApiClient
{
    Task<IEnumerable<ApiAnnualFee>> GetAllAsync();
}
