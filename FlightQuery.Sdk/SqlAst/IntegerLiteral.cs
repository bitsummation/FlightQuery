namespace FlightQuery.Sdk.SqlAst
{
    public class IntegerLiteral : VariableExpresion
    {
        public IntegerLiteral(ParseInfo parseInfo) : base(parseInfo) { }

        public int Value { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}