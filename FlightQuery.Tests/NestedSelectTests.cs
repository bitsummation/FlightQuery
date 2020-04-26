using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Reflection;

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

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length > 0);
            Assert.AreEqual(result.Columns[0], "(No column name)");
            Assert.AreEqual(result.Rows[0].Values[0], "OAE2412");
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

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length == 15);

            Assert.AreEqual(result.Columns[0], "ident");
            Assert.AreEqual(result.Columns[1], "faFlightID");
            Assert.AreEqual(result.Rows[0].Values[0], "OAE2412");
            Assert.AreEqual(result.Rows[0].Values[1], "AAL2594-1586309220-schedule-0000");

        }
    }
}
