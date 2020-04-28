using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class ProgramElement : Element
    {
        public ProgramElement(Cursor parseInfo) : base(parseInfo) { }

        public Element[] Statements
        {
            get { return Children.ToArray(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
