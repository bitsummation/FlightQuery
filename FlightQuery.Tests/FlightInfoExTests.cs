using FlightQuery.Context;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
