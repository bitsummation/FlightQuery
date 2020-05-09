using FlightQuery.Context;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class GetFlightIdTests
    {
        [Test]
        public void TestMissingRequired()
        {
            string code = @"
select faFlightID
from GetFlightId
where ident = 'DAL503'
";
            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "departureTime is required");
        }

        [Test]
        public void TestQuerableParameters()
        {
            string code = @"
select faFlightID
from GetFlightId
where ident = 'DAL503' and departuretime = '2020-3-7 9:15'
";

            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>(args =>
            {
                Assert.IsTrue(args.Variables.Count() == 2);
                var start = args.Variables.Where(x => x.Variable == "ident").SingleOrDefault();
                Assert.IsTrue(start != null);
                Assert.IsTrue(start.Value == "DAL503");

                var end = args.Variables.Where(x => x.Variable == "departureTime").SingleOrDefault();
                Assert.IsTrue(end.Value == "1583572500");
            }).Returns(() => new ApiExecuteResult<GetFlightId>(new GetFlightId()));

            var context = RunContext.CreateSemanticContext(code, mock.Object);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            mock.Verify(v => v.GetFlightID(It.IsAny<HttpExecuteArg>()), Times.Once());
        }

        [Test]
        public void TestExecute()
        {
            string code = @"
select faFlightID
from GetFlightId
where ident = 'DAL503' and departuretime = '2020-3-7 9:15'
";
            var mock = new Mock<IHttpExecutor>();
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns(() => new ApiExecuteResult<GetFlightId>(new GetFlightId() {faFlightID = "XYZ1234-1530000000-airline-0500" }));

            var context = RunContext.CreateRunContext(code, mock.Object);
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Columns.Length == 1);
            Assert.IsTrue(result.First().Columns[0].Name == "faFlightID");

            Assert.IsTrue(result.First().Rows.Length == 1);
            Assert.AreEqual(result.First().Rows[0].Values[0], "XYZ1234-1530000000-airline-0500");
        }

        [Test]
        public void TestExecuteNoFlightID()
        {
            string code = @"
select faFlightID
from GetFlightId
where ident = 'DAL503' and departuretime = '2020-3-7 9:15'
";

            var mock = new Mock<IHttpExecutorRaw>();
            
            mock.Setup(x => x.GetFlightID(It.IsAny<HttpExecuteArg>())).Returns(
                new ExecuteResult() { Result = @"{""error"":""NO_DATA flight not found""}" }
            );

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 0);
        }

    }
}
