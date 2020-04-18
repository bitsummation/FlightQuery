using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Reflection;

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
            mock.Setup(x => x.AirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                string source = string.Empty;
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("FlightQuery.Tests.AirportInfo.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    source = reader.ReadToEnd();
                }

                return new ExecuteResult() { Result = source };
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Columns.Length == 2);
            Assert.IsTrue(result.Columns[0] == "name");
            Assert.IsTrue(result.Columns[1] == "loc");
        }

        [Test]
        public void TestSelectCaseWithElse()
        {
            string code = @"
select a.ident,
    case
        when i.actualarrivaltime != -1 and i.actualdeparturetime != -1 and i.estimatedarrivaltime != -1
        then 'cancelled'
        when i.actualdeparturetime != 0 and i.actualarrivaltime = 0
        then 'enroute'
        when i.actualdeparturetime != 0 and i.actualarrivaltime != 0 and i.actualdeparturetime != i.actualarrivaltime
        then 'arrived'
        else 'scheduled'
    end as status
from AirlineFlightSchedules a
join GetFlightId f on f.ident = a.ident and f.departureTime = a.departureTime 
join FlightInfoEx i on i.faFlightID = f.faFlightID
where a.departuretime > '2020-1-21 9:15' and a.ident = 'ACI4600'
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
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns<HttpExecuteArg>((args) =>
            {
                string source = string.Empty;
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("FlightQuery.Tests.GetFlightId.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    source = reader.ReadToEnd();
                }
                return new ExecuteResult() { Result = source };
            });
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
                .Returns<HttpExecuteArg>((args) =>
                {
                    string source = string.Empty;
                    var assembly = Assembly.GetExecutingAssembly();
                    using (Stream stream = assembly.GetManifestResourceStream("FlightQuery.Tests.FlightInfoEx.json"))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        source = reader.ReadToEnd();
                    }

                    return new ExecuteResult() { Result = source };
                });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);

            Assert.IsTrue(result.Columns[0] == "ident");
            Assert.IsTrue(result.Columns[1] == "status");
            Assert.AreEqual(result.Rows[0].Values[0], "ACI4600"); 
            Assert.AreEqual(result.Rows[0].Values[1], "scheduled");

        }
    }
}
