using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryResults
{
    public class InboundFlightInfoQueryTable : QueryTable
    {
        public InboundFlightInfoQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "InboundFlightInfo"; } }

        public override TableBase Create()
        {
            return new InboundFlightInfoQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(InboundFlightInfo)));
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.GetInboundFlightInfo(args);
            //{ "error":"INVALID_ARGUMENT Inbound flight is not known"}
            //{ "error":"INVALID_ARGUMENT: invalid {faFlightID}"}
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.InvalidArgument) //don't show invalid args as errors
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(InboundFlightInfo));

            var dto = result.Data;
            dto.ifaFlightID = dto.faFlightID;

            string faFlightID = string.Empty;
            if (QueryArgs.ContainsVariable("faFlightID"))
                faFlightID = (string)QueryArgs["faFlightID"].PropertyValue.Value;

            dto.faFlightID = faFlightID;

            var rows = new List<Row>();
            if (result.Error == null)
                rows.Add(new Row() { Values = ToValues(dto, tableDescriptor) });

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
        }
    }
}
