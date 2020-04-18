using FlightQuery.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlightQuery.Interpreter.Descriptors.Model
{
    public class PropertyDescriptor
    {
        public PropertyDescriptor()
        {
        }

        public string Name { get; set; }
        public Type Type { get; set; }
        public bool Queryable { get; set; }
        public bool Required { get; set; }

        public static PropertyDescriptor[] GenerateQueryDescriptor(Type dto)
        {
            var descriptors = new List<PropertyDescriptor>();

            var properties = dto.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                var descriptor = new PropertyDescriptor() { Queryable = false, Required = false };
                var attributes = p.GetCustomAttributes();
                var queryAttribute = attributes.SingleOrDefault(x => x is QueryableAttribute) as QueryableAttribute;
                var requiredAttribute = attributes.SingleOrDefault(x => x is RequiredAttribute) as RequiredAttribute;
                if (queryAttribute != null)
                    descriptor.Queryable = true;
                if (requiredAttribute != null)
                    descriptor.Required = true;

                descriptor.Type = p.PropertyType;
                descriptor.Name = p.Name;

                descriptors.Add(descriptor);
            }

            return descriptors.ToArray();
        }

        public static PropertyDescriptor[] GenerateRunDescriptor(Type dto)
        {
            var descriptors = new List<PropertyDescriptor>();

            var properties = dto.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                var descriptor = new PropertyDescriptor() { Queryable = true, Required = false };

                descriptor.Type = p.PropertyType;
                descriptor.Name = p.Name;

                descriptors.Add(descriptor);
            }

            return descriptors.ToArray();

        }
    }
}
