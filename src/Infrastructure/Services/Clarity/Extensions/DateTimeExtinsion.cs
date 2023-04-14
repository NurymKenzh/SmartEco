using System;
using System.Collections.Generic;
using System.Text;

namespace Clarity.Extensions
{
    public static class DateTimeExtinsion
    {
        public static DateTime Trim(this DateTime date, long roundTicks)
            => new DateTime(date.Ticks - date.Ticks % roundTicks, date.Kind);
    }
}
