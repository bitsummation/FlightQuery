using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class WhereStatement : Element
    {
        public WhereStatement(ElementBounds parseInfo) : base(parseInfo) { }

        public BooleanExpression BooleanExpression
        {
            get { return Children.OfType<BooleanExpression>().SingleOrDefault(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
