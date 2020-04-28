using FlightQuery.Sdk;

namespace FlightQuery.Interpreter
{
    public class QueryBounds
    {
        public QueryBounds()
            : this(new Cursor(int.MaxValue, int.MaxValue), new Cursor(int.MinValue, int.MinValue))
        {
        }

        public QueryBounds(Cursor min, Cursor max)
        {
            Min = min;
            Max = max;
        }

        public Cursor Min { get; private set; }
        public Cursor Max { get; private set; }

        public bool Contains(Cursor cursor)
        {
            return cursor.Line >= Min.Line
                && cursor.Line <= Max.Line+1;
        }
    }
}
