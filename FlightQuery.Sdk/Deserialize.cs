using Newtonsoft.Json;
using System;

namespace FlightQuery.Sdk
{
    public static class Deserialize
    {
        public static TValue DeserializeObject<TValue>(string json)
        {
            return JsonConvert.DeserializeObject<TValue>(json, new UnixDateTimeConverter());
        }

        private class UnixDateTimeConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(DateTime);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                long unixTimeStamp = (long)reader.Value;
                return Conversion.ConvertLongToDateTime(unixTimeStamp);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
