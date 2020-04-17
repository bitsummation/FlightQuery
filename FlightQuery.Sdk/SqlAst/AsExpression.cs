using System;

namespace FlightQuery.Sdk.SqlAst
{
    public class AsExpression : Element
    {
        public AsExpression(ParseInfo parseInfo) : base(parseInfo) { }

        public string Alias { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
