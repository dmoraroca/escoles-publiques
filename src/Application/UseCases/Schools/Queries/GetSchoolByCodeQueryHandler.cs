using Application.Interfaces;
using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;

public sealed class GetSchoolByCodeQueryHandler : IQueryHandler<GetSchoolByCodeQuery, School?>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolByCodeQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<School?> HandleAsync(GetSchoolByCodeQuery query, CancellationToken cancellationToken = default)
        => await _schoolService.GetSchoolByCodeAsync(query.Code);
}
