using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;
using System;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(SingleVariableExpression expression)
        {
            var tables = _scope.FindTableFromPropertySameLevel(expression.Id);
            if (tables.Count == 0)
            {
                Errors.Add(new VariableNotFound(expression.Id, expression.ParseInfo));
                return;
            }
            if(tables.Count > 1)
            {
                Errors.Add(new AmbiguousVariable(expression.Id, expression.ParseInfo));
                return;
            }

            var table = tables[0];
            
            var arg = _visitStack.Peek().BoolQueryArg;
            var prop = table.Descriptor[expression.Id];
            if (arg == null) //in select context. No boolean result needed
            {
                prop.SelectedIndex = table.SelectIndex;
                return;
            }

            arg.Variable = prop.Name;
            arg.Property = prop;
            if (prop.Queryable)
                table.AddArg(arg);

            if (table.HasExecuted)
            {
                arg.PropertyValue = table.Rows[table.RowIndex].Values[table.Descriptor.GetDataRowIndex(expression.Id)];
            }
        }
    }
}
