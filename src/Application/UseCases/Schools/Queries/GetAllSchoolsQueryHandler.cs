using Application.Interfaces;
using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;
/// <summary>
/// Encapsulates the functional responsibility of get all schools query handler within the application architecture.
/// </summary>
public sealed class GetAllSchoolsQueryHandler : IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>>
{
    private readonly ISchoolService _schoolService;
            /// <summary>
            /// Initializes a new instance of the GetAllSchoolsQueryHandler class with its required dependencies.
            /// </summary>
            public GetAllSchoolsQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }
            /// <summary>
            /// Handles async and executes the corresponding use case.
            /// </summary>
            public async Task<IEnumerable<School>> HandleAsync(GetAllSchoolsQuery query, CancellationToken cancellationToken = default)
        => await _schoolService.GetAllSchoolsAsync();
}
