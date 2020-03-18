using System;
using System.Collections.Generic;
using System.Text;

namespace FlightQuery.Sdk.SqlAst
{
    public class ParseInfo
    {
        public ParseInfo(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; private set; }
        public int Column { get; private set; }
    }
}
