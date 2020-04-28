using FlightQuery.Sdk.SqlAst;

namespace FlightQuery.Sdk
{
    public class ParseError : ErrorBase
    {
        public ParseError(string message, Cursor parseInfo)
        {
            ParseInfo = parseInfo;
            Error = message;
        }

        public string Error { get; private set; }
        public Cursor ParseInfo { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("{0} at line={1}, column={2}", Error, ParseInfo.Line, ParseInfo.Column);
            }
        }
    }
}
