using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public interface IHttpExecutor
    {
        ApiExecuteResult<IEnumerable<AirlineFlightSchedule>> AirlineFlightSchedule(HttpExecuteArg args);
        ApiExecuteResult<GetFlightId> GetFlightID(HttpExecuteArg args);
        ApiExecuteResult<AirportInfo> AirportInfo(HttpExecuteArg args);
        ApiExecuteResult<AirlineInfo> AirlineInfo(HttpExecuteArg args);
        ApiExecuteResult<IEnumerable<FlightInfoEx>> GetFlightInfoEx(HttpExecuteArg args);
        ApiExecuteResult<InboundFlightInfo> GetInboundFlightInfo(HttpExecuteArg args);
        ApiExecuteResult<InFlightInfo> GetInFlightInfo(HttpExecuteArg args);
        ApiExecuteResult<IEnumerable<Scheduled>> GetScheduled(HttpExecuteArg args);
        ApiExecuteResult<IEnumerable<Departed>> GetDeparted(HttpExecuteArg args);
        ApiExecuteResult<IEnumerable<Arrived>> GetArrived(HttpExecuteArg args);
        ApiExecuteResult<IEnumerable<Enroute>> GetEnroute(HttpExecuteArg args);
        ApiExecuteResult<IEnumerable<GetHistoricalTrack>> GetHistoricalTrack(HttpExecuteArg args);
        ApiExecuteResult<MapFlight> GetMapFlight(HttpExecuteArg args);

    }
}
