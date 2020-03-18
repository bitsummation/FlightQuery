using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightQuery.Sdk.Semantic
{
    public abstract class SemanticError : IEquatable<SemanticError>
    {
        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }

        public bool Equals(SemanticError other)
        {
            return Message == other.Message;
        }

        public abstract string Message { get;  }
    }
}
