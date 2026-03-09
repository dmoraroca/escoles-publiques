using Application.Interfaces.Search;
using Domain.Entities;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;
/// <summary>
/// Encapsulates the functional responsibility of school search source within the application architecture.
/// </summary>
public class SchoolSearchSource : ISchoolSearchSource
{
    private readonly ISchoolsApiClient _schoolsApi;
            /// <summary>
            /// Initializes a new instance of the SchoolSearchSource class with its required dependencies.
            /// </summary>
            public SchoolSearchSource(ISchoolsApiClient schoolsApi)
    {
        _schoolsApi = schoolsApi;
    }
            /// <summary>
            /// Retrieves all async and returns it to the caller.
            /// </summary>
            public Task<IEnumerable<School>> GetAllAsync()
    {
        return _schoolsApi.GetAllAsync();
    }
}
