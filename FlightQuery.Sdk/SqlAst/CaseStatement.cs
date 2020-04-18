using System;
using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class CaseStatement : Element
    {
        public CaseStatement(ParseInfo parseInfo) : base(parseInfo) { }

        public WhenExpression[] WhenExpression
        {
            get { return Children.OfType<WhenExpression>().ToArray(); }

        }

        public Element ElseVariable
        {
            get { return Children.Where(x => !typeof(WhenExpression).IsInstanceOfType(x)).SingleOrDefault(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
