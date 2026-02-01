namespace Web.Services.Api;

public interface IAuthApiClient
{
    Task<string?> GetTokenAsync(string email, string password);
}
