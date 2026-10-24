#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Models;
using EngineCompiler::MudSharp.Celestial.Authored;

namespace DatabaseSeeder.Seeders;

/// <summary>Validated native-clock units for authored seeder weather templates.</summary>
internal sealed record SeederWeatherTiming(long CelestialId, int HoursPerDay, int MinutesPerHour, double YearDays)
{
	internal static readonly int[] ConventionalOnsets = [334, 0, 31, 61, 92, 122, 153, 183, 214, 244, 274, 304];
	internal static readonly double[] ConventionalHeating =
		[.0866, .0472, .0236, .0079, 0, .0315, .126, .2835, .3622, .4409, .5197, .5984,
		 .6772, .7559, .8346, .8740, .9134, .9370, .9685, 1, .8346, .6772, .3622, .2047];
	internal static SeederWeatherTiming Canonical(long celestialId) => new(celestialId, 24, 60, 365);
	internal int MinutesPerDay => checked(HoursPerDay * MinutesPerHour);

	internal int DaysToMinutes(double days)
	{
		var minutes = Math.Round(days * MinutesPerDay, MidpointRounding.AwayFromZero);
		if (!double.IsFinite(minutes) || minutes < 1 || minutes > int.MaxValue)
			throw new InvalidOperationException("Weather fluctuation period cannot be represented in native clock minutes.");
		return (int)minutes;
	}

	internal int ScaleOnset(int originalDay) => checked((int)Math.Round(originalDay * YearDays / 365.0, MidpointRounding.AwayFromZero));

	internal void Validate()
	{
		if (HoursPerDay <= 0 || MinutesPerHour <= 0 || (long)HoursPerDay * MinutesPerHour > int.MaxValue ||
			!double.IsFinite(YearDays) || YearDays <= 0 || YearDays > int.MaxValue)
			throw new InvalidOperationException("Weather requires positive, representable clock dimensions and celestial year length.");
		var starts = ConventionalOnsets.Select(ScaleOnset).ToArray();
		if (starts.Distinct().Count() != starts.Length || starts.Any(x => x < 0 || x >= YearDays))
			throw new InvalidOperationException("This celestial year cannot represent twelve distinct integer season onsets.");
		_ = DaysToMinutes(8);
	}

	internal XElement Temperatures(double minimum, double maximum, IReadOnlyList<double>? factors = null)
	{
		factors ??= ConventionalHeating;
		return new XElement("Temperatures", Enumerable.Range(0, HoursPerDay).Select(hour =>
		{
			var position = (double)factors.Count * hour / HoursPerDay;
			var index = (int)Math.Floor(position);
			var factor = factors[index] + (factors[(index + 1) % factors.Count] - factors[index]) * (position - index);
			return new XElement("Value", new XAttribute("hour", hour), minimum + (maximum - minimum) * factor);
		}));
	}

	internal static IReadOnlyList<SeederWeatherTiming> Eligible(FuturemudDatabaseContext db) => db.Celestials
		.AsEnumerable().OrderBy(x => x.Id).Select(x => TryRead(db, x)).OfType<SeederWeatherTiming>().ToArray();

	internal static ConsoleQuestionDisplay Display(FuturemudDatabaseContext db, IReadOnlyDictionary<string, string> answers)
	{
		var candidates = Eligible(db);
		var lines = candidates.Select(timing =>
		{
			var row = db.Celestials.Find(timing.CelestialId)!;
			var name = row.CelestialType == "Sun" ? XElement.Parse(row.Definition).Element("Name")?.Value : AuthoredCelestialFormat.Parse(row.Definition).Name;
			return $"  {timing.CelestialId}: {name ?? row.CelestialType} ({timing.HoursPerDay} hours/day; {timing.YearDays.ToString(CultureInfo.InvariantCulture)} days/year)";
		});
		return new("Select the daylight celestial ID that governs weather seasons:\n" + string.Join("\n", lines),
			candidates.FirstOrDefault()?.CelestialId.ToString(CultureInfo.InvariantCulture));
	}

	internal static SeederWeatherTiming Resolve(FuturemudDatabaseContext db, IReadOnlyDictionary<string, string> answers)
	{
		var candidates = Eligible(db);
		if (answers.TryGetValue("weathercelestial", out var answer))
		{
			if (long.TryParse(answer, out var id) && candidates.SingleOrDefault(x => x.CelestialId == id) is { } selected) return selected;
			throw new InvalidOperationException("Select an eligible daylight celestial with a supported, valid feed clock and year.");
		}
		return candidates.Count == 1 ? candidates[0] : throw new InvalidOperationException(
			candidates.Count == 0 ? "No eligible daylight celestial has a supported clock and year. No weather was installed." :
			"Select the daylight celestial ID using weathercelestial before installing weather.");
	}

	private static SeederWeatherTiming? TryRead(FuturemudDatabaseContext db, Celestial celestial)
	{
		try
		{
			var clock = db.Clocks.Find(celestial.FeedClockId);
			if (clock is null) return null;
			var root = XElement.Parse(clock.Definition);
			var hours = int.Parse(root.Element("HoursPerDay")!.Value, CultureInfo.InvariantCulture);
			var minutes = int.Parse(root.Element("MinutesPerHour")!.Value, CultureInfo.InvariantCulture);
			if (int.Parse(root.Element("SecondsPerMinute")!.Value, CultureInfo.InvariantCulture) <= 0) return null;
			double year;
			if (celestial.CelestialType is "RailSun" or "ScriptedSun" or "RailMoon" or "ScriptedMoon")
			{
				var source = AuthoredCelestialFormat.Parse(celestial.Definition);
				if (!source.TimeOfDay || source.Kind.ToString() != celestial.CelestialType ||
					db.Calendars.Find(source.CalendarId) is not { } calendar || calendar.FeedClockId != clock.Id) return null;
				year = source.AnnualPeriod / ((double)hours * minutes);
			}
			else if (celestial.CelestialType == "Sun")
			{
				var source = XElement.Parse(celestial.Definition);
				if (!long.TryParse(source.Element("Calendar")?.Value, out var calendarId) ||
					db.Calendars.Find(calendarId) is not { } calendar || calendar.FeedClockId != clock.Id) return null;
				year = double.Parse(source.Element("Orbital")!.Element("CelestialDaysPerYear")!.Value, CultureInfo.InvariantCulture);
			}
			else return null; // Indirect satellite views need a separate explicit timing contract.
			var timing = new SeederWeatherTiming(celestial.Id, hours, minutes, year);
			timing.Validate();
			return timing;
		}
		catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or FormatException or OverflowException or System.Xml.XmlException or NullReferenceException or System.Text.Json.JsonException)
		{
			return null;
		}
	}
}
