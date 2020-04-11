namespace FlightQuery.Sdk
{
    public class ApiExecuteError : ErrorBase
    {
        public ApiExecuteError(ApiExecuteErrorType type, string message)
        {
            Error = message;
            Type = type;
        }

        public ApiExecuteErrorType Type { get; private set; }
        public string Error { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("Error executing request: {0}", Error);
            }
        }
    }
}
