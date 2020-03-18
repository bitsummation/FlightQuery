using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(StringLiteral literal)
        {
            var arg = _visitStack.Peek().BoolQueryArg;
            arg.PropertyValue =  new PropertyValue(literal.Value);
        }
    }
}
