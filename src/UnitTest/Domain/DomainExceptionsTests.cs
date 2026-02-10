using System.Collections.Generic;
using Domain.DomainExceptions;
using Xunit;

namespace UnitTest.DomainTests
{
    public class DomainExceptionsTests
    {
        private sealed class TestDomainException : DomainException
        {
            public TestDomainException(string message) : base(message) { }
            public TestDomainException(string message, System.Exception inner) : base(message, inner) { }
        }

        [Fact]
        public void DomainException_StoresMessage()
        {
            var ex = new TestDomainException("msg");

            Assert.Equal("msg", ex.Message);
        }

        [Fact]
        public void DomainException_StoresInnerException()
        {
            var inner = new System.Exception("inner");
            var ex = new TestDomainException("msg", inner);

            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void NotFoundException_FormatsMessage()
        {
            var ex = new NotFoundException("School", 1);

            Assert.Contains("School", ex.Message);
        }

        [Fact]
        public void NotFoundException_UsesCustomMessage()
        {
            var ex = new NotFoundException("custom");

            Assert.Equal("custom", ex.Message);
        }

        [Fact]
        public void ValidationException_BuildsErrors()
        {
            var ex = new ValidationException("Field", "error");

            Assert.True(ex.Errors.ContainsKey("Field"));
        }

        [Fact]
        public void ValidationException_FromDictionary()
        {
            var errors = new Dictionary<string, string[]> { { "A", new[] { "B" } } };
            var ex = new ValidationException(errors);

            Assert.True(ex.Errors.ContainsKey("A"));
        }
    }
}
