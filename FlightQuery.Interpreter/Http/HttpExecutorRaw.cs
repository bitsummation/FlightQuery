using FlightQuery.Sdk;
using System;
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

        private async Task<ExecuteResult> ExecuteInternal(HttpExecuteArg args)
        {
            var json = string.Empty;
            ApiExecuteError error = null;
            try
            {
               
                using (var client = new HttpClient())
                {
                    var dict = new Dictionary<string, string>();
                    foreach (var arg in args.Variables)
                        dict.Add(arg.Variable, arg.Value.ToString());

                    client.DefaultRequestHeaders.Add("Authorization", Authorization);

                    var result = await client.PostAsync(BaseUrl + args.TableName, new FormUrlEncodedContent(dict));
                    if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)  //auth error
                    {
                        error = new ApiExecuteError(ApiExecuteErrorType.AuthError, "Authentication error");
                    }
                    else if (result.StatusCode != HttpStatusCode.OK) // some other error
                    {
                        string message = await result.Content.ReadAsStringAsync();
                        error = new ApiExecuteError(ApiExecuteErrorType.Fail, message);
                    }
                    else
                    {
                        //{"error":"NO_DATA unknown airport INVALID"}
                        //{"error":"NO_DATA flight not found"}
                        //{"error":"INVALID_ARGUMENT endDate is too far in the future (12 months)"}
                        json = await result.Content.ReadAsStringAsync();
                    }
                }
            }
            catch(Exception)
            {
                return new ExecuteResult() { Result = string.Empty, Error = new ApiExecuteError(ApiExecuteErrorType.Fail, "Api error") };
            }

            return new ExecuteResult() { Result = json, Error = error };
        }

        public ExecuteResult GetAirlineFlightSchedule(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }

        public ExecuteResult GetFlightID(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }

        public ExecuteResult GetFlightInfoEx(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }

        public ExecuteResult GetAirportInfo(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }

        public ExecuteResult GetInboundFlightInfo(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }

        public ExecuteResult GetInFlightInfo(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }

        public ExecuteResult GetAirlineInfo(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }

        public ExecuteResult GetScheduled(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }

        public ExecuteResult GetArrived(HttpExecuteArg args)
        {
            return Task.Run(async () => await ExecuteInternal(args)).Result;
        }
    }
}
