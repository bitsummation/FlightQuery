using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class SelectStatement : Element
    {
        public SelectStatement(ParseInfo parseInfo) : base(parseInfo) { All = false; }

        public bool All { get; set; }

        public VariableExpresion[] Args
        {
            get { return Children.OfType<VariableExpresion>().ToArray(); }
        }

        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
