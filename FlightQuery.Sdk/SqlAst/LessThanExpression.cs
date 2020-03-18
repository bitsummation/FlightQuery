namespace FlightQuery.Sdk.SqlAst
{
    public class LessThanExpression : BooleanExpression
    {
        public LessThanExpression(ParseInfo parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
