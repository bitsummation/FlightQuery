using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public class EmptyHttpExecutor : IHttpExecutor
    {
        public ApiExecuteResult<IEnumerable<AirlineFlightSchedule>> AirlineFlightSchedule(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<AirlineFlightSchedule>>(new AirlineFlightSchedule[] { new AirlineFlightSchedule() });
        }

        public ApiExecuteResult<GetFlightId> GetFlightID(HttpExecuteArg args)
        {
            return new ApiExecuteResult<GetFlightId>(new GetFlightId());
        }

        public ApiExecuteResult<IEnumerable<FlightInfoEx>> GetFlightInfoEx(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<FlightInfoEx>>(new FlightInfoEx[] { new FlightInfoEx() });
        }
    }
}
