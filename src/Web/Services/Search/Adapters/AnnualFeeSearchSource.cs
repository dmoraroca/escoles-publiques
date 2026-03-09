using Application.DTOs;
using Application.Interfaces.Search;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;
/// <summary>
/// Encapsulates the functional responsibility of annual fee search source within the application architecture.
/// </summary>
public class AnnualFeeSearchSource : IAnnualFeeSearchSource
{
    private readonly IAnnualFeesApiClient _annualFeesApi;
            /// <summary>
            /// Initializes a new instance of the AnnualFeeSearchSource class with its required dependencies.
            /// </summary>
            public AnnualFeeSearchSource(IAnnualFeesApiClient annualFeesApi)
    {
        _annualFeesApi = annualFeesApi;
    }
            /// <summary>
            /// Retrieves all async and returns it to the caller.
            /// </summary>
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
