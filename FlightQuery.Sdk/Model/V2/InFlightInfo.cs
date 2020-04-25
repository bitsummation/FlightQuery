using System;

namespace FlightQuery.Sdk.Model.V2
{
    public class InFlightInfo
    {
        public int altitude { get; set; }
        public string altitudeChange { get; set; }
        public string altitudeStatus { get; set; }

        public DateTime arrivalTime { get; set; }
        public DateTime departureTime { get; set; }

        public string destination { get; set; }
        public string faFlightID { get; set; }

        public DateTime firstPositionTime { get; set; }
        public int groundspeed { get; set; }
        public int heading { get; set; }
        public float highLatitude { get; set; }
        public float highLongitude { get; set; }

        [Required(1)]
        [Queryable]
        public string ident { get; set; }

        public float latitude { get; set; }
        public float longitude { get; set; }
        public float lowLatitude { get; set; }
        public float lowLongitude { get; set; }

        public string origin { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
        public string timeout { get; set; }

        public DateTime timestamp { get; set; }
        public string type { get; set; }
        public string updateType { get; set; }

        public string waypoints { get; set; }
    }
}
