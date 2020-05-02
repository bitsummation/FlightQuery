using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(NestedFromStatement statement)
        {
            var args = new QueryPhaseArgs();
            VisitChild(statement.Query, args);

            if(args.QueryTable != null && statement.Alias != null)
                _scope.AddTable(statement.Alias, args.QueryTable);

            VisitChild(statement.InnerJoin);
        }
    }
}
