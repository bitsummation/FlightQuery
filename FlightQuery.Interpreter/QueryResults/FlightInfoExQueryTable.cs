using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryResults
{
    public class FlightInfoExQueryTable : QueryTable
    {
        public FlightInfoExQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "FlightInfoEx"; } }

        public override TableBase Create()
        {
            return new FlightInfoExQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(FlightInfoEx)));
        }

        protected override void ValidateArgs()
        {
            base.ValidateArgs();

            if(QueryArgs.ContainsVariable("ident"))
            {
                QueryArgs["ident"].PropertyValue = new PropertyValue(((string)(QueryArgs["ident"].PropertyValue.Value ?? "")).ToUpper());
            }
            else if (QueryArgs.ContainsVariable("faFlightID"))
            {
                var flightid = QueryArgs["faFlightID"];
                QueryArgs.Clear();
                QueryArgs.Add(new QueryArgs { Variable = "ident", PropertyValue = new PropertyValue(flightid.PropertyValue.Value) });
            }
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.GetFlightInfoEx(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(FlightInfoEx));

            var rows = new List<Row>();
            if (result.Data != null && result.Error == null)
            {
                foreach (var d in result.Data)
                {
                    var row = new Row() { Values = ToValues(d, tableDescriptor) };
                    rows.Add(row);
                }
            }

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
        }
    }
}
