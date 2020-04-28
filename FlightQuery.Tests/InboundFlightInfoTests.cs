using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class InboundFlightInfoTests
    {
        [Test]
        public void TestExecute()
        {
            string code = @"
select faFlightID, ifaFlightID
from InboundFlightInfo
where faFlightID = 'unique-flight-id'
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetInboundFlightInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.InboundFlightInfo.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length > 0);

            Assert.AreEqual(result.Rows[0].Values[0], "unique-flight-id");
            Assert.AreEqual(result.Rows[0].Values[1], "SWA2055-1587444311-airline-0873");

        }
    }
}
