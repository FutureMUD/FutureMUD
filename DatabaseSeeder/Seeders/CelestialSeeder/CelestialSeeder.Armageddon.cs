#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Models;
using EngineCompiler::MudSharp.Celestial.Authored;

namespace DatabaseSeeder.Seeders;

public partial class CelestialSeeder
{
	private const int ArmageddonDayMinutes = 540;
	private const int ArmageddonMonthMinutes = 231 * ArmageddonDayMinutes;
	private const int ArmageddonYearMinutes = 3 * ArmageddonMonthMinutes;
	private static readonly string[] ArmageddonPresets = ["ArmageddonSun", "ArmageddonJihae", "ArmageddonLirathu"];

	internal static string ResolveInstallArmageddonDefault(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> _) =>
		ArmageddonCalendar(context) is not null ? "yes" : "no";

	private static bool HasArmageddonPackage(FuturemudDatabaseContext context) =>
		ArmageddonPresets.All(AuthoredPresetsInstalled(context).Contains);

	private static Calendar? ArmageddonCalendar(FuturemudDatabaseContext context) =>
		context.Calendars.AsEnumerable().FirstOrDefault(x =>
			XElement.Parse(x.Definition).Element("alias")?.Value.Equals("armageddon", StringComparison.OrdinalIgnoreCase) == true);

	private static (bool Success, string error) ValidateArmageddonInstall(string answer, FuturemudDatabaseContext context)
	{
		var valid = ValidateYesNo(answer, context);
		if (!valid.Success || !answer.Equals("yes", StringComparison.OrdinalIgnoreCase)) return valid;
		try
		{
			_ = RequireArmageddonCalendar(context);
			return (true, string.Empty);
		}
		catch (InvalidOperationException ex)
		{
			return (false, ex.Message);
		}
	}

	internal static Calendar RequireArmageddonCalendar(FuturemudDatabaseContext context)
	{
		var calendar = ArmageddonCalendar(context) ?? throw new InvalidOperationException(
			"Install the Armageddon option in Time Seeder before installing its celestial package.");
		var root = XElement.Parse(calendar.Definition);
		var months = root.Element("months")?.Elements("month").ToArray() ?? [];
		var clock = context.Clocks.Find(calendar.FeedClockId);
		var dimensions = clock is null ? null : XElement.Parse(clock.Definition);
		if (months.Length != 3 || months.Any(x => x.Element("normaldays")?.Value != "231" ||
		        x.Element("intercalarydays")?.HasElements == true || x.Element("nonweekdays")?.HasElements == true) ||
		    root.Element("intercalarymonths")?.HasElements == true || root.Element("epochyear")?.Value != "1" ||
		    root.Element("algorithm")?.Attribute("type")?.Value != "fixed-months" ||
		    root.Element("dayboundary")?.Attribute("type")?.Value != "ClockMidnight" ||
		    dimensions?.Element("SecondsPerMinute")?.Value != "60" ||
		    dimensions.Element("MinutesPerHour")?.Value != "60" || dimensions.Element("HoursPerDay")?.Value != "9")
		{
			throw new InvalidOperationException("The Armageddon sky requires its midnight-based, epoch-year-1, three-by-231-day calendar and nine-hour, 60-by-60 clock. Existing customisations were not changed.");
		}

		return calendar;
	}

	private static void EnsureArmageddonPackage(FuturemudDatabaseContext context)
	{
		var calendar = RequireArmageddonCalendar(context);
		var installed = AuthoredPresetsInstalled(context);
		foreach (var preset in ArmageddonPresets.Where(x => !installed.Contains(x)))
		{
			var definition = BuildArmageddonCelestial(preset, calendar.Id);
			var compiled = new CompiledAuthoredCelestial(definition, 60, 60, 9);
			context.Celestials.Add(new Celestial
			{
				CelestialType = definition.Kind.ToString(), FeedClockId = calendar.FeedClockId,
				Definition = compiled.Serialize()
			});
		}
	}

