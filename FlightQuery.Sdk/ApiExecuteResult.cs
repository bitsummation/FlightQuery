namespace FlightQuery.Sdk
{
    public class ApiExecuteResult<TData> 
    {
        public ApiExecuteResult(TData data) : this(data, null) { }

        public ApiExecuteResult(TData data, ApiExecuteError error)
        {
            Data = data;
            Error = error;
        }

        public TData Data { get; private set; }
        public ApiExecuteError Error { get; private set; }
    }
}
