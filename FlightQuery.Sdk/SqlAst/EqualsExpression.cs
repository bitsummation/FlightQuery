namespace FlightQuery.Sdk.SqlAst
{
    public class EqualsExpression : BooleanExpression
    {
        public EqualsExpression(ParseInfo parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
