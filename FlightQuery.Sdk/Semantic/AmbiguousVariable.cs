using FlightQuery.Sdk.SqlAst;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightQuery.Sdk.Semantic
{
    public class AmbiguousVariable : SemanticLineError
    {
        public AmbiguousVariable(string variable, ParseInfo parseInfo) : base(parseInfo)
        {
            Variable = variable;
        }

        public string Variable { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("{0} is ambiguous {1}", Variable, base.Message);
            }
        }
    }
}
