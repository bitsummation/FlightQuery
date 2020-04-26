﻿using System;
using System.Linq;

namespace FlightQuery.Sdk.SqlAst
{
    public class NestedFromStatement : FromStatement
    {
        public NestedFromStatement(ParseInfo parseInfo) : base(parseInfo) { }

        public QueryStatement Query
        {
            get { return Children.OfType<QueryStatement>().SingleOrDefault(); }
        }

        public override bool IsNestedSelect
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
