using System;

namespace FakeMessageBus
{
    public class InvalidCallbackException : Exception
    {
        public InvalidCallbackException(string message) : base(message) { }
    }
}
