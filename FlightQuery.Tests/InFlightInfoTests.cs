using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;

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

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.Rows.Length > 0);

            Assert.AreEqual(result.Rows[0].Values[0], 400);
            Assert.AreEqual(result.Rows[0].Values[6], "SWA5302-1587617128-airline-0319");
        }
    }
}
