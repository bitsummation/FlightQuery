namespace FlightQuery.Sdk.SqlAst
{
    public class NotEqualsExpression : BooleanExpression
    {
        public NotEqualsExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
