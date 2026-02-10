using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Search;

public interface IScopeLookupSource
{
    Task<IEnumerable<ScopeLookupDto>> GetAllAsync();
}

public interface ISchoolSearchSource
{
    Task<IEnumerable<School>> GetAllAsync();
}

public interface IStudentSearchSource
{
    Task<IEnumerable<StudentSearchDto>> GetAllAsync();
}

public interface IEnrollmentSearchSource
{
    Task<IEnumerable<EnrollmentSearchDto>> GetAllAsync();
}

public interface IAnnualFeeSearchSource
{
    Task<IEnumerable<AnnualFeeSearchDto>> GetAllAsync();
}
