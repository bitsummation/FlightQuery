using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class QueryStatement : Element
    {
        public QueryStatement(ParseInfo parseInfo) : base(parseInfo) { }

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

        public override WhereStatement ParentWhere
        {
            get { return Where; }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
