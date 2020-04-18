using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(CaseStatement statement)
        {
            object value = null;
            foreach (var when in statement.WhenExpression)
            {
                var arg = new QueryPhaseArgs();
                VisitChild(when.BooleanExpression);
                if (arg.RowResult) //we matched
                {
                    arg = new QueryPhaseArgs();
                    arg.BoolQueryArg = new Http.QueryArgs();
                    VisitChild(when.Variable, arg); //want to get it's value
                    value = arg.BoolQueryArg.PropertyValue.Value;
                    break;
                }
            }

            if(value == null && statement.ElseVariable != null)
            {
                var arg = new QueryPhaseArgs();
                arg.BoolQueryArg = new Http.QueryArgs();
                VisitChild(statement.ElseVariable, arg); //want to get it's value
                value = arg.BoolQueryArg.PropertyValue.Value;
            }

            _visitStack.Peek().BoolQueryArg.PropertyValue = new PropertyValue(value);
        }
    }

}