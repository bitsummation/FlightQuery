﻿using FlightQuery.Context;
using NUnit.Framework;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class ParserTests
    {
    
        [Test]
        public void WrongSelectParamsSytax()
        {
            string code = @"
select asdf, 
";

            var context = new RunContext(code, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
        }

        [Test]
        public void NoFrom()
        {
            string code = @"
select * asdfsad
";

            var context = new RunContext(code, ExecuteFlags.Semantic);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
        }


    }
}
