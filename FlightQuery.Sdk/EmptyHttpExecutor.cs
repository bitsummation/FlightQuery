﻿using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public class EmptyHttpExecutor : IHttpExecutor
    {
        public ApiExecuteResult<IEnumerable<AirlineFlightSchedule>> AirlineFlightSchedule(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<AirlineFlightSchedule>>(new AirlineFlightSchedule[] { new AirlineFlightSchedule() });
        }

        public ApiExecuteResult<AirlineInfo> AirlineInfo(HttpExecuteArg args)
        {
            return new ApiExecuteResult<AirlineInfo>(new AirlineInfo());
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

        public ApiExecuteResult<IEnumerable<Scheduled>> GetScheduled(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<Scheduled>>(new [] { new Scheduled() });
        }

        public ApiExecuteResult<IEnumerable<Arrived>> GetArrived(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<Arrived>>(new [] { new Arrived() });
        }

        public ApiExecuteResult<IEnumerable<Enroute>> GetEnroute(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<Enroute>>(new[] { new Enroute() });
        }

        public ApiExecuteResult<IEnumerable<Departed>> GetDeparted(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<Departed>>(new[] { new Departed() });
        }

        public ApiExecuteResult<InboundFlightInfo> GetInboundFlightInfo(HttpExecuteArg args)
        {
            return new ApiExecuteResult<InboundFlightInfo>(new InboundFlightInfo());
        }

        public ApiExecuteResult<InFlightInfo> GetInFlightInfo(HttpExecuteArg args)
        {
            return new ApiExecuteResult<InFlightInfo>(new InFlightInfo());
        }

        public ApiExecuteResult<IEnumerable<GetHistoricalTrack>> GetHistoricalTrack(HttpExecuteArg args)
        {
            return new ApiExecuteResult<IEnumerable<GetHistoricalTrack>>(new[] { new GetHistoricalTrack() });
        }

        public ApiExecuteResult<MapFlight> GetMapFlight(HttpExecuteArg args)
        {
            return new ApiExecuteResult<MapFlight>(new MapFlight());
        }
    }
}
