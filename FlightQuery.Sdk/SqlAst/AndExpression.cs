namespace FlightQuery.Sdk.SqlAst
{
    public class AndExpression : BooleanExpression
    {
        public AndExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
