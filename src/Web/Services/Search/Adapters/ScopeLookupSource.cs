using Application.DTOs;
using Application.Interfaces.Search;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;
/// <summary>
/// Encapsulates the functional responsibility of scope lookup source within the application architecture.
/// </summary>
public class ScopeLookupSource : IScopeLookupSource
{
    private readonly IScopesApiClient _scopesApi;
            /// <summary>
            /// Initializes a new instance of the ScopeLookupSource class with its required dependencies.
            /// </summary>
            public ScopeLookupSource(IScopesApiClient scopesApi)
    {
        _scopesApi = scopesApi;
    }
            /// <summary>
            /// Retrieves all async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<ScopeLookupDto>> GetAllAsync()
    {
        var scopes = await _scopesApi.GetAllAsync();
        return scopes.Select(s => new ScopeLookupDto(s.Id, s.Name));
    }
}
