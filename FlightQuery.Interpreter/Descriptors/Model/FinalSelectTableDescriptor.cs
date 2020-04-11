namespace FlightQuery.Interpreter.Descriptors.Model
{
    public class FinalSelectTableDescriptor : TableDescriptor
    {
        public static implicit operator FinalSelectTableDescriptor(PropertyDescriptor[] p)
        {
            var tableDescriptor = new FinalSelectTableDescriptor();
            tableDescriptor.Properties = p;
            return tableDescriptor;
        }

        protected override void HashInsert(string key, PropertyDescriptor p)
        {   
            //don't add to internal has for properties as there might be dup names
        }
    }
}
