using System;

namespace FlightQuery.Sdk.SqlAst
{
    public abstract class Element
    {
        protected Element(ParseInfo parseInfo)
        {
            Children = new ChildCollection(this);
            ParseInfo = parseInfo;
        }

        public ParseInfo ParseInfo { get; private set; }
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

        public virtual bool IsNestedSelect
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.IsNestedSelect;
                }
                return false;
            }
        }

        public abstract void Accept(IElementVisitor visitor);
    }
}
