namespace FlightQuery.Sdk.Model.V2
{
    public class AirportInfo
    {
        [Queryable]
        [Required(1)]
        public string airportCode { get; set; }

        public float latitude { get; set; }
        public float longitude { get; set; }

        public string location { get; set; }
       
        public string name { get; set; }
        public string timezone { get; set; }
    }
}
