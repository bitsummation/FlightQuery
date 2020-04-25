using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Reflection;

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
                string source = string.Empty;
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("FlightQuery.Tests.InboundFlightInfo.json"))
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

            Assert.AreEqual(result.Rows[0].Values[0], "unique-flight-id");
            Assert.AreEqual(result.Rows[0].Values[1], "SWA2055-1587444311-airline-0873");

        }
    }
}
