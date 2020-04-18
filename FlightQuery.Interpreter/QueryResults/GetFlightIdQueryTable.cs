using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;

namespace FlightQuery.Interpreter.QueryResults
{
    public class GetFlightIdQueryTable : QueryTable
    {
        public GetFlightIdQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "GetFlightID"; } }

        public override TableBase Create()
        {
            return new GetFlightIdQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(GetFlightId)));
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.GetFlightID(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            bool noDataError = result.Error != null && result.Error.Type == ApiExecuteErrorType.NoData;
            
            long departerTimeValue = 0;
            if (QueryArgs.ContainsVariable("departuretime"))
            {
                if (noDataError)
                    departerTimeValue = 0;
                else
                    departerTimeValue = (long)(QueryArgs["departuretime"].PropertyValue.Value ?? 0L);
            }

            string identValue = string.Empty;
            if (QueryArgs.ContainsVariable("ident"))
            {
                if (noDataError)
                    identValue = null;
                else
                    identValue = (string)QueryArgs["ident"].PropertyValue.Value;
            }

            var dto = result.Data;
            dto.departureTime = (DateTime)Conversion.ConvertLongToDateTime(departerTimeValue);
            dto.ident = identValue;

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(GetFlightId));

            var row = new Row() { Values = ToValues(dto, tableDescriptor) };
            return new ExecutedTable(tableDescriptor) { Rows = new Row[] { row } };
        }
    }
}
