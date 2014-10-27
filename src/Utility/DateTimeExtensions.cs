// --------------------------------------------------------------------------------
// Copyright (c) 2014, XLR8 Development
// --------------------------------------------------------------------------------
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// --------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace XLR8.Utility
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Number of ticks per millisecond
        /// </summary>

        public const int TICKS_PER_MILLI = 10000;

        /// <summary>
        /// Number of nanoseconds per tick
        /// </summary>

        public const int NANOS_PER_TICK = 100;

        /// <summary>
        /// Converts ticks to milliseconds
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>

        public static long TicksToMillis(long ticks)
        {
            return ticks / TICKS_PER_MILLI;
        }

        /// <summary>
        /// Converts ticks to nanoseconds
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>

        public static long TicksToNanos(long ticks)
        {
            return ticks * NANOS_PER_TICK;
        }

        /// <summary>
        /// Converts milliseconds to ticks
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>

        public static long MillisToTicks(long millis)
        {
            return millis * TICKS_PER_MILLI;
        }

        /// <summary>
        /// Nanoses to ticks.
        /// </summary>
        /// <param name="nanos">The nanos.</param>
        public static long NanosToTicks(long nanos)
        {
            return nanos / NANOS_PER_TICK;
        }

        /// <summary>
        /// Converts milliseconds to DateTime 
        /// </summary>
        /// <param name="millis">The millis.</param>
        /// <returns></returns>
        public static DateTime MillisToDateTime(long millis)
        {
            return new DateTime(MillisToTicks(millis));
        }

        /// <summary>
        /// Gets the number of nanoseconds needed to represent
        /// the datetime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long TimeInNanos(DateTime dateTime)
        {
            return TicksToNanos(dateTime.Ticks);
        }

        /// <summary>
        /// Gets the number of nanoseconds.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long InNanos(this DateTime dateTime)
        {
            return TicksToNanos(dateTime.Ticks);
        }

        /// <summary>
        /// Gets the number of milliseconds needed to represent
        /// the datetime.  This is needed to convert from Java
        /// datetime granularity (milliseconds) to CLR datetimes.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>

        public static long TimeInMillis(this DateTime dateTime)
        {
            return TicksToMillis(dateTime.Ticks);
        }

        /// <summary>
        /// Gets the number of milliseconds needed to represent
        /// the datetime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long InMillis(this DateTime dateTime)
        {
            return TicksToMillis(dateTime.Ticks);
        }

        /// <summary>
        /// Gets the datetime that matches the number of milliseconds provided.
        /// As with TimeInMillis, this is needed to convert from Java datetime
        /// granularity to CLR granularity.
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>

        public static DateTime TimeFromMillis(this long millis)
        {
            return new DateTime(MillisToTicks(millis));
        }

        public static DateTime GetCurrentTimeUniversal()
        {
            return DateTime.UtcNow;
        }

        public static DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Returns the current time in millis
        /// </summary>

        public static long GetCurrentTimeMillis()
        {
            return DateTime.Now.Ticks / TICKS_PER_MILLI;
        }

        /// <summary>
        /// Returns the current time in millis
        /// </summary>

        public static long CurrentTimeMillis
        {
            get { return TimeInMillis(DateTime.Now); }
        }

        /// <summary>
        /// Gets the current time in nanoseconds.
        /// </summary>
        /// <value>The current time nanos.</value>
        public static long CurrentTimeNanos
        {
            get { return TimeInNanos(DateTime.Now); }
        }

        public static DateTime ToLocalTime(DateTime source)
        {
            return source.ToLocalTime();
        }

        public static DateTime ToUniversalTime(DateTime source)
        {
            return source.ToUniversalTime();
        }

        public static DateTime ToTimeZone(DateTime source, string targetTimeZone)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(source, targetTimeZone);
        }

        public static DateTime ToTimeZone(DateTime source, string sourceTimeZone, string targetTimeZone)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(source, sourceTimeZone, targetTimeZone);
        }

        private static readonly Calendar Calendar = DateTimeFormatInfo.CurrentInfo.Calendar;

        public static int GetWeekOfYear(this DateTime dateTime)
        {
            return Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static DateTime GetWithMaximumDay(this DateTime dateTime)
        {
            var daysInMonth = Calendar.GetDaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTime(dateTime.Year, dateTime.Month, daysInMonth, dateTime.Hour, dateTime.Minute,
                                dateTime.Second, dateTime.Millisecond);
        }

        public static DateTime GetWithMaximumMonth(this DateTime dateTime)
        {
            var daysInMonth = Calendar.GetDaysInMonth(dateTime.Year, 12);
            if (dateTime.Day < daysInMonth)
                daysInMonth = dateTime.Day;

            return new DateTime(dateTime.Year, 12, daysInMonth, dateTime.Hour, dateTime.Minute,
                                dateTime.Second, dateTime.Millisecond);
        }

        public static DateTime MoveToWeek(this DateTime dateTime, int targetWeek)
        {
            if ((targetWeek < 1) || (targetWeek > 52))
                throw new ArgumentException("invalid target week", "targetWeek");

            var week = GetWeekOfYear(dateTime);
            if (week == targetWeek)
                return dateTime;
            for (; week > targetWeek; week = GetWeekOfYear(dateTime))
                dateTime = dateTime.AddDays(-7);
            for (; week < targetWeek; week = GetWeekOfYear(dateTime))
                dateTime = dateTime.AddDays(7);

            return dateTime;
        }

        public static DateTime GetWithMaximumWeek(this DateTime dateTime)
        {
            do
            {
                var nextTime = dateTime.AddDays(7);
                if ((GetWeekOfYear(dateTime) > 2) && (GetWeekOfYear(nextTime) <= 2))
                {
                    return dateTime;
                }

                dateTime = nextTime;
            } while (true);
        }

        public static DateTime GetWithMinimumWeek(this DateTime dateTime)
        {
            var week = GetWeekOfYear(dateTime);
            if (week == 1)
            {
                return dateTime;
            }

            do
            {
                var nextTime = dateTime.AddDays(-7);
                if (GetWeekOfYear(dateTime) == 2)
                {
                    // See if this day in the previous week is still in week 1.  It's
                    // possible that a week started with a day like Friday and that the
                    // date in question was a Thursday.  Technically, Thursday would
                    // have begun on week 2 not 1.
                    if (GetWeekOfYear(nextTime) == 1)
                        return nextTime;
                    // First occurrence of this date occurred on week 2
                    return dateTime;
                }

                dateTime = nextTime;
            } while (true);
        }

        public static DateTime ParseDefaultDate(string dateTimeString)
        {
            return ParseDefault(dateTimeString);
        }

        public static DateTime ParseDefault(string dateTimeString)
        {
            DateTime dateTime;

            var match = Regex.Match(dateTimeString, @"(\d+)-(\d+)-(\d+)T(\d+):(\d+):(\d+)\.(\d+)");
            if (match != Match.Empty)
            {
                dateTimeString = string.Format(
                    "{0}-{1}-{2} {3}:{4}:{5}.{6}",
                    int.Parse(match.Groups[1].Value).ToString(CultureInfo.InvariantCulture).PadLeft(4, '0'),
                    int.Parse(match.Groups[2].Value).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    int.Parse(match.Groups[3].Value).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    int.Parse(match.Groups[4].Value).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    int.Parse(match.Groups[5].Value).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    int.Parse(match.Groups[6].Value).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    match.Groups[7].Value);
            }

            if ((DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd hh:mm:ss.fff", null, DateTimeStyles.None, out dateTime)) ||
                (DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd hh:mm:ss.ff", null, DateTimeStyles.None, out dateTime)))
                return dateTime;

            // there is an odd situation where we intend to parse down to milliseconds but someone passes a four digit value
            // - in this case, Java interprets this as a millisecond value but the CLR will interpret this as a tenth of a
            // - millisecond value.  to be consistent, I've made our implementation behave in a fashion similar to the java
            // - implementation.

            if (DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd hh:mm:ss.ffff", null, DateTimeStyles.None, out dateTime))
            {
                var millis = (dateTime.Ticks % 10000000) / 1000;
                dateTime = dateTime.AddMilliseconds(-millis / 10).AddMilliseconds(millis);
                return dateTime;
            }

            return DateTime.Parse(dateTimeString);
        }
    }
}
