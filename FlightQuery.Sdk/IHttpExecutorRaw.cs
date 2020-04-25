namespace FlightQuery.Sdk
{
    public interface IHttpExecutorRaw
    {
        ExecuteResult GetAirlineFlightSchedule(HttpExecuteArg args);
        ExecuteResult GetAirportInfo(HttpExecuteArg args);
        ExecuteResult GetFlightID(HttpExecuteArg args);
        ExecuteResult GetFlightInfoEx(HttpExecuteArg args);
        ExecuteResult GetInboundFlightInfo(HttpExecuteArg args);
        ExecuteResult GetInFlightInfo(HttpExecuteArg args);
    }
}
