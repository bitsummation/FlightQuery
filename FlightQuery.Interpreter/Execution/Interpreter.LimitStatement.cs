using FlightQuery.Sdk.SqlAst;
using System;
using System.Linq;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(LimitStatement statement)
        {
            var executedTables = _scope.FetchAllExecutedTablesSameLevel();
            Array.ForEach(executedTables, (e) =>
            {
                e.Rows = e.Rows.Take(statement.Count).ToArray();
            });
        }
    }
}