	private static AuthoredCelestialDefinition BuildArmageddonCelestial(string preset, long calendarId)
	{
		var sun = preset == ArmageddonPresets[0];
		var jihae = preset == ArmageddonPresets[1];
		var numerator = sun ? 1 : jihae ? 6 : 10;
		var denominator = sun ? 1 : jihae ? 7 : 11;
		var definition = new AuthoredCelestialDefinition
		{
			Kind = sun ? AuthoredCelestialKind.ScriptedSun : AuthoredCelestialKind.ScriptedMoon,
			Name = sun ? "Suk-Krath" : jihae ? "Jihae" : "Lirathu",
			Description = sun ? "The fierce red sun of Zalanthas follows its descending, low and ascending arcs."
				: jihae ? "The red moon Jihae follows its appointed course across the Zalanthan sky."
				: "The white moon Lirathu follows its appointed course across the Zalanthan sky.",
			CalendarId = calendarId, AnchorTicks = 0, AnnualPeriod = ArmageddonYearMinutes,
			TimeOfDay = sun, SeederPreset = preset,
			Path = new AuthoredPathDefinition { Mode = AuthoredPathMode.Sparse, Period = ArmageddonYearMinutes },
			LightMode = sun ? AuthoredLightMode.Absolute : AuthoredLightMode.PhaseScaled,
			// Starter light tuning, not measurements recovered from Armageddon.
			LightProfile = sun ? [new(-90, 0), new(-12, 0), new(0, 400), new(30, 70000), new(60, 98000)]
				: [new(-90, 0), new(0, 0), new(30, jihae ? 0.15 : 0.3), new(60, jihae ? 0.3 : 0.6)],
			Milestones = [new("custom:conjunction", 77L * ArmageddonDayMinutes, 0)]
		};

		// At each instant all bodies share the same inclined, injective closed track.
		// S=t, J=6t/7, L=10t/11: equal positions require t/7 and t/11 integral.
		// Fifteen-minute keys include every solar horizon, transit and conjunction.
		for (var minute = 0; minute <= ArmageddonYearMinutes; minute += 15)
		{
			var yearMinute = minute % ArmageddonYearMinutes;
			var peak = yearMinute < ArmageddonMonthMinutes ? 60.0 - 30.0 * yearMinute / ArmageddonMonthMinutes
				: yearMinute < 2 * ArmageddonMonthMinutes ? 30.0
				: 30.0 + 30.0 * (yearMinute - 2 * ArmageddonMonthMinutes) / ArmageddonMonthMinutes;
			var fraction = (double)((long)minute * numerator % (ArmageddonDayMinutes * denominator)) /
			               (ArmageddonDayMinutes * denominator);
			var angle = fraction < 1.0 / 9 ? -Math.PI / 2 + fraction * 9 * Math.PI / 2
				: fraction <= 8.0 / 9 ? (fraction - 1.0 / 9) * 9 * Math.PI / 7
				: Math.PI + (fraction - 8.0 / 9) * 9 * Math.PI / 2;
			var sin = Math.Sin(angle);
			if (Math.Abs(sin) < 1e-14) sin = 0;
			var radians = peak * Math.PI / 180;
			var east = Math.Cos(angle);
			var north = -Math.Cos(radians) * sin;
			var up = Math.Sin(radians) * sin;
			var bearing = (Math.Atan2(east, north) * 180 / Math.PI + 360) % 360;
			definition.Path.Keys.Add(new PositionKey(minute, bearing, Math.Asin(up) * 180 / Math.PI));
		}

		if (!sun)
		{
			definition.Phase = new AuthoredPhaseDefinition
			{
				Mode = AuthoredPhaseMode.Authored,
				Track = new AuthoredScalarTrack { Period = ArmageddonMonthMinutes }
			};
			for (var day = 1; day <= 232; day++)
			{
				var sourceDay = day == 232 ? 1 : day;
				var phase = ArmageddonPhase(sourceDay, jihae);
				definition.Phase.Track.Keys.Add(new ScalarKey((day - 1L) * ArmageddonDayMinutes, phase, TrackTransition.Jump));
			}
		}

		return definition;
	}

	private static double ArmageddonPhase(int day, bool jihae)
	{
		// Exact source day bands; the two crescent thicknesses use distinct fractions
		// within the corresponding native named phase, without inventing extra phases.
		if (!jihae)
		{
			return day switch
			{
				<= 2 or >= 230 => .5,
				<= 30 => .58, <= 59 => .65, <= 86 => .75, <= 114 => .875,
				<= 118 => 0, <= 146 => .125, <= 173 => .25, <= 201 => .35, _ => .42
			};
		}

		return day switch
		{
			<= 8 => .125, <= 22 => .25, <= 35 => .35, <= 48 => .42, <= 51 => .5,
			<= 64 => .58, <= 77 => .65, <= 91 => .75, <= 105 => .875, <= 109 => 0,
			<= 123 => .125, <= 137 => .25, <= 150 => .35, <= 163 => .42, <= 167 => .5,
			<= 180 => .58, <= 193 => .65, <= 207 => .75, <= 221 => .875, <= 225 => 0, _ => .125
		};
	}
}
