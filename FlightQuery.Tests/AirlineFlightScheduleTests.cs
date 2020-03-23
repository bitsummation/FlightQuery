using FlightQuery.Context;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class AirlineFlightScheduleTests
    {
        [Test]
        public void TestRequiredDepartureTime()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules
where origin = 'katl'
";

            var context = new RunContext(code, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "departuretime is required");
        }

        [Test]
        public void TestSimpleExecute()
        {
            string code = @"
select aircrafttype, actual_ident, ident, seats_cabin_business
from AirlineFlightSchedules
where departuretime > '2020-1-21 9:15' and origin = 'KATL' and destination = 'KEWR' and ident = 'DAL503'
";

            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                string source = string.Empty;
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("FlightQuery.Tests.AirlineFlightSchedule.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    source = reader.ReadToEnd();
                }
                return Deserialize.DeserializeObject<AirlineFlightSchedule[]>(source);
            });

            var context = new RunContext(code, ExecuteFlags.Run, new EmptyHttpExecutor(), mock.Object);
            var result = context.Run();
            Assert.IsTrue(result.Columns.Length == 4);
            Assert.IsTrue(result.Columns[0] == "aircrafttype");

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length == 1);
            Assert.AreEqual(result.Rows[0].Values[0], "B739");
            Assert.AreEqual(result.Rows[0].Values[1], "");
            Assert.AreEqual(result.Rows[0].Values[2], "DAL503");
            Assert.AreEqual(result.Rows[0].Values[3], 20);
        }

        [Test]
        public void TestQuerableParameters()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where '2020-3-7 9:15' > a.departuretime and origin = 'kaus' and destination = 'kpdx' and ident = 'UAL6879'
";
            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 7);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572500");

                Assert.IsTrue(args.Variables.Where(x => x.Variable == "origin").Count() == 1);
                Assert.IsTrue(args.Variables.Where(x => x.Variable == "origin").Select(x => x.Value).Single() == "kaus");

                Assert.IsTrue(args.Variables.Where(x => x.Variable == "destination").Count() == 1);
                Assert.IsTrue(args.Variables.Where(x => x.Variable == "destination").Select(x => x.Value).Single() == "kpdx");

                Assert.IsTrue(args.Variables.Where(x => x.Variable == "airline").Count() == 1);
                Assert.IsTrue(args.Variables.Where(x => x.Variable == "airline").Select(x => x.Value).Single() == "UAL");

                Assert.IsTrue(args.Variables.Where(x => x.Variable == "flightno").Count() == 1);
                Assert.IsTrue(args.Variables.Where(x => x.Variable == "flightno").Select(x => x.Value).Single() == "6879");
            });

            var context = new RunContext(code, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            mock.Verify(v => v.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

        [Test]
        public void TestDepartureTimeLessThanPropertyRight()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where '2020-3-7 9:15' > departuretime
";
            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 2);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572500");
            });

            var context = new RunContext(code, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            mock.Verify(v => v.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());

        }

        [Test]
        public void TestDepartureTimeLessThanPropertyLeft()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where departuretime < '2020-3-7 9:15'
";
            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 2);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572500");
            });

            var context = new RunContext(code, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            mock.Verify(v => v.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());

        }

        [Test]
        public void TestDepartureTimeGreaterThanPropertyRight()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where '2020-3-7 9:15' < departuretime
";

            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 2);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1583572500");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "2147483646");
            });

            var context = new RunContext(code, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            mock.Verify(v => v.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());

        }

        [Test]
        public void TestDepartureTimeGreaterThanPropertyLeft()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where departuretime > '2020-3-7 9:15'
";
            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 2);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1583572500");

                var end  = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "2147483646");
            });

            var context = new RunContext(code, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            mock.Verify(v => v.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }
    }
}
