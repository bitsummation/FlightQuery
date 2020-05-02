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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 0);
        }


        [Test]
        public void TestFromComments()
        {
            string code = @"
/*
this is a comment
*/

select *
from 
";
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(7, 5));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 0);
        }

        [Test]
        public void TestInnerQueryEditSelect()
        {
            string code = @"
select *
from (
    select 
    from airlineflightschedules
    where actual_ident = ''
) a
join airlineinfo
";

            var context = RunContext.CreateIntellisenseContext(code, new Cursor(4, 10));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 1);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("airlineflightschedules"));
        }

        [Test]
        public void TestOuterQueryNestedJoin()
        {
            string code = @"
select a.ident, f.faFlightID
from (
    select
        departureTime,
        case
            when actual_ident != ''
                then actual_ident
            else
                ident
            end as ident
    from airlineflightschedules
    where departuretime > '2020-1-21 9:15'
) a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
";

            var context = RunContext.CreateIntellisenseContext(code, new Cursor(15, 5));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 2);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("f"));

        }

        [Test]
        public void TestInnerQueryNestedJoin()
        {
            string code = @"
select a.ident, f.faFlightID
from (
    select
        departureTime,
        case
            when actual_ident != ''
                then actual_ident
            else
                ident
            end as ident
    from airlineflightschedules
    where departuretime > '2020-1-21 9:15'
) a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
";

            var context = RunContext.CreateIntellisenseContext(code, new Cursor(7, 5));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 1);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("airlineflightschedules"));
        }

        [Test]
        public void TestFrom()
        {
            string code = @"
select *
from 
";
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(3, 5));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 2);
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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 2);
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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 2);
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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 2);
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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 2);
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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();


            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
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
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
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

            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
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

            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 2);
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

            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.Count == 2);
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("ar"));
        }

        [Test]
        public void TestAliasFromFirst()
        {
            string code = @"
select *
from (
    select *
    from scheduled
    where 
) f
";
            var context = RunContext.CreateIntellisenseContext(code, new Cursor(2, 3));
            context.Run();

            Assert.IsTrue(context.ScopeModel.Global.Items.Count != 0);
        }

    }
}
