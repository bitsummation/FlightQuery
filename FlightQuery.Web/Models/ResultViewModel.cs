using FlightQuery.Sdk;
using FlightQuery.Sdk.Semantic;

namespace FlightQuery.Web.Models
{
    public class ResultViewModel
    {
        public ErrorsCollection Errors { get; set; }
        public SelectTable[] Tables { get; set; }
    }
}
