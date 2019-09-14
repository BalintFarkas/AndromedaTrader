using System;
using System.Configuration;

namespace Andromeda.Common
{
    public static class TimeGetter
    {
        /// <summary>
        /// Returns the current local time at the home location of the Andromeda Trader competition,
        /// irrespective of the server's time zone.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLocalTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["LocalTimeZone"]));
        }
    }
}