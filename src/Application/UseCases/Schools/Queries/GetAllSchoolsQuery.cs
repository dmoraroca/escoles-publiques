using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;
/// <summary>
/// Represents values and data structure for get all schools query.
/// </summary>
public sealed record GetAllSchoolsQuery : IQuery<IEnumerable<School>>;
