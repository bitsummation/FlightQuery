using System;
using System.Linq;

namespace FlightQuery.Sdk.Model.V2
{
    public class AirlineFlightSchedule
    {
        [Queryable]
        public string ident { get; set; }
        public string actual_ident { get; set; }

        [Required]
        [Queryable]
        public DateTime departuretime { get; set; }

        public DateTime arrivaltime { get; set; }
        [Queryable]
        public string origin { get; set; }
        [Queryable]
        public string destination { get; set; }
        public string aircrafttype { get; set; }
        public string meal_service { get; set; }
        public int seats_cabin_first { get; set; }
        public int seats_cabin_business { get; set; }
        public int seats_cabin_coach { get; set; }
    }
}
