using FlightQuery.Context;
using FlightQuery.Sdk;
using FlightQuery.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace FlightQuery.Web.Controllers
{
    [ApiController]
    public class QueryController : ControllerBase
    {
        [HttpPost]
        [Route("scope")]
        public async Task<ScopeModel> Scope()
        {
            string query = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                query = await reader.ReadToEndAsync();
            }
 
            var context = new RunContext(query, "", ExecuteFlags.Semantic | ExecuteFlags.SkipParseErrors);
            context.Run();

            return context.ScopeModel;
        }

        [HttpPost]
        [Route("query")]
        public async Task<ResultViewModel> Query()
        {
            string query = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                query = await reader.ReadToEndAsync();
            }

            var authorization = Request.Headers["Authorization"];
            var context = new RunContext(query, authorization, ExecuteFlags.Run);
            var result = context.Run();

            return new ResultViewModel() {Table = result, Errors = context.Errors };
        }
    }
}
