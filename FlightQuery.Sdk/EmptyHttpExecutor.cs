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

        public ApiExecuteResult<AirportInfo> AirportInfo(HttpExecuteArg args)
        {
            return new ApiExecuteResult<AirportInfo>(new AirportInfo());
        }

        public ApiExecuteResult<GetFlightId> GetFlightID(HttpExecuteArg args)
        {
            return new ApiExecuteResult<GetFlightId>(new GetFlightId());
        }

        public ApiExecuteResult<IEnumerable<FlightInfoEx>> GetFlightInfoEx(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<FlightInfoEx>>(new FlightInfoEx[] { new FlightInfoEx() });
        }

        public ApiExecuteResult<InboundFlightInfo> GetInboundFlightInfo(HttpExecuteArg args)
        {
            return new ApiExecuteResult<InboundFlightInfo>(new InboundFlightInfo());
        }

        public ApiExecuteResult<InFlightInfo> GetInFlightInfo(HttpExecuteArg args)
        {
            return new ApiExecuteResult<InFlightInfo>(new InFlightInfo());
        }
    }
}
