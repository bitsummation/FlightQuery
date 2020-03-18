using FlightQuery.Sdk.SqlAst;
using System;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(MemberVariableExpression expression)
        {
            var table = _scope.TableLookupSameLevel(expression.Alias);
            if (!table.Descriptor.ContainsKey(expression.Id))
                throw new InvalidOperationException();

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

            if(table.HasExecuted)
            {
                arg.PropertyValue = table.Rows[table.RowIndex].Values[table.Descriptor.GetDataRowIndex(expression.Id)];
            }
        }
    }
}
