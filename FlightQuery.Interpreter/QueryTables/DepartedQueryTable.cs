using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryTables
{
    public class DepartedQueryTable : LimitQueryTable
    {
        public DepartedQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "Departed"; } }

        public override TableBase Create()
        {
            return new DepartedQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(Departed)));
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
            var result = HttpExecutor.GetDeparted(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(Departed));

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
