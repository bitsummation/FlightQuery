using FlightQuery.Interpreter.Http;

namespace FlightQuery.Interpreter.Execution
{
    public class QueryPhaseArgs 
    {
        public QueryArgs BoolQueryArg { get; set; }

        public bool RowResult { get; set; }
    }
}
