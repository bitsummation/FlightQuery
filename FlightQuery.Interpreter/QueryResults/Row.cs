namespace FlightQuery.Interpreter.QueryResults
{
    public class Row
    {
        public Row()
        {
            Match = true;
            Expand = 1;
        }

        public int Expand { get; set; } //for joins when we need to add rows
        public bool Match { get; set; }
        public PropertyValue[] Values { get; set; }
    }
}
