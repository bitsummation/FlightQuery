using FlightQuery.Sdk.Model;
using System;

namespace FlightQuery.Interpreter.QueryResults
{
    public class PropertyValue
    {
        private IComparable _comparable;

        public object Value
        {
            get
            {
                return _comparable;
            }
        }

        public PropertyValue(object comparable)
        {
            _comparable = comparable as IComparable;
            if (_comparable == null && comparable is IValue)
            {
                _comparable = (comparable as IValue).ToValue();
            }
        }

        public override bool Equals(object obj)
        {
            return _comparable.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _comparable.GetHashCode();
        }

        public static PropertyValue operator -(PropertyValue p, PropertyValue c)
        {
            if (p._comparable is DateTime && c._comparable is DateTime)
                return new PropertyValue(((DateTime)p._comparable) - ((DateTime)c._comparable));

            return null;
        }

        public static bool operator ==(PropertyValue p, PropertyValue c)
        {
            if(p._comparable is string && c._comparable is string)
            {
                return string.Compare(((string)p._comparable).ToLower(), ((string)c._comparable).ToLower(), true) == 0;
            }

            return p._comparable.CompareTo(c._comparable) == 0;
        }

        public static bool operator !=(PropertyValue p, PropertyValue c)
        {
            if (p._comparable is string && c._comparable is string)
            {
                return string.Compare(((string)p._comparable).ToLower(), ((string)c._comparable).ToLower(), true) != 0;
            }

            return p._comparable.CompareTo(c._comparable) != 0;
        }

        public static bool operator >(PropertyValue p, PropertyValue c)
        {
            if (p._comparable is string && c._comparable is string)
            {
                return string.Compare(((string)p._comparable).ToLower(), ((string)c._comparable).ToLower(), true) > 0;
            }

            return p._comparable.CompareTo(c._comparable) > 0;
        }

        public static bool operator <(PropertyValue p, PropertyValue c)
        {
            if (p._comparable is string && c._comparable is string)
            {
                return string.Compare(((string)p._comparable).ToLower(), ((string)c._comparable).ToLower(), true) < 0;
            }

            return p._comparable.CompareTo(c._comparable) < 0;
        }

        public static bool operator <=(PropertyValue p, PropertyValue c)
        {
            if (p._comparable is string && c._comparable is string)
            {
                return string.Compare(((string)p._comparable).ToLower(), ((string)c._comparable).ToLower(), true) <= 0;
            }

            return p._comparable.CompareTo(c._comparable) <= 0;
        }
        public static bool operator >=(PropertyValue p, PropertyValue c)
        {
            if (p._comparable is string && c._comparable is string)
            {
                return string.Compare(((string)p._comparable).ToLower(), ((string)c._comparable).ToLower(), true) >= 0;
            }

            return p._comparable.CompareTo(c._comparable) >= 0;
        }
    }
}
