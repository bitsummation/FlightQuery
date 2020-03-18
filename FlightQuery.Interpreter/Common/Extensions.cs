using NodaTime;
using NodaTime.Extensions;
using System;

namespace FlightQuery.Interpreter.Common
{
    public static class Extensions
    {
        public static DateTime ToLocal(this DateTime dateTime, string timezone)
        {
            if (dateTime.Kind == DateTimeKind.Local)
                throw new ArgumentException("Expected non-local kind of DateTime");

            var zone = DateTimeZoneProviders.Tzdb[timezone];
            Instant instant = dateTime.ToInstant();
            ZonedDateTime inZone = instant.InZone(zone);
            DateTime unspecified = inZone.ToDateTimeUnspecified();

            return unspecified;
        }

        public static DateTime ToUtc(this DateTime dateTime, string timezone)
        {
            if (dateTime.Kind == DateTimeKind.Local)
                throw new ArgumentException("Expected non-local kind of DateTime");

            var zone = DateTimeZoneProviders.Tzdb[timezone];
            LocalDateTime asLocal = dateTime.ToLocalDateTime();
            ZonedDateTime asZoned = asLocal.InZoneLeniently(zone);
            
            Instant instant = asZoned.ToInstant();
            ZonedDateTime asZonedInUtc = instant.InUtc();
            DateTime utc = asZonedInUtc.ToDateTimeUtc();
            
            return utc;
        }
    }
}
