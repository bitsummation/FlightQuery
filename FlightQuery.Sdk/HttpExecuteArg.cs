using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public class HttpExecuteArg
    {
        public string UserName { get; set; }
        public string Pass { get; set; }

        public string TableName { get; set; }

        public IEnumerable<HttpQueryVariabels> Variables { get; set; }
    }
}
