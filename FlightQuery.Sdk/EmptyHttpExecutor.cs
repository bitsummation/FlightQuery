using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public class EmptyHttpExecutor : IHttpExecutor
    {
        public IEnumerable<AirlineFlightSchedule> AirlineFlightSchedule(HttpExecuteArg args)
        {
            return new AirlineFlightSchedule[]{ new AirlineFlightSchedule()};
        }

        public FlightId GetFlightID(HttpExecuteArg args)
        {
            return new FlightId();
        }
    }
}
