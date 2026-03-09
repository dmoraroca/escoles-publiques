namespace Web.Services.Api;
/// <summary>
/// Encapsulates the functional responsibility of api response helper within the application architecture.
/// </summary>
public static class ApiResponseHelper
{
            /// <summary>
            /// Executes the ensure success or unauthorized operation as part of this component.
            /// </summary>
            public static void EnsureSuccessOrUnauthorized(HttpResponseMessage response, string? context = null)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException(context ?? "API unauthorized");
        }

        response.EnsureSuccessStatusCode();
    }
}
