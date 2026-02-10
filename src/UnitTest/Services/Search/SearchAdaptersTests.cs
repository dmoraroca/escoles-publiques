using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Moq;
using Web.Services.Api;
using Web.Services.Search.Adapters;
using Xunit;

namespace UnitTest.Services.Search
{
    public class SearchAdaptersTests
    {
        [Fact]
        public async Task ScopeLookupSource_MapsScopes()
        {
            var scopesApi = new Mock<IScopesApiClient>();
            scopesApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope> { new ApiScope(1, "Primary") });

            var source = new ScopeLookupSource(scopesApi.Object);
            var result = await source.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task SchoolSearchSource_ReturnsSchools()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            schoolsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<School> { new School { Id = 1, Name = "A", Code = "A1" } });

            var source = new SchoolSearchSource(schoolsApi.Object);
            var result = await source.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task StudentSearchSource_MapsStudents()
        {
            var studentsApi = new Mock<IStudentsApiClient>();
            studentsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiStudent>
            {
                new ApiStudent(1, null, "A", "B", "", null, 1, "School")
            });

            var source = new StudentSearchSource(studentsApi.Object);
            var result = await source.GetAllAsync();

            var student = Assert.Single(result);
            Assert.Null(student.Email);
        }

        [Fact]
        public async Task EnrollmentSearchSource_MapsEnrollments()
        {
            var enrollmentsApi = new Mock<IEnrollmentsApiClient>();
            enrollmentsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiEnrollment>
            {
                new ApiEnrollment(1, 1, "Student", "2025", null, "Active", DateTime.UtcNow, 1, "School")
            });

            var source = new EnrollmentSearchSource(enrollmentsApi.Object);
            var result = await source.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task AnnualFeeSearchSource_MapsFees()
        {
            var feesApi = new Mock<IAnnualFeesApiClient>();
            feesApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiAnnualFee>
            {
                new ApiAnnualFee(1, 1, "Info", "Student", "2025", null, 100m, "EUR", new DateOnly(2025, 9, 1), null, null, null, null)
            });

            var source = new AnnualFeeSearchSource(feesApi.Object);
            var result = await source.GetAllAsync();

            Assert.Single(result);
        }
    }
}
