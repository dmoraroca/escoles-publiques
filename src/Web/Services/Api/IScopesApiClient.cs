namespace Web.Services.Api;
/// <summary>
/// Defines the contract required by i scopes api client components.
/// </summary>
public interface IScopesApiClient
{
    Task<IEnumerable<ApiScope>> GetAllAsync();
}
