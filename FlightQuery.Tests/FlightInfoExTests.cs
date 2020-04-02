using FlightQuery.Context;
using FlightQuery.Sdk;
using FlightQuery.Sdk.Model.V2;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class FlightInfoExTests
    {
        [Test]
        public void TestMissingRequired()
        {
            string code = @"
select diverted
from FlightInfoEx
where filed_ete = 'whatever'
";
            var context = new RunContext(code, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "faFlightID is required");
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

            }).Returns(() => new FlightInfoEx[] {new FlightInfoEx() });

            var context = new RunContext(code, ExecuteFlags.Semantic, mock.Object);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            mock.Verify(x => x.GetFlightInfoEx(It.IsAny<HttpExecuteArg>()), Times.Once());

        }
    }
}
