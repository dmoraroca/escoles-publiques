using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Api.Services;
/// <summary>
/// Intercepts the HTTP pipeline to apply request metrics behavior.
/// </summary>
public sealed class RequestMetricsMiddleware
{
    private static readonly Meter Meter = new("Api.Requests", "1.0.0");
    private static readonly Counter<long> RequestCount = Meter.CreateCounter<long>("api_requests_total");
    private static readonly Histogram<double> RequestDurationMs = Meter.CreateHistogram<double>("api_request_duration_ms");

    private readonly RequestDelegate _next;
            /// <summary>
            /// Initializes a new instance of the RequestMetricsMiddleware class with its required dependencies.
            /// </summary>
            public RequestMetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }
            /// <summary>
            /// Executes middleware logic for the current HTTP request.
            /// </summary>
            public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        await _next(context);

        sw.Stop();

        var path = context.Request.Path.HasValue ? context.Request.Path.Value! : "unknown";
        var method = context.Request.Method;
        var statusCode = context.Response.StatusCode.ToString();

        var tags = new TagList
        {
            { "method", method },
            { "path", path },
            { "status_code", statusCode }
        };

        RequestCount.Add(1, tags);
        RequestDurationMs.Record(sw.Elapsed.TotalMilliseconds, tags);
    }
}
