using System;

namespace FlightQuery.Sdk.Model.V2
{
    public class Arrived
    {
        [Required(1)]
        [Queryable]
        public string airport { get; set; }

        public string ident { get; set; }
        public DateTime actualarrivaltime { get; set; }
        public DateTime actualdeparturetime { get; set; }

        public string aircrafttype { get; set; }
        public string destination { get; set; }
        public string destinationCity { get; set; }
        public string destinationName { get; set; }

        public string origin { get; set; }
        public string originCity { get; set; }
        public string originName { get; set; }

        [Queryable]
        public string filter { get; set; }
    }
}
