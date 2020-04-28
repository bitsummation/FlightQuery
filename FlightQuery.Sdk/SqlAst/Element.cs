using System;

namespace FlightQuery.Sdk.SqlAst
{
    public abstract class Element
    {
        protected Element(Cursor parseInfo)
        {
            Children = new ChildCollection(this);
            Cursor = parseInfo;
        }

        public Cursor Cursor { get; private set; }
        public ChildCollection Children { get; private set; }
        public Element Parent { get; set; }

        public virtual WhereStatement ParentWhere
        {
            get
            {
                if(Parent != null)
                {
                    return Parent.ParentWhere;
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
