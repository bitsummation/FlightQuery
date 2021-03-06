﻿using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;
using System;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(FromStatement statement)
        {
            string tableName = statement.Name;
            if (tableName == null)
                return;

            string tableVariable;
            if (statement.Alias != null)
                tableVariable = statement.Alias;
            else
                tableVariable = statement.Name;

            if (_scope.IsTableDefineSameLevel(tableName)) //if variable exists at this level we have conflict.
                throw new InvalidOperationException("");

            if (!_scope.IsTableDefinedAnyLevel(tableName)) //we don't know this table at any level
            {
                Errors.Add(new TableNotDefined(tableName, statement.Bounds.Start));
                return;
            }

            var table =  _scope.TableLookupAnyLevel(tableName);
            table = table.Create();
            _scope.AddTable(tableVariable, table);

            VisitWhereIgnoreErrors(statement.ParentQueryStatement.Where);

            var result = table.Execute(statement.ParentQueryStatement.Limit);
            foreach (var e in table.Errors)
                Errors.Add(e);

            _scope.RemoveTable(tableVariable);
            _scope.AddTable(tableVariable, result);

            VisitChild(statement.InnerJoin);
        }
    }
}
