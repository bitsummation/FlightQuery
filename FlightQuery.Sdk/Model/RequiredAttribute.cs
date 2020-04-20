using System;

namespace FlightQuery.Sdk.Model
{
    public class RequiredAttribute : Attribute
    {
        public RequiredAttribute(int group)
        {
            Group = group;
        }

        public int Group { get; private set; }
    }
}
