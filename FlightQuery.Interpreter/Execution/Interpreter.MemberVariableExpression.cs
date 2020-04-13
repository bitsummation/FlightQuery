using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(MemberVariableExpression expression)
        {
            var table = _scope.TableLookupSameLevel(expression.Alias);
            if(table == null) //table is null when invalid alias
            {
                Errors.Add(new VariableNotFound(expression.Alias, expression.ParseInfo));
                return;
            }
            
            if (!table.Descriptor.ContainsKey(expression.Id))
            {
                Errors.Add(new VariableNotFound(expression.Id, expression.ParseInfo));
                return;
            }

            var arg = _visitStack.Peek().BoolQueryArg;
            var prop = table.Descriptor[expression.Id];
            if (arg == null) //in select context. No boolean result needed
            {
                prop.SelectedIndex.Add(table.SelectIndex);
                return;
            }

            arg.Variable = prop.Name;
            arg.Property = prop;
            if (prop.Queryable)
                table.AddArg(arg);

            if(table.HasExecuted && table.Rows.Length > 0)
            {
                arg.PropertyValue = table.Rows[table.RowIndex].Values[table.Descriptor.GetDataRowIndex(expression.Id)];
            }
        }
    }
}
