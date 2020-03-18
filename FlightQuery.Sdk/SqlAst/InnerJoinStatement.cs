using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class InnerJoinStatement : Element
    {
        public InnerJoinStatement(ParseInfo parseInfo) : base(parseInfo) { }

        public string Name { get; set; }
        public string Alias { get; set; }

        public InnerJoinStatement InnerJoin
        {
            get { return Children.OfType<InnerJoinStatement>().SingleOrDefault(); }
        }

        public BooleanExpression BooleanExpression
        {
            get { return Children.OfType<BooleanExpression>().SingleOrDefault(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
