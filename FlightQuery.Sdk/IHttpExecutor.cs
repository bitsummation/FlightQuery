using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public interface IHttpExecutor
    {
        ApiExecuteResult<IEnumerable<AirlineFlightSchedule>> AirlineFlightSchedule(HttpExecuteArg args);
        ApiExecuteResult<GetFlightId> GetFlightID(HttpExecuteArg args);
        ApiExecuteResult<AirportInfo> AirportInfo(HttpExecuteArg args);
        ApiExecuteResult<IEnumerable<FlightInfoEx>> GetFlightInfoEx(HttpExecuteArg args);
    }
}
