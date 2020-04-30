using System;

namespace FlightQuery.Sdk.SqlAst
{
    public class LessThanEqualExpression : BooleanExpression
    {
        public LessThanEqualExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
