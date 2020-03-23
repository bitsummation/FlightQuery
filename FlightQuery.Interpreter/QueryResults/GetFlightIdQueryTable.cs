using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;
using System.Linq;

namespace FlightQuery.Interpreter.QueryResults
{
    public class GetFlightIdQueryTable : QueryTable
    {
        public GetFlightIdQueryTable(IHttpExecutor httpExecutor) : base(httpExecutor) { }

        protected override string TableName { get { return "GetFlightID"; } }

        public override TableBase Create()
        {
            return new GetFlightIdQueryTable(HttpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(GetFlightId)) };
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var dto = HttpExecutor.GetFlightID(args);

            long departerTimeValue = 0;
            if (QueryArgs.Count(x => x.Variable == "departuretime") > 0)
                departerTimeValue = (long)QueryArgs.Where(x => x.Variable == "departuretime").Single().PropertyValue.Value;

            string identValue = string.Empty;
            if(QueryArgs.Count(x => x.Variable == "ident") > 0)
                identValue = (string)QueryArgs.Where(x => x.Variable == "ident").Single().PropertyValue.Value;

            dto.departuretime = (DateTime)Conversion.ConvertLongToDateTime(departerTimeValue);
            dto.ident = identValue;

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(GetFlightId));

            var row = new Row() { Values = ToValues(dto, tableDescriptor) };
            return new ExecutedTable() { Rows = new Row[] { row }, Descriptor = tableDescriptor };
        }
    }
}
