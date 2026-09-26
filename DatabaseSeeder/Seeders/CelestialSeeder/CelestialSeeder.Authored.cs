#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Database;
using EngineCompiler::MudSharp.Celestial.Authored;

namespace DatabaseSeeder.Seeders;

public partial class CelestialSeeder
{
	private static HashSet<string> AuthoredPresetsInstalled(FuturemudDatabaseContext context)
	{
		var types = Enum.GetNames<AuthoredCelestialKind>();
		return context.Celestials.Where(x => types.Contains(x.CelestialType)).AsEnumerable()
			.Select(x => AuthoredCelestialFormat.Parse(x.Definition).SeederPreset)
			.Where(x => x is not null)
			.ToHashSet(StringComparer.OrdinalIgnoreCase)!;
	}

	private static bool HasAuthoredPackage(FuturemudDatabaseContext context) =>
		AuthoredCelestialPresets.Names.All(AuthoredPresetsInstalled(context).Contains);

	private static void EnsureAuthoredPackage(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers)
	{
		var installed = AuthoredPresetsInstalled(context);
		var calendar = GetCalendar(context, answers["authoredcalendar"]);
		var clock = context.Clocks.Find(calendar.FeedClockId) ?? throw new InvalidOperationException("The authored calendar's clock is missing.");
		var xml = XElement.Parse(clock.Definition);
		int Dimension(string name) => int.Parse(xml.Element(name)?.Value ?? throw new InvalidOperationException($"The feed clock is missing {name}."), CultureInfo.InvariantCulture);
		var seconds = Dimension("SecondsPerMinute");
		var minutes = Dimension("MinutesPerHour");
		var hours = Dimension("HoursPerDay");
		foreach (var preset in AuthoredCelestialPresets.Names.Where(x => !installed.Contains(x)))
		{
			var definition = AuthoredCelestialPresets.Create(preset, calendar.Id, minutes, hours);
			var compiled = new CompiledAuthoredCelestial(definition, seconds, minutes, hours);
			context.Celestials.Add(new Celestial { CelestialType = definition.Kind.ToString(), FeedClockId = clock.Id, Definition = compiled.Serialize() });
		}
	}
}
