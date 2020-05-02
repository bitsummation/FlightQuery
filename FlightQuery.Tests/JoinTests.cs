using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class JoinTests
    {

        [Test]
        public void TestJoinMultiple()
        {
            string code = @"
select a.ident, i.faFlightID, i.route
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

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 1);
            Assert.AreEqual(result.First().Rows[0].Values[2], "ELOEL2 FORSS GUTZZ SOCKK3");
        }

        [Test]
        public void TestJoinSameTable()
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

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Columns.Length == 5);
            Assert.IsTrue(result.First().Rows.Length == 1);

            mock.Verify(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once);
            mock.Verify(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>()), Times.Exactly(30));
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

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 0);

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

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "Error executing request: INVALID_ARGUMENT startDate is too far in the past(3 months)");
        }

        [Test]
        public void TestJoinFlightId()
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

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 15);

            mock.Verify(v => v.GetFlightID(It.IsAny<HttpExecuteArg>()), Times.Exactly(15));
            mock.Verify(v => v.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

        [Test]
        public void TestMultipleResultsRightTable()
        {
            string code = @"
select a.ident, f.faFlightID, altitude, h.timestamp
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime
join GetHistoricalTrack h on h.faFlightID = f.faFlightID
where a.departuretime > '2020-1-21 9:15'
";
          
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirlineFlightSchedule(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirlineFlightSchedule.json");
            });
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns<HttpExecuteArg>((args) =>
            {
                var ident = args.Variables.Where(x => x.Variable == "ident").Single();
                if(ident.Value == "DAL1381")
                    return new ExecuteResult() { Result = @"{""GetFlightIDResult"": ""flight-id-a""}" };
                
                return new ExecuteResult() { Result = @"{""error"":""NO_DATA flight not found""}" };
            });
            mock.Setup(x => x.GetHistoricalTrack(It.IsAny<HttpExecuteArg>())).Returns<HttpExecuteArg>((args) =>
            {
                var faflight = args.Variables.Where(x => x.Variable == "faFlightID").Single();
                if(faflight.Value == "flight-id-a")
                {
                    return TestHelper.LoadJson("FlightQuery.Tests.GetHistoricTrack.json");
                }

                return new ExecuteResult() { Result = @"{""error"":""NO_DATA no data available""}" };
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 392);

        }

        [Test]
        public void TestLinqJoin()
        {
            var tableA = new[] {
                new A() {Key = "A"},
                new A() {Key = "B"},
                new A() {Key = "A"}
            };

            var tableB = new[]
            {
                 new B() {Key = "A", Name = "Data 1"},
                 new B() {Key = "A", Name = "Data 2"},
                 new B() {Key = "A", Name = "Data 3"},
            };

            var join = tableA.Join(tableB,
                a => a.Key, b => b.Key,
                (post, meta) => new { A = post, B = meta }).ToArray();
        }

        private class A
        {
            public string Key { get; set; }
        }

        private class B
        {
            public string Key { get; set; }
            public string Name { get; set; }
        }

    }
}
