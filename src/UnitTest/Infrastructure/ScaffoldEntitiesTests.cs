using System;
using Domain.Entities;
using Xunit;

namespace UnitTest.Infrastructure
{
    public class ScaffoldEntitiesTests
    {
        [Fact]
        public void Users_CanSetProperties()
        {
            var user = new User
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
            var student = new Student
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
            var school = new School
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
            var enrollment = new Enrollment
            {
                Id = 1,
                StudentId = 1,
                AcademicYear = "2025",
                Status = "Active",
                EnrolledAt = DateTime.UtcNow,
                SchoolId = 2
            };

            Assert.Equal("2025", enrollment.AcademicYear);
        }

        [Fact]
        public void AnnualFees_CanSetProperties()
        {
            var fee = new AnnualFee
            {
                Id = 1,
                EnrollmentId = 1,
                Amount = 100m,
                Currency = "EUR",
                DueDate = new DateOnly(2025, 9, 1),
                PaidAt = DateTime.UtcNow,
                PaymentRef = "REF",
                Enrollment = new Enrollment
                {
                    Id = 1,
                    StudentId = 1,
                    AcademicYear = "2025",
                    Status = "Active",
                    EnrolledAt = DateTime.UtcNow,
                    SchoolId = 2
                }
            };

            Assert.Equal(100m, fee.Amount);
        }

        [Fact]
        public void ScopeMnt_CanSetProperties()
        {
            var scope = new Scope
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
