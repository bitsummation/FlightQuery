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
        }

        public override bool Equals(object obj)
        {
            return _comparable.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _comparable.GetHashCode();
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
