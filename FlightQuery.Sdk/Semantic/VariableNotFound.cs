using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Sdk.Semantic
{
    public class VariableNotFound : SemanticLineError
    {
        public VariableNotFound(string variable, ParseInfo parseInfo) : base(parseInfo)
        {
            Variable = variable;
        }

        public string Variable { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("{0} variable not found {1}", Variable, base.Message);
            }
        }

    }
}
