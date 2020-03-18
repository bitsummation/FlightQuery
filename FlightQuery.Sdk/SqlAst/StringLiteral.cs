namespace FlightQuery.Sdk.SqlAst
{
    public class StringLiteral : Element
    {
        public StringLiteral(ParseInfo parseInfo) : base(parseInfo) { }

        public string Value { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
