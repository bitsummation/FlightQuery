using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryTables
{
    public class ScheduledQueryTable : QueryTable
    {
        public ScheduledQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected override string TableName { get { return "Scheduled"; } }

        public override TableBase Create()
        {
            return new ScheduledQueryTable(HttpExecutor, PropertyDescriptor.GenerateQueryDescriptor(typeof(Scheduled)));
        }

        protected override void ValidateArgs()
        {
            base.ValidateArgs();

            if (QueryArgs.ContainsVariable("airport"))
            {
                QueryArgs["airport"].PropertyValue = new PropertyValue(((string)(QueryArgs["airport"].PropertyValue.Value ?? "")).ToUpper());
            }
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var result = HttpExecutor.GetScheduled(args);
            if (result.Error != null && result.Error.Type != ApiExecuteErrorType.NoData)
                Errors.Add(result.Error);

            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(Scheduled));

            var rows = new List<Row>();
            if (result.Data != null && result.Error == null)
            {
                foreach (var d in result.Data)
                {
                    if (QueryArgs.ContainsVariable("airport"))
                        d.airport = (string)QueryArgs["airport"].PropertyValue.Value;

                    var row = new Row() { Values = ToValues(d, tableDescriptor) };
                    rows.Add(row);
                }
            }

            return new ExecutedTable(tableDescriptor) { Rows = rows.ToArray() };
        }
    }
}
