using System;

namespace FlightQuery.Sdk.SqlAst
{
    public class UtcTimestampExpression : Element
    {
        public UtcTimestampExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public DateTime Value { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
