using Application.DTOs;
using Application.Interfaces.Search;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;

public class AnnualFeeSearchSource : IAnnualFeeSearchSource
{
    private readonly IAnnualFeesApiClient _annualFeesApi;

    public AnnualFeeSearchSource(IAnnualFeesApiClient annualFeesApi)
    {
        _annualFeesApi = annualFeesApi;
    }

    public async Task<IEnumerable<AnnualFeeSearchDto>> GetAllAsync()
    {
        var fees = await _annualFeesApi.GetAllAsync();
        return fees.Select(f => new AnnualFeeSearchDto(
            f.Id,
            f.StudentName,
            f.Amount,
            f.Currency,
            f.DueDate,
            f.PaidAt));
    }
}
