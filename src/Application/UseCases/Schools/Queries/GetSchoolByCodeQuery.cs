using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Queries;

public sealed record GetSchoolByCodeQuery(string Code) : IQuery<School?>;
