using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class FromStatement : Element
    {
        public FromStatement(ParseInfo parseInfo) : base(parseInfo) {}

        public string Name { get; set; }
        public string Alias { get; set; }

        public InnerJoinStatement InnerJoin
        {
           get { return Children.OfType<InnerJoinStatement>().SingleOrDefault(); }   
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
