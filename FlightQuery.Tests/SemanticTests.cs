using FlightQuery.Context;
using FlightQuery.Sdk;
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

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 2);
            Assert.IsTrue(context.Errors[0].Message == "blah variable not found at line=4, column=6");
            Assert.IsTrue(context.Errors[1].Message == "departuretime is required");
        }

        [Test]
        public void TestAmbiguousVariable()
        {
            string code = @"
select a.ident, faFlightID
from AirlineFlightSchedules a
join GetFlightId f on ident = a.ident and f.departureTime = a.departureTime 
where a.departuretime < '2020-3-7 9:15' and a.origin = 'katl'
";

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 2);
            Assert.IsTrue(context.Errors[0].Message == "ident is ambiguous at line=4, column=22");
        }


        [Test]
        public void TestAmbiguousVariableInWhere()
        {
            string code = @"
select a.ident, o.faFlightID
from airlineflightschedules a
join getflightid o on o.ident = a.ident and o.departuretime = a.departuretime
where departuretime > '2020-4-10 1:00' and origin = 'kaus'
";

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "departuretime is ambiguous at line=5, column=6");
        }


    }
    
}
