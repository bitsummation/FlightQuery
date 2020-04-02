using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public interface IHttpExecutor
    {
        IEnumerable<AirlineFlightSchedule> AirlineFlightSchedule(HttpExecuteArg args);
        GetFlightId GetFlightID(HttpExecuteArg args);
        IEnumerable<FlightInfoEx> GetFlightInfoEx(HttpExecuteArg args);
    }
}
