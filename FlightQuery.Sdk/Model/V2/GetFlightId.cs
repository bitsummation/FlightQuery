using System;

namespace FlightQuery.Sdk.Model.V2
{
    public class GetFlightId
    {
        [Required]
        [Queryable]
        public string ident { get; set; }

        [Required]
        [Queryable]
        public DateTime departureTime { get; set; }

        public string faFlightID { get; set; }
    }
}
