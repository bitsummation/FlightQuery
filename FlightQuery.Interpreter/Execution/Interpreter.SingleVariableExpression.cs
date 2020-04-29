using FlightQuery.Sdk.Semantic;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.Execution
{
    public partial class Interpreter
    {
        public void Visit(SingleVariableExpression expression)
        {
            var tables = _scope.FindTableFromPropertySameLevel(expression.Id);
            if (tables.Count == 0)
            {
                Errors.Add(new VariableNotFound(expression.Id, expression.Bounds.Start));
                return;
            }
            if(tables.Count > 1)
            {
                Errors.Add(new AmbiguousVariable(expression.Id, expression.Bounds.Start));
                return;
            }

            var table = tables[0];
           
            var arg = _visitStack.Peek().BoolQueryArg;
            arg.Table = table;
            var prop = table.Descriptor[expression.Id];
           
            arg.Variable = prop.Name;
            arg.Property = prop;
            if (prop.Queryable)
                table.AddArg(arg);

            if (table.HasExecuted && table.Rows.Length > 0 && table.RowIndex < table.Rows.Length)
            {
                arg.PropertyValue = table.Rows[table.RowIndex].Values[table.Descriptor.GetDataRowIndex(expression.Id)];
            }
        }
    }
}
