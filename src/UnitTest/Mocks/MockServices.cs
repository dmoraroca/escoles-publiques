using Moq;
using Application.Interfaces;

namespace UnitTest.Mocks
{
    public static class MockServices
    {
        public static Mock<IEnrollmentService> GetEnrollmentService()
            => new Mock<IEnrollmentService>();
        public static Mock<IStudentService> GetStudentService()
            => new Mock<IStudentService>();
        public static Mock<IUserService> GetUserService()
            => new Mock<IUserService>();
        public static Mock<IAuthService> GetAuthService()
            => new Mock<IAuthService>();
        public static Mock<ISchoolService> GetSchoolService()
            => new Mock<ISchoolService>();
    }
}
