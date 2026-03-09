using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;
/// <summary>
/// Represents values and data structure for get school by id query.
/// </summary>
public sealed record GetSchoolByIdQuery(long Id) : IQuery<School?>;
