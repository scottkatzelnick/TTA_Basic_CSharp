using System;

namespace Casino
{
    public class FraudException : Exception
    {
        public FraudException()

            : base() { }

        public FraudException(string message)
            : base(message) { }
    }
}