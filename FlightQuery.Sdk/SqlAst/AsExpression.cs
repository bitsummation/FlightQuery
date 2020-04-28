using System;

namespace FlightQuery.Sdk.SqlAst
{
    public class AsExpression : Element
    {
        public AsExpression(Cursor parseInfo) : base(parseInfo) { }

        public string Alias { get; set; }

        public override void Accept(IElementVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
