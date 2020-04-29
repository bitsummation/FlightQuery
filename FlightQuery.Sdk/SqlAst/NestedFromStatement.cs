using System;
using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class NestedFromStatement : FromStatement
    {
        public NestedFromStatement(ElementBounds parseInfo) : base(parseInfo) { }

        public QueryStatement Query
        {
            get { return Children.OfType<QueryStatement>().SingleOrDefault(); }
        }

        public override bool IsNestedQuery
        {
            get
            {
                return true;
            }
        }
        
        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
