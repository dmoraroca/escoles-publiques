using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;
/// <summary>
/// Represents values and data structure for get school by code query.
/// </summary>
public sealed record GetSchoolByCodeQuery(string Code) : IQuery<School?>;
