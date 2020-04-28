namespace FlightQuery.Sdk.SqlAst
{
    public class StringLiteral : Element
    {
        public StringLiteral(Cursor parseInfo) : base(parseInfo) { }

        public string Value { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
