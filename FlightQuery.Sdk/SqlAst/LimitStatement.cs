using System;

namespace FlightQuery.Sdk.SqlAst
{
    public class LimitStatement : Element
    {
        public LimitStatement(ElementBounds parseInfo) : base(parseInfo)
        {
            Offset = 0;
            Count = 15;
        }

        public int Offset { get; set; }
        public int Count { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
