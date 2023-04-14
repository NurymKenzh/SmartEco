using System;
using System.Collections.Generic;
using System.Text;

namespace Clarity.Helpers
{
    public static class TimeZoneConverter
    {
        public static DateTime ToCentralAsia (DateTime dateTimeBasic)
        {
            var centralAsiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time");
            DateTimeOffset dateTimeOffset = TimeZoneInfo.ConvertTime(dateTimeBasic, centralAsiaTimeZone);
            return dateTimeOffset.DateTime;
        }

        public static DateTime ToUtc(DateTime dateTimeBasic)
        {
            var dateTime = new DateTimeOffset(dateTimeBasic, new TimeSpan(6, 0, 0));
            return dateTime.UtcDateTime;
        }
    }
}
