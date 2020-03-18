using FlightQuery.Interpreter.Http;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(LessThanExpression expression)
        {
            var leftArg = new QueryPhaseArgs() { BoolQueryArg = new QueryLessThan() };
            VisitChild(expression.Left, leftArg);

            var rightArg = new QueryPhaseArgs() { BoolQueryArg = new QueryLessThan() };
            VisitChild(expression.Right, rightArg);

            var parentArgs = _visitStack.Peek();
            parentArgs.RowResult = ValidateTypes(leftArg.BoolQueryArg, rightArg.BoolQueryArg, (l, r) => l < r);
        }
    }
}