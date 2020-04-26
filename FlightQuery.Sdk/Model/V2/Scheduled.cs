using System;

namespace FlightQuery.Sdk.Model.V2
{
    public class Scheduled
    {
        [Required(1)]
        [Queryable]
        public string airport { get; set; }

        public string ident { get; set; }

        public DateTime estimatedarrivaltime { get; set; }
        public DateTime filed_departuretime { get; set; }

        public string origin { get; set; }
        public string destination { get; set; }

        public string aircrafttype { get; set; }
        public string destinationCity { get; set; }
        public string destinationName { get; set; }
        
        public string originCity { get; set; }
        public string originName { get; set; }

    }
}
