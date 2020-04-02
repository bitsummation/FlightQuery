using System.Collections.Generic;

namespace FlightQuery.Interpreter
{
    public class VariableContainer<TArgs> where TArgs: IPropertyArgs
    {
        private IList<TArgs> _properties;
        private Dictionary<string, TArgs> _hash;

        public VariableContainer()
        {
            _properties = new List<TArgs>();
            _hash = new Dictionary<string, TArgs>();
        }

        public void Add(TArgs args)
        {
            _properties.Add(args);
            _hash[args.Variable.ToLower()] = args;
        }

        public bool ContainsVariable(string name)
        {
            return _hash.ContainsKey(name.ToLower());
        }

        public TArgs this[string name]
        {
            get
            {
                return _hash[name.ToLower()];
            }
        }

        public void Clear()
        {
            _hash.Clear();
            _properties.Clear();
        }

        public IEnumerable<TArgs> Args { get { return _properties; } }

    }
}
