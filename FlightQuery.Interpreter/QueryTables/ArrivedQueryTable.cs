using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryTables
{
    public class ArrivedQueryTable : LimitQueryTable
    {
        public ArrivedQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "Arrived "; } }

        public override TableBase Create()
        {
            return new ArrivedQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(Arrived)));
        }

        protected override void ValidateArgs()
        {
            base.ValidateArgs();

            if (QueryArgs.ContainsVariable("airport"))
            {
                QueryArgs["airport"].PropertyValue = new PropertyValue(((string)(QueryArgs["airport"].PropertyValue.Value ?? "")).ToUpper());
            }
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.GetArrived(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(Arrived));

            var rows = new List<Row>();
            if (result.Data != null && result.Error == null)
            {
                foreach (var d in result.Data)
                {
                    if (QueryArgs.ContainsVariable("airport"))
                        d.airport = (string)QueryArgs["airport"].PropertyValue.Value;

                    if (QueryArgs.ContainsVariable("filter"))
                        d.filter = (string)QueryArgs["filter"].PropertyValue.Value;

                    var row = new Row() { Values = ToValues(d, tableDescriptor) };
                    rows.Add(row);
                }
            }

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
        }
    }
}
