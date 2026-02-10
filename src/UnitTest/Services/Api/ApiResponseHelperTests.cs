using System.Net;
using System.Net.Http;
using Web.Services.Api;
using Xunit;

namespace UnitTest.Services.Api
{
    public class ApiResponseHelperTests
    {
        [Fact]
        public void EnsureSuccessOrUnauthorized_ThrowsUnauthorized_OnUnauthorized()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            Assert.Throws<UnauthorizedAccessException>(() =>
                ApiResponseHelper.EnsureSuccessOrUnauthorized(response, "ctx"));
        }

        [Fact]
        public void EnsureSuccessOrUnauthorized_ThrowsUnauthorized_OnForbidden()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);

            Assert.Throws<UnauthorizedAccessException>(() =>
                ApiResponseHelper.EnsureSuccessOrUnauthorized(response));
        }

        [Fact]
        public void EnsureSuccessOrUnauthorized_DoesNotThrow_OnSuccess()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            ApiResponseHelper.EnsureSuccessOrUnauthorized(response, "ctx");
        }
    }
}
