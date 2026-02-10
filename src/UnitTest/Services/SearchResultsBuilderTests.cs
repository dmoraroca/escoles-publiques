using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Search;
using Moq;
using Web.Services.Search;
using Xunit;

namespace UnitTest.Services
{
    public class SearchResultsBuilderTests
    {
        [Fact]
        public async Task BuildAsync_MapsDtoToViewModel()
        {
            var dto = new SearchResultsDto
            {
                SearchQuery = "alpha",
                ScopeName = "Primary",
                Schools = new List<SchoolResultDto>
                {
                    new SchoolResultDto { Id = 1, Name = "Alpha", Code = "A1", ScopeName = "Primary", Scope = "Primary" }
                },
                Students = new List<StudentResultDto>
                {
                    new StudentResultDto { Id = 2, FirstName = "Ana", LastName = "Perez", Email = null, SchoolName = "Alpha" }
                },
                Enrollments = new List<EnrollmentResultDto>
                {
                    new EnrollmentResultDto { Id = 3, StudentName = "Ana Perez", SchoolName = "Alpha", AcademicYear = "2025", EnrollmentDate = new DateTime(2025, 1, 1) }
                },
                AnnualFees = new List<AnnualFeeResultDto>
                {
                    new AnnualFeeResultDto { Id = 4, StudentName = "Ana Perez", Amount = 100m, Currency = "EUR", DueDate = new DateOnly(2025, 9, 1), IsPaid = false }
                }
            };

            var queryMock = new Mock<ISearchResultsQuery>();
            queryMock.Setup(q => q.ExecuteAsync("alpha", "Primary")).ReturnsAsync(dto);

            var builder = new SearchResultsBuilder(queryMock.Object);

            var result = await builder.BuildAsync("alpha", "Primary");

            Assert.Equal("alpha", result.SearchQuery);
            Assert.Equal("Primary", result.ScopeName);
            Assert.Single(result.Schools);
            Assert.Single(result.Students);
            Assert.Single(result.Enrollments);
            Assert.Single(result.AnnualFees);
            Assert.Equal("", result.Students[0].Email);
        }
    }
}
