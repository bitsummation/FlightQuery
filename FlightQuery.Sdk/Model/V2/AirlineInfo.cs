namespace FlightQuery.Sdk.Model.V2
{
    public class AirlineInfo
    {
        [Queryable]
        [Required(1)]
        public string airlineCode { get; set; }

        public string callsign { get; set; }
        public string country { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string shortname { get; set; }
        public string url { get; set; }
    }
}
