using System;

namespace FlightQuery.Sdk.Model.V2
{
    public class GetFlightId
    {
        [Required(1)]
        [Queryable]
        public string ident { get; set; }

        [Required(1)]
        [Queryable]
        public DateTime departureTime { get; set; }

        public string faFlightID { get; set; }
    }
}
