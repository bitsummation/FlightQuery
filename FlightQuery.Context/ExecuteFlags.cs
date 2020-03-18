using System;

namespace FlightQuery.Context
{
    [Flags]
    public enum ExecuteFlags
    {
        Semantic = 1,
        Execute = 2, 
        Run = Semantic | Execute, //needs symatic check and exexute.
    }
}
