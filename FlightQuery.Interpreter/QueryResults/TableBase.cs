using FlightQuery.Interpreter.Descriptors.Model;
using FlightQuery.Interpreter.Http;
using FlightQuery.Sdk.Semantic;
using System.Collections.Generic;

namespace FlightQuery.Interpreter.QueryResults
{
    public abstract class TableBase
    {
        protected TableBase()
        {
            Errors = new List<SemanticError>();
        }

        public IList<SemanticError> Errors { get; private set; }

        public TableDescriptor Descriptor { get; set; }

        public int SelectIndex { get; set; }
        public int RowIndex { get; set; }
        public Row[] Rows { get; set; }

        public abstract TableBase Create();

        public abstract bool HasExecuted { get; }
        public abstract void AddArg(QueryArgs args);
        public abstract ExecutedTable Execute();

        protected static PropertyValue[] ToValues(object value, TableDescriptor table)
        {
            var values = new List<PropertyValue>();
            foreach (var p in table.Properties)
            {
                var prop = value.GetType().GetProperty(p.Name);
                values.Add(
                    new PropertyValue(prop.GetValue(value))
                    );
            }

            return values.ToArray();
        }
    }
}
