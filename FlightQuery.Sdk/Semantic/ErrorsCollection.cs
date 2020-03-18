using System.Collections.ObjectModel;
using System.Linq;

namespace FlightQuery.Sdk.Semantic
{
    public class ErrorsCollection : ObservableCollection<SemanticError>
    {
        protected override void InsertItem(int index, SemanticError item)
        {
            if(!Items.Contains(item))
                base.InsertItem(index, item);
        }
    }
}
