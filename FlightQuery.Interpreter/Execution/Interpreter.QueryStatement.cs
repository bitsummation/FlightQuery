using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk.SqlAst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(QueryStatement statement)
        {
            using (var s = _scope.Push())
            {
                //Visit from to get tables in scope
                statement.From.Accept(this);

                var executedTables = _scope.FetchAllExecutedTablesSameLevel();
                var rowCount = executedTables.First().Rows.Length;
                for (int row = 0; row < rowCount; row++)
                {
                    Array.ForEach(executedTables, (x) => x.RowIndex = row);
                    var arg = new QueryPhaseArgs();
                    VisitChild(statement.Where, arg);
                    if(!arg.RowResult)
                    {
                        Array.ForEach(executedTables, (x) => x.Rows[x.RowIndex].Match = false);
                    }
                }

                //remove rows that didn't match through all executed tables
                executedTables = _scope.FetchAllExecutedTablesSameLevel();
                Array.ForEach(executedTables, (e) =>
                {
                    var joinedRows = new List<Row>();
                    Array.ForEach(e.Rows, r => {
                        if (r.Match)
                            joinedRows.Add(r);
                    });

                    e.Rows = joinedRows.ToArray();
                });

                statement.Select.Accept(this);
            }
        }
    }
}
