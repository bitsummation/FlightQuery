using System.Collections.ObjectModel;
using System.Linq;

namespace FlightQuery.Sdk.Semantic
{
    public class ErrorsCollection : ObservableCollection<ErrorBase>
    {
        protected override void InsertItem(int index, ErrorBase item)
        {
            if(!Items.Contains(item))
                base.InsertItem(index, item);
        }
    }
}
