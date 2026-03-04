using Application.Interfaces;
using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;

public sealed class GetSchoolByIdQueryHandler : IQueryHandler<GetSchoolByIdQuery, School?>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolByIdQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<School?> HandleAsync(GetSchoolByIdQuery query, CancellationToken cancellationToken = default)
        => await _schoolService.GetSchoolByIdAsync(query.Id);
}
