using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Linq;
using System.Text;

namespace MudSharp.TimeAndDate;

public enum StoredMudDateTimeFallback
{
	CurrentDateTime,
	Never,
	EpochStart
}

public enum StoredMudDateFallback
{
	CurrentDate,
	EpochStart
}

public enum StoredMudTimeFallback
{
	CurrentTime,
	MidnightPrimaryTimezone
}

public static class StoredTimeFallbacks
{
	public static MudDate GetStoredDateOrFallback(this ICalendar calendar, string text,
		StoredMudDateFallback fallback, string ownerType, long? ownerId, string ownerName, string fieldName)
	{
		try
		{
			return calendar.GetDate(text);
		}
		catch (Exception ex)
		{
			var fallbackDate = fallback switch
			{
				StoredMudDateFallback.EpochStart => FirstValidDate(calendar),
				_ => calendar.CurrentDate is null ? FirstValidDate(calendar) : new MudDate(calendar.CurrentDate)
			};
			NotifyFallback(calendar?.FeedClock?.Gameworld ?? fallbackDate?.Calendar?.FeedClock?.Gameworld,
				"date", text, calendar, calendar?.FeedClock, fallbackDate?.GetDateString() ?? "null",
				ownerType, ownerId, ownerName, fieldName, ex.Message);
			return fallbackDate;
		}
	}

	public static MudTime GetStoredTimeOrFallback(this IClock clock, string text,
		StoredMudTimeFallback fallback, string ownerType, long? ownerId, string ownerName, string fieldName,
		ICalendar calendar = null)
	{
		if (MudTime.TryParseLocalTime(text, clock, out var time, out var error))
		{
			return time;
		}

		var fallbackTime = fallback switch
		{
			StoredMudTimeFallback.MidnightPrimaryTimezone => MudTime.FromLocalTime(0, 0, 0, clock.PrimaryTimezone, clock),
			_ => MudTime.CopyOf(clock.CurrentTime, true)
		};
		NotifyFallback(clock?.Gameworld, "time", text, calendar, clock, fallbackTime.GetTimeString(),
			ownerType, ownerId, ownerName, fieldName, error);
		return fallbackTime;
	}

	public static MudDate FirstValidDate(ICalendar calendar)
	{
		var yearNumber = calendar?.EpochYear ?? 1;
		var year = calendar.CreateYear(yearNumber);
		var month = year.Months.First();
		return new MudDate(calendar, 1, yearNumber, month, year, false);
	}

	public static MudDateTime CurrentDateTimeOrNever(ICalendar calendar, IClock clock)
	{
		if (calendar?.CurrentDate is null || clock?.CurrentTime is null || clock.PrimaryTimezone is null)
		{
			return MudDateTime.Never;
		}

		var time = clock.CurrentTime.Timezone == clock.PrimaryTimezone
			? MudTime.CopyOf(clock.CurrentTime, true)
			: clock.CurrentTime.GetTimeByTimezone(clock.PrimaryTimezone);
		return new MudDateTime(new MudDate(calendar.CurrentDate), time, clock.PrimaryTimezone);
	}

	public static MudDateTime EpochStartOrNever(ICalendar calendar, IClock clock)
	{
		if (calendar is null || clock?.PrimaryTimezone is null)
		{
			return MudDateTime.Never;
		}

		return new MudDateTime(FirstValidDate(calendar), MudTime.FromLocalTime(0, 0, 0, clock.PrimaryTimezone, clock),
			clock.PrimaryTimezone);
	}

	public static void NotifyFallback(IFuturemud gameworld, string valueType, string badValue, ICalendar calendar,
		IClock clock, string fallbackValue, string ownerType, long? ownerId, string ownerName, string fieldName,
		string reason)
	{
		var ownerText = string.IsNullOrWhiteSpace(ownerName)
			? $"{ownerType ?? "Unknown"}{(ownerId.HasValue ? $" #{ownerId.Value:N0}" : string.Empty)}"
			: $"{ownerType ?? "Unknown"}{(ownerId.HasValue ? $" #{ownerId.Value:N0}" : string.Empty)} ({ownerName})";
		var sb = new StringBuilder();
		sb.AppendLine("**TimeAndDate stored value fallback used**");
		sb.AppendLine($"Value Type: {valueType}");
		sb.AppendLine($"Bad Value: `{badValue ?? "<null>"}`");
		sb.AppendLine($"Owner: {ownerText}");
		sb.AppendLine($"Field: {fieldName ?? "Unknown"}");
		sb.AppendLine($"Calendar: {(calendar is null ? "None" : $"#{calendar.Id:N0} {calendar.Name}")}");
		sb.AppendLine($"Clock: {(clock is null ? "None" : $"#{clock.Id:N0} {clock.Name}")}");
		sb.AppendLine($"Fallback: `{fallbackValue ?? "<null>"}`");
		if (!string.IsNullOrWhiteSpace(reason))
		{
			sb.AppendLine($"Reason: {reason}");
		}

		var message = sb.ToString();
		try
		{
			if (gameworld?.DiscordConnection is null)
			{
				ConsoleUtilities.WriteLine(message);
				return;
			}

			gameworld.DiscordConnection.NotifyAdmins(message);
		}
		catch (Exception ex)
		{
			ConsoleUtilities.WriteLine(message);
			ConsoleUtilities.WriteLine($"TimeAndDate fallback Discord notification failed: {ex}");
		}
	}
}
