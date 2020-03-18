namespace FlightQuery.Sdk.Model.V2
{
    public class AirportInfo
    {
        [Queryable]
        [Required]
        public string airportCode { get; set; }

        public string callsign { get; set; }
        public string country { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string shortname { get; set; }
        public string url { get; set; }
    }
}
