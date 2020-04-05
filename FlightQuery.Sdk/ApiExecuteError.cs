namespace FlightQuery.Sdk
{
    public class ApiExecuteError : ErrorBase
    {
        public ApiExecuteError(string message)
        {
            Error = message;
        }

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
