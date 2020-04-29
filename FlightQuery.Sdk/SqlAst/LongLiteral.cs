namespace FlightQuery.Sdk.SqlAst
{
    public class LongLiteral : VariableExpresion
    {
        public LongLiteral(ElementBounds parseInfo) : base(parseInfo) { }

        public long Value { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}