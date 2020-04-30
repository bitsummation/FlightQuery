using System;

namespace FlightQuery.Sdk.SqlAst
{
    public class GreaterThanEqualExpression : BooleanExpression
    {
        public GreaterThanEqualExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
