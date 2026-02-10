using System;
using Infrastructure.src.Domain.Entities;
using Xunit;

namespace UnitTest.Infrastructure
{
    public class ScaffoldEntitiesTests
    {
        [Fact]
        public void Users_CanSetProperties()
        {
            var user = new Users
            {
                Id = 1,
                FirstName = "A",
                LastName = "B",
                Email = "a@b.com",
                PasswordHash = "hash",
                Role = "USER",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                BirthDate = new DateOnly(2000, 1, 1)
            };

            Assert.Equal("A", user.FirstName);
        }

        [Fact]
        public void Students_CanSetProperties()
        {
            var student = new Students
            {
                Id = 1,
                SchoolId = 2,
                UserId = 3,
                CreatedAt = DateTime.UtcNow
            };

            Assert.Equal(2, student.SchoolId);
        }

        [Fact]
        public void Schools_CanSetProperties()
        {
            var school = new Schools
            {
                Id = 1,
                Name = "School",
                Code = "C1",
                City = "City",
                IsFavorite = true
            };

            Assert.True(school.IsFavorite);
        }

        [Fact]
        public void Enrollments_CanSetProperties()
        {
            var enrollment = new Enrollments
            {
                Id = 1,
                StudentId = 1,
                AcademicYear = "2025",
                Status = "Active",
                EnrolledAt = DateTime.UtcNow
            };

            Assert.Equal("2025", enrollment.AcademicYear);
        }

        [Fact]
        public void AnnualFees_CanSetProperties()
        {
            var fee = new AnnualFees
            {
                Id = 1,
                EnrollmentId = 1,
                Amount = 100m,
                Currency = "EUR",
                DueDate = new DateOnly(2025, 9, 1),
                PaidAt = DateTime.UtcNow,
                PaymentRef = "REF",
                StudentId = 2,
                Enrollment = new Enrollments { Id = 1, StudentId = 1, AcademicYear = "2025", Status = "Active", EnrolledAt = DateTime.UtcNow },
                Student = new Students { Id = 2, SchoolId = 1, CreatedAt = DateTime.UtcNow }
            };

            Assert.Equal(100m, fee.Amount);
        }

        [Fact]
        public void ScopeMnt_CanSetProperties()
        {
            var scope = new ScopeMnt
            {
                Id = 1,
                Name = "Primary",
                Description = "Desc",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            Assert.True(scope.IsActive);
        }
    }
}
