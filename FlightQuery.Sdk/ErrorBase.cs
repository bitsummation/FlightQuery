using System;

namespace FlightQuery.Sdk
{
    public abstract class ErrorBase : IEquatable<ErrorBase>
    {
        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }

        public bool Equals(ErrorBase other)
        {
            return Message == other.Message;
        }

        public abstract string Message { get; }
    }
}
