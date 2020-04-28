using System;
using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class WhenExpression : Element
    {
        public WhenExpression(Cursor parseInfo) : base(parseInfo) { }

        public BooleanExpression BooleanExpression
        {
            get { return Children.OfType<BooleanExpression>().SingleOrDefault(); }
        }

        public Element Variable
        {
            get { return Children.Where(x => !typeof(BooleanExpression).IsInstanceOfType(x)).SingleOrDefault(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
