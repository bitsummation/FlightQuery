using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;

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
            var result = HttpExecutor.GetFlightID(args);
            if (result.Error != null)
                Errors.Add(result.Error);

            long departerTimeValue = 0;
            if (QueryArgs.ContainsVariable("departuretime"))
                departerTimeValue = (long)QueryArgs["departuretime"].PropertyValue.Value;

            string identValue = string.Empty;
            if (QueryArgs.ContainsVariable("ident"))
                identValue = (string)QueryArgs["ident"].PropertyValue.Value;

            var dto = result.Data;
            dto.departuretime = (DateTime)Conversion.ConvertLongToDateTime(departerTimeValue);
            dto.ident = identValue;

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(GetFlightId));

            var row = new Row() { Values = ToValues(dto, tableDescriptor) };
            return new ExecutedTable() { Rows = new Row[] { row }, Descriptor = tableDescriptor };
        }
    }
}
