using FlightQuery.Context;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class GetFlightIdTests
    {
        [Test]
        public void TestMissingRequired()
        {
            string code = @"
select faFlightID
from GetFlightId
where ident = 'DAL503'
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "departureTime is required");
        }

        [Test]
        public void TestQuerableParameters()
        {
            string code = @"
select faFlightID
from GetFlightId
where ident = 'DAL503' and departuretime = '2020-3-7 9:15'
";

            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 2);
                var start = args.Variables.Where(x => x.Variable == "ident").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "DAL503");

                var end = args.Variables.Where(x => x.Variable == "departureTime").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572500");
            }).Returns(() => new ApiExecuteResult<GetFlightId>(new GetFlightId()));

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            mock.Verify(v => v.GetFlightID(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

        [Test]
        public void TestExecute()
        {
            string code = @"
select faFlightID
from GetFlightId
where ident = 'DAL503' and departuretime = '2020-3-7 9:15'
";
            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns(() => new ApiExecuteResult<GetFlightId>(new GetFlightId() {faFlightID = "XYZ1234-1530000000-airline-0500" }));

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), mock.Object);
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Columns.Length == 1);
            Assert.IsTrue(result.Columns[0] == "faFlightID");

            Assert.IsTrue(result.Rows.Length == 1);
            Assert.AreEqual(result.Rows[0].Values[0], "XYZ1234-1530000000-airline-0500");
        }

        [Test]
        public void TestExecuteNoFlightID()
        {
            string code = @"
select faFlightID
from GetFlightId
where ident = 'DAL503' and departuretime = '2020-3-7 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns(
                new ExecuteResult() { Result = @"{""error"":""NO_DATA flight not found""}" }
            );

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length == 0);
        }

        [Test]
        public void TestJoinNoFlightId()
        {
            string code = @"
select a.ident, faFlightID
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
where a.departuretime > '2020-1-21 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json");
            });
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns(
                 new ExecuteResult() { Result = @"{""error"":""NO_DATA flight not found""}" }
             );

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length == 0);

        }

        [Test]
        public void TestJoinWithApi200Error()
        {
            string code = @"
select *   
from airlineflightschedules a
join getflightid i on i.departureTime = a.departuretime and a.ident = i.ident
where a.departuretime > '2020-4-10 8:00' and a.origin = 'PHLI' and a.ident = 'DAL1381'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(
                new ExecuteResult() { Result = @"{""error"":""INVALID_ARGUMENT startDate is too far in the past(3 months)""}" }
            );
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns<HttpExecuteArg>((args) =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.GetFlightId.json");
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "Error executing request: INVALID_ARGUMENT startDate is too far in the past(3 months)");
        }

        [Test]
        public void TestJoin()
        {
            string code = @"
select a.ident, faFlightID
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
where a.departuretime > '2020-1-21 9:15'
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
            
            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length == 15);

            mock.Verify(v => v.GetFlightID(It.IsAny<HttpExecuteArg>()), Times.Exactly(15));
            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

    }
}
