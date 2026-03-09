using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Web.Services.Api;
/// <summary>
/// Encapsulates the functional responsibility of api auth token handler within the application architecture.
/// </summary>
public class ApiAuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiAuthTokenHandler> _logger;
            /// <summary>
            /// Initializes a new instance of the ApiAuthTokenHandler class with its required dependencies.
            /// </summary>
            public ApiAuthTokenHandler(IHttpContextAccessor httpContextAccessor, ILogger<ApiAuthTokenHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
            /// <summary>
            /// Executes the send async operation as part of this component.
            /// </summary>
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.ApiToken);
        if (!string.IsNullOrWhiteSpace(token) && request.Headers.Authorization == null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogInformation("API token attached to request {Method} {Url}", request.Method, request.RequestUri);
        }
        else
        {
            _logger.LogWarning("API token missing for request {Method} {Url}", request.Method, request.RequestUri);
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx != null)
            {
                ctx.Session.Remove(SessionKeys.ApiToken);
                await ctx.SignOutAsync("CookieAuth");
            }
            throw new UnauthorizedAccessException($"API unauthorized for {request.Method} {request.RequestUri}");
        }

        return response;
    }
}
/// <summary>
/// Encapsulates the functional responsibility of session keys within the application architecture.
/// </summary>
public static class SessionKeys
{
    public const string ApiToken = "ApiToken";
}
