#nullable enable

namespace MudSharp.Celestial.Authored;

/// <summary>Example content, deliberately separate from numerical engine behaviour.</summary>
public static class AuthoredCelestialPresets
{
	public static IReadOnlyList<string> Names { get; } = Array.AsReadOnly(new[] { "RailSun", "RailMoon", "ScriptedSun", "ScriptedMoon", "MorningStar", "MorningGlow" });
	public static AuthoredCelestialDefinition Create(string preset, long calendarId, int minutesPerHour, int hoursPerDay)
	{
		AuthoredMath.Require(Names.Contains(preset, StringComparer.OrdinalIgnoreCase), "Unknown preset. Use celestial types to list presets.");
		var day = checked((long)minutesPerHour * hoursPerDay);
		AuthoredMath.Require(day >= 24, "Example presets require a clock day of at least 24 minutes.");
		var moon = preset.EndsWith("Moon", StringComparison.OrdinalIgnoreCase);
		var script = preset.StartsWith("Scripted", StringComparison.OrdinalIgnoreCase) || preset.StartsWith("Morning", StringComparison.OrdinalIgnoreCase);
		var d = new AuthoredCelestialDefinition
		{
			Kind = script ? moon ? AuthoredCelestialKind.ScriptedMoon : AuthoredCelestialKind.ScriptedSun : moon ? AuthoredCelestialKind.RailMoon : AuthoredCelestialKind.RailSun,
			CalendarId = calendarId, AnnualPeriod = checked(day * 360), TimeOfDay = !moon,
			Name = preset, SeederPreset = preset,
			Path = new() { Period = day, Rise = moon ? day * 3 / 4 : day / 4, Set = moon ? day / 4 : day * 3 / 4 },
			LightMode = moon ? AuthoredLightMode.PhaseScaled : AuthoredLightMode.Absolute,
			LightProfile = moon ? [new(-90, 0), new(0, 0), new(90, .4)] : [new(-90, 0), new(-12, 0), new(0, 10), new(90, 1000)],
			Phase = moon ? new() { Mode = AuthoredPhaseMode.Regular, Track = new() { Period = checked(day * 28) } } : null
		};
		if (script)
		{
			d.Path.Mode = AuthoredPathMode.Sparse;
			d.Path.Keys = moon
				? [new(0, 0, 90, TrackTransition.Hold), new(day, 0, 90)]
				: [new(0, 90, -30), new(day / 4, 90, 0), new(day / 2, 180, 70), new(day * 3 / 4, 270, 0), new(day, 90, -30)];
		}
		if (!preset.StartsWith("Morning", StringComparison.OrdinalIgnoreCase)) return d;
		var glow = preset.Equals("MorningGlow", StringComparison.OrdinalIgnoreCase);
		d.Name = glow ? "The Bound Radiance" : "The Morning Star";
		d.Description = "A struggling radiance rises, pauses against invisible bonds, and falls away.";
		// Examples use these explicit proportions of a feed-clock day. They are not engine timing constants.
		long Minute(long ordinaryMinute) => day * ordinaryMinute / 1440;
		var minutes = new long[] { 0, 300, 360, 540, 600, 610, 620, 1440 };
		var elevations = new double[] { -20, -20, -12, glow ? -1 : 2, glow ? -1 : 2, -12, -20, -20 };
		var lux = new double[] { 0, 0, 1, 50, 50, 1, 0, 0 };
		d.Path.Keys = minutes.Select((x, i) => new PositionKey(Minute(x), 90, elevations[i], i < 7 && elevations[i] == elevations[i + 1] ? TrackTransition.Hold : TrackTransition.Travel)).ToList();
		d.LightProfile.Clear();
		d.LightTrack = new() { Period = day, Keys = minutes.Select((x, i) => new ScalarKey(Minute(x), lux[i], i < 7 && lux[i] == lux[i + 1] ? TrackTransition.Hold : TrackTransition.Travel)).ToList() };
		var stories = new[]
		{
			("stir", 300L, "A pale disturbance stirs the eastern darkness.", true),
			("struggle", 360L, "Radiance gathers in the east, struggling against the darkness.", true),
			("bound", 540L, "The morning radiance strains against invisible bonds.", glow),
			("defeated", 600L, "The bound radiance falters, its struggle defeated.", glow),
			("extinguished", 620L, "The last trace of eastern radiance fades away.", true)
		};
		foreach (var (key, minute, text, sky) in stories)
		{
			d.Milestones.Add(new("custom:" + key, day, Minute(minute)));
			d.Echoes.Add(new(key, text, sky ? CelestialEchoAudience.SkyVisible : CelestialEchoAudience.BodyVisible, Milestone: "custom:" + key));
		}
		return d;
	}
}
