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
    public class StudentsApiClientTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsStudents()
        {
            var students = new List<ApiStudent>
            {
                new ApiStudent(1, 1, "A", "B", "a@b.com", null, 1, "School")
            };
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(students), Encoding.UTF8, "application/json")
            });
            var client = new StudentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_OnNotFound()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var client = new StudentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsStudent()
        {
            var student = new ApiStudent(2, null, "C", "D", "c@d.com", null, 2, "School");
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json")
            });
            var client = new StudentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            var result = await client.CreateAsync(new ApiStudentIn("C", "D", "c@d.com", null, 2));

            Assert.Equal(2, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_DoesNotThrow()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var client = new StudentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            await client.UpdateAsync(1, new ApiStudentIn("A", "B", "a@b.com", null, 1));
        }

        [Fact]
        public async Task DeleteAsync_DoesNotThrow()
        {
            var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var client = new StudentsApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

            await client.DeleteAsync(1);
        }
    }
}
