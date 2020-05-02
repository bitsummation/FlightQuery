using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryTables
{
    public class GetHistoricalTrackQueryTable : QueryTable
    {
        public GetHistoricalTrackQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "GetHistoricalTrack "; } }

        public override TableBase Create()
        {
            return new GetHistoricalTrackQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(GetHistoricalTrack)));
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            //{ "error":"INVALID: invalid {faFlightID}"}
            var result = HttpExecutor.GetHistoricalTrack(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(GetHistoricalTrack));

            var rows = new List<Row>();
            if (result.Data != null && result.Error == null)
            {
                foreach (var d in result.Data)
                {
                    if (QueryArgs.ContainsVariable("faFlightID"))
                        d.faFlightID = (string)QueryArgs["faFlightID"].PropertyValue.Value;

                    var row = new Row() { Values = ToValues(d, tableDescriptor) };
                    rows.Add(row);
                }
            }

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
        }
    }
}
