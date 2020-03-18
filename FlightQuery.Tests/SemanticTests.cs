using FlightQuery.Context;
using NUnit.Framework;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class SemanticTests
    {
        [Test]
        public void TestInvalidWhereVariable()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules
where blah < 55
";

            var context = new RunContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 2);
            Assert.IsTrue(context.Errors[0].Message == "blah variable not found at line=4, column=6");
            Assert.IsTrue(context.Errors[1].Message == "departuretime is required");
        }

        
    }
    
}
