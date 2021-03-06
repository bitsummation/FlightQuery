﻿using FlightQuery.Sdk.Model;
using FlightQuery.Sdk.Model.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace FlightQuery.Sdk
{
    public class HttpExecutor : IHttpExecutor
    {
        private IHttpExecutorRaw _raw;
        private IHttpExecutor _empty;

        public HttpExecutor(IHttpExecutorRaw executor)
        {
            _raw = executor;
            _empty = new EmptyHttpExecutor();
        }

        private ApiExecuteError ParseFlightAwareError(string error)
        {
            if(error.StartsWith("NO_DATA"))
                return new ApiExecuteError(ApiExecuteErrorType.NoData, error);
            if(error.StartsWith("INVALID_ARGUMENT") || error.StartsWith("INVALID"))
                return new ApiExecuteError(ApiExecuteErrorType.InvalidArgument, error);

            return new ApiExecuteError(ApiExecuteErrorType.Fail, error);
        }

        private ApiExecuteResult<TData> ParseFindResult<TData>(Func<ExecuteResult> execute, Func<TData> fetchEmpty, Func<dynamic, object> fetchData)
        {
            var result = execute();
            if (result == null || result.Error != null) //api error
            {
                return new ApiExecuteResult<TData>(fetchEmpty(), result != null ? result.Error : null);
            }

            dynamic dynamicResult = JsonConvert.DeserializeObject(result.Result, null, new UnixDateTimeConverter());
            if (dynamicResult.error != null) //200 ok but error message
            {
                return new ApiExecuteResult<TData>(fetchEmpty(), ParseFlightAwareError((string)dynamicResult.error));
            }

            var raw = JsonConvert.SerializeObject(fetchData(dynamicResult), new UnixDateTimeConverter());
            return new ApiExecuteResult<TData>(Json.DeserializeObject<TData>(raw), result.Error);
        }

        public ApiExecuteResult<IEnumerable<AirlineFlightSchedule>> AirlineFlightSchedule(HttpExecuteArg args)
        {
            return ParseFindResult(
                () => _raw.GetAirlineFlightSchedule(args),
                () => _empty.AirlineFlightSchedule(args).Data,
                (dynamic json) => json.AirlineFlightSchedulesResult.data
                );
        }

        public ApiExecuteResult<IEnumerable<FlightInfoEx>> GetFlightInfoEx(HttpExecuteArg args)
        {
            return ParseFindResult(
                () => _raw.GetFlightInfoEx(args),
                () => _empty.GetFlightInfoEx(args).Data,
                (dynamic json) => json.FlightInfoExResult.flights
                );
        }

        public ApiExecuteResult<IEnumerable<Scheduled>> GetScheduled(HttpExecuteArg args)
        {
            return ParseFindResult(
               () => _raw.GetScheduled(args),
               () => _empty.GetScheduled(args).Data,
               (dynamic json) => json.ScheduledResult.scheduled
               );
        }

        public ApiExecuteResult<IEnumerable<Arrived>> GetArrived(HttpExecuteArg args)
        {
            return ParseFindResult(
              () => _raw.GetArrived(args),
              () => _empty.GetArrived(args).Data,
              (dynamic json) => json.ArrivedResult.arrivals
              );
        }

        public ApiExecuteResult<IEnumerable<Enroute>> GetEnroute(HttpExecuteArg args)
        {
            return ParseFindResult(
              () => _raw.GetEnroute(args),
              () => _empty.GetEnroute(args).Data,
              (dynamic json) => json.EnrouteResult.enroute
              );
        }

        public ApiExecuteResult<IEnumerable<Departed>> GetDeparted(HttpExecuteArg args)
        {
            return ParseFindResult(
             () => _raw.GetDeparted(args),
             () => _empty.GetDeparted(args).Data,
             (dynamic json) => json.DepartedResult.departures
             );
        }

        public ApiExecuteResult<AirportInfo> AirportInfo(HttpExecuteArg args)
        {
            return ParseFindResult(
                () => _raw.GetAirportInfo(args),
                () => _empty.AirportInfo(args).Data,
                (dynamic json) => json.AirportInfoResult
                );
        }

        public ApiExecuteResult<AirlineInfo> AirlineInfo(HttpExecuteArg args)
        {
            return ParseFindResult(
                () => _raw.GetAirlineInfo(args),
                () => _empty.AirlineInfo(args).Data,
                (dynamic json) => json.AirlineInfoResult
                );
        }

        public ApiExecuteResult<GetFlightId> GetFlightID(HttpExecuteArg args)
        {
            var result = _raw.GetFlightID(args);
            if (result == null || result.Error != null)
            {
                return new ApiExecuteResult<GetFlightId>(_empty.GetFlightID(args).Data, result != null ? result.Error : null);
            }
            dynamic dynamicResult = JsonConvert.DeserializeObject(result.Result, null, new UnixDateTimeConverter());
            if (dynamicResult.error != null)
            {
                return new ApiExecuteResult<GetFlightId>(_empty.GetFlightID(args).Data, ParseFlightAwareError((string)dynamicResult.error));
            }

            return new ApiExecuteResult<GetFlightId>(new GetFlightId() {faFlightID = dynamicResult.GetFlightIDResult }, result.Error);
        }

        public ApiExecuteResult<InboundFlightInfo> GetInboundFlightInfo(HttpExecuteArg args)
        {
            return ParseFindResult(
                () => _raw.GetInboundFlightInfo(args),
                () => _empty.GetInboundFlightInfo(args).Data,
                (dynamic json) => json.InboundFlightInfoResult
                );
        }

        public ApiExecuteResult<InFlightInfo> GetInFlightInfo(HttpExecuteArg args)
        {
            return ParseFindResult(
                () => _raw.GetInFlightInfo(args),
                () => _empty.GetInFlightInfo(args).Data,
                (dynamic json) => json.InFlightInfoResult
                );
        }

        public ApiExecuteResult<IEnumerable<GetHistoricalTrack>> GetHistoricalTrack(HttpExecuteArg args)
        {
            return ParseFindResult(
              () => _raw.GetHistoricalTrack(args),
              () => _empty.GetHistoricalTrack(args).Data,
              (dynamic json) => json.GetHistoricalTrackResult.data
              );
        }

        public ApiExecuteResult<MapFlight> GetMapFlight(HttpExecuteArg args)
        {
            var result = _raw.GetMapFlight(args);
            if (result == null || result.Error != null)
            {
                return new ApiExecuteResult<MapFlight>(_empty.GetMapFlight(args).Data, result != null ? result.Error : null);
            }
            dynamic dynamicResult = JsonConvert.DeserializeObject(result.Result, null, new UnixDateTimeConverter());
            if (dynamicResult.error != null)
            {
                return new ApiExecuteResult<MapFlight>(_empty.GetMapFlight(args).Data, ParseFlightAwareError((string)dynamicResult.error));
            }

            return new ApiExecuteResult<MapFlight>(new MapFlight() { image = new Base64Image((string)dynamicResult.MapFlightResult) }, result.Error);
        }
    }
}
