using System;

namespace FlightQuery.Sdk.Model.V2
{
    public class FlightId
    {
        [Required]
        [Queryable]
        public string ident { get; set; }

        [Required]
        [Queryable]
        public DateTime departuretime { get; set; }

        public string faFlightID { get; set; }
    }
}
