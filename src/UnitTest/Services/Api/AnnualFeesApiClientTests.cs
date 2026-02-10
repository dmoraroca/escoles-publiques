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
    public class AnnualFeesApiClientTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsFees()
        {
            var fees = new List<ApiAnnualFee>
            {
                new ApiAnnualFee(1, 1, "Info", "Student", "2025", null, 100m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null)
            };
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(fees), Encoding.UTF8, "application/json")
            });
            var client = new AnnualFeesApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_OnNotFound()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var client = new AnnualFeesApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsFee()
        {
            var fee = new ApiAnnualFee(2, 1, "Info", "Student", "2025", null, 200m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null);
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(fee), Encoding.UTF8, "application/json")
            });
            var client = new AnnualFeesApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var dto = new ApiAnnualFeeIn(1, 200m, "EUR", new DateOnly(2025, 9, 1), false, null);
            var result = await client.CreateAsync(dto);

            Assert.Equal(2, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_DoesNotThrow()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var client = new AnnualFeesApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var dto = new ApiAnnualFeeIn(1, 100m, "EUR", new DateOnly(2025, 9, 1), false, null);
            await client.UpdateAsync(1, dto);
        }

        [Fact]
        public async Task DeleteAsync_DoesNotThrow()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var client = new AnnualFeesApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            await client.DeleteAsync(1);
        }
    }
}
