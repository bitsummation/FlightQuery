using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Interpreter.QueryResults;
using FlightQuery.Sdk;
using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Interpreter.QueryTables
{
    public abstract class LimitQueryTable : QueryTable
    {
        protected LimitQueryTable(IHttpExecutor httpExecutor, TableDescriptor descriptor) : base(httpExecutor, descriptor) { }

        protected sealed override void LimitQuery(LimitStatement statement)
        {
            if (statement != null)
            {
                QueryArgs.Add(new EqualQueryArg() { Variable = "offset", PropertyValue = new PropertyValue(statement.Offset) });
                QueryArgs.Add(new EqualQueryArg() { Variable = "howMany", PropertyValue = new PropertyValue(statement.Count) });
            }
        }

    }
}
