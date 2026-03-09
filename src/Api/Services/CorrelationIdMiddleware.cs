namespace Api.Services;
/// <summary>
/// Intercepts the HTTP pipeline to apply correlation id behavior.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;
            /// <summary>
            /// Initializes a new instance of the CorrelationIdMiddleware class with its required dependencies.
            /// </summary>
            public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }
            /// <summary>
            /// Executes middleware logic for the current HTTP request.
            /// </summary>
            public async Task Invoke(HttpContext context)
    {
        var incoming = context.Request.Headers.TryGetValue(HeaderName, out var values)
            ? values.FirstOrDefault()
            : null;
        var correlationId = !string.IsNullOrWhiteSpace(incoming)
            ? incoming.Trim()
            : Guid.NewGuid().ToString("N");

        context.TraceIdentifier = correlationId;
        context.Response.Headers[HeaderName] = correlationId;
        context.Items[HeaderName] = correlationId;

        await _next(context);
    }
}
