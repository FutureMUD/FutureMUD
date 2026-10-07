#nullable enable

using MudSharp.Celestial;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.NPC.AI;

/// <summary>Native game-time predicates: alternatives within fields, conjunction between fields.</summary>
public sealed class MonsterActivityWindow
{
	public HashSet<TimeOfDay> Times { get; } = [];
	public HashSet<string> Seasons { get; } = new(StringComparer.OrdinalIgnoreCase);
	public long CalendarId { get; set; }
	public HashSet<string> Months { get; } = new(StringComparer.OrdinalIgnoreCase);
	public int? FirstDay { get; set; }
	public int? LastDay { get; set; }
	public long MoonId { get; set; }
	public HashSet<MoonPhase> Phases { get; } = [];
	public long ConditionProgId { get; set; }
	public string? LoadError { get; private set; }

	public string? ConfigurationError(IFuturemud world)
	{
		if (LoadError is not null) return LoadError;
		if ((Months.Count > 0 || FirstDay.HasValue || LastDay.HasValue) && CalendarId == 0)
			return "month/day restrictions require a calendar";
		if (FirstDay.HasValue != LastDay.HasValue || FirstDay < 1 || FirstDay > LastDay)
			return "day bounds must be inclusive positive days in ascending order";
		if (CalendarId != 0)
		{
			var calendar = world.Calendars.Get(CalendarId);
			if (calendar is null) return "the selected calendar is unavailable";
			var aliases = MonthAliases(calendar).ToHashSet(StringComparer.OrdinalIgnoreCase);
			if (Months.Any(x => !aliases.Contains(x))) return "a selected month is no longer defined by the calendar";
		}
		if (Phases.Count > 0 && MoonId == 0) return "lunar phases require a selected moon";
		if (MoonId != 0 && world.CelestialObjects.Get(MoonId) is not ILunarPhase)
			return "the selected moon is unavailable or does not expose lunar phases";
		if (Seasons.Any(x => !world.Seasons.Any(y => y.SeasonGroup.EqualTo(x))))
			return "a selected season group is unavailable";
		if (ConditionProgId != 0)
		{
			var prog = world.FutureProgs.Get(ConditionProgId);
			if (prog is null || !prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			    !prog.MatchesParameters([ProgVariableTypes.Character]))
				return "the activity condition must return Boolean and accept one character";
		}
		return null;
	}

	public string? InactiveReason(ICharacter actor)
	{
		if (ConfigurationError(actor.Gameworld) is { } error) return error;
		if (actor.Location is null) return "no current room";
		if (Times.Count > 0 && !Times.Contains(actor.Location.CurrentTimeOfDay)) return "outside the active time bands";
		if (Seasons.Count > 0 && !Seasons.Contains(actor.Location.CurrentSeason(actor)?.SeasonGroup ?? ""))
			return "outside the active season groups";
		if (CalendarId != 0 && !MatchesDate(actor.Gameworld.Calendars.Get(CalendarId)!.CurrentDate))
			return "outside the active calendar dates";
		if (MoonId != 0)
		{
			var moon = actor.Location.Zone.Celestials.FirstOrDefault(x => x.Id == MoonId) as ILunarPhase;
			if (moon is null) return "the selected moon is not present in this zone";
			if (Phases.Count > 0 && !Phases.Contains(moon.CurrentPhase())) return "outside the active lunar phases";
		}
		if (ConditionProgId != 0 && actor.Gameworld.FutureProgs.Get(ConditionProgId)?.ExecuteBool(false, actor) != true)
			return "the activity condition is false";
		return null;
	}

	internal bool MatchesDate(MudDate date) =>
		(Months.Count == 0 || Months.Contains(date.Month.Alias)) &&
		(!FirstDay.HasValue || date.Day >= FirstDay && date.Day <= LastDay);

	internal static IEnumerable<string> MonthAliases(ICalendar calendar) =>
		calendar.Months.Select(x => x.Alias).Concat(calendar.Intercalaries.Select(x => x.Month.Alias));

	public XElement Save() => new("ActivityWindow", new XAttribute("version", 1),
		Times.Order().Select(x => new XElement("Time", x)), Seasons.Order().Select(x => new XElement("Season", x)),
		new XElement("Calendar", CalendarId), Months.Order().Select(x => new XElement("Month", x)),
		new XElement("FirstDay", FirstDay), new XElement("LastDay", LastDay), new XElement("Moon", MoonId),
		Phases.Order().Select(x => new XElement("Phase", x)), new XElement("ConditionProg", ConditionProgId),
		LoadError is null ? null : new XElement("LoadError", LoadError));

	public static MonsterActivityWindow Load(XElement? root)
	{
		var result = new MonsterActivityWindow();
		if (root is null) return result;
		result.LoadError = root.Element("LoadError")?.Value;
		if (root.Attribute("version")?.Value is { } version && version != "1") result.LoadError = "unsupported activity window version";
		foreach (var item in root.Elements("Time"))
			if (Enum.TryParse<TimeOfDay>(item.Value, true, out var time) && Enum.IsDefined(time)) result.Times.Add(time);
			else result.LoadError = "invalid time band in saved activity window";
		foreach (var item in root.Elements("Phase"))
			if (Enum.TryParse<MoonPhase>(item.Value, true, out var phase) && Enum.IsDefined(phase)) result.Phases.Add(phase);
			else result.LoadError = "invalid phase in saved activity window";
		result.Seasons.UnionWith(root.Elements("Season").Select(x => x.Value));
		result.Months.UnionWith(root.Elements("Month").Select(x => x.Value));
		long Identifier(string name)
		{
			if (root.Element(name) is not { } element) return 0;
			if (long.TryParse(element.Value, out var id) && id >= 0) return id;
			result.LoadError = $"invalid {name} reference in activity window";
			return 0;
		}
		int? Day(string name)
		{
			if (string.IsNullOrEmpty(root.Element(name)?.Value)) return null;
			if (int.TryParse(root.Element(name)!.Value, out var day) && day > 0) return day;
			result.LoadError = "invalid day in saved activity window";
			return null;
		}
		result.CalendarId = Identifier("Calendar"); result.MoonId = Identifier("Moon");
		result.ConditionProgId = Identifier("ConditionProg");
		result.FirstDay = Day("FirstDay"); result.LastDay = Day("LastDay");
		return result;
	}
}
