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
                if(statement.From != null)
                    statement.From.Accept(this);

                var executedTables = _scope.FetchAllExecutedTablesSameLevel();
                if (executedTables.Length > 0 && statement.Where != null)
                {
                    var rowCount = executedTables.First().Rows.Length;
                    if (rowCount == 0) //even if no rows we still want to validate where variables
                    {
                        Array.ForEach(executedTables, (x) => x.RowIndex = 0);
                        var arg = new QueryPhaseArgs();
                        VisitChild(statement.Where, arg);
                    }
                    else
                    {
                        for (int row = 0; row < rowCount; row++)
                        {
                            Array.ForEach(executedTables, (x) => x.RowIndex = row);
                            var arg = new QueryPhaseArgs();
                            VisitChild(statement.Where, arg);
                            if (!arg.RowResult)
                            {
                                Array.ForEach(executedTables, (x) => x.Rows[x.RowIndex].Match = false);
                            }
                        }
                    }

                    //remove rows that didn't match through all executed tables
                    executedTables = _scope.FetchAllExecutedTablesSameLevel();
                    Array.ForEach(executedTables, (e) =>
                    {
                        var joinedRows = new List<Row>();
                        Array.ForEach(e.Rows, r =>
                        {
                            if (r.Match)
                                joinedRows.Add(r);
                        });

                        e.Rows = joinedRows.ToArray();
                    });
                }

                var args = new QueryPhaseArgs();
                if (statement.Select != null)
                    VisitChild(statement.Select, args);

                _visitStack.Peek().QueryTable = args.QueryTable;
                ScopeModel = _scope.BuildScopeModel();
            }
        }
    }
}
