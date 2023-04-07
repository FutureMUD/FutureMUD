using System;
using System.Collections.Generic;
using System.Globalization;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.TimeAndDate;

namespace MudSharp.Framework {
    public static partial class DateUtilities {
        /// <summary>
        ///     Changes a TimeSpan to a "wordy" string representation of the time passed
        /// </summary>
        /// <param name="source">The source timespan</param>
        /// <param name="format">An optional IFormatProvider for number formatting</param>
        /// <returns></returns>
        public static string Describe(this TimeSpan source, IFormatProvider format = null) {
            format ??= CultureInfo.InvariantCulture;
            var sb = new List<string>();

            if (source.TotalSeconds < 0) {
                sb.Add("negative ");
                source = new TimeSpan(source.Ticks*-1);
            }

            if (source.TotalSeconds < 1) {
                return "less than a second";
            }

            if (source.Days > 365) {
                var days = source.Days;
                sb.Add(string.Format(format, "{0:N0} year{1}", days/365, days > 730 ? "s" : ""));
                if (source.Days%365 > 0) {
                    sb.Add($"{source.Days%365:N0} day{(source.Days%365 == 1 ? "" : "s")}");
                }
            }
            else {
                if (source.Days > 0) {
                    sb.Add(string.Format(format, "{0:N0} day{1}", source.Days, source.Days == 1 ? "" : "s"));
                }
            }

            if (source.Hours%24 > 0) {
                sb.Add(string.Format(format, "{0:N0} hour{1}", source.Hours%24, source.Hours%24 == 1 ? "" : "s"));
            }

            if ((sb.Count <= 2) && (source.Minutes%60 > 0)) {
                sb.Add(string.Format(format, "{0:N0} minute{1}", source.Minutes%60, source.Minutes%60 == 1 ? "" : "s"));
            }

            if ((sb.Count <= 2) && (source.Seconds%60 > 0)) {
                sb.Add(string.Format(format, "{0:N0} second{1}", source.Seconds%60, source.Seconds%60 == 1 ? "" : "s"));
            }

            return sb.ListToString();
        }

        public static string Describe(this MudTimeSpan source, IFormatProvider format = null)
        {
            format ??= CultureInfo.InvariantCulture;
            var sb = new List<string>();

            if (source.Milliseconds < 0)
            {
                sb.Add("negative ");
                source = source.Inverse();
            }

            if (source.TotalSeconds < 1)
            {
                return "less than a second";
            }

            if (source.Years > 0)
            {
                sb.Add(string.Format(format, "{0:N0} year{1}", source.Years, source.Years == 1 ? "" : "s"));
            }

            if (source.Months > 0)
            {
                sb.Add(string.Format(format, "{0:N0} month{1}", source.Months, source.Months == 1 ? "" : "s"));
            }

            if (source.Weeks > 0)
            {
                sb.Add(string.Format(format, "{0:N0} week{1}", source.Weeks, source.Weeks == 1 ? "" : "s"));
            }

            if (source.DayComponentOnly > 0)
            {
                sb.Add(string.Format(format, "{0:N0} day{1}", source.DayComponentOnly, source.DayComponentOnly == 1 ? "" : "s"));
            }

            if (source.HourComponentOnly > 0)
            {
                sb.Add(string.Format(format, "{0:N0} hour{1}", source.HourComponentOnly , source.HourComponentOnly == 1 ? "" : "s"));
            }

            if (source.MinuteComponentOnly > 0)
            {
                sb.Add(string.Format(format, "{0:N0} minute{1}", source.MinuteComponentOnly, source.MinuteComponentOnly == 1 ? "" : "s"));
            }

            if (source.SecondComponentOnly > 0)
            {
                sb.Add(string.Format(format, "{0:N0} second{1}", source.SecondComponentOnly, source.SecondComponentOnly == 1 ? "" : "s"));
            }

            return sb.ListToString();
        }

        public static DateTime GetLocalDate(this DateTime date, ICharacter character)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, character.Account.TimeZone);
        }

        public static DateTime GetLocalDate(this DateTime date, IAccount account)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, account.TimeZone);
        }

        public static string GetLocalDateString(this DateTime date, ICharacter character, bool shortForm = false)
        {
            if (date == default(DateTime))
            {
                return "Never";
            }

            if (date.Kind != DateTimeKind.Utc) {
                date = date.ToUniversalTime();
            }

            return date.GetLocalDate(character).ToString(shortForm ? "g" : "G", character);
        }

        public static string GetLocalDateString(this DateTime date, IAccount account, bool shortForm = false)
        {
            if (date == default(DateTime))
            {
                return "Never";
            }
            if (date.Kind != DateTimeKind.Utc)
            {
                date = date.ToUniversalTime();
            }
            return date.GetLocalDate(account).ToString(shortForm ? "g" : "G", account);
        }

        /// <summary>
        /// Returns a date that was the last Monday prior to or on this date
        /// </summary>
        /// <param name="date">The date in question</param>
        /// <returns>The last monday prior to this date, or the date if monday</returns>
        public static DateTime GetStartOfWeek(this DateTime date)
        {
			switch (date.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					return date.Date.AddDays(-6);
				case DayOfWeek.Monday:
					return date.Date;
				case DayOfWeek.Tuesday:
					return date.Date.AddDays(-1);
				case DayOfWeek.Wednesday:
					return date.Date.AddDays(-2);
				case DayOfWeek.Thursday:
					return date.Date.AddDays(-3);
				case DayOfWeek.Friday:
					return date.Date.AddDays(-4);
				case DayOfWeek.Saturday:
					return date.Date.AddDays(-5);
				default:
					throw new ArgumentOutOfRangeException(nameof(date));
			}
		}
	}
}