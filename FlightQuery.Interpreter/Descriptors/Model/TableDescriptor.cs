﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Interpreter.Descriptors.Model
{
    public class TableDescriptor
    {
        private PropertyDescriptor[] _propertyDescriptor;
        private Dictionary<string, PropertyDescriptor> _hash;

        public TableDescriptor()
        {
            _propertyDescriptor = new PropertyDescriptor[0];
            _hash = new Dictionary<string, PropertyDescriptor>();
        }

        public PropertyDescriptor[] Properties
        {
            get
            {
                return _propertyDescriptor;
            }
            set
            {
                _hash = new Dictionary<string, PropertyDescriptor>();
                _propertyDescriptor = value;
                foreach(var p in _propertyDescriptor)
                    HashInsert(p.Name.ToLower(), p);
            }
        }

        protected virtual void HashInsert(string key, PropertyDescriptor p)
        {
            _hash.Add(key, p);
        }

        public int GetDataRowIndex(string name)
        {
            return Array.IndexOf(_propertyDescriptor, _hash[name.ToLower()]);
        }

        public bool ContainsKey(string name)
        {
            return _hash.ContainsKey(name.ToLower());
        }

        public PropertyDescriptor this[string name]
        {
            get
            {
                return _hash[name.ToLower()];
            }
        }

        public PropertyDescriptor this[int index]
        {
            get
            {
                return _propertyDescriptor[index];
            }
        }

        public IDictionary<int, PropertyDescriptor[]> RequiredProperties
        {
            get
            {
                return _propertyDescriptor.Where(x => x.Required).GroupBy(x => x.RequiredGroup).ToDictionary(x => x.Key, x => x.ToArray());
            }
        }

        public static implicit operator TableDescriptor(PropertyDescriptor[] p)
        {
            var tableDescriptor = new TableDescriptor();
            tableDescriptor.Properties = p;
            return tableDescriptor;
        }

    }
}
