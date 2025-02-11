using System;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.TimeAndDate.Intervals {
	public class RecurringInterval {
		private static readonly Regex ParseRegex =
			new(
				@"every\s*(?<amount>[0-9]+)*\s*(?<interval>minutes?|hours?|days?|months?|weekdays?|weeks?|years?)(?:\s(?<modifier>.+))*");

		public IntervalType Type { get; init; }
		public int IntervalAmount { get; init; }
		public int Modifier { get; init; }

		#region Overrides of Object

		public override string ToString()
		{
			return $"every {IntervalAmount:F0} {Type switch { IntervalType.Daily => "days", IntervalType.Hourly => "hours", IntervalType.Minutely => "minutes", IntervalType.Monthly => "months", IntervalType.SpecificWeekday => "weekdays", IntervalType.Weekly => "weeks", IntervalType.Yearly => "years", _ => "days" }} {Modifier:+000}";
		}

		#endregion

		public static RecurringInterval Parse(string text)
		{
			if (!TryParse(text, out var interval))
			{
				throw new ArgumentOutOfRangeException(nameof(text));
			}

			return interval;
		}

		public static bool TryParse(string text, out RecurringInterval interval) {
			var match = ParseRegex.Match(text);
			if (!match.Success) {
				interval = null;
				return false;
			}

			var amount = 1;
			if (match.Groups["amount"].Length > 0) {
				amount = int.Parse(match.Groups["amount"].Value);
			}

			IntervalType intervalType;
			switch (match.Groups["interval"].Value) {
				case "minute":
				case "minutes":
					intervalType = IntervalType.Minutely;
					break;
				case "hour":
				case "hours":
					intervalType = IntervalType.Hourly;
					break;
				case "day":
				case "days":
					intervalType = IntervalType.Daily;
					break;
				case "week":
				case "weeks":
					intervalType = IntervalType.Weekly;
					break;
				case "month":
				case "months":
					intervalType = IntervalType.Monthly;
					break;
				case "year":
				case "years":
					intervalType = IntervalType.Yearly;
					break;
				case "weekday":
				case "weekdays":
					intervalType = IntervalType.SpecificWeekday;
					break;
				default:
#if DEBUG
					throw new ApplicationException($"Invalid interval type \"{match.Groups["interval"].Value}\" in RecurringInterval.TryParse");
#else
					interval = null;
					return false;
#endif
			}

			var modifier = 0;
			if (match.Groups["modifier"].Length > 0)
			{
				if (!int.TryParse(match.Groups["modifier"].Value, out modifier))
				{
#if DEBUG
					throw new ApplicationException($"Invalid interval modifier \"{match.Groups["modifier"].Value}\" in RecurringInterval.TryParse");
#else
					interval = null;
					return false;
#endif
				}
			}
			
			interval = new RecurringInterval {IntervalAmount = amount, Modifier = modifier, Type = intervalType};
			return true;
		}

		public string Describe(ICalendar whichCalendar) {
			switch (Type) {
				case IntervalType.Minutely:
					return IntervalAmount == 1 ? "every minute" : $"every {IntervalAmount} minutes";
				case IntervalType.Hourly:
					return IntervalAmount == 1 ? "every hour" : $"every {IntervalAmount} hours";
				case IntervalType.Daily:
					return IntervalAmount == 1 ? "every day" : $"every {IntervalAmount} days";
				case IntervalType.Monthly:
					return IntervalAmount == 1 ? "every month" : $"every {IntervalAmount} months";
				case IntervalType.SpecificWeekday:
					return IntervalAmount == 1
						? $"every {whichCalendar.Weekdays[Modifier]}"
						: $"every {IntervalAmount} {whichCalendar.Weekdays[Modifier].Pluralise()}";
				case IntervalType.Weekly:
					return IntervalAmount == 1 ? "every week" : $"every {IntervalAmount} weeks";
				case IntervalType.Yearly:
					return IntervalAmount == 1 ? "every year" : $"every {IntervalAmount} years";
				default:
					throw new NotSupportedException("RecurringInveral.Describe found an unknown IntervalType.");
			}
		}

		public MudDateTime GetNextAdjacentToCurrent(MudDateTime referenceDateTime)
		{
			var calendar = referenceDateTime.Calendar;
			var newDate = new MudDateTime(referenceDateTime);
			var currentDateTime = new MudDateTime(referenceDateTime.Calendar.CurrentDate,
				referenceDateTime.Clock.CurrentTime, referenceDateTime.Clock.PrimaryTimezone).GetByTimeZone(
				referenceDateTime.TimeZone);
			newDate.Time.DaysOffsetFromDatum = 0;
			currentDateTime.Time.DaysOffsetFromDatum = 0;

			while (newDate > currentDateTime)
			{
				switch (Type)
				{
					case IntervalType.Minutely:
						newDate.Time.DaysOffsetFromDatum = 0;
						newDate.Time.AddMinutes(-1 * IntervalAmount);
						if (newDate.Time.DaysOffsetFromDatum < 0)
						{
							newDate.Date.AdvanceDays(newDate.Time.DaysOffsetFromDatum);
							newDate.Time.DaysOffsetFromDatum = 0;
						}
						break;
					case IntervalType.Hourly:
						newDate.Time.DaysOffsetFromDatum = 0;
						newDate.Time.AddHours(-1 * IntervalAmount);
						if (newDate.Time.DaysOffsetFromDatum < 0)
						{
							newDate.Date.AdvanceDays(newDate.Time.DaysOffsetFromDatum);
							newDate.Time.DaysOffsetFromDatum = 0;
						}
						break;
					case IntervalType.Daily:
						newDate.Date.AdvanceDays(-1 * IntervalAmount);
						break;
					case IntervalType.Monthly:
						newDate.Date.AdvanceMonths(-1 * IntervalAmount, true, true);
						break;
					case IntervalType.SpecificWeekday:
						newDate.Date.AdvanceToNextWeekday(Modifier, -1 * IntervalAmount);
						break;
					case IntervalType.Weekly:
						newDate.Date.AdvanceDays(-1 * IntervalAmount * calendar.Weekdays.Count);
						break;
					case IntervalType.Yearly:
						newDate.Date.AdvanceYears(-1 * IntervalAmount, true);
						break;
				}
			}

			return GetNextDateTime(newDate);
		}

		public MudDateTime GetLastDateTime(MudDateTime referenceDateTime)
		{
			var calendar = referenceDateTime.Calendar;
			var newDate = new MudDateTime(referenceDateTime);
			var currentDateTime = new MudDateTime(referenceDateTime.Calendar.CurrentDate,
				referenceDateTime.Clock.CurrentTime, referenceDateTime.Clock.PrimaryTimezone).GetByTimeZone(
				referenceDateTime.TimeZone);
			newDate.Time.DaysOffsetFromDatum = 0;
			currentDateTime.Time.DaysOffsetFromDatum = 0;

			while (newDate > currentDateTime)
			{
				switch (Type)
				{
					case IntervalType.Minutely:
						newDate.Time.DaysOffsetFromDatum = 0;
						newDate.Time.AddMinutes(-1 * IntervalAmount);
						if (newDate.Time.DaysOffsetFromDatum < 0)
						{
							newDate.Date.AdvanceDays(newDate.Time.DaysOffsetFromDatum);
							newDate.Time.DaysOffsetFromDatum = 0;
						}
						break;
					case IntervalType.Hourly:
						newDate.Time.DaysOffsetFromDatum = 0;
						newDate.Time.AddHours(-1 * IntervalAmount);
						if (newDate.Time.DaysOffsetFromDatum < 0)
						{
							newDate.Date.AdvanceDays(newDate.Time.DaysOffsetFromDatum);
							newDate.Time.DaysOffsetFromDatum = 0;
						}
						break;
					case IntervalType.Daily:
						newDate.Date.AdvanceDays(-1 * IntervalAmount);
						break;
					case IntervalType.Monthly:
						newDate.Date.AdvanceMonths(-1 * IntervalAmount, true, true);
						break;
					case IntervalType.SpecificWeekday:
						newDate.Date.AdvanceToNextWeekday(Modifier, -1 * IntervalAmount);
						break;
					case IntervalType.Weekly:
						newDate.Date.AdvanceDays(-1 * IntervalAmount * calendar.Weekdays.Count);
						break;
					case IntervalType.Yearly:
						newDate.Date.AdvanceYears(-1 * IntervalAmount, true);
						break;
				}
			}

			return newDate;
		}

		public DateTime GetNextDateTime(DateTime referenceDateTime)
		{
			switch (Type)
			{
				case IntervalType.Daily:
					return referenceDateTime.AddDays(IntervalAmount);
				case IntervalType.Weekly:
					return referenceDateTime.AddDays(7);
				case IntervalType.Monthly:
					return referenceDateTime.AddMonths(IntervalAmount);
				case IntervalType.Yearly:
					return referenceDateTime.AddYears(IntervalAmount);
				case IntervalType.SpecificWeekday:
					var result = referenceDateTime;
					for (var i = 0; i < 7; i++)
					{
						result = result.AddDays(1);
						if (result.DayOfWeek == (DayOfWeek)Modifier)
						{
							if (IntervalAmount > 1)
							{
								return result.AddDays(7 * IntervalAmount);
							}

							return result;
						}
					}

					throw new NotSupportedException("There were more than 7 days of the week. A non-UTC date was probably passed in.");
				case IntervalType.Hourly:
					return referenceDateTime.AddHours(IntervalAmount);
				case IntervalType.Minutely:
					return referenceDateTime.AddMinutes(IntervalAmount);
			}

			throw new NotSupportedException("The IntervalType was not supported in RecurringInverval.GetNextDateTime");
		}

		public MudDateTime GetNextDateTime(MudDateTime referenceDateTime) {
			var calendar = referenceDateTime.Calendar;
			var newDate = new MudDateTime(referenceDateTime);
			var currentDateTime = new MudDateTime(referenceDateTime.Calendar.CurrentDate,
				referenceDateTime.Clock.CurrentTime, referenceDateTime.Clock.PrimaryTimezone).GetByTimeZone(
				referenceDateTime.TimeZone);
			newDate.Time.DaysOffsetFromDatum = 0;
			currentDateTime.Time.DaysOffsetFromDatum = 0;

			while (newDate <= currentDateTime) {
				switch (Type) {
					case IntervalType.Minutely:
						newDate.Time.DaysOffsetFromDatum = 0;
						newDate.Time.AddMinutes(IntervalAmount);
						if (newDate.Time.DaysOffsetFromDatum > 0)
						{
							newDate.Date.AdvanceDays(newDate.Time.DaysOffsetFromDatum);
							newDate.Time.DaysOffsetFromDatum = 0;
						}
						break;
					case IntervalType.Hourly:
						newDate.Time.DaysOffsetFromDatum = 0;
						newDate.Time.AddHours(IntervalAmount);
						if (newDate.Time.DaysOffsetFromDatum > 0) {
							newDate.Date.AdvanceDays(newDate.Time.DaysOffsetFromDatum);
							newDate.Time.DaysOffsetFromDatum = 0;
						}
						break;
					case IntervalType.Daily:
						newDate.Date.AdvanceDays(IntervalAmount);
						break;
					case IntervalType.Monthly:
						newDate.Date.AdvanceMonths(IntervalAmount, true, true);
						break;
					case IntervalType.SpecificWeekday:
						newDate.Date.AdvanceToNextWeekday(Modifier, IntervalAmount);
						break;
					case IntervalType.Weekly:
						newDate.Date.AdvanceDays(IntervalAmount*calendar.Weekdays.Count);
						break;
					case IntervalType.Yearly:
						newDate.Date.AdvanceYears(IntervalAmount, true);
						break;
				}
			}
			return newDate;
		}

		public MudDate GetNextDateExclusive(ICalendar whichCalendar, MudDate referenceDate) {
			var newDate = new MudDate(referenceDate);
			while (newDate <= whichCalendar.CurrentDate) {
				switch (Type) {
					case IntervalType.Daily:
						newDate.AdvanceDays(IntervalAmount);
						break;
					case IntervalType.Monthly:
						newDate.AdvanceMonths(IntervalAmount, true, true);
						break;
					case IntervalType.SpecificWeekday:
						newDate.AdvanceToNextWeekday(Modifier, IntervalAmount);
						break;
					case IntervalType.Weekly:
						newDate.AdvanceDays(IntervalAmount*whichCalendar.Weekdays.Count);
						break;
					case IntervalType.Yearly:
						newDate.AdvanceYears(IntervalAmount, true);
						break;
				}
			}

			return newDate;
		}

		public MudDate GetNextDate(ICalendar whichCalendar, MudDate referenceDate) {
			var newDate = new MudDate(referenceDate);
			while (newDate < whichCalendar.CurrentDate) {
				switch (Type) {
					case IntervalType.Minutely:
					case IntervalType.Hourly:
					case IntervalType.Daily:
						newDate.AdvanceDays(IntervalAmount);
						break;
					case IntervalType.Monthly:
						newDate.AdvanceMonths(IntervalAmount, true, true);
						break;
					case IntervalType.SpecificWeekday:
						newDate.AdvanceToNextWeekday(Modifier, IntervalAmount);
						break;
					case IntervalType.Weekly:
						newDate.AdvanceDays(IntervalAmount*whichCalendar.Weekdays.Count);
						break;
					case IntervalType.Yearly:
						newDate.AdvanceYears(IntervalAmount, true);
						break;
				}
			}

			return newDate;
		}

		public double DaysPerInterval(ICalendar calendar)
		{
			switch (Type)
				{
					case IntervalType.Daily:
						return IntervalAmount;
					case IntervalType.Weekly:
					case IntervalType.SpecificWeekday:
					return calendar.Weekdays.Count * IntervalAmount;
					case IntervalType.Monthly:
						return calendar.CurrentDate.ThisYear.Months.Average(x => x.Days) * IntervalAmount;
					case IntervalType.Yearly:
						return calendar.CountDaysInYear(calendar.EpochYear) * IntervalAmount;
					case IntervalType.Hourly:
						return IntervalAmount / (double)calendar.FeedClock.HoursPerDay;
					case IntervalType.Minutely:
						return IntervalAmount / ((double)calendar.FeedClock.MinutesPerHour * calendar.FeedClock.HoursPerDay);
				default:
						throw new ArgumentOutOfRangeException();
				}
			
		}
	}
}