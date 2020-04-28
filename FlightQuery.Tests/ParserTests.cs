using FlightQuery.Context;
using FlightQuery.Sdk;
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

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
        }

        [Test]
        public void NoFrom()
        {
            string code = @"
select * asdfsad
";

            var context = RunContext.CreateSemanticContext(code);
            context.Run();

            Assert.IsTrue(context.Errors.Count == 1);
        }


    }
}
