using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class AirportInfoTests
    {
        [Test]
        public void TestRequiredAirportCode()
        {
            string code = @"
select location
from AirportInfo 
where location = 'kaus'
";

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "airportCode is required");
        }

        [Test]
        public void TestSelectStar()
        {
            string code = @"
select *
from AirportInfo 
where airportCode = 'kaus'
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirportInfo.json");
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Columns.Length == 6);
            Assert.IsTrue(result.Columns[0] == "airportCode");
            Assert.IsTrue(result.Columns[1] == "latitude");
            Assert.IsTrue(result.Columns[2] == "longitude");
            Assert.IsTrue(result.Columns[3] == "location");
            Assert.IsTrue(result.Columns[4] == "name");
            Assert.IsTrue(result.Columns[5] == "timezone");

            Assert.AreEqual(result.Rows[0].Values[0], "kaus");
            Assert.AreEqual(result.Rows[0].Values[1], 30.1945272f);
            Assert.AreEqual(result.Rows[0].Values[2], -97.6698761f);
            Assert.AreEqual(result.Rows[0].Values[3], "Austin, TX");
            Assert.AreEqual(result.Rows[0].Values[5], ":America/Chicago");
        }

        [Test]
        public void TestExecute()
        {
            string code = @"
select location
from AirportInfo 
where airportCode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirportInfo.json");
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Columns.Length == 1);
            Assert.IsTrue(result.Columns[0] == "location");

            Assert.IsTrue(result.Rows.Length == 1);
            Assert.AreEqual(result.Rows[0].Values[0], "Austin, TX");
        }

        [Test]
        public void TestJoin()
        {
            string code = @"
select f.ident, f.departuretime, f.arrivaltime, o.name, d.name
from airlineflightschedules f
join airportinfo d on d.airportCode = f.destination
join airportinfo o on o.airportCode = f.origin
where f.departuretime > '2020-1-16 1:46'and f.ident = 'DAL1381'";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json");
            });
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirportInfo.json");
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Columns.Length == 5);
            Assert.IsTrue(result.Rows.Length == 1);

            mock.Verify(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once);
            mock.Verify(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>()), Times.Exactly(30));
        }

        [Test]
        public void TestNoAuth()
        {
            string code = @"
select location
from AirportInfo 
where airportCode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return new ExecuteResult() { Result = "", Error = new ApiExecuteError(ApiExecuteErrorType.AuthError, "Authentication error") };
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 1);
        }

        [Test]
        public void Test200OkWithError()
        {
            string code = @"
select location
from AirportInfo 
where airportCode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return new ExecuteResult() { Result = @"{""error"":""NO_DATA unknown airport INVALID""}"};
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "Error executing request: NO_DATA unknown airport INVALID");
        }

        [Test]
        public void TestQuerableParameters()
        {
            string code = @"
select location
from AirportInfo 
where airportCode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>((args) =>
            {
                Assert.IsTrue(args.Variables.Count() == 1);
                var start = args.Variables.Where(x => x.Variable == "airportCode").SingleOrDefault();
                Assert.IsTrue(start != null);
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
        }

        [Test]
        public void TestQuerableParametersLowerCase()
        {
            string code = @"
select location
from AirportInfo 
where airportcode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>((args) =>
            {
                Assert.IsTrue(args.Variables.Count() == 1);
                var start = args.Variables.Where(x => x.Variable == "airportCode").SingleOrDefault();
                Assert.IsTrue(start != null);
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
        }
    }
}
