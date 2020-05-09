using System;

namespace FlightQuery.Sdk.Model
{
    public class Base64Image : IValue
    {
        public string Image { get; private set; }

        public Base64Image(string image)
        {
            Image = image;
        }

        public IComparable ToValue()
        {
            return Image;
        }
    }
}
