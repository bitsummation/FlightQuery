using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class AirportInfoTests
    {
        [Test]
        public void TestExecute()
        {
            string code = @"
select location
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
            Assert.IsTrue(result.Columns.Length == 1);
            Assert.IsTrue(result.Columns[0] == "location");

            Assert.IsTrue(result.Rows.Length == 1);
            Assert.AreEqual(result.Rows[0].Values[0], "Austin, TX");
        }

        [Test]
        public void TestNoAuth()
        {
            string code = @"
select location
from AirportInfo 
where airportCode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.AirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return new ExecuteResult() { Result = "", Error = new ApiExecuteError("Authentication error") };
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 1);
        }

        [Test]
        public void Test200OkWithError()
        {
            string code = @"
select location
from AirportInfo 
where airportCode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.AirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return new ExecuteResult() { Result = @"{""error"":""NO_DATA unknown airport INVALID""}"};
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "Error executing request: NO_DATA unknown airport INVALID");
        }

        [Test]
        public void TestQuerableParameters()
        {
            string code = @"
select location
from AirportInfo 
where airportCode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.AirportInfo(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>((args) =>
            {
                Assert.IsTrue(args.Variables.Count() == 1);
                var start = args.Variables.Where(x => x.Variable == "airportCode").SingleOrDefault();
                Assert.IsTrue(start != null);
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
        }

        [Test]
        public void TestQuerableParametersLowerCase()
        {
            string code = @"
select location
from AirportInfo 
where airportcode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.AirportInfo(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>((args) =>
            {
                Assert.IsTrue(args.Variables.Count() == 1);
                var start = args.Variables.Where(x => x.Variable == "airportCode").SingleOrDefault();
                Assert.IsTrue(start != null);
            });

            var context = new RunContext(code, string.Empty, ExecuteFlags.Run, new EmptyHttpExecutor(), new HttpExecutor(mock.Object));
            var result = context.Run();
        }
    }
}
