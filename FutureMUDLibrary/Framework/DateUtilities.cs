using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.TimeAndDate;

namespace MudSharp.Framework;

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

	public static string DescribePrecise(this TimeSpan source, IFormatProvider format = null)
	{
		format ??= CultureInfo.InvariantCulture;
		var sb = new List<string>();

		if (source.TotalSeconds < 0)
		{
			sb.Add("negative ");
			source = new TimeSpan(source.Ticks * -1);
		}

		if (source.TotalSeconds < 1)
		{
			if (source.TotalMilliseconds < 1)
			{
				if (source.TotalMicroseconds < 1)
				{
					return string.Format(format, "{0:N0} nanosecond{1}", source.TotalNanoseconds, source.Nanoseconds == 1 ? "" : "s");
				}

				return string.Format(format, "{0:N0} microsecond{1}", source.TotalMicroseconds, source.Microseconds == 1 ? "" : "s");
			}

			return string.Format(format, "{0:N0} millisecond{1}", source.TotalMilliseconds, source.Milliseconds == 1 ? "" : "s");
		}

		if (source.Days > 365)
		{
			var days = source.Days;
			sb.Add(string.Format(format, "{0:N0} year{1}", days / 365, days > 730 ? "s" : ""));
			if (source.Days % 365 > 0)
			{
				sb.Add($"{source.Days % 365:N0} day{(source.Days % 365 == 1 ? "" : "s")}");
			}
		}
		else
		{
			if (source.Days > 0)
			{
				sb.Add(string.Format(format, "{0:N0} day{1}", source.Days, source.Days == 1 ? "" : "s"));
			}
		}

		if (source.Hours % 24 > 0)
		{
			sb.Add(string.Format(format, "{0:N0} hour{1}", source.Hours % 24, source.Hours % 24 == 1 ? "" : "s"));
		}

		if ((sb.Count <= 2) && (source.Minutes % 60 > 0))
		{
			sb.Add(string.Format(format, "{0:N0} minute{1}", source.Minutes % 60, source.Minutes % 60 == 1 ? "" : "s"));
		}

		if ((sb.Count <= 2) && (source.Seconds % 60 > 0))
		{
			sb.Add(string.Format(format, "{0:N0} second{1}", source.Seconds % 60, source.Seconds % 60 == 1 ? "" : "s"));
		}

		return sb.ListToString();
	}

	public static string DescribePreciseBrief(this TimeSpan source, IFormatProvider format = null)
	{
		format ??= CultureInfo.InvariantCulture;
		var sb = new List<string>();

		if (source.TotalSeconds < 0)
		{
			sb.Add("-");
			source = new TimeSpan(source.Ticks * -1);
		}

		if (source.TotalSeconds < 1)
		{
			if (source.TotalMilliseconds < 1)
			{
				if (source.TotalMicroseconds < 1)
				{
					return string.Format(format, "{0:N0}ns", source.TotalNanoseconds);
				}

				return string.Format(format, "{0:N0}us", source.TotalMicroseconds);
			}

			return string.Format(format, "{0:N0}ms", source.TotalMilliseconds);
		}

		if (source.Days > 365)
		{
			var days = source.Days;
			sb.Add(string.Format(format, "{0:N0}y", days / 365));
			if (source.Days % 365 > 0)
			{
				sb.Add($"{source.Days % 365:N0}d");
			}
		}
		else
		{
			if (source.Days > 0)
			{
				sb.Add(string.Format(format, "{0:N0}d", source.Days));
			}
		}

		if (source.Hours % 24 > 0)
		{
			sb.Add(string.Format(format, "{0:N0}h", source.Hours % 24));
		}

		if ((sb.Count <= 2) && (source.Minutes % 60 > 0))
		{
			sb.Add(string.Format(format, "{0:N0}m", source.Minutes % 60));
		}

		if ((sb.Count <= 2) && (source.Seconds % 60 > 0))
		{
			sb.Add(string.Format(format, "{0:N0}s", source.Seconds % 60));
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

	/// <summary>
	/// Determines the date ordering for the given CultureInfo.
	/// Returns:
	/// - 1 if Year comes first (e.g., yyyy/MM/dd)
	/// - 2 if Month comes first (e.g., MM/dd/yyyy)
	/// - 3 if Day comes first (e.g., dd/MM/yyyy)
	/// </summary>
	public enum DateOrder
	{
		YearFirst = 1,
		MonthFirst,
		DayFirst
	}

	/// <summary>
	/// Determines the date ordering for the given CultureInfo based on MonthDayPattern, YearMonthPattern, and ShortDatePattern.
	/// Returns:
	/// - DateOrder.MonthFirst if the month appears before the day.
	/// - DateOrder.DayFirst if the day appears before the month.
	/// </summary>

	public static DateOrder GetDateOrder(CultureInfo culture)
	{
		if (culture == null)
			throw new ArgumentNullException(nameof(culture));

		// Attempt to determine order using MonthDayPattern
		string monthDayPattern = culture.DateTimeFormat.MonthDayPattern;
		var mdOrder = DateOrder.DayFirst;
		if (!string.IsNullOrWhiteSpace(monthDayPattern))
		{
			var order = DetermineMonthDayOrder(monthDayPattern);
			if (order.HasValue)
				mdOrder = order.Value;
		}

		// Attempt to determine order using YearMonthPattern
		string yearMonthPattern = culture.DateTimeFormat.YearMonthPattern;
		var ymOrder = DateOrder.DayFirst;
		if (!string.IsNullOrWhiteSpace(yearMonthPattern))
		{
			var order = DetermineMonthDayOrder(yearMonthPattern);
			if (order.HasValue)
				ymOrder = order.Value;
		}

		switch ((mdOrder, ymOrder))
		{
			// Nonsense values
			case (DateOrder.YearFirst, DateOrder.YearFirst):
			case (DateOrder.YearFirst, DateOrder.MonthFirst):
			case (DateOrder.YearFirst, DateOrder.DayFirst):
			case (DateOrder.MonthFirst, DateOrder.DayFirst):
			case (DateOrder.DayFirst, DateOrder.DayFirst):
			case (DateOrder.DayFirst, DateOrder.YearFirst):
				break;
			case (DateOrder.MonthFirst, DateOrder.YearFirst):
				return DateOrder.YearFirst;
			case (DateOrder.MonthFirst, DateOrder.MonthFirst):
				return DateOrder.MonthFirst;
			case (DateOrder.DayFirst, DateOrder.MonthFirst):
				return DateOrder.DayFirst;
		}

		// Fallback to ShortDatePattern
		string shortDatePattern = culture.DateTimeFormat.ShortDatePattern;
		if (!string.IsNullOrWhiteSpace(shortDatePattern))
		{
			var order = DetermineMonthDayOrder(shortDatePattern);
			if (order.HasValue)
				return order.Value;
		}

		throw new InvalidOperationException("Unable to determine the date order for the provided culture.");
	}

	/// <summary>
	/// Determines the order of month and day in the given date pattern.
	/// Returns:
	/// - DateOrder.MonthFirst if month appears before day.
	/// - DateOrder.DayFirst if day appears before month.
	/// - null if unable to determine.
	/// </summary>
	private static DateOrder? DetermineMonthDayOrder(string pattern)
	{
		// Define a regex to match individual specifiers (e.g., "yyyy", "MM", "dd")
		var regex = new Regex(@"(?<specifier>d+|M+|y+)", RegexOptions.IgnoreCase);
		var matches = regex.Matches(pattern);

		// List to hold the sequence of month and day specifiers
		var orderList = new System.Collections.Generic.List<string>();

		foreach (Match match in matches)
		{
			string spec = match.Groups["specifier"].Value.ToUpperInvariant();
			if (spec.StartsWith("M"))
			{
				if (!orderList.Contains("M"))
					orderList.Add("M");
			}
			else if (spec.StartsWith("D"))
			{
				if (!orderList.Contains("D"))
					orderList.Add("D");
			}
			else if (spec.StartsWith("Y"))
			{
				if (!orderList.Contains("Y"))
					orderList.Add("Y");
			}
		}

		// Determine the order based on the first occurrence
		if (orderList.Count < 2)
		{
			// Not enough information to determine order
			return null;
		}

		if (orderList[0] == "Y" && orderList[1] == "M")
		{
			return DateOrder.YearFirst;
		}
		else if (orderList[0] == "M" && orderList[1] == "Y")
		{
			return DateOrder.MonthFirst;
		}
		else if (orderList[0] == "M" && orderList[1] == "D")
		{
			return DateOrder.MonthFirst;
		}
		else if (orderList[0] == "D" && orderList[1] == "M")
		{
			return DateOrder.DayFirst;
		}

		// If the order cannot be determined from the first two specifiers
		return null;
	}
}