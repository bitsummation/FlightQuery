using FlightQuery.Context;
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

            var model = new ResultViewModel() {Table = result, Errors = context.Errors };
          
            /*var table = new SelectTable()
            {
                Columns = new[] { "aircrafttype", "actual_ident", "ident" },
                Rows = new[] {
                    new SelectRow() { Values = new object[] { "B739", "", "DAL503" } },
                    new SelectRow() { Values = new object[] { "B739", "DAL503", "VIR2123" }},
                    new SelectRow() { Values = new object[] { "B739", "DAL503", "AMX3102" }} }

            };
            model = new ResultViewModel() { Table = table };*/

            return model;
        }
    }
}
