using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTest.Helpers;
using Web.Services.Api;
using Xunit;

namespace UnitTest.Services.Api
{
    public class ApiAuthTokenHandlerTests
    {
        private sealed class TestSessionFeature : ISessionFeature
        {
            public ISession Session { get; set; } = default!;
        }

        private sealed class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _store = new();

            public bool IsAvailable => true;
            public string Id => "test";
            public IEnumerable<string> Keys => _store.Keys;

            public void Clear() => _store.Clear();
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => _store.Remove(key);
            public void Set(string key, byte[] value) => _store[key] = value;
            public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
        }

        [Fact]
        public async Task SendAsync_AttachesBearerToken_WhenSessionHasToken()
        {
            var session = new TestSession();
            session.SetString(SessionKeys.ApiToken, "token-123");
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new TestSessionFeature { Session = session });

            var accessor = new HttpContextAccessor { HttpContext = httpContext };
            var logger = new Mock<ILogger<ApiAuthTokenHandler>>();

            HttpRequestMessage? captured = null;
            var inner = new TestHttpMessageHandler(req =>
            {
                captured = req;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var handler = new ApiAuthTokenHandler(accessor, logger.Object) { InnerHandler = inner };
            var client = new HttpClient(handler);

            var response = await client.GetAsync("http://localhost/test");

            Assert.NotNull(captured);
            Assert.Equal("Bearer", captured!.Headers.Authorization?.Scheme);
            Assert.Equal("token-123", captured.Headers.Authorization?.Parameter);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SendAsync_DoesNotAttachToken_WhenSessionMissingToken()
        {
            var session = new TestSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new TestSessionFeature { Session = session });

            var accessor = new HttpContextAccessor { HttpContext = httpContext };
            var logger = new Mock<ILogger<ApiAuthTokenHandler>>();

            HttpRequestMessage? captured = null;
            var inner = new TestHttpMessageHandler(req =>
            {
                captured = req;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var handler = new ApiAuthTokenHandler(accessor, logger.Object) { InnerHandler = inner };
            var client = new HttpClient(handler);

            await client.GetAsync("http://localhost/test");

            Assert.NotNull(captured);
            Assert.Null(captured!.Headers.Authorization);
        }

        [Fact]
        public async Task SendAsync_ThrowsUnauthorizedAndClearsSession_WhenUnauthorized()
        {
            var session = new TestSession();
            session.SetString(SessionKeys.ApiToken, "token-123");
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new TestSessionFeature { Session = session });

            var authService = new Mock<IAuthenticationService>();
            var services = new ServiceCollection();
            services.AddSingleton(authService.Object);
            httpContext.RequestServices = services.BuildServiceProvider();

            var accessor = new HttpContextAccessor { HttpContext = httpContext };
            var logger = new Mock<ILogger<ApiAuthTokenHandler>>();

            var inner = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));

            var handler = new ApiAuthTokenHandler(accessor, logger.Object) { InnerHandler = inner };
            var client = new HttpClient(handler);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => client.GetAsync("http://localhost/test"));

            Assert.False(session.TryGetValue(SessionKeys.ApiToken, out _));
            authService.Verify(s => s.SignOutAsync(httpContext, "CookieAuth", null), Times.Once);
        }
    }
}
