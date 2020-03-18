using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(WhereStatement statement)
        {
            var args = _visitStack.Peek();
            VisitChild(statement.BooleanExpression, args);
        }
    }
}
