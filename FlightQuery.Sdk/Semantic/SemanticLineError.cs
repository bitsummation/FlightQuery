using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Sdk.Semantic
{
    public abstract class SemanticLineError : SemanticError
    {
        protected SemanticLineError(ParseInfo parseInfo)
        {
            ParseInfo = parseInfo;
        }
        
        public ParseInfo ParseInfo { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("at line={0}, column={1}", ParseInfo.Line, ParseInfo.Column);
            }
        }
    }
}
