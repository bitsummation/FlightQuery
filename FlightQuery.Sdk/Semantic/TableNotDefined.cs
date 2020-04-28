using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Sdk.Semantic
{
    public class TableNotDefined : SemanticLineError
    {
        public TableNotDefined(string variable, Cursor parseInfo) : base(parseInfo)
        {
            Variable = variable;
        }

        public string Variable { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("{0} table not found {1}", Variable, base.Message);
            }
        }

    }
}
