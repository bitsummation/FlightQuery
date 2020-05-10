using Newtonsoft.Json;
using System;

namespace FlightQuery.Sdk
{
    public static class Json
    {
        public static TValue DeserializeObject<TValue>(string json)
        {
            return JsonConvert.DeserializeObject<TValue>(json, new UnixDateTimeConverter());
        }

        public static string SerializeObject(object val)
        {
            return JsonConvert.SerializeObject(val, new UnixDateTimeConverter());
        }

        public class UnixDateTimeConverter : JsonConverter
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
                var val = (DateTime)value;
                writer.WriteValue((string)Conversion.ConvertDateTimeToString(val));
            }
        }
    }
}
