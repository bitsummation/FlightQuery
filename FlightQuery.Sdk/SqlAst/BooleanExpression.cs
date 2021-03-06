﻿namespace FlightQuery.Sdk.SqlAst
{
    public abstract class BooleanExpression : Element
    {
        protected BooleanExpression(ElementBounds parseInfo) : base(parseInfo) { }

        public Element Left { get;  set; }
        public Element Right { get; set; }
    }
}
