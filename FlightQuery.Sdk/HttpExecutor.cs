using FlightQuery.Sdk.Model.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

        public ApiExecuteResult<IEnumerable<AirlineFlightSchedule>> AirlineFlightSchedule(HttpExecuteArg args)
        {
            var result = _raw.AirlineFlightSchedule(args);
            return new ApiExecuteResult<IEnumerable<AirlineFlightSchedule>>(
                Deserialize.DeserializeObject<AirlineFlightSchedule[]>(result.Result),
                result.Error);
        }

        public ApiExecuteResult<AirportInfo> AirportInfo(HttpExecuteArg args)
        {
            var result = _raw.AirportInfo(args);
            
            if (result == null || result.Error != null)
            {
                return new ApiExecuteResult<AirportInfo>(_empty.AirportInfo(args).Data, result != null ? result.Error : null);
            }
            else
            {
                dynamic dynamicResult = JsonConvert.DeserializeObject(result.Result, null, new UnixDateTimeConverter());
                if(dynamicResult.error != null)
                {
                    return new ApiExecuteResult<AirportInfo>(_empty.AirportInfo(args).Data, new ApiExecuteError((string)dynamicResult.error));
                }
                var raw = JsonConvert.SerializeObject(dynamicResult.AirportInfoResult);
                return new ApiExecuteResult<AirportInfo>(Deserialize.DeserializeObject<AirportInfo>(raw), result.Error);
            }
        }

        public ApiExecuteResult<GetFlightId> GetFlightID(HttpExecuteArg args)
        {
            var result = _raw.GetFlightID(args);
            return new ApiExecuteResult<GetFlightId>(new GetFlightId() { faFlightID = result.Result }, result.Error);
        }

        public ApiExecuteResult<IEnumerable<FlightInfoEx>> GetFlightInfoEx(HttpExecuteArg args)
        {
            var result = _raw.GetFlightID(args);
            return new ApiExecuteResult<IEnumerable<FlightInfoEx>>(
                Deserialize.DeserializeObject<FlightInfoEx[]>(result.Result),
                result.Error);
        }
    }
}
