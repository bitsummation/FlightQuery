using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class NestedSelectTests
    {
        [Test]
        public void TestNestedSelect()
        {
            string code = @"
select *
from (
    select
        case
            when actual_ident != ''
                then actual_ident
            else
                ident
            end
    from airlineflightschedules
    where departuretime > '2020-1-21 9:15'
) a
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            
            Assert.IsTrue(result.First().Rows.Length > 0);
            Assert.AreEqual(result.First().Columns[0].Name, "(No column name)");
            Assert.AreEqual(result.First().Rows[0].Values[0], "OAE2412");
        }

        [Test]
        public void TestNestedSelectJoin()
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

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json");
            });
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns<HttpExecuteArg>((args) =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.GetFlightId.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 15);

            Assert.AreEqual(result.First().Columns[0].Name, "ident");
            Assert.AreEqual(result.First().Columns[1].Name, "faFlightID");
            Assert.AreEqual(result.First().Rows[0].Values[0], "OAE2412");
            Assert.AreEqual(result.First().Rows[0].Values[1], "AAL2594-1586309220-schedule-0000");

        }

        [Test]
        public void TestNestedSelectJoinInvalidSelectArgInner()
        {
            string code = @"
select a.ident, f.faFlightID
from (
    select
        blah,
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

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json");
            });
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns<HttpExecuteArg>((args) =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.GetFlightId.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 2);
        }

        [Test]
        public void TestNestedSelectInnerError()
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

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(
                new ExecuteResult() { Result = @"{""error"":""INVALID_ARGUMENT startDate is too far in the past(3 months)""}" }
            );
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns<HttpExecuteArg>((args) =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.GetFlightId.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "Error executing request: INVALID_ARGUMENT startDate is too far in the past(3 months)");
            Assert.IsTrue(result.First().Rows.Length == 0);
        }
    }
}
