namespace FlightQuery.Sdk.Model.V2
{
    public class MapFlight
    {
        [Required(1)]
        [Queryable]
        public string ident { get; set; }

        [Queryable]
        public long mapHeight { get; set; }
        [Queryable]
        public long mapWidth { get; set; }

        public Base64Image image { get; set; }
    }
}
