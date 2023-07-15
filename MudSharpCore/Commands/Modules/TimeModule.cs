using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Commands.Modules;

internal class TimeModule : Module<ICharacter>
{
	private TimeModule()
		: base("Time")
	{
		IsNecessary = true;
	}

	public static TimeModule Instance { get; } = new();

	public static void DisplayCalendarToCharacter(ICharacter actor, ICalendar calendar, [CanBeNull] Year year)
	{
		var sb = new StringBuilder();
		if (year is not null)
		{
			sb.AppendLine(
				$"Calendar: {calendar.FullName.Colour(Telnet.Cyan)} for year {year.YearName.ToString("F0", actor).ColourValue()}");
		}
		else
		{
			sb.AppendLine($"Calendar: {calendar.FullName.Colour(Telnet.Cyan)}");
		}

		sb.AppendLine($"Ancient Epoch: {calendar.AncientEraLongString.Colour(Telnet.Green)}");
		sb.AppendLine($"Modern Epoch: {calendar.ModernEraLongString.Colour(Telnet.Green)}");
		sb.AppendLine($"Weekdays: {calendar.Weekdays.Select(x => x.Colour(Telnet.Green)).ListToString()}");
		sb.AppendLine(
			$"Days in Normal Year: {(calendar.Months.Sum(x => x.NormalDays) + calendar.Intercalaries.Where(x => x.Rule.DivisibleBy == 1).Sum(x => x.Month.NormalDays)).ToString("N0", actor).Colour(Telnet.Green)}");
		if (year is not null)
		{
			sb.AppendLine(
				$"Days in Year {year.YearName.ToString("F0", actor)}: {year.Months.Sum(x => x.Days).ToString("N0", actor).Colour(Telnet.Green)}");
		}

		sb.AppendLine();
		sb.AppendLine("Months:");
		if (year is not null)
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from month in year.Months
				select
					new[]
					{
						month.NominalOrder.ToString("N0", actor),
						month.FullName,
						month.ShortName,
						month.Days.ToString("N0", actor),
						month.DayNames.Select(x => $"{x.Value.FullName.ColourValue()} ({x.Key.ToOrdinal()})")
						     .ListToCommaSeparatedValues(", ")
					},
				new[] { "#", "Name", "Abbr", "Days", "Special Days" },
				actor,
				Telnet.Green
			));
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from month in calendar.Months
				select
					new[]
					{
						month.NominalOrder.ToString("N0", actor),
						month.FullName,
						month.ShortName,
						month.NormalDays.ToString("N0", actor),
						month.SpecialDayNames.Select(x => $"{x.Value.FullName.ColourValue()} ({x.Key.ToOrdinal()})")
						     .ListToCommaSeparatedValues(", ")
					},
				new[] { "#", "Name", "Abbr", "Days", "Special Days" },
				actor,
				Telnet.Green
			));
			sb.AppendLine("Intercalary Months:");
			sb.AppendLine(StringUtilities.GetTextTable(
				from month in calendar.Intercalaries
				select
					new[]
					{
						month.Month.FullName,
						month.Month.ShortName,
						month.Month.NormalDays.ToString("N0", actor),
						calendar.Months.FirstOrDefault(x => x.NominalOrder == month.Month.NominalOrder - 1)?.FullName ??
						"None",
						month.Month.SpecialDayNames
						     .Select(x => $"{x.Value.FullName.ColourValue()} ({x.Key.ToOrdinal()})")
						     .ListToCommaSeparatedValues(", "),
						month.Rule.ToString()
					},
				new[] { "Name", "Abbr", "Days", "After", "Special Days", "Rule" },
				actor,
				Telnet.Green
			));
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Calendar", "calendar")]
	[CustomModuleName("World")]
	[HelpInfo("calendar",
		@"The calendar command allows you to view information about a calendar. The syntax is either #3calendar#0 to view information about the calendar in general terms, or #3calendar <year>#0 to view the calendar for a specific year.",
		AutoHelp.HelpArg)]
	protected static void Calendar(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var calendar = actor.Location.Calendars.First();
		if (ss.IsFinished)
		{
			DisplayCalendarToCharacter(actor, calendar, null);
			return;
		}

		if (!int.TryParse(ss.Pop(), out var year))
		{
			actor.Send(
				"You must either specify nothing to view the calendar definition, or specify a year for which to see the calendar.");
			return;
		}

		var trueYear = calendar.CreateYear(year);
		DisplayCalendarToCharacter(actor, calendar, trueYear);
	}

	[PlayerCommand("Time", "time")]
	[CustomModuleName("World")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Time(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		sb.AppendLine("You know the following things about time:");
		sb.AppendLine();
		sb.AppendLine($"\tIt is currently {actor.Location.CurrentTimeOfDay.DescribeColour()}.");
		if (actor.Location.Zone.Weather?.CurrentSeason != null)
		{
			sb.AppendLine($"\tIt is currently {actor.Location.Zone.Weather.CurrentSeason.Name.Colour(Telnet.Green)}.");
		}

		var celestialList = new List<string>();
		var celestials = actor.Location.Celestials;
		foreach (var info in celestials.Select(x => actor.Location.GetInfo(x)))
		{
			if (actor.Body.CanSee(info.Origin))
			{
				var description = info.Origin.Describe(info).Fullstop().Wrap(actor.InnerLineFormatLength, "\t");
				if (actor.IsAdministrator())
				{
					description += $" (Asc: {$"{info.LastAscensionAngle.RadiansToDegrees().ToString("N3", actor)}°".ColourValue()} Azi: {$"{info.LastAzimuthAngle.RadiansToDegrees().ToString("N3", actor)}°".ColourValue()} DN: {info.Origin.CurrentCelestialDay.ToString("N2", actor).ColourValue()})";
				}

				sb.AppendLine(description);
			}
		}

		var calendars = actor.Body.Location.Calendars;
		if (actor.Gameworld.GetCheck(CheckType.ExactTimeCheck).Check(actor, Difficulty.Automatic).IsPass())
		{
			foreach (var calendar in calendars)
			{
				var date = actor.Location.Date(calendar);
				var time = actor.Location.Time(calendar.FeedClock);
				sb.AppendLine($"It is {calendar.FeedClock.DisplayTime(time, TimeDisplayTypes.Long).ColourValue()} on {calendar.DisplayDate(date, CalendarDisplayMode.Long).ColourValue().Fullstop()}".Wrap(actor.InnerLineFormatLength, "\t"));
			}
		}
		else if (actor.Gameworld.GetCheck(CheckType.VagueTimeCheck).Check(actor, Difficulty.Automatic).IsPass())
		{
			foreach (var calendar in calendars)
			{
				var date = actor.Location.Date(calendar);
				var time = actor.Location.Time(calendar.FeedClock);
				sb.AppendLine($"It is about {calendar.FeedClock.DisplayTime(time, TimeDisplayTypes.Vague).ColourValue()} on {calendar.DisplayDate(date, CalendarDisplayMode.Long).ColourValue().Fullstop()}".Wrap(actor.InnerLineFormatLength, "\t"));
			}
		}
		else
		{
			foreach (var calendar in calendars)
			{
				var date = actor.Location.Date(calendar);
				var time = actor.Location.Time(calendar.FeedClock);
				sb.AppendLine($"It is {calendar.FeedClock.DisplayTime(time, TimeDisplayTypes.Crude).ColourValue()} on {calendar.DisplayDate(date, CalendarDisplayMode.Long).ColourValue().Fullstop()}".Wrap(actor.InnerLineFormatLength, "\t"));
			}
		}

		var visibleTimepieces = actor.Body.ExternalItems.Concat(actor.Location.LayerGameItems(actor.RoomLayer))
		                             .Where(x => actor.CanSee(x)).SelectNotNull(x => x.GetItemType<ITimePiece>())
		                             .ToList();
		foreach (var item in visibleTimepieces)
		{
			sb.AppendLine(
				$"\t{item.Parent.HowSeen(actor, true)} shows the time as {item.Clock.DisplayTime(item.CurrentTime, item.TimeDisplayString).ColourValue()}.".Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send(sb.ToString());
	}
}