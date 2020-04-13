using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(EqualsExpression expression)
        {
            var leftArg = new QueryPhaseArgs() {BoolQueryArg = new EqualQueryArg() };
            VisitChild(expression.Left, leftArg);

            var rightArg = new QueryPhaseArgs() { BoolQueryArg = new EqualQueryArg() };
            VisitChild(expression.Right, rightArg);
            
            var parentArgs = _visitStack.Peek();
            parentArgs.RowResult = ValidateTypes(leftArg.BoolQueryArg, rightArg.BoolQueryArg, (l, r) => l == r);
        }
    }

}
