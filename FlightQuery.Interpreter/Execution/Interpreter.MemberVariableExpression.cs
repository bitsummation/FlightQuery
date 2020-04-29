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
                Errors.Add(new VariableNotFound(expression.Alias, expression.Bounds.Start));
                return;
            }
            
            if (!table.Descriptor.ContainsKey(expression.Id))
            {
                Errors.Add(new VariableNotFound(expression.Id, expression.Bounds.Start));
                return;
            }

            var arg = _visitStack.Peek().BoolQueryArg;
            arg.Table = table;

            var prop = table.Descriptor[expression.Id];

            arg.Variable = prop.Name;
            arg.Property = prop;
            if (prop.Queryable)
                table.AddArg(arg);

            if(table.HasExecuted && table.Rows.Length > 0 && table.RowIndex < table.Rows.Length)
            {
                arg.PropertyValue = table.Rows[table.RowIndex].Values[table.Descriptor.GetDataRowIndex(expression.Id)];
            }
        }
    }
}
