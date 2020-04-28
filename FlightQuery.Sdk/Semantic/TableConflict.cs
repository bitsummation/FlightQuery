using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Sdk.Semantic
{
    public class TableConflict :  SemanticLineError
    {
        public TableConflict(string variable, Cursor parseInfo) : base(parseInfo)
        {
            Variable = variable;
        }

        public string Variable { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("{0} table is already defined", Variable, base.Message);
            }
        }
    }
}
