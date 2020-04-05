using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryResults
{
    public class FlightInfoExQueryTable : QueryTable
    {
        public FlightInfoExQueryTable(IHttpExecutor httpExecutor) : base(httpExecutor) { }

        protected override string TableName { get { return "FlightInfoEx"; } }

        public override TableBase Create()
        {
            return new FlightInfoExQueryTable(HttpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(FlightInfoEx)) };
        }

        protected override bool ValidateArgs()
        {
            base.ValidateArgs();

            if (QueryArgs.ContainsVariable("faFlightID"))
            {
                var flightid = QueryArgs["faFlightID"];
                QueryArgs.Clear();
                QueryArgs.Add(new QueryArgs { Variable = "ident", PropertyValue = new PropertyValue(flightid.PropertyValue.Value) });
            }

            return true;
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.GetFlightInfoEx(args);
            if (result.Error != null)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(FlightInfoEx));

            var rows = new List<Row>();
            if (result.Data != null)
            {
                foreach (var d in result.Data)
                {
                    var row = new Row() { Values = ToValues(d, tableDescriptor) };
                    rows.Add(row);
                }
            }

            return new ExecutedTable() { Rows = rows.ToArray(), Descriptor = tableDescriptor };
        }
    }
}
