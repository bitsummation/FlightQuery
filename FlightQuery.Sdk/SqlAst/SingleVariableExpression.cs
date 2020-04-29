namespace FlightQuery.Sdk.SqlAst
{
    public class SingleVariableExpression : VariableExpresion
    {
        public SingleVariableExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public string Id { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
