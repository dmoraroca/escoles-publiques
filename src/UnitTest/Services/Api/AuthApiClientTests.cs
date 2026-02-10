using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTest.Helpers;
using Web.Services.Api;
using Xunit;

namespace UnitTest.Services.Api
{
    public class AuthApiClientTests
    {
        [Fact]
        public async Task GetTokenAsync_ReturnsToken_WhenSuccessAndTokenProperty()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new { token = "abc" }), Encoding.UTF8, "application/json")
            });
            var http = new HttpClient(handler) { BaseAddress = new System.Uri("http://localhost/") };
            var logger = new Mock<ILogger<AuthApiClient>>();
            var client = new AuthApiClient(http, logger.Object);

            var token = await client.GetTokenAsync("a@b.com", "pass");

            Assert.Equal("abc", token);
        }

        [Fact]
        public async Task GetTokenAsync_ReturnsToken_WhenTokenResponseUsesTokenProperty()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Token\":\"XYZ\"}", Encoding.UTF8, "application/json")
            });
            var http = new HttpClient(handler) { BaseAddress = new System.Uri("http://localhost/") };
            var logger = new Mock<ILogger<AuthApiClient>>();
            var client = new AuthApiClient(http, logger.Object);

            var token = await client.GetTokenAsync("a@b.com", "pass");

            Assert.Equal("XYZ", token);
        }

        [Fact]
        public async Task GetTokenAsync_ReturnsNull_WhenStatusNotSuccess()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
            var http = new HttpClient(handler) { BaseAddress = new System.Uri("http://localhost/") };
            var logger = new Mock<ILogger<AuthApiClient>>();
            var client = new AuthApiClient(http, logger.Object);

            var token = await client.GetTokenAsync("a@b.com", "pass");

            Assert.Null(token);
        }

        [Fact]
        public async Task GetTokenAsync_ReturnsNull_WhenResponseMissingToken()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
            var http = new HttpClient(handler) { BaseAddress = new System.Uri("http://localhost/") };
            var logger = new Mock<ILogger<AuthApiClient>>();
            var client = new AuthApiClient(http, logger.Object);

            var token = await client.GetTokenAsync("a@b.com", "pass");

            Assert.Null(token);
        }
    }
}
