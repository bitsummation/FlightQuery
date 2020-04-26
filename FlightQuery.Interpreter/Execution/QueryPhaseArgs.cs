using FlightQuery.Interpreter.Http;
using FlightQuery.Interpreter.QueryResults;

namespace FlightQuery.Interpreter.Execution
{
    public class QueryPhaseArgs 
    {
        public TableBase QueryTable { get; set; }

        public QueryArgs BoolQueryArg { get; set; }

        public bool RowResult { get; set; }
    }
}
