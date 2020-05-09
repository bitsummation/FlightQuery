namespace FlightQuery.Sdk.Model.V2
{
    public class MapFlight
    {
        [Required(1)]
        [Queryable]
        public string ident { get; set; }

        [Queryable]
        public int mapHeight { get; set; }
        [Queryable]
        public int mapWidth { get; set; }

        public string image { get; set; }
    }
}
