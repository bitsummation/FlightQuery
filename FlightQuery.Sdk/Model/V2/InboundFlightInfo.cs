using Newtonsoft.Json;
using System;

namespace FlightQuery.Sdk.Model.V2
{
    public class InboundFlightInfo
    {
        [Required(1)]
        [Queryable]
        public string faFlightID { get; set; }

        public string ifaFlightID { get; set; }

        public DateTime actualarrivaltime { get; set; }
        public DateTime actualdeparturetime { get; set; }
        public DateTime estimatedarrivaltime { get; set; }
        public DateTime filed_departuretime { get; set; }
        public DateTime filed_time { get; set; }

        public string aircrafttype { get; set; }
        public string destination { get; set; }
        public string destinationCity { get; set; }
        public string destinationName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool diverted { get; set; }

        public int filed_airspeed_kts { get; set; }
        public string filed_airspeed_mach { get; set; }

        public int filed_altitude { get; set; }

        public string filed_ete { get; set; }

        public string ident { get; set; }

        public string origin { get; set; }
        public string originCity { get; set; }
        public string originName { get; set; }
        public string route { get; set; }
    }
}
