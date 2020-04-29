namespace FlightQuery.Sdk.SqlAst
{
    public class MemberVariableExpression : VariableExpresion
    {
        public MemberVariableExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public string Alias { get; set; }
        public string Id { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
