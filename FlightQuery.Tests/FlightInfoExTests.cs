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
    public class FlightInfoExTests
    {
        [Test]
        public void FlightIdRequiredJoin()
        {
            string code = @"
select *   
from airlineflightschedules a
join getflightid i on i.departureTime = a.departuretime and a.ident = i.ident
join FlightInfoEx f on i.faFlightID = f.destination
where a.departuretime > '2020-4-10 8:00' and a.origin = 'kaus'
";

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic);
            context.Run();
            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "faFlightID is required");

        }

        [Test]
        public void TestMissingRequired()
        {
            string code = @"
select diverted
from FlightInfoEx
where filed_ete = 'whatever'
";
            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "faFlightID is required");
        }

        [Test]
        public void TestExecuteCancelled()
        {
            string code = @"
select *
from FlightInfoEx
where faFlightID = 'AAL2594-1586309220-schedule-0000' and actualdeparturetime = -1 and estimatedarrivaltime = -1 and actualarrivaltime = -1
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
               .Returns<HttpExecuteArg>((args) =>
               {
                   string source = string.Empty;
                   var assembly = Assembly.GetExecutingAssembly();
                   using (Stream stream = assembly.GetManifestResourceStream("FlightQuery.Tests.FlightInfoExCancelled.json"))
                   using (StreamReader reader = new StreamReader(stream))
                   {
                       source = reader.ReadToEnd();
                   }

                   return new ExecuteResult() { Result = source };
               });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length == 1);
        }

        [Test]
        public void TestExecuteCancelledEmpty()
        {
            string code = @"
select *
from FlightInfoEx
where faFlightID = 'AAL2594-1586309220-schedule-0000' and actualdeparturetime != -1 and estimatedarrivaltime != -1 and actualarrivaltime != -1
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()))
               .Returns<HttpExecuteArg>((args) =>
               {
                   string source = string.Empty;
                   var assembly = Assembly.GetExecutingAssembly();
                   using (Stream stream = assembly.GetManifestResourceStream("FlightQuery.Tests.FlightInfoExCancelled.json"))
                   using (StreamReader reader = new StreamReader(stream))
                   {
                       source = reader.ReadToEnd();
                   }

                   return new ExecuteResult() { Result = source };
               });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length == 0);
        }

        [Test]
        public void TestQuerableParameters()
        {
            string code = @"
select diverted
from FlightInfoEx
where faFlightID = 'some-flight-number'
";

            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 1);
                var ident = args.Variables.Where(x => x.Variable == "ident").SingleOrDefault();
                Assert.IsTrue(ident != null);
                Assert.IsTrue(ident.Value == "some-flight-number");

            }).Returns(() => new ApiExecuteResult<IEnumerable<FlightInfoEx>>(new FlightInfoEx[] {new FlightInfoEx() }));

            var context = new RunContext(code, string.Empty, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            mock.Verify(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()), Times.Once());

        }

        [Test]
        public void TestExecute()
        {
            string code = @"
select aircrafttype
from FlightInfoEx
where faFlightID = 'some-flight-number'
";

            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>())).Returns(() => new ApiExecuteResult<IEnumerable<FlightInfoEx>>(new FlightInfoEx[] { new FlightInfoEx() {aircrafttype = "B739", faFlightID = "some-flight-number" } }));

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), mock.Object);
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Columns.Length == 1);
            Assert.IsTrue(result.Columns[0] == "aircrafttype");

            Assert.IsTrue(result.Rows.Length == 1);
            Assert.AreEqual(result.Rows[0].Values[0], "B739");
        }

       

        [Test]
        public void TestJoin()
        {
            string code = @"
select a.ident, i.faFlightID, i.route
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
            Assert.IsTrue(result.Rows.Length == 1);
            Assert.AreEqual(result.Rows[0].Values[2], "ELOEL2 FORSS GUTZZ SOCKK3");
        }
    }
}
