namespace Web.Services.Api;
/// <summary>
/// Defines the contract required by i auth api client components.
/// </summary>
public interface IAuthApiClient
{
    Task<string?> GetTokenAsync(string email, string password);
}
