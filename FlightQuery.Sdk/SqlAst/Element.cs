using System;

namespace FlightQuery.Sdk.SqlAst
{
    public abstract class Element
    {
        protected Element(ElementBounds parseInfo)
        {
            Children = new ChildCollection(this);
            Bounds = parseInfo;
        }

        public ElementBounds Bounds { get; private set; }
        public ChildCollection Children { get; private set; }
        public Element Parent { get; set; }

        public virtual QueryStatement ParentQueryStatement
        {
            get
            {
                if(Parent != null)
                {
                    return Parent.ParentQueryStatement;
                }

                return null;
            }
        }

        public virtual bool IsNestedQuery
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.IsNestedQuery;
                }
                return false;
            }
        }

        public abstract void Accept(IElementVisitor visitor);
    }
}
