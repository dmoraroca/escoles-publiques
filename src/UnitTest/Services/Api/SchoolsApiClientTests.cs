using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Domain.Entities;
using UnitTest.Helpers;
using Web.Services.Api;
using Xunit;

namespace UnitTest.Services.Api
{
    public class SchoolsApiClientTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsSchools()
        {
            var schools = new List<School>
            {
                new School { Id = 1, Name = "A", Code = "A1", City = "City", CreatedAt = DateTime.UtcNow }
            };
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(schools), Encoding.UTF8, "application/json")
            });
            var client = new SchoolsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_OnNotFound()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var client = new SchoolsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSchool_OnSuccess()
        {
            var school = new School { Id = 2, Name = "B", Code = "B1", City = "City", CreatedAt = DateTime.UtcNow };
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(school), Encoding.UTF8, "application/json")
            });
            var client = new SchoolsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetByIdAsync(2);

            Assert.NotNull(result);
            Assert.Equal("B", result!.Name);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedSchool()
        {
            var school = new School { Id = 3, Name = "C", Code = "C1", City = "City", CreatedAt = DateTime.UtcNow };
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(school), Encoding.UTF8, "application/json")
            });
            var client = new SchoolsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.CreateAsync(school);

            Assert.Equal(3, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_DoesNotThrow()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var client = new SchoolsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            await client.UpdateAsync(1, new School { Id = 1, Name = "A", Code = "A1" });
        }

        [Fact]
        public async Task DeleteAsync_DoesNotThrow()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var client = new SchoolsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            await client.DeleteAsync(1);
        }
    }
}
