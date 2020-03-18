using FlightQuery.Interpreter.QueryResults;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.Execution
{
    public class ScopeEntry
    {
        public ScopeEntry()
        {
            Variables = new Dictionary<string, TableBase>();
        }

        public Dictionary<string, TableBase> Variables { get; private set; }
    }
}
