namespace FlightQuery.Sdk.SqlAst
{
    public class OrExpression : BooleanExpression
    {
        public OrExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
