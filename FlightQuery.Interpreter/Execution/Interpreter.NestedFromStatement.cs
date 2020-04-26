using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(NestedFromStatement statement)
        {
            var args = new QueryPhaseArgs();
            VisitChild(statement.Query, args);

            _scope.AddTable(statement.Alias, args.QueryTable);

            VisitChild(statement.InnerJoin);
        }
    }
}
