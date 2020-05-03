using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class QueryStatement : Element
    {
        public QueryStatement(ElementBounds parseInfo) : base(parseInfo) { }

        public SelectStatement Select
        {
            get
            { return Children.OfType<SelectStatement>().SingleOrDefault(); }
        }

        public FromStatement From
        {
            get { return Children.OfType<FromStatement>().SingleOrDefault(); }
        }
        
        public WhereStatement Where
        {
            get { return Children.OfType<WhereStatement>().SingleOrDefault(); }
        }

        public LimitStatement Limit
        {
            get
            {
                return Children.OfType<LimitStatement>().SingleOrDefault();
            }
        }

        public override QueryStatement ParentQueryStatement
        {
            get { return this; }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
