using FlightQuery.Context;
using NUnit.Framework;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void TestRun()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
join FlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
where departuretime < '2020-3-7 9:15' and departuretime < '2020-3-12 9:15' and origin = 'katl'
";

            var context = new RunContext(code, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
        }
    }
}
