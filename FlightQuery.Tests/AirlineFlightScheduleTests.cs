using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

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

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "departuretime is required");
        }

        [Test]
        public void TestSimpleExecuteGreaterThanEqual()
        {
            string code = @"
select aircrafttype, actual_ident, ident, seats_cabin_business, arrivaltime
from AirlineFlightSchedules
where departuretime >= '2020-1-21 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(result.First().Columns.Length == 5);
            Assert.IsTrue(result.First().Columns[0] == "aircrafttype");

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 15);
            Assert.AreEqual(result.First().Rows[0].Values[0], "B762");
            Assert.AreEqual(result.First().Rows[0].Values[1], "OAE2412");
            Assert.AreEqual(result.First().Rows[0].Values[2], "ACA2412");
            Assert.AreEqual(result.First().Rows[0].Values[3], 18);
            Assert.AreEqual(((DateTime)result.First().Rows[0].Values[4]).ToString(Json.PrintDateTimeFormat), "2020-03-07 14:55");
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
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json"); 
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(result.First().Columns.Length == 5);
            Assert.IsTrue(result.First().Columns[0] == "aircrafttype");

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 15);
            Assert.AreEqual(result.First().Rows[0].Values[0], "B762");
            Assert.AreEqual(result.First().Rows[0].Values[1], "OAE2412");
            Assert.AreEqual(result.First().Rows[0].Values[2], "ACA2412");
            Assert.AreEqual(result.First().Rows[0].Values[3], 18);
            Assert.AreEqual(((DateTime)result.First().Rows[0].Values[4]).ToString(Json.PrintDateTimeFormat), "2020-03-07 14:55");
        }

        [Test]
        public void TestQuerableParametersTwoDepartureTimeGreaterLessEqual()
        {
            string code = @"
select aircrafttype, actual_ident, ident, seats_cabin_business, arrivaltime
from AirlineFlightSchedules
where departuretime >= '2020-1-21 9:15' and departuretime <= '2020-11-21 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 4);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start.Value == "1579598100");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1605950100");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

        [Test]
        public void TestQuerableParametersTwoDepartureTime()
        {
            string code = @"
select aircrafttype, actual_ident, ident, seats_cabin_business, arrivaltime
from AirlineFlightSchedules
where departuretime > '2020-1-21 9:15' and departuretime < '2020-11-21 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 4);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start.Value == "1579598100");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1605950100");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

        [Test]
        public void TestApiReturn200WithError()
        {
            string code = @"
select aircrafttype, actual_ident, ident, seats_cabin_business, arrivaltime
from AirlineFlightSchedules
where departuretime < '2020-1-21 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(
                new ExecuteResult() { Result = @"{""error"":""INVALID_ARGUMENT startDate is too far in the past(3 months)""}" }
            );

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "Error executing request: INVALID_ARGUMENT startDate is too far in the past(3 months)");
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
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 8);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1582967700");

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

            var context = RunContext.CreateSemanticContext(code, new HttpExecutor(mock.Object));
            context.Run();

            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

        [Test]
        public void TestDepartureTimeEqual()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where '2020-3-7 9:15' = departuretime
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 4);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1583572440");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572560");
            });

            var context = RunContext.CreateSemanticContext(code, new HttpExecutor(mock.Object));
            context.Run();

            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

        [Test]
        public void TestDepartureTimeLessThanPropertyRight()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where '2020-3-7 9:15' > departuretime
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 4);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1582967700");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572500");
            });

            var context = RunContext.CreateSemanticContext(code, new HttpExecutor(mock.Object));
            context.Run();

            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());

        }

        [Test]
        public void TestDepartureTimeLessThanPropertyLeft()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where departuretime < '2020-3-7 9:15'
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 4);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1582967700");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572500");
            });

            var context = RunContext.CreateSemanticContext(code, new HttpExecutor(mock.Object));
            context.Run();

            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());

        }

        [Test]
        public void TestDepartureTimeGreaterThanPropertyRight()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where '2020-3-7 9:15' < departuretime
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 4);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1583572500");

                var end = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1584177300");
            });

            var context = RunContext.CreateSemanticContext(code, new HttpExecutor(mock.Object));
            context.Run();

            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());

        }

        [Test]
        public void TestDepartureTimeGreaterThanPropertyLeft()
        {
            string code = @"
select arrivaltime, aircrafttype
from AirlineFlightSchedules a
where departuretime > '2020-3-7 9:15'
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 4);
                var start = args.Variables.Where(x => x.Variable == "startDate").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "1583572500");

                var end  = args.Variables.Where(x => x.Variable == "endDate").SingleOrDefault();
                Assert.IsTrue(end.Value == "1584177300");
            });

            var context = RunContext.CreateSemanticContext(code, new HttpExecutor(mock.Object));
            context.Run();

            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }
    }
}
