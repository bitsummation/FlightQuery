using FlightQuery.Context;
using FlightQuery.Sdk;
using NUnit.Framework;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class SemanticTests
    {

        [Test]
        public void TestJoinConditionInWhereNoError()
        {
            string code = @"
select *
from airlineflightschedules a
join getflightid f on f.departureTime = a.departuretime and f.ident = a.ident
join flightinfoex e on e.faFlightID = f.faFlightID 
where a.departuretime > '2020-4-14 1:31' and a.origin = 'kaus' and e.actualarrivaltime = 0 and e.actualdeparturetime != 0
";

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
        }

        [Test]
        public void TestInvalidAliasinSelect()
        {
            string code = @"
select f.ident, f.departuretime, f.arrivaltime, a.airportCode
from airlineflightschedules f
join airportinfo d on d.airportCode = f.destination
where f.departuretime > '2020-4-13 2:8' and f.origin = 'kaus'
";

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "a variable not found at line=2, column=48");
        }

        [Test]
        public void TestInvalidWhereVariable()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules
where blah < 55
";

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 2);

            Assert.IsTrue(context.Errors[0].Message == "departuretime is required");
            Assert.IsTrue(context.Errors[1].Message == "blah variable not found at line=4, column=6");
        }

        [Test]
        public void TestAmbiguousVariableInSelect()
        {
            string code = @"
select a.ident, faFlightID, ident
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
where a.departuretime < '2020-3-7 9:15' and a.origin = 'katl'
";

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "ident is ambiguous at line=2, column=28");
        }

        [Test]
        public void TestAmbiguousVariableInJoin()
        {
            string code = @"
select a.ident, faFlightID
from AirlineFlightSchedules a
join GetFlightId f on ident = a.ident and f.departureTime = a.departureTime 
where a.departuretime < '2020-3-7 9:15' and a.origin = 'katl'
";

            var context = RunContext.CreateSemanticContext(code);
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

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "departuretime is ambiguous at line=5, column=6");
        }
    }
    
}
