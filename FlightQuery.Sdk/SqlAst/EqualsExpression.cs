namespace FlightQuery.Sdk.SqlAst
{
    public class EqualsExpression : BooleanExpression
    {
        public EqualsExpression(Cursor parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
