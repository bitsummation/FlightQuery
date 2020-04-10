using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public class ScopeDescriptor
    {
        public IEnumerable<string> Keys { get; set; }
        public Dictionary<string, IEnumerable<string>> Items { get; set; }
    }
}
