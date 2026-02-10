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
    public class EnrollmentsApiClientTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsEnrollments()
        {
            var enrollments = new List<ApiEnrollment>
            {
                new ApiEnrollment(1, 1, "Student", "2025", null, "Active", DateTime.UtcNow, 1, "School")
            };
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(enrollments), Encoding.UTF8, "application/json")
            });
            var client = new EnrollmentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_OnNotFound()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var client = new EnrollmentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsEnrollment()
        {
            var enrollment = new ApiEnrollment(2, 2, "Student", "2026", null, "Active", DateTime.UtcNow, 1, "School");
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(enrollment), Encoding.UTF8, "application/json")
            });
            var client = new EnrollmentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var dto = new ApiEnrollmentIn(2, "2026", null, "Active", null, 1);
            var result = await client.CreateAsync(dto);

            Assert.Equal(2, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_DoesNotThrow()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var client = new EnrollmentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            await client.UpdateAsync(1, new ApiEnrollmentIn(1, "2025", null, "Active", null, 1));
        }

        [Fact]
        public async Task DeleteAsync_DoesNotThrow()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var client = new EnrollmentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            await client.DeleteAsync(1);
        }
    }
}
