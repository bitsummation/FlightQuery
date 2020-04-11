using System;

namespace FlightQuery.Sdk
{
    [Flags]
    public enum ExecuteFlags
    {
        Semantic = 1,
        Execute = 2, 
        Intellisense = 4,
        Run = Semantic | Execute, //needs symatic check and exexute.
    }
}
