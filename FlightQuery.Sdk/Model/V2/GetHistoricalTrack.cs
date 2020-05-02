using System;

namespace FlightQuery.Sdk.Model.V2
{
    public class GetHistoricalTrack
    {
        [Required(1)]
        [Queryable]
        public string faFlightID { get; set; }

        public DateTime timestamp { get; set; }
        public string updateType { get; set; }

        public int altitude { get; set; }
        public string altitudeChange { get; set; }
        public string altitudeStatus { get; set; }
        public int groundspeed { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }
}
