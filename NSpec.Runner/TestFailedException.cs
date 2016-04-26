using System;

namespace NSpec.Runner
{
    public class TestFailedException : Exception
    {
        public TestFailedException(string message) : base(message) { }
        public TestFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
