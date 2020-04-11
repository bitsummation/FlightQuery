using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public class HttpExecuteArg
    {
        public string TableName { get; set; }
        public IEnumerable<HttpQueryVariabels> Variables { get; set; }
    }
}
