namespace FlightQuery.Sdk.SqlAst
{
    public class AndExpression : BooleanExpression
    {
        public AndExpression(ParseInfo parseInfo) : base(parseInfo) { }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
