using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FlightQuery.Interpreter.Http
{
    public class HttpExecutor : IHttpExecutor
    {
        private const string BaseUrl = "https://flightxml.flightaware.com/json/FlightXML2/";

        private async Task<string> Execute(HttpExecuteArg args)
        {
            var json = string.Empty;
            using (var client = new HttpClient())
            {
                var dict = new Dictionary<string, string>();
                foreach (var arg in args.Variables)
                    dict.Add(arg.Variable, arg.Value.ToString());

                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes("username:password"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
                var result = await client.PostAsync(BaseUrl + args.TableName, new FormUrlEncodedContent(dict));

                json = await result.Content.ReadAsStringAsync();
            }

            return json;
        }

        public IEnumerable<AirlineFlightSchedule> AirlineFlightSchedule(HttpExecuteArg args)
        {
            string json = Task.Run(async () => await Execute(args)).Result;
            return Deserialize.DeserializeObject<AirlineFlightSchedule[]>(json);
        }

        public GetFlightId GetFlightID(HttpExecuteArg args)
        {
            string flightId = Task.Run(async () => await Execute(args)).Result;
            return new GetFlightId() { faFlightID = flightId };
        }       
    }
}
