using System.Text.Json;
using Api.Services;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace UnitTest.Api;

public class ApiExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task Invoke_ReturnsProblemDetails_WithTraceIdAndErrorCode_OnValidationException()
    {
        var middleware = new ApiExceptionHandlingMiddleware(
            _ => throw new ValidationException("Code", "Invalid"),
            NullLogger<ApiExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "trace-123";

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Position = 0;
        using var doc = await JsonDocument.ParseAsync(context.Response.Body);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("traceId", out var trace));
        Assert.Equal("trace-123", trace.GetString());
        Assert.Equal("validation_error", root.GetProperty("errorCode").GetString());
        Assert.True(root.TryGetProperty("fieldErrors", out _));
    }

    [Fact]
    public async Task Invoke_Returns500ProblemDetails_OnUnhandledException()
    {
        var middleware = new ApiExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("boom"),
            NullLogger<ApiExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "trace-500";

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        using var doc = await JsonDocument.ParseAsync(context.Response.Body);
        var root = doc.RootElement;

        Assert.Equal("internal_error", root.GetProperty("errorCode").GetString());
        Assert.Equal("trace-500", root.GetProperty("traceId").GetString());
    }
}
