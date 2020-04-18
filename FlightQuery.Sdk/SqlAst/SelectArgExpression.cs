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

        public Element Statement
        {
            get { return Children.Where(x => !typeof(AsExpression).IsInstanceOfType(x)).Single(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
