using Application.DTOs;
using Application.Interfaces.Search;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;

public class ScopeLookupSource : IScopeLookupSource
{
    private readonly IScopesApiClient _scopesApi;

    public ScopeLookupSource(IScopesApiClient scopesApi)
    {
        _scopesApi = scopesApi;
    }

    public async Task<IEnumerable<ScopeLookupDto>> GetAllAsync()
    {
        var scopes = await _scopesApi.GetAllAsync();
        return scopes.Select(s => new ScopeLookupDto(s.Id, s.Name));
    }
}
