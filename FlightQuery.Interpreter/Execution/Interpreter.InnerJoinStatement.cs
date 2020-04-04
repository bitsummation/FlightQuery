using FlightQuery.Interpreter.QueryResults;
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

            if (_scope.IsTableDefineSameLevel(tableName)) //if variable exists at this level we have conflict.
                throw new InvalidOperationException("");

            if (!_scope.IsTableDefinedAnyLevel(tableName)) //we don't know this table at any level
                throw new InvalidOperationException("");

            if (_scope.IsTableDefineSameLevel(tableVariable)) //if variable exists we have conflict
                throw new InvalidOperationException("");

            var executedTables = _scope.FetchAllExecutedTablesSameLevel();
            var rowCount = executedTables.First().Rows.Length;

            var rightTableRows = new List<Row>();
            for(int row = 0; row < rowCount; row++)
            {
                var table = _scope.TableLookupAnyLevel(tableName);
                table = table.Create();
                _scope.AddTable(tableVariable, table);

                Array.ForEach(executedTables, (x) => x.RowIndex = row);
                VisitChild(statement.BooleanExpression);
                var executeTable = table.Execute();

                _scope.RemoveTable(tableVariable);
                _scope.AddTable(tableVariable, executeTable);
                var arg = new QueryPhaseArgs();
                VisitChild(statement.BooleanExpression, arg);
                if(arg.RowResult) //row passed join. If didn't row needs to be removed from the left joining result
                {
                    rightTableRows.AddRange(executeTable.Rows);
                }
                else
                {
                    Array.ForEach(executedTables, (x) => x.Rows[x.RowIndex].Match = false);
                }

                _scope.RemoveTable(tableVariable);
            }


            var returnTable = _scope.TableLookupAnyLevel(tableName);
            var returnExecuteTable = new ExecutedTable() { Descriptor = returnTable.Create().Descriptor, Rows = rightTableRows.ToArray() };
            _scope.AddTable(tableVariable, returnExecuteTable);

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

            VisitChild(statement.InnerJoin);
        }
    }
}
