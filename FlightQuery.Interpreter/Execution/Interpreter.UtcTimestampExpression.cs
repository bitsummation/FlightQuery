using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(UtcTimestampExpression expression)
        {
            var arg = _visitStack.Peek().BoolQueryArg;
            arg.PropertyValue = new PropertyValue(expression.Value);
        }
    }
}
