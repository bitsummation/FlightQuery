namespace FlightQuery.Sdk
{
    public class Cursor
    {
        public Cursor() : this(int.MinValue, int.MinValue) { }

        public Cursor(int row, int column)
        {
            Line = row;
            Column = column;
        }

        public int Line {get; private set;}
        public int Column { get; private set; }

        public static bool operator >=(Cursor c1, Cursor c2)
        {
            return c1.Line >= c2.Line && c1.Column >= c2.Column;
        }

        public static bool operator <=(Cursor c1, Cursor c2)
        {
            return c1.Line <= c2.Line && c1.Column <= c2.Column;
        }
    }
}
