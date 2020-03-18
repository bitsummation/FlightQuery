namespace FlightQuery.Interpreter.QueryResults
{
    public class Row
    {
        public Row()
        {
            Match = true;
        }

        public bool Match { get; set; }
        public PropertyValue[] Values { get; set; }
    }
}
