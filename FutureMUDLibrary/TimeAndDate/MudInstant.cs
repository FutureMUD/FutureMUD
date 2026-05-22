#nullable enable

using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Globalization;

namespace MudSharp.TimeAndDate;

/// <summary>
/// A deterministic absolute in-game timestamp measured in clock seconds from a calendar epoch.
/// </summary>
public readonly struct MudInstant : IComparable<MudInstant>, IEquatable<MudInstant>
{
	public const string CurrentEpoch = "v1";
	private const string Prefix = "mudinstant";

	public static MudInstant Never => new(CurrentEpoch, 0, 0, 0, true);

	public MudInstant(long ticks)
		: this(CurrentEpoch, ticks, 0, 0, false)
	{
	}

	public MudInstant(string epoch, long ticks)
		: this(string.IsNullOrWhiteSpace(epoch) ? CurrentEpoch : epoch, ticks, 0, 0, false)
	{
	}

	public MudInstant(string epoch, long ticks, long sourceCalendarId, long sourceClockId)
		: this(string.IsNullOrWhiteSpace(epoch) ? CurrentEpoch : epoch, ticks, sourceCalendarId, sourceClockId, false)
	{
	}

	private MudInstant(string epoch, long ticks, long sourceCalendarId, long sourceClockId, bool isNever)
	{
		Epoch = epoch;
		Ticks = ticks;
		SourceCalendarId = sourceCalendarId;
		SourceClockId = sourceClockId;
		IsNever = isNever;
	}

	public string Epoch { get; }

	public long Ticks { get; }

	public long SourceCalendarId { get; }

	public long SourceClockId { get; }

	public bool HasSourceContext => SourceCalendarId > 0 && SourceClockId > 0;

	public bool IsNever { get; }

	public string GetStorageString()
	{
		if (IsNever)
		{
			return "Never";
		}

		return HasSourceContext
			? $"{Prefix}:{Epoch}:{SourceCalendarId.ToString(CultureInfo.InvariantCulture)}:{SourceClockId.ToString(CultureInfo.InvariantCulture)}:{Ticks.ToString(CultureInfo.InvariantCulture)}"
			: $"{Prefix}:{Epoch}:{Ticks.ToString(CultureInfo.InvariantCulture)}";
	}

	public override string ToString()
	{
		return GetStorageString();
	}

	public static bool TryParse(string? text, out MudInstant instant)
	{
		instant = Never;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (text.Equals("never", StringComparison.InvariantCultureIgnoreCase))
		{
			instant = Never;
			return true;
		}

		var split = text.Split(':', StringSplitOptions.RemoveEmptyEntries);
		if (split.Length == 0 || !split[0].Equals(Prefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return false;
		}

		switch (split.Length)
		{
			case 3:
				if (!long.TryParse(split[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var legacyTicks))
				{
					return false;
				}

				instant = new MudInstant(split[1], legacyTicks);
				return true;
			case 5:
				if (!long.TryParse(split[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var calendarId) ||
				    !long.TryParse(split[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var clockId) ||
				    !long.TryParse(split[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks))
				{
					return false;
				}

				instant = new MudInstant(split[1], ticks, calendarId, clockId);
				return true;
			default:
				return false;
		}
	}

	public static MudInstant Parse(string text)
	{
		if (TryParse(text, out var instant))
		{
			return instant;
		}

		throw new FormatException($"The text '{text}' is not a valid MudInstant storage string.");
	}

	public static MudInstant FromMudDateTime(MudDateTime dateTime)
	{
		if (dateTime?.Date is null || dateTime.Clock is null)
		{
			return Never;
		}

		var clock = dateTime.Clock;
		var calendar = dateTime.Calendar;
		var primary = dateTime.GetByTimeZone(clock.PrimaryTimezone);
		var day = DaysSinceEpoch(primary.Date);
		var ticks = checked(day * SecondsPerDay(clock) + SecondsSinceMidnight(primary.Time));
		return new MudInstant(CurrentEpoch, ticks, calendar.Id, clock.Id);
	}

	public static MudInstant FromLegacyState(ICalendar calendar, IClock? clock = null)
	{
		if (calendar?.CurrentDate is null)
		{
			return Never;
		}

		clock ??= calendar.FeedClock;
		if (clock is null)
		{
			return Never;
		}

		return FromMudDateTime(new MudDateTime(calendar.CurrentDate, clock.CurrentTime, clock.PrimaryTimezone));
	}

	public MudDateTime ToMudDateTime(ICalendar calendar, IClock? clock = null, IMudTimeZone? timezone = null)
	{
		if (IsNever || calendar is null)
		{
			return MudDateTime.Never;
		}

		clock ??= calendar.FeedClock;
		if (clock is null)
		{
			return MudDateTime.Never;
		}

		timezone ??= clock.PrimaryTimezone;
		if (TryResolveSourceContext(calendar, clock, out var sourceCalendar, out var sourceClock) &&
		    (sourceCalendar.Id != calendar.Id || sourceClock.Id != clock.Id) &&
		    sourceClock.Id == clock.Id)
		{
			var sourceDateTime = ToMudDateTimeInCalendar(sourceCalendar, sourceClock, timezone);
			return sourceDateTime.ConvertToOtherCalendar(calendar);
		}

		return ToMudDateTimeInCalendar(calendar, clock, timezone);
	}

	public MudInstant WithTicks(long ticks)
	{
		return new MudInstant(Epoch, ticks, SourceCalendarId, SourceClockId, IsNever);
	}

	public MudInstant AddSeconds(long seconds)
	{
		return WithTicks(checked(Ticks + seconds));
	}

	private MudDateTime ToMudDateTimeInCalendar(ICalendar calendar, IClock clock, IMudTimeZone timezone)
	{
		var secondsPerDay = SecondsPerDay(clock);
		var dayOffset = FloorDiv(Ticks, secondsPerDay);
		var secondOfDay = Ticks - dayOffset * secondsPerDay;
		var date = FirstDateOfEpoch(calendar);
		date.AdvanceDays(checked((int)dayOffset));

		var hours = (int)(secondOfDay / (clock.MinutesPerHour * clock.SecondsPerMinute));
		secondOfDay %= clock.MinutesPerHour * clock.SecondsPerMinute;
		var minutes = (int)(secondOfDay / clock.SecondsPerMinute);
		var seconds = (int)(secondOfDay % clock.SecondsPerMinute);
		var primaryTime = MudTime.FromLocalTime(seconds, minutes, hours, clock.PrimaryTimezone, clock);
		var localTime = primaryTime.GetTimeByTimezone(timezone);
		if (localTime.DaysOffsetFromDatum != 0)
		{
			date.AdvanceDays(localTime.DaysOffsetFromDatum);
		}

		return new MudDateTime(date, localTime, timezone);
	}

	private bool TryResolveSourceContext(ICalendar targetCalendar, IClock targetClock, out ICalendar sourceCalendar,
		out IClock sourceClock)
	{
		sourceCalendar = targetCalendar;
		sourceClock = targetClock;

		if (!HasSourceContext)
		{
			return false;
		}

		if (targetCalendar.Id == SourceCalendarId && targetClock.Id == SourceClockId)
		{
			return true;
		}

		var gameworld = targetCalendar.Gameworld ?? targetClock.Gameworld;
		var resolvedCalendar = gameworld?.Calendars.Get(SourceCalendarId);
		var resolvedClock = gameworld?.Clocks.Get(SourceClockId);
		if (resolvedCalendar is null || resolvedClock is null)
		{
			return false;
		}

		sourceCalendar = resolvedCalendar;
		sourceClock = resolvedClock;
		return true;
	}

	private static MudDate FirstDateOfEpoch(ICalendar calendar)
	{
		var year = calendar.CreateYear(calendar.EpochYear);
		var month = year.Months[0];
		return new MudDate(calendar, 1, calendar.EpochYear, month, year, false);
	}

	private static long DaysSinceEpoch(MudDate date)
	{
		var calendar = date.Calendar;
		var dayOfYear = date.DayNumberInYear() - 1;
		if (date.Year >= calendar.EpochYear)
		{
			return calendar.CountDaysBetweenYears(calendar.EpochYear, date.Year) + dayOfYear;
		}

		return -(calendar.CountDaysBetweenYears(date.Year, calendar.EpochYear) - dayOfYear);
	}

	private static long SecondsSinceMidnight(MudTime time)
	{
		return (long)time.Hours * time.Clock.MinutesPerHour * time.Clock.SecondsPerMinute +
		       (long)time.Minutes * time.Clock.SecondsPerMinute +
		       time.Seconds;
	}

	private static long SecondsPerDay(IClock clock)
	{
		return (long)clock.HoursPerDay * clock.MinutesPerHour * clock.SecondsPerMinute;
	}

	private static long FloorDiv(long value, long divisor)
	{
		var quotient = value / divisor;
		var remainder = value % divisor;
		return remainder != 0 && ((remainder > 0) != (divisor > 0)) ? quotient - 1 : quotient;
	}

	public int CompareTo(MudInstant other)
	{
		if (IsNever)
		{
			return other.IsNever ? 0 : -1;
		}

		if (other.IsNever)
		{
			return 1;
		}

		var epochCompare = string.Compare(Epoch, other.Epoch, StringComparison.Ordinal);
		if (epochCompare != 0)
		{
			return epochCompare;
		}

		var tickCompare = Ticks.CompareTo(other.Ticks);
		if (tickCompare != 0)
		{
			return tickCompare;
		}

		var calendarCompare = SourceCalendarId.CompareTo(other.SourceCalendarId);
		return calendarCompare != 0 ? calendarCompare : SourceClockId.CompareTo(other.SourceClockId);
	}

	public bool Equals(MudInstant other)
	{
		return IsNever == other.IsNever &&
		       Ticks == other.Ticks &&
		       SourceCalendarId == other.SourceCalendarId &&
		       SourceClockId == other.SourceClockId &&
		       string.Equals(Epoch, other.Epoch, StringComparison.Ordinal);
	}

	public override bool Equals(object? obj)
	{
		return obj is MudInstant other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Epoch, Ticks, SourceCalendarId, SourceClockId, IsNever);
	}

	public static bool operator <(MudInstant left, MudInstant right)
	{
		return left.CompareTo(right) < 0;
	}

	public static bool operator >(MudInstant left, MudInstant right)
	{
		return left.CompareTo(right) > 0;
	}

	public static bool operator <=(MudInstant left, MudInstant right)
	{
		return left.CompareTo(right) <= 0;
	}

	public static bool operator >=(MudInstant left, MudInstant right)
	{
		return left.CompareTo(right) >= 0;
	}

	public static bool operator ==(MudInstant left, MudInstant right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(MudInstant left, MudInstant right)
	{
		return !left.Equals(right);
	}
}
