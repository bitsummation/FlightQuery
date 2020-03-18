using FlightQuery.Interpreter.Http;

namespace FlightQuery.Interpreter.QueryResults
{
    public class ExecutedTable : TableBase
    {
        public override bool HasExecuted { get { return true; } }

        public override void AddArg(QueryArgs args)
        {
        }

        public override TableBase Create()
        {
            return this;
        }

        public override ExecutedTable Execute()
        {
            return this;
        }
    }
}
