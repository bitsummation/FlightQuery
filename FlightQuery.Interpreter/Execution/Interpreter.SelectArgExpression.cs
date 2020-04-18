using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(SelectArgExpression statement)
        {
            var arg = _visitStack.Peek();
            arg.BoolQueryArg = new Http.QueryArgs();
            VisitChild(statement.Statement, arg);

            if (statement.As != null)
            {
                arg.BoolQueryArg.Variable = statement.As.Alias;
            }
        }
    }
}
