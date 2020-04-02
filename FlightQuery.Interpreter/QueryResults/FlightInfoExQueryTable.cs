using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Interpreter.QueryResults
{
    public class FlightInfoExQueryTable : QueryTable
    {
        public FlightInfoExQueryTable(IHttpExecutor httpExecutor) : base(httpExecutor) { }

        protected override string TableName { get { return "FlightInfoEx"; } }

        public override TableBase Create()
        {
            return new FlightInfoExQueryTable(HttpExecutor) { Descriptor = PropertyDescriptor.GenerateQueryDescriptor(typeof(FlightInfoEx)) };
        }

        protected override bool ValidateArgs()
        {
            base.ValidateArgs();
            var param = QueryArgs.Where(x => x.Variable == "faFlightID").SingleOrDefault();
            if (param != null)
                QueryArgs.Add(new QueryArgs { Variable = "ident", PropertyValue = new PropertyValue(param.PropertyValue.Value) });

            return true;
        }

        protected override ExecutedTable ExecuteCore(HttpExecuteArg args)
        {
            var dto = HttpExecutor.GetFlightInfoEx(args);
            TableDescriptor tableDescriptor = PropertyDescriptor.GenerateRunDescriptor(typeof(FlightInfoEx));

            var rows = new List<Row>();
            foreach (var d in dto)
            {
                var row = new Row() { Values = ToValues(d, tableDescriptor) };
                rows.Add(row);
            }

            return new ExecutedTable() { Rows = rows.ToArray(), Descriptor = tableDescriptor };
        }
    }
}
