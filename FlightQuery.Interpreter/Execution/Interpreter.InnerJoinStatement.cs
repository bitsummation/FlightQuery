using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(InnerJoinStatement statement)
        {
            string tableName = statement.Name;

            string tableVariable;
            if (statement.Alias != null)
                tableVariable = statement.Alias;
            else
                tableVariable = statement.Name;

            if (_scope.IsTableDefineSameLevel(tableName)) //table exists already
                Errors.Add(new TableConflict(tableName, statement.Bounds.Start));

            if (!_scope.IsTableDefinedAnyLevel(tableName)) //we don't know this table at any level
            {
                Errors.Add(new TableNotDefined(tableName, statement.Bounds.Start));
                return;
            }

            if (_scope.IsTableDefineSameLevel(tableVariable)) //if alias variable exists we have conflict
                throw new InvalidOperationException("");

            var executedTables = _scope.FetchAllExecutedTablesSameLevel();
            int rowCount = 0;
            if(executedTables.Length > 0)
                rowCount = executedTables.First().Rows.Length;

            var rightTableRows = new List<Row>();
            if (rowCount == 0) //need to visit for semantic errors
            {
                var table = _scope.TableLookupAnyLevel(tableName);
                table = table.Create();
                _scope.AddTable(tableVariable, table);
                VisitChild(statement.BooleanExpression);
                VisitWhereIgnoreErrors(statement.ParentQueryStatement.Where);

                var executeTable = table.Execute(statement.ParentQueryStatement.Limit.Offset, statement.ParentQueryStatement.Limit.Count);
                foreach (var e in table.Errors)
                    Errors.Add(e);

                _scope.RemoveTable(tableVariable);
            }
            else
            {
                for (int row = 0; row < rowCount; row++)
                {
                    var table = _scope.TableLookupAnyLevel(tableName);
                    table = table.Create();
                    _scope.AddTable(tableVariable, table);

                    Array.ForEach(executedTables, (x) => x.RowIndex = row);
                    VisitChild(statement.BooleanExpression);
                    VisitWhereIgnoreErrors(statement.ParentQueryStatement.Where);
                    var executeTable = table.Execute(statement.ParentQueryStatement.Limit.Offset, statement.ParentQueryStatement.Limit.Count);

                    foreach (var e in table.Errors)
                        Errors.Add(e);

                    _scope.RemoveTable(tableVariable); //remove query table
                    _scope.AddTable(tableVariable, executeTable); // add execute table
                    var arg = new QueryPhaseArgs();
                    VisitChild(statement.BooleanExpression, arg);
                    if (arg.RowResult) //row passed join. If didn't row needs to be removed from the left joining result
                    {
                        Array.ForEach(executedTables, (x) => x.Rows[x.RowIndex].Expand = executeTable.Rows.Length);
                        rightTableRows.AddRange(executeTable.Rows);
                    }
                    else //flag row as unmatching for all executed tables
                    {
                        Array.ForEach(executedTables, (x) => x.Rows[x.RowIndex].Match = false);
                    }

                    _scope.RemoveTable(tableVariable);
                }
            }

            var returnTable = _scope.TableLookupAnyLevel(tableName);
            var returnExecuteTable = new ExecutedTable(returnTable.Create().Descriptor) { Rows = rightTableRows.ToArray() };
            _scope.AddTable(tableVariable, returnExecuteTable);

            executedTables = _scope.FetchAllExecutedTablesSameLevel();
            Array.ForEach(executedTables, (e) =>
            {
                var joinedRows = new List<Row>();

                //Add matched rows
                Array.ForEach(e.Rows, r => {
                    if (r.Match)
                    {
                        for(int x = 0; x < r.Expand; x++)
                            joinedRows.Add(r);
                    }
                });

                e.Rows = joinedRows.ToArray();
            });

            VisitChild(statement.InnerJoin);
        }
    }
}
