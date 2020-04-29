using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class SelectStatement : Element
    {
        public SelectStatement(ElementBounds parseInfo) : base(parseInfo) { All = false; }

        public bool All { get; set; }

        public SelectArgExpression[] Args
        {
            get { return Children.OfType<SelectArgExpression>().ToArray(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
