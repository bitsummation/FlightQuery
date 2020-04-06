using FlightQuery.Context;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
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

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "departuretime is required");
        }

        [Test]
        public void TestSimpleExecute()
        {
            string code = @"
select aircrafttype, actual_ident, ident, seats_cabin_business, arrivaltime
from AirlineFlightSchedules
where departuretime > '2020-1-21 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                string source = string.Empty;
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("FlightQuery.Tests.AirlineFlightSchedule.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    source = reader.ReadToEnd();
                }
                return new ExecuteResult() { Result = source };
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(result.Columns.Length == 5);
            Assert.IsTrue(result.Columns[0] == "aircrafttype");

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length == 15);
            Assert.AreEqual(result.Rows[0].Values[0], "B762");
            Assert.AreEqual(result.Rows[0].Values[1], "OAE2412");
            Assert.AreEqual(result.Rows[0].Values[2], "ACA2412");
            Assert.AreEqual(result.Rows[0].Values[3], 18);
        }

        [Test]
        public void TestQuerableParameters()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where '2020-3-7 9:15' > a.departuretime and origin = 'kaus' and destination = 'kpdx' and ident = 'UAL6879'
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 6);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572500");

                Assert.IsTrue(args.Variables.Where(x => x.Variable == "origin").Count() == 1);
                Assert.IsTrue(args.Variables.Where(x => x.Variable == "origin").Select(x => x.Value).Single() == "KAUS");

                Assert.IsTrue(args.Variables.Where(x => x.Variable == "destination").Count() == 1);
                Assert.IsTrue(args.Variables.Where(x => x.Variable == "destination").Select(x => x.Value).Single() == "KPDX");

                Assert.IsTrue(args.Variables.Where(x => x.Variable == "airline").Count() == 1);
                Assert.IsTrue(args.Variables.Where(x => x.Variable == "airline").Select(x => x.Value).Single() == "UAL");

                Assert.IsTrue(args.Variables.Where(x => x.Variable == "flightno").Count() == 1);
                Assert.IsTrue(args.Variables.Where(x => x.Variable == "flightno").Select(x => x.Value).Single() == "6879");
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic, new HttpExecutor(mock.Object));
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
            }).Returns(new ApiExecuteResult<IEnumerable<AirlineFlightSchedule>>(new AirlineFlightSchedule[] { new AirlineFlightSchedule() }));

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic, mock.Object);
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
            }).Returns(new ApiExecuteResult<IEnumerable<AirlineFlightSchedule>>(new AirlineFlightSchedule[] { new AirlineFlightSchedule() }));

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic, mock.Object);
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
                Assert.IsTrue(end.Value == "1584177300");
            }).Returns(new ApiExecuteResult<IEnumerable<AirlineFlightSchedule>>(new AirlineFlightSchedule[] { new AirlineFlightSchedule() }));

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic, mock.Object);
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
                Assert.IsTrue(end.Value == "1584177300");
            }).Returns(new ApiExecuteResult<IEnumerable<AirlineFlightSchedule>>(new AirlineFlightSchedule[] { new AirlineFlightSchedule() }));

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            mock.Verify(v => v.AirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }
    }
}
