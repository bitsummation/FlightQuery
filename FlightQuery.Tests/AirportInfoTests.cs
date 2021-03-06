﻿using FlightQuery.Context;
using FlightQuery.Sdk;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class AirportInfoTests
    {
        [Test]
        public void TestRequiredAirportCode()
        {
            string code = @"
select location
from AirportInfo 
where location = 'kaus'
";

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
            Assert.IsTrue(context.Errors[0].Message == "airportCode is required");
        }

        [Test]
        public void TestSelectStar()
        {
            string code = @"
select *
from AirportInfo 
where airportCode = 'kaus'
";
            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirportInfo.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();

            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Columns.Length == 6);
            Assert.IsTrue(result.First().Columns[0].Name == "airportCode");
            Assert.IsTrue(result.First().Columns[1].Name == "latitude");
            Assert.IsTrue(result.First().Columns[2].Name == "longitude");
            Assert.IsTrue(result.First().Columns[3].Name == "location");
            Assert.IsTrue(result.First().Columns[4].Name == "name");
            Assert.IsTrue(result.First().Columns[5].Name == "timezone");

            Assert.AreEqual(result.First().Rows[0].Values[0], "kaus");
            Assert.AreEqual(result.First().Rows[0].Values[1], 30.1945272f);
            Assert.AreEqual(result.First().Rows[0].Values[2], -97.6698761f);
            Assert.AreEqual(result.First().Rows[0].Values[3], "Austin, TX");
            Assert.AreEqual(result.First().Rows[0].Values[5], ":America/Chicago");
        }

        [Test]
        public void TestExecute()
        {
            string code = @"
select location
from AirportInfo 
where airportCode = 'kaus'
";

            var mock = new Mock<IHttpExecutorRaw>();
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return TestHelper.LoadJson("FlightQuery.Tests.AirportInfo.json");
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
            Assert.IsTrue(context.Errors.Count == 0);
            Assert.IsTrue(result.First().Columns.Length == 1);
            Assert.IsTrue(result.First().Columns[0].Name == "location");

            Assert.IsTrue(result.First().Rows.Length == 1);
            Assert.AreEqual(result.First().Rows[0].Values[0], "Austin, TX");
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
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return new ExecuteResult() { Result = "", Error = new ApiExecuteError(ApiExecuteErrorType.AuthError, "Authentication error") };
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
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
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Returns(() =>
            {
                return new ExecuteResult() { Result = @"{""error"":""NO_DATA unknown airport INVALID""}"};
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
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
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>((args) =>
            {
                Assert.IsTrue(args.Variables.Count() == 1);
                var start = args.Variables.Where(x => x.Variable == "airportCode").SingleOrDefault();
                Assert.IsTrue(start != null);
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
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
            mock.Setup(x => x.GetAirportInfo(It.IsAny<HttpExecuteArg>())).Callback<HttpExecuteArg>((args) =>
            {
                Assert.IsTrue(args.Variables.Count() == 1);
                var start = args.Variables.Where(x => x.Variable == "airportCode").SingleOrDefault();
                Assert.IsTrue(start != null);
            });

            var context = RunContext.CreateRunContext(code, new HttpExecutor(mock.Object));
            var result = context.Run();
        }
    }
}
