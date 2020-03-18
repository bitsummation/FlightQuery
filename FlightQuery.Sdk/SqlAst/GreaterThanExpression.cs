namespace FlightQuery.Sdk.SqlAst
{
    public class GreaterThanExpression : BooleanExpression
    {
        public GreaterThanExpression(ParseInfo parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
