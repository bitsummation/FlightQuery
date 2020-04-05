using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightQuery.Interpreter.Http
{
    public class HttpExecutor : IHttpExecutor
    {
        private const string BaseUrl = "https://flightxml.flightaware.com/json/FlightXML2/";

        public HttpExecutor(string authorization)
        {
            Authorization = authorization;
        }

        public string Authorization { get; private set; }

        private async Task<ExecuteResult> Execute(HttpExecuteArg args)
        {
            var json = string.Empty;
            ApiExecuteError error = null;
            using (var client = new HttpClient())
            {
                var dict = new Dictionary<string, string>();
                foreach (var arg in args.Variables)
                    dict.Add(arg.Variable, arg.Value.ToString());

                client.DefaultRequestHeaders.Add("Authorization", Authorization);
               
                var result = await client.PostAsync(BaseUrl + args.TableName, new FormUrlEncodedContent(dict));
                if(result.StatusCode == System.Net.HttpStatusCode.Unauthorized)  //auth error
                {
                    error = new ApiExecuteError("Authentication error");
                }
                else if(result.StatusCode != HttpStatusCode.OK) // some other error
                {
                    error = new ApiExecuteError("Unexpected error");
                }
                else
                {
                    json = await result.Content.ReadAsStringAsync();
                }

            }

            return new ExecuteResult() {Result = json, Error = error };
        }

        public ApiExecuteResult<IEnumerable<AirlineFlightSchedule>> AirlineFlightSchedule(HttpExecuteArg args)
        {
            var result = Task.Run(async () => await Execute(args)).Result;
            return new ApiExecuteResult<IEnumerable<AirlineFlightSchedule>>(
                Deserialize.DeserializeObject<AirlineFlightSchedule[]>(result.Result),
                result.Error);
        }

        public ApiExecuteResult<GetFlightId> GetFlightID(HttpExecuteArg args)
        {
            var result = Task.Run(async () => await Execute(args)).Result;
            return new ApiExecuteResult<GetFlightId>(new GetFlightId() { faFlightID = result.Result}, result.Error);
        }

        public ApiExecuteResult<IEnumerable<FlightInfoEx>> GetFlightInfoEx(HttpExecuteArg args)
        {
            var result = Task.Run(async () => await Execute(args)).Result;
            return new ApiExecuteResult<IEnumerable<FlightInfoEx>>(
                Deserialize.DeserializeObject<FlightInfoEx[]>(result.Result),
                result.Error);
        }

        private class ExecuteResult
        {
            public string Result { get; set; }
            public ApiExecuteError Error { get; set; }
        }


    }
}
