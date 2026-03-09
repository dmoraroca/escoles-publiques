using Application.Interfaces;
using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;
/// <summary>
/// Encapsulates the functional responsibility of get school by code query handler within the application architecture.
/// </summary>
public sealed class GetSchoolByCodeQueryHandler : IQueryHandler<GetSchoolByCodeQuery, School?>
{
    private readonly ISchoolService _schoolService;
    /// <summary>
    /// Initializes a new instance of the GetSchoolByCodeQueryHandler class with its required dependencies.
    /// </summary>
    public GetSchoolByCodeQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }
    /// <summary>
    /// Handles async and executes the corresponding use case.
    /// </summary>
    public async Task<School?> HandleAsync(GetSchoolByCodeQuery query, CancellationToken cancellationToken = default)
=> await _schoolService.GetSchoolByCodeAsync(query.Code);
}
