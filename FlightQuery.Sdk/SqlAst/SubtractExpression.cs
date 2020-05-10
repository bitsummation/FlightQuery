namespace FlightQuery.Sdk.SqlAst
{
    public class SubtractExpression : Element
    {
        public SubtractExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public Element Left { get; set; }
        public Element Right { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
