using System;
using System.Collections.Generic;
using System.Globalization;

namespace SoftwareMind.Utils.Extensions.DateTimeExtensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ExtractDatePart(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static bool IsInRange(this DateTime date, DateRange range)
        {
            DateTime dateConverted = date.ExtractDatePart();
            return range.From <= dateConverted && dateConverted <= range.To;
        }

        public static bool IsBeforeRange(this DateTime date, DateRange range)
        {
            DateTime dateConverted = ExtractDatePart(date);
            return dateConverted < range.From;
        }

        public static bool IsAfterRange(this DateTime date, DateRange range)
        {
            DateTime dateConverted = ExtractDatePart(date);
            return dateConverted > range.To;
        }

        /// <summary>
        /// Return the date that is the start of the week relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfWeek(this DateTime date)
        {
            DayOfWeek day = date.DayOfWeek;
            int days = day - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            if(date.DayOfWeek == DayOfWeek.Sunday && CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Monday)
                days = 6;
            DateTime start = date.AddDays(-days);
            return start.Date;
        }

        /// <summary>
        /// Return the date that is the start of the week relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfLastWeek(this DateTime date)
        {
            return date.GetStartOfWeek().AddDays(-7);
        }

        /// <summary>
        /// Return the date that is the end of the week relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfWeek(this DateTime date)
        {
            return date.GetStartOfWeek().AddDays(6);
        }

        /// <summary>
        /// Return the date that is the end of the week relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfLastWeek(this DateTime date)
        {
            return date.GetEndOfWeek().AddDays(-7);
        }

        /// <summary>
        /// Return the date that is the start of the month relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Return the date that is the start of previous month relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfLastMonth(this DateTime date)
        {
            return date.GetStartOfMonth().AddMonths(-1);
        }

        /// <summary>
        /// Return the date that is the end of the month relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.GetDaysInMonth(), 23, 59, 59, 999);
        }

        public static DateTime LastMonthDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.GetDaysInMonth());
        }

        /// <summary>
        /// Return the date that is the start of previous month relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfLastMonth(this DateTime date)
        {
            return date.GetStartOfLastMonth().GetEndOfMonth();
        }

        /// <summary>
        /// Returns the number of days in the month of the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetDaysInMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }

        /// <summary>
        /// Return the first day of the year relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        /// <summary>
        /// Return the first day of the last year relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfLastYear(this DateTime date)
        {
            return new DateTime(date.Year - 1, 1, 1);
        }

        /// <summary>
        /// Return the last day of the year relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 12, 31, 23, 59, 59, 999);
        }

        /// <summary>
        /// Return the last day of the last year relative to the specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfLastYear(this DateTime date)
        {
            return new DateTime(date.Year - 1, 12, 31, 23, 59, 59, 999);
        }

        /// <summary>
        /// Examine the value of the DateTime, if the value is equal to DateTime.MinValue
        /// then the result is null, otherwise the supplied value is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? ToNullableDateTime(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        public static string[] GetDateFormats()
        {
            return new[] { "dd-MM-yyyy", "d-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "yyyy-MM-dd", "yyyy-MM-d", "yyyy-M-d", "yyyy-M-dd" };
        }
    }

    public class DateRange : IEquatable<DateRange>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public DateRange(DateTime from, DateTime to)
        {
            if (to == DateTime.MinValue) to = DateTime.MaxValue;
            From = from.ExtractDatePart();
            To = to.ExtractDatePart();
        }

        public bool HasIntersection(DateRange range)
        {
            if (this.From <= range.From)
            {
                return this.To >= range.From;
            }
            else
            {
                return this.From <= range.To;
            }
        }

        public override string ToString()
        {
            string dateFormat = "dd-MM-yyyy";
            List<string> result = new List<string>();
            if (this.From > DateTime.MinValue)
            {
                result.Add(String.Format("od {0}", this.From.ToString(dateFormat)));
            }
            if (this.To < DateTime.MaxValue)
            {
                result.Add(String.Format("do {0}", this.To.ToString(dateFormat)));
            }
            if (result.Count == 0)
            {
                result.Add("cały czas");
            }
            return String.Join(" ", result.ToArray());
        }

        #region IEquatable<DateRange> Members

        public bool Equals(DateRange other)
        {
            return this.From == other.From && this.To == other.To;
        }

        #endregion
    }
}
