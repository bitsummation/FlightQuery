using FlightQuery.Context;
using FlightQuery.Sdk;
using FlightQuery.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
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
 
            var context = new RunContext(query, "", ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            return context.ScopeModel;
        }

        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> Query()
        {
            string query = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                query = await reader.ReadToEndAsync();
            }

            var authorization = Request.Headers["Authorization"];
            var context = new RunContext(query, authorization, ExecuteFlags.Run);
            var result = context.Run();

            var isUnauthorized = context.Errors.Any(x =>
            {
                var apiError = x as ApiExecuteError;
                if(apiError != null)
                {
                    return apiError.Type == ApiExecuteErrorType.AuthError;
                }
                return false;
            });
            if(isUnauthorized)
                return Unauthorized();

            return Ok(new ResultViewModel() {Table = result, Errors = context.Errors });
        }
    }
}
