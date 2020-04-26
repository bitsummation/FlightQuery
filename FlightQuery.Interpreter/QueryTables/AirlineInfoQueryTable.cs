using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryTables
{
    public class AirlineInfoQueryTable : QueryTable
    {
        public AirlineInfoQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "AirlineInfo"; } }

        public override TableBase Create()
        {
            return new AirlineInfoQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(AirlineInfo)));
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            //{"error":"NO_DATA unknown airline INVALID"}
            var result = HttpExecutor.AirlineInfo(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            if (QueryArgs.ContainsVariable("airlineCode"))
                result.Data.airlineCode = (string)QueryArgs["airlineCode"].PropertyValue.Value;

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(AirlineInfo));

            var rows = new List<Row>();
            if (result.Error == null)
                rows.Add(new Row() { Values = ToValues(result.Data, tableDescriptor) });

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
        }
    }
}
