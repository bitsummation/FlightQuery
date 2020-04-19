using FlightQuery.Context;
using FlightQuery.Sdk;
using NUnit.Framework;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class IntellisenseTests
    {
        [Test]
        public void TestSelect()
        {
            string code = @"
select
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 0);
        }

        [Test]
        public void TestFrom()
        {
            string code = @"
select *
from 
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic|ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 0);
        }

        [Test]
        public void TestJoin()
        {
            string code = @"
select *
from airportinfo
join 
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("airportinfo"));
        }

        [Test]
        public void TestJoinOn()
        {
            string code = @"
select *
from airportinfo a
join airlineflightschedules s on 
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("s"));
        }


        [Test]
        public void TestJoinOnWithWhere()
        {
            string code = @"
select *
from airlineflightschedules a
join getflightid o on 
where departuretime > '2020-4-10 1:00' and origin = 'kaus'
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("o"));
        }

        [Test]
        public void TestJoinOnWithWhereOnAlias()
        {
            string code = @"
select *
from airlineflightschedules a
join getflightid o on o.
where departuretime > '2020-4-10 1:00' and origin = 'kaus'
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("o"));
        }

        [Test]
        public void TestJoinConditionNoWhere()
        {
            string code = @"
select *
from airlineflightschedules a
join getflightid o on o.ident = a.
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("o"));
        }


        [Test]
        public void TestJoinConditionNoWhereAnd()
        {
            string code = @"
select *
from airlineflightschedules a
join getflightid o on o.ident = a.ident and o.departuretime = a.
where departuretime > '2020-4-10 1:00' and origin = 'kaus'
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("o"));
        }

        [Test]
        public void TestMultpleJoin()
        {
            string code = @"
select f.ident, f.departuretime, f.arrivaltime, d.name
from airlineflightschedules f
join airportinfo d on d.airportCode = f.departuretime
join 
where f.departuretime > '2020-4-13 2:8' and f.origin = 'kaus'
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

        
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("f"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("d"));

        }

        [Test]
        public void TestWhere()
        {
            string code = @"
select *
from airportinfo
where 
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("airportinfo"));
        }

        [Test]
        public void TestSameSelectVariable()
        {
            string code = @"
select airportCode, airportCode, 
from AirportInfo
where airportCode = 'kaus'
";

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("airportinfo"));
        }

        [Test]
        public void TwoSameTable()
        {
            string code = @"
select *
from airlineflightschedules a
join airlineflightschedules f on f.destination = a.
";

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("f"));
        }

        [Test]
        public void TestMismatchingComparisionTypes()
        {
            string code = @"
select 
from airlineflightschedules a
join airportinfo ar on ar.airportCode = a.arrivaltime
";

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("ar"));
        }

        [Test]
        public void TestCase()
        {
            string code = @"
select
    a.ident,
    a.
    a.departuretime,
    case
        when e.actualarrivaltime = -1 and e.actualdeparturetime = -1 and e.estimatedarrivaltime = -1
        then 'cancelled'
        when e.actualdeparturetime != 0 and e.actualarrivaltime = 0
        then 'enroute'
        when e.actualdeparturetime != 0 and e.actualarrivaltime != 0 and e.actualdeparturetime != e.actualarrivaltime
        then 'arrived'
        else 'not departed'
    end as status
from airlineflightschedules a
join getflightid f on f.departureTime = a.departuretime and f.ident = a.ident
join flightinfoex e on e.faFlightID = f.faFlightID
where a.departuretime > '2020-4-18 19:34' and a.origin = 'kaus'
";

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.Intellisense);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("f"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("e"));
        }

    }
}
