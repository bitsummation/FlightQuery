using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(LessThanEqualExpression expression)
        {
            var leftArg = new QueryPhaseArgs() { BoolQueryArg = new QueryLessThanEqual() };
            VisitChild(expression.Left, leftArg);

            var rightArg = new QueryPhaseArgs() { BoolQueryArg = new QueryLessThanEqual() };
            VisitChild(expression.Right, rightArg);

            var parentArgs = _visitStack.Peek();
            parentArgs.RowResult = ValidateTypes(leftArg.BoolQueryArg, rightArg.BoolQueryArg, (l, r) => l <= r);
        }
    }
}