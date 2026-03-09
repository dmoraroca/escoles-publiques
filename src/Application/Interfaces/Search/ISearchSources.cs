using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Search;
/// <summary>
/// Defines the contract required by i scope lookup source components.
/// </summary>
public interface IScopeLookupSource
{
    Task<IEnumerable<ScopeLookupDto>> GetAllAsync();
}
/// <summary>
/// Defines the contract required by i school search source components.
/// </summary>
public interface ISchoolSearchSource
{
    Task<IEnumerable<School>> GetAllAsync();
}
/// <summary>
/// Defines the contract required by i student search source components.
/// </summary>
public interface IStudentSearchSource
{
    Task<IEnumerable<StudentSearchDto>> GetAllAsync();
}
/// <summary>
/// Defines the contract required by i enrollment search source components.
/// </summary>
public interface IEnrollmentSearchSource
{
    Task<IEnumerable<EnrollmentSearchDto>> GetAllAsync();
}
/// <summary>
/// Defines the contract required by i annual fee search source components.
/// </summary>
public interface IAnnualFeeSearchSource
{
    Task<IEnumerable<AnnualFeeSearchDto>> GetAllAsync();
}
