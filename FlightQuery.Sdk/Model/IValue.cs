using System;

namespace FlightQuery.Sdk.Model
{
    public interface IValue
    {
        IComparable ToValue();
    }
}
