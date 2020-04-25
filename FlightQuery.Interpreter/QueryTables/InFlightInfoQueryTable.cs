using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryTables
{
    public class InFlightInfoQueryTable : QueryTable
    {
        public InFlightInfoQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "InFlightInfo"; } }

        public override TableBase Create()
        {
            return new InFlightInfoQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(InFlightInfo)));
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.GetInFlightInfo(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(InFlightInfo));

            var dto = result.Data;
            var rows = new List<Row>();
            if (result.Error == null)
                rows.Add(new Row() { Values = ToValues(dto, tableDescriptor) });

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
        }
    }
}
