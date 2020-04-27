using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class SelectArgsTests
    {

        [Test]
        public void TestSelectAs()
        {
            string code = @"
select name, location as loc
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
            Assert.IsTrue(result.Columns.Length == 2);
            Assert.IsTrue(result.Columns[0] == "name");
            Assert.IsTrue(result.Columns[1] == "loc");
        }

        [Test]
        public void TestAirlineflightScheduleTestCase()
        {
            string code = @"
select
    case
        when actual_ident != ''
            then actual_ident
        else
            ident
        end
from airlineflightschedules
where departuretime > '2020-1-21 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json");
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);

        }

        [Test]
        public void TestSelectCaseWithElse()
        {
            string code = @"
select a.ident,
    case
        when i.actualarrivaltime = -1 and i.actualdeparturetime = -1 and i.estimatedarrivaltime = -1
        then 'cancelled'
        when i.actualdeparturetime != 0 and i.actualarrivaltime = 0
        then 'enroute'
        else 'scheduled'
    end as status
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
join FlightInfoEx i on i.faFlightID = f.faFlightID
where a.departuretime > '2020-1-21 9:15' and a.ident = 'ACI4600'
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
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
                .Returns<HttpExecuteArg>((args) =>
                {
                    return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoEx.json");
                });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);

            Assert.IsTrue(result.Columns[0] == "ident");
            Assert.IsTrue(result.Columns[1] == "status");
            Assert.AreEqual(result.Rows[0].Values[0], "ACI4600"); 
            Assert.AreEqual(result.Rows[0].Values[1], "scheduled");

        }


        [Test]
        public void TestSelectCaseWithElseNoAs()
        {
            string code = @"
select a.ident,
    case
        when i.actualarrivaltime = -1 and i.actualdeparturetime = -1 and i.estimatedarrivaltime = -1
        then 'cancelled'
        when i.actualdeparturetime != 0 and i.actualarrivaltime = 0
        then 'enroute'
        else 'scheduled'
    end
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
join FlightInfoEx i on i.faFlightID = f.faFlightID
where a.departuretime > '2020-1-21 9:15' and a.ident = 'ACI4600'
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
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
                .Returns<HttpExecuteArg>((args) =>
                {
                    return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoEx.json");
                });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);

            Assert.IsTrue(result.Columns[0] == "ident");
            Assert.IsTrue(result.Columns[1] == "(No column name)");
            Assert.AreEqual(result.Rows[0].Values[0], "ACI4600");
            Assert.AreEqual(result.Rows[0].Values[1], "scheduled");

        }


        [Test]
        public void TestSelectCaseWithElseNoAsNoElse()
        {
            string code = @"
select a.ident,
    case
        when i.actualarrivaltime = -1 and i.actualdeparturetime = -1 and i.estimatedarrivaltime = -1
        then 'cancelled'
        when i.actualdeparturetime != 0 and i.actualarrivaltime = 0
        then 'enroute'
        
    end
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
join FlightInfoEx i on i.faFlightID = f.faFlightID
where a.departuretime > '2020-1-21 9:15' and a.ident = 'ACI4600'
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
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
                .Returns<HttpExecuteArg>((args) =>
                {
                    return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoEx.json");
                });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);

            Assert.IsTrue(result.Columns[0] == "ident");
            Assert.IsTrue(result.Columns[1] == "(No column name)");
            Assert.AreEqual(result.Rows[0].Values[0], "ACI4600");
            Assert.AreEqual(result.Rows[0].Values[1], null);

        }


        [Test]
        public void TestSelectCaseWithElseNoAsElseReturnVariable()
        {
            string code = @"
select a.ident,
    case
        when i.actualarrivaltime = -1 and i.actualdeparturetime = -1 and i.estimatedarrivaltime = -1
        then 'cancelled'
        when i.actualdeparturetime != 0 and i.actualarrivaltime = 0
        then 'enroute'
      
        else a.seats_cabin_business
    end
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
join FlightInfoEx i on i.faFlightID = f.faFlightID
where a.departuretime > '2020-1-21 9:15' and a.ident = 'ACI4600'
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
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
                .Returns<HttpExecuteArg>((args) =>
                {
                    return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoEx.json");
                });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);

            Assert.IsTrue(result.Columns[0] == "ident");
            Assert.IsTrue(result.Columns[1] == "(No column name)");
            Assert.AreEqual(result.Rows[0].Values[0], "ACI4600");
            Assert.AreEqual(result.Rows[0].Values[1], 30);

        }


        [Test]
        public void TestSelectFirstCase()
        {
            string code = @"
select a.ident,
    case
        when i.actualarrivaltime = -1 and i.actualdeparturetime = -1 and i.estimatedarrivaltime = -1
        then 'cancelled'
        when i.actualdeparturetime != 0 and i.actualarrivaltime = 0
        then 'enroute'
        when i.actualdeparturetime != 0 and i.actualarrivaltime != 0 and i.actualdeparturetime != i.actualarrivaltime
        then 'arrived'
        else a.seats_cabin_business
    end
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
join FlightInfoEx i on i.faFlightID = f.faFlightID
where a.departuretime > '2020-1-21 9:15' and a.ident = 'ACI4600'
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
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
                .Returns<HttpExecuteArg>((args) =>
                {
                    return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoExCancelled.json");
                });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);

            Assert.IsTrue(result.Columns[0] == "ident");
            Assert.IsTrue(result.Columns[1] == "(No column name)");
            Assert.AreEqual(result.Rows[0].Values[0], "ACI4600");
            Assert.AreEqual(result.Rows[0].Values[1], "cancelled");

        }


        [Test]
        public void TestSelecThirdCase()
        {
            string code = @"
select a.ident,
    case
        when i.actualarrivaltime = -1 and i.actualdeparturetime = -1 and i.estimatedarrivaltime = -1
        then 'cancelled'
        when i.actualdeparturetime != 0 and i.actualarrivaltime = 0
        then 'enroute'
        when i.actualdeparturetime != 0 and i.actualarrivaltime != 0 and i.actualdeparturetime != i.actualarrivaltime
        then 'arrived'
        else a.seats_cabin_business
    end
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
join FlightInfoEx i on i.faFlightID = f.faFlightID
where a.departuretime > '2020-1-21 9:15' and a.ident = 'ACI4600'
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
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
                .Returns<HttpExecuteArg>((args) =>
                {
                    return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoEx.json");
                });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);

            Assert.IsTrue(result.Columns[0] == "ident");
            Assert.IsTrue(result.Columns[1] == "(No column name)");
            Assert.AreEqual(result.Rows[0].Values[0], "ACI4600");
            Assert.AreEqual(result.Rows[0].Values[1], "arrived");

        }
    }
}

