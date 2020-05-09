namespace FlightQuery.Sdk
{
    public interface IHttpExecutorRaw
    {
        ExecuteResult GetAirlineFlightSchedule(HttpExecuteArg args);
        ExecuteResult GetAirportInfo(HttpExecuteArg args);
        ExecuteResult GetAirlineInfo(HttpExecuteArg args);
        ExecuteResult GetFlightID(HttpExecuteArg args);
        ExecuteResult GetFlightInfoEx(HttpExecuteArg args);
        ExecuteResult GetInboundFlightInfo(HttpExecuteArg args);
        ExecuteResult GetInFlightInfo(HttpExecuteArg args);
        ExecuteResult GetScheduled(HttpExecuteArg args);
        ExecuteResult GetArrived(HttpExecuteArg args);
        ExecuteResult GetEnroute(HttpExecuteArg args);
        ExecuteResult GetDeparted(HttpExecuteArg args);
        ExecuteResult GetHistoricalTrack(HttpExecuteArg args);
        ExecuteResult GetMapFlight(HttpExecuteArg args);
    }
}
