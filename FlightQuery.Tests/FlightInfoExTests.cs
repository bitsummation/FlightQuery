using FlightQuery.Context;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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

            var context = RunContext.CreateSemanticContext(code);
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
            var context = RunContext.CreateSemanticContext(code);
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
                   return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoExCancelled.json");
               });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 1);
        }

        [Test]
        public void TestNoDataError()
        {
            string code = @"
select destination
from FlightInfoEx
where ident = 'AAL2563'
";
            var mock = new Mock<IHttpExecutorRaw>();

            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>())).Returns(
               new ExecuteResult() { Result = @"{""error"":""NO_DATA flight not found""}" }
               );

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 0);
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
                   return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoExCancelled.json");
               });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 0);
        }

        [Test]
        public void TestQuerableParametersIdent()
        {
            string code = @"
select diverted
from FlightInfoEx
where ident = 'AAL2563'
";

            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 1);
                var ident = args.Variables.Where(x => x.Variable == "ident").SingleOrDefault();
                Assert.IsTrue(ident != null);
                Assert.IsTrue(ident.Value == "AAL2563");

            }).Returns(() => new ApiExecuteResult<IEnumerable<FlightInfoEx>>(new FlightInfoEx[] { new FlightInfoEx() }));

            var context = RunContext.CreateSemanticContext(code, mock.Object);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            mock.Verify(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()), Times.Once());

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

            var context = RunContext.CreateSemanticContext(code, mock.Object);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            mock.Verify(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()), Times.Once());

        }

        [Test]
        public void TestExecuteIdent()
        {
            string code = @"
select destination
from FlightInfoEx
where ident = 'AAL2563'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.FlightInfoEx.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 1);
            Assert.IsTrue(result.First().Columns.Length == 1);
            Assert.AreEqual(result.First().Rows[0].Values[0], "KDFW");

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

            var context = RunContext.CreateRunContext(code, mock.Object);
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Columns.Length == 1);
            Assert.IsTrue(result.First().Columns[0].Name == "aircrafttype");

            Assert.IsTrue(result.First().Rows.Length == 1);
            Assert.AreEqual(result.First().Rows[0].Values[0], "B739");
        }

    }
}
