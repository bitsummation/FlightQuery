namespace FlightQuery.Sdk.SqlAst
{
    public class OrExpression : BooleanExpression
    {
        public OrExpression(Cursor parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
