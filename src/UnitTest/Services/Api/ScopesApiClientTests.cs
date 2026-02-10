using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using UnitTest.Helpers;
using Web.Services.Api;
using Xunit;

namespace UnitTest.Services.Api
{
    public class ScopesApiClientTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsScopes()
        {
            var scopes = new List<ApiScope> { new ApiScope(1, "Primary") };
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(scopes), Encoding.UTF8, "application/json")
            });
            var client = new ScopesApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetAllAsync();

            Assert.Single(result);
        }
    }
}
