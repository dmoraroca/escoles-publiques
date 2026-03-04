using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;

public sealed record GetAllSchoolsQuery : IQuery<IEnumerable<School>>;
