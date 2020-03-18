using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public interface IHttpExecutor
    {
        IEnumerable<AirlineFlightSchedule> AirlineFlightSchedule(HttpExecuteArg args);
        FlightId GetFlightID(HttpExecuteArg args);
    }
}
