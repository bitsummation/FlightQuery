using FlightQuery.Interpreter.Common;
using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;
using System.Linq;

namespace FlightQuery.Interpreter.QueryResults
{
    public class FlightIdQueryTable : QueryTable
    {
        public FlightIdQueryTable(IHttpExecutor httpExecutor) : base(httpExecutor) { }

        protected override string TableName { get { return "GetFlightID"; } }

        public override TableBase Create()
        {
            return new FlightIdQueryTable(HttpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(FlightId)) };
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var dto = HttpExecutor.GetFlightID(args);

            var departerTimeValue = (DateTime)Conversion.ConvertLongToDateTime(QueryArgs.Where(x => x.Variable == "departuretime").Single().PropertyValue.Value);
            var identValue = (string)QueryArgs.Where(x => x.Variable == "ident").Single().PropertyValue.Value;

            dto.departuretime = departerTimeValue;
            dto.ident = identValue;

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(FlightId));

            var row = new Row() { Values = ToValues(dto, tableDescriptor) };
            return new ExecutedTable() { Rows = new Row[] { row }, Descriptor = tableDescriptor };
        }
    }
}
