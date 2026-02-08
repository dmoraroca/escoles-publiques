namespace Web.Services.Api;

public static class ApiResponseHelper
{
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
