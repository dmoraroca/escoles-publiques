using Api.Services;
using Microsoft.AspNetCore.Http;

namespace UnitTest.Api;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task Invoke_SetsHeaderAndTraceIdentifier_WhenMissing()
    {
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);
        var context = new DefaultHttpContext();

        await middleware.Invoke(context);

        Assert.True(context.Response.Headers.ContainsKey(CorrelationIdMiddleware.HeaderName));
        Assert.False(string.IsNullOrWhiteSpace(context.TraceIdentifier));
    }

    [Fact]
    public async Task Invoke_UsesIncomingCorrelationId_WhenProvided()
    {
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = "req-123";

        await middleware.Invoke(context);

        Assert.Equal("req-123", context.TraceIdentifier);
        Assert.Equal("req-123", context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString());
    }
}
