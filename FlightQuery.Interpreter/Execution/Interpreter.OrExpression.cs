using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(OrExpression expression)
        {
            var parentArgs = _visitStack.Peek() as QueryPhaseArgs;

            var leftArts = new QueryPhaseArgs();
            VisitChild(expression.Left, leftArts);
            var rightArgs = new QueryPhaseArgs();
            VisitChild(expression.Right, rightArgs);

            parentArgs.RowResult = leftArts.RowResult || rightArgs.RowResult;
        }
    }
}
