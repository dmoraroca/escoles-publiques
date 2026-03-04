namespace Api.Services;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

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
