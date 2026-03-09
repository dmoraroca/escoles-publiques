using Application.Interfaces;
using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;
/// <summary>
/// Encapsulates the functional responsibility of get school by id query handler within the application architecture.
/// </summary>
public sealed class GetSchoolByIdQueryHandler : IQueryHandler<GetSchoolByIdQuery, School?>
{
    private readonly ISchoolService _schoolService;
    /// <summary>
    /// Initializes a new instance of the GetSchoolByIdQueryHandler class with its required dependencies.
    /// </summary>
    public GetSchoolByIdQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }
    /// <summary>
    /// Handles async and executes the corresponding use case.
    /// </summary>
    public async Task<School?> HandleAsync(GetSchoolByIdQuery query, CancellationToken cancellationToken = default)
=> await _schoolService.GetSchoolByIdAsync(query.Id);
}
