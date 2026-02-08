namespace Web.Services.Api;

public interface IScopesApiClient
{
    Task<IEnumerable<ApiScope>> GetAllAsync();
}
