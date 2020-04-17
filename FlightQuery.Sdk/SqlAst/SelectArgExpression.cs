using System;
using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class SelectArgExpression : Element
    {
        public SelectArgExpression(ParseInfo parseInfo) : base(parseInfo) { }

        public AsExpression As
        {
            get { return Children.OfType<AsExpression>().SingleOrDefault(); }
        }

        public VariableExpresion Variable
        {
            get { return Children.OfType<VariableExpresion>().SingleOrDefault(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
