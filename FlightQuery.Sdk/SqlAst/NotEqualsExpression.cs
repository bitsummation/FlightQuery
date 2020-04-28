namespace FlightQuery.Sdk.SqlAst
{
    public class NotEqualsExpression : BooleanExpression
    {
        public NotEqualsExpression(Cursor parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
