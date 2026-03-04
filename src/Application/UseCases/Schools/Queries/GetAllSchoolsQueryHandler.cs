using Application.Interfaces;
using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;

public sealed class GetAllSchoolsQueryHandler : IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>>
{
    private readonly ISchoolService _schoolService;

    public GetAllSchoolsQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IEnumerable<School>> HandleAsync(GetAllSchoolsQuery query, CancellationToken cancellationToken = default)
        => await _schoolService.GetAllSchoolsAsync();
}
