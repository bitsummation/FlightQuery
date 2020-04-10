using FlightQuery.Sdk;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightQuery.Interpreter.Http
{
    public class HttpExecutorRaw : IHttpExecutorRaw
    {
        private const string BaseUrl = "https://flightxml.flightaware.com/json/FlightXML2/";

        public HttpExecutorRaw(string authorization)
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
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)  //auth error
                {
                    error = new ApiExecuteError("Authentication error");
                }
                else if (result.StatusCode != HttpStatusCode.OK) // some other error
                {
                    string message = await result.Content.ReadAsStringAsync();
                    error = new ApiExecuteError(message);
                }
                else
                {
                    //{"error":"NO_DATA unknown airport INVALID"}
                    //{"error":"INVALID_ARGUMENT endDate is too far in the future (12 months)"}
                    json = await result.Content.ReadAsStringAsync();
                }
            }

            return new ExecuteResult() { Result = json, Error = error };
        }

        public ExecuteResult AirlineFlightSchedule(HttpExecuteArg args)
        {
            return Task.Run(async () => await Execute(args)).Result;
        }

        public ExecuteResult GetFlightID(HttpExecuteArg args)
        {
            return Task.Run(async () => await Execute(args)).Result;
        }

        public ExecuteResult GetFlightInfoEx(HttpExecuteArg args)
        {
            return Task.Run(async () => await Execute(args)).Result;
        }

        public ExecuteResult AirportInfo(HttpExecuteArg args)
        {
            return Task.Run(async () => await Execute(args)).Result;
        }
    }
}
