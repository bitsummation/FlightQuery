using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;

namespace FlightQuery.Interpreter.Http
{
    public class QueryArgs : IPropertyArgs
    {
        public QueryArgs()
        {
            PropertyValue = new PropertyValue(null);
            LeftProperty = false;
        }

        public bool LeftProperty { get; set; }
        public PropertyDescriptor Property { get; set;}
        public string Variable { get; set; }
        public TableBase Table { get; set; }

        public bool HasProperty
        {
            get
            {
                return Property != null;
            }
        }

        public bool HasValue
        {
            get
            {
                return PropertyValue.Value != null;
            }
        }


        public PropertyValue PropertyValue { get; set; }
    }
}
