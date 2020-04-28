namespace FlightQuery.Sdk.SqlAst
{
    public class LessThanExpression : BooleanExpression
    {
        public LessThanExpression(Cursor parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
