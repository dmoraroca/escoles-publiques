using Application.Interfaces.Search;
using Domain.Entities;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;

public class SchoolSearchSource : ISchoolSearchSource
{
    private readonly ISchoolsApiClient _schoolsApi;

    public SchoolSearchSource(ISchoolsApiClient schoolsApi)
    {
        _schoolsApi = schoolsApi;
    }

    public Task<IEnumerable<School>> GetAllAsync()
    {
        return _schoolsApi.GetAllAsync();
    }
}
