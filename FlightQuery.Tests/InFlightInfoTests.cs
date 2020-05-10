using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class InFlightInfoTests
    {
        [Test]
        public void TestExecute()
        {
            string code = @"
select *
from inflightinfo
where ident = ""SWA5302""
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetInFlightInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.InFightInfo.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length > 0);

            Assert.AreEqual(result.First().Rows[0].Values[0], 400);
            Assert.AreEqual(result.First().Rows[0].Values[6], "SWA5302-1587617128-airline-0319");
        }

        [Test]
        public void TestArrivalTimeLeft()
        {
            string code = @"
select departureTime,
    arrivalTime - UTC_TIMESTAMP()
from inflightinfo
where ident = ""SWA5302""
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetInFlightInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.InFightInfo.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Rows.Length == 1);
            Assert.IsTrue(result.First().Columns.Length == 2);

            Assert.IsTrue(result.First().Rows[0].Values[1] != null);


        }
    }
}
