namespace FlightQuery.Sdk.SqlAst
{
    public class ElementBounds
    {
        public ElementBounds(Cursor start, Cursor stop)
        {
            Start = start;
            Stop = stop;
        }

        public Cursor Start { get; private set; }
        public Cursor Stop { get; private set; }

        private bool IsValid()
        {
            return Start != null && Stop != null;
        }

        public bool Contains(Cursor cursor)
        {
            return IsValid() && cursor.Line >= Start.Line
                && cursor.Line <= Stop.Line;
        }
    }
}
