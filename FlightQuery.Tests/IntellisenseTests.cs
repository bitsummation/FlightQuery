using FlightQuery.Context;
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
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.SkipParseErrors);
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
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic|ExecuteFlags.SkipParseErrors);
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
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.SkipParseErrors);
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
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.SkipParseErrors);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("a"));
            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("s"));
        }

        [Test]
        public void TestWhere()
        {
            string code = @"
select *
from airportinfo
where 
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic | ExecuteFlags.SkipParseErrors);
            context.Run();

            Assert.IsTrue(context.ScopeModel.QueryScope.Items.ContainsKey("airportinfo"));
        }

     

    }
}
