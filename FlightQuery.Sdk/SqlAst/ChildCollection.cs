using System.Collections.ObjectModel;

namespace FlightQuery.Sdk.SqlAst
{
    public class ChildCollection : ObservableCollection<Element>
    {
        private Element _parent;

        public ChildCollection(Element parent)
        {
            _parent = parent;
        }

        protected override void InsertItem(int index, Element item)
        {
            if (item != null)
            {
                item.Parent = _parent;
                base.InsertItem(index, item);
            }
        }
    }
}
