namespace FlightQuery.Sdk
{
    public interface IHttpExecutorRaw
    {
        ExecuteResult AirlineFlightSchedule(HttpExecuteArg args);
        ExecuteResult AirportInfo(HttpExecuteArg args);
        ExecuteResult GetFlightID(HttpExecuteArg args);
        ExecuteResult GetFlightInfoEx(HttpExecuteArg args);
    }
}
