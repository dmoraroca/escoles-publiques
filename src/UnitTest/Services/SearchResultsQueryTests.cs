using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Search;
using Application.UseCases.Queries.SearchResults;
using Domain.Entities;
using Moq;
using Xunit;

namespace UnitTest.Services
{
    public class SearchResultsQueryTests
    {
        [Fact]
        public async Task ExecuteAsync_FiltersSchoolsBySearchTerms_AndMapsScope()
        {
            var scopes = new List<ScopeLookupDto>
            {
                new ScopeLookupDto(1, "Primary")
            };
            var schools = new List<School>
            {
                new School { Id = 1, Name = "Alpha School", Code = "A1", City = "Barcelona", ScopeId = 1 },
                new School { Id = 2, Name = "Beta School", Code = "B1", City = "Madrid", ScopeId = 1 }
            };
            var students = new List<StudentSearchDto>
            {
                new StudentSearchDto(1, "Ana", "Perez", "ana@barcelona.cat", "Alpha School")
            };

            var query = CreateQuery(
                schools: schools,
                scopes: scopes,
                students: students,
                enrollments: Array.Empty<EnrollmentSearchDto>(),
                fees: Array.Empty<AnnualFeeSearchDto>());

            var result = await query.ExecuteAsync("barcelona", null);

            Assert.Single(result.Schools);
            Assert.Equal("Alpha School", result.Schools[0].Name);
            Assert.Equal("Primary", result.Schools[0].ScopeName);
            Assert.Equal("Primary", result.Schools[0].Scope);
            Assert.Single(result.Students);
            Assert.Equal("Ana", result.Students[0].FirstName);
        }

        [Fact]
        public async Task ExecuteAsync_WithOnlyScopeName_ReturnsSchoolsInScopeOnly()
        {
            var scopes = new List<ScopeLookupDto>
            {
                new ScopeLookupDto(2, "Primary Education"),
                new ScopeLookupDto(3, "Secondary Education")
            };
            var schools = new List<School>
            {
                new School { Id = 10, Name = "North School", Code = "N1", City = "Girona", ScopeId = 2 },
                new School { Id = 11, Name = "South School", Code = "S1", City = "Tarragona", ScopeId = 3 }
            };

            var query = CreateQuery(
                schools: schools,
                scopes: scopes,
                students: Array.Empty<StudentSearchDto>(),
                enrollments: Array.Empty<EnrollmentSearchDto>(),
                fees: Array.Empty<AnnualFeeSearchDto>());

            var result = await query.ExecuteAsync(null, "  primary   education ");

            Assert.Single(result.Schools);
            Assert.Equal(10, result.Schools[0].Id);
            Assert.Empty(result.Students);
            Assert.Empty(result.Enrollments);
            Assert.Empty(result.AnnualFees);
        }

        [Fact]
        public async Task ExecuteAsync_FiltersAnnualFeesByAmountOrCurrency()
        {
            var fees = new List<AnnualFeeSearchDto>
            {
                new AnnualFeeSearchDto(1, "Student A", 100m, "EUR", new DateOnly(2025, 9, 1), null),
                new AnnualFeeSearchDto(2, "Student B", 200m, "USD", new DateOnly(2025, 9, 1), null)
            };

            var query = CreateQuery(
                schools: Array.Empty<School>(),
                scopes: Array.Empty<ScopeLookupDto>(),
                students: Array.Empty<StudentSearchDto>(),
                enrollments: Array.Empty<EnrollmentSearchDto>(),
                fees: fees);

            var result = await query.ExecuteAsync("100, eur", null);

            Assert.Single(result.AnnualFees);
            Assert.Equal(1, result.AnnualFees[0].Id);
            Assert.Equal(100m, result.AnnualFees[0].Amount);
            Assert.False(result.AnnualFees[0].IsPaid);
        }

        private static SearchResultsQuery CreateQuery(
            IEnumerable<School> schools,
            IEnumerable<ScopeLookupDto> scopes,
            IEnumerable<StudentSearchDto> students,
            IEnumerable<EnrollmentSearchDto> enrollments,
            IEnumerable<AnnualFeeSearchDto> fees)
        {
            var schoolSource = new Mock<ISchoolSearchSource>();
            var scopeSource = new Mock<IScopeLookupSource>();
            var studentSource = new Mock<IStudentSearchSource>();
            var enrollmentSource = new Mock<IEnrollmentSearchSource>();
            var annualFeeSource = new Mock<IAnnualFeeSearchSource>();

            schoolSource.Setup(x => x.GetAllAsync()).ReturnsAsync(schools);
            scopeSource.Setup(x => x.GetAllAsync()).ReturnsAsync(scopes);
            studentSource.Setup(x => x.GetAllAsync()).ReturnsAsync(students);
            enrollmentSource.Setup(x => x.GetAllAsync()).ReturnsAsync(enrollments);
            annualFeeSource.Setup(x => x.GetAllAsync()).ReturnsAsync(fees);

            return new SearchResultsQuery(
                schoolSource.Object,
                scopeSource.Object,
                studentSource.Object,
                enrollmentSource.Object,
                annualFeeSource.Object);
        }
    }
}
