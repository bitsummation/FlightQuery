using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;

namespace FlightQuery.Interpreter.QueryTables
{
    public abstract class LimitQueryTable : QueryTable
    {
        protected LimitQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected sealed override void LimitQuery(int offset, int limit)
        {
            QueryArgs.Add(new EqualQueryArg() { Variable = "offset", PropertyValue = new PropertyValue(offset) });
            QueryArgs.Add(new EqualQueryArg() { Variable = "howMany", PropertyValue = new PropertyValue(limit) });
        }

    }
}
