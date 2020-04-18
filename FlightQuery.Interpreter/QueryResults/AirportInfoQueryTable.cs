using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;

namespace FlightQuery.Interpreter.QueryResults
{
    public class AirportInfoQueryTable : QueryTable
    {
        public AirportInfoQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "AirportInfo"; } }

        public override TableBase Create()
        {
            return new AirportInfoQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(AirportInfo)));
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.AirportInfo(args);
            if (result.Error != null)
                Errors.Add(result.Error);

            if (QueryArgs.ContainsVariable("airportCode"))
                result.Data.airportCode = (string)QueryArgs["airportCode"].PropertyValue.Value;

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(AirportInfo));

            var row = new Row() { Values = ToValues(result.Data, tableDescriptor) };
            return new ExecutedTable(tableDescriptor) { Rows = new Row[] { row }};
        }
    }
}
