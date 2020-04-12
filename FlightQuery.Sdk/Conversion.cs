using System;
using System.Collections.Generic;
using System.Globalization;

namespace FlightQuery.Sdk
{
    public class Conversion
    {
        public static long MaxUnixDate = int.MaxValue - 1;
        public static long MinUnixDate = 1;

        public static Dictionary<string, Func<object, object>> Map = new Dictionary<string, Func<object, object>>
        {
            {"String-DateTime", ConvertStringToDateTime},
            {"DateTime-String", ConvertDateTimeToString},

            {"DateTime-Int64", ConvertDateTimeToLong},
            {"Int64-DateTime", ConvertLongToDateTime}
        };

        public static object NoOp (object f)
        {
            return f;
        }

        public static object ConvertDateTimeToString(object f)
        {
            var date = (DateTime)f;
            return date.ToString(Json.PrintDateTimeFormat);
        }

        public static object ConvertLongToDateTime(object f)
        {
            long unixTime = (long)f;
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static object ConvertDateTimeToLong(object f)
        {
            var date = (DateTime)f;
            var offset = new DateTimeOffset(date);
            return offset.ToUnixTimeSeconds();
        }

        // 1973-12-30
        // YYYY-MM-DD
        // YYYY-MM-DD HH:MM:SS
        public static object ConvertStringToDateTime(object f)
        {
            var s = f as string;
            DateTime date;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }
            
            /*if (DateTime.TryParseExact(s, "YYYY-MM-DD HH:MM:SS", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }*/

            return null;
        }

    }
}
