using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.QueryResults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightQuery.Interpreter.Execution
{
    internal class Scope : IDisposable
    {
        private Stack<ScopeEntry> _variableScope;

        public Scope()
        {
            _variableScope = new Stack<ScopeEntry>();
        }

        public bool IsTableDefinedAnyLevel(string v)
        {
            return TableLookupAnyLevel(v) != null;
        }

        public TableBase TableLookupAnyLevel(string v)
        {
            foreach(var entry in _variableScope.ToArray())
            {
                if (entry.Variables.ContainsKey(v.ToLower()))
                    return entry.Variables[v.ToLower()];
            }

            return null;
        }

        public IDisposable Push()
        {
            _variableScope.Push(new ScopeEntry());
            return this;
        }

        public bool IsTableDefineSameLevel(string v)
        {
            var entry = _variableScope.Peek();
            return entry.Variables.ContainsKey(v.ToLower());
        }

        public TableBase TableLookupSameLevel(string v)
        {
            var entry = _variableScope.Peek();
            if(entry.Variables.ContainsKey(v.ToLower()))
                return entry.Variables[v.ToLower()];

            return null;
        }

        public TableBase[] FetchAllTablesSameLevel()
        {
            return _variableScope.Peek().Variables.Values.ToArray();
        }

        public TableBase[] FetchAllExecutedTablesSameLevel()
        {
            return _variableScope.Peek().Variables.Values.Where(x => x is ExecutedTable).ToArray();
        }

        public TableBase FindTableFromPropertySameLevel(string property)
        {
            foreach(KeyValuePair<string, TableBase> entry in _variableScope.Peek().Variables)
            {
                if (entry.Value.Descriptor.ContainsKey(property.ToLower()))
                    return entry.Value;
            }   

            return null;
        }

        public void AddTable(string v, TableBase table)
        {
            var entry = _variableScope.Peek();
            entry.Variables.Add(v.ToLower(), table);
        }

        public void RemoveTable(string v)
        {
            var entry = _variableScope.Peek();
            entry.Variables.Remove(v.ToLower());
        }

        public void Dispose()
        {
            _variableScope.Pop();
        }
    }
}
