using Api.Services;
using Microsoft.AspNetCore.Http;

namespace UnitTest.Api;

public class RequestMetricsMiddlewareTests
{
    [Fact]
    public async Task Invoke_ExecutesNext_AndSetsResponseStatus()
    {
        var middleware = new RequestMetricsMiddleware(async ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status201Created;
            await ctx.Response.WriteAsync("ok");
        });

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Method = "POST";
        context.Request.Path = "/api/test";

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status201Created, context.Response.StatusCode);
    }
}
