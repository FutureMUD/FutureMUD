#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuthoredCelestialNumericalTests
{
	[DataTestMethod]
	[DataRow("R1")][DataRow("R2")][DataRow("R3")][DataRow("S1")][DataRow("S2")][DataRow("S3")]
	[DataRow("H1")][DataRow("H2")][DataRow("H3")][DataRow("M1")][DataRow("M2")][DataRow("M3")][DataRow("C1")][DataRow("A1")]
	public void SuppliedFixture_ProductionCompilation_MatchesIndependentExpectedValues(string id)
	{
		using var document = JsonDocument.Parse(File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "Fixtures", "authored-celestial-test-fixtures.json")));
		var fixture = document.RootElement.GetProperty("fixtures").EnumerateArray().Single(x => x.GetProperty("id").GetString() == id);
		var definition = FromFixture(fixture);
		var seconds = id == "C1" ? 100 : 60;
		var compiled = new CompiledAuthoredCelestial(definition, seconds, id == "C1" ? 80 : 60, id == "C1" ? 20 : 24);
		if (fixture.TryGetProperty("expected_states", out var states))
		foreach (var expected in states.EnumerateArray())
		{
			var minute = expected.GetProperty("minute").GetInt64();
			var actual = compiled.Evaluate(minute * seconds);
			void Near(string property, double value, double tolerance)
			{
				if (expected.TryGetProperty(property, out var number) && number.ValueKind != JsonValueKind.Null)
					Assert.AreEqual(number.GetDouble(), value, tolerance, $"{id} minute {minute}: {property}");
			}
			Near("azimuth_degrees", actual.Azimuth / AuthoredMath.Radians, 1e-8);
			Near("elevation_degrees", actual.Elevation / AuthoredMath.Radians, 1e-8);
			Near("intrinsic_lux", actual.SourceLux, 1e-9);
			Near("phase_turns", actual.Phase?.Turns ?? -1, 1e-10);
			Near("phase_fraction", actual.Phase?.IlluminationFraction ?? -1, 1e-10);
			if (expected.TryGetProperty("unit_vector_enu", out var vector))
			{
				Assert.AreEqual(vector[0].GetDouble(), actual.DirectionVector.East, 1e-10);
				Assert.AreEqual(vector[1].GetDouble(), actual.DirectionVector.North, 1e-10);
				Assert.AreEqual(vector[2].GetDouble(), actual.DirectionVector.Up, 1e-10);
			}
			if (expected.TryGetProperty("compatibility_direction", out var direction)) Assert.AreEqual(direction.GetString(), actual.Direction.ToString(), $"{id}/{minute}");
			if (expected.TryGetProperty("time_of_day", out var tod)) Assert.AreEqual(tod.GetString(), AuthoredMath.TimeOfDay(actual, compiled.Eligible).ToString(), $"{id}/{minute}");
			if (expected.TryGetProperty("phase_name", out var phase)) Assert.AreEqual(phase.GetString(), actual.Phase?.Name.ToString());
			if (expected.TryGetProperty("motion", out var motion))
			{
				var mapped = motion.GetString() switch { "Ascending" => CelestialMotion.Rising, "Descending" => CelestialMotion.Falling, _ => CelestialMotion.Stationary };
				Assert.AreEqual(mapped, actual.Motion, $"{id}/{minute}");
			}
			Assert.AreEqual(actual, compiled.Evaluate(minute * seconds + seconds - 1), "State must hold for the entire feed minute.");
		}
		if (fixture.TryGetProperty("expected_events", out var events))
		foreach (var (property, kind) in new[] { ("rise_minutes", AstronomicalEventType.Sunrise), ("set_minutes", AstronomicalEventType.Sunset), ("new_moon_minutes", AstronomicalEventType.NewMoon), ("full_moon_minutes", AstronomicalEventType.FullMoon) })
		{
			if (events.TryGetProperty(property, out var offsets))
				CollectionAssert.AreEqual(offsets.EnumerateArray().Select(x => x.GetInt64() * seconds).ToArray(), compiled.EventOffsets(kind).ToArray(), $"{id}/{property}");
		}
		if (fixture.TryGetProperty("event_queries", out var queries))
		foreach (var query in queries.EnumerateArray())
		{
			var type = query.GetProperty("event").GetString() switch { "rise" => AstronomicalEventType.Sunrise, "new_moon" => AstronomicalEventType.NewMoon, "full_moon" => AstronomicalEventType.FullMoon, _ => AstronomicalEventType.SolarLongitude };
			var result = compiled.FindNext(new(query.GetProperty("reference_minute").GetInt64() * seconds), new(type, query.GetProperty("occurrence").GetInt64(), query.TryGetProperty("longitude_degrees", out var longitude) ? longitude.GetDouble() * AuthoredMath.Radians : 0));
			Assert.AreEqual(CelestialEventStatus.Found, result.Status, result.Error);
			Assert.AreEqual(query.GetProperty("expected_minute").GetInt64() * seconds, result.Instant.Ticks);
		}
		if (fixture.TryGetProperty("negative_epoch_cases", out var negatives))
		foreach (var row in negatives.EnumerateArray())
		{
			var ticks = row.GetProperty("tick").GetInt64();
			Assert.AreEqual(row.GetProperty("expected_absolute_minute").GetInt64(), AuthoredMath.FloorDiv(ticks, seconds));
			Assert.AreEqual(row.GetProperty("expected_cycle_minute").GetInt64(), compiled.Evaluate(ticks).PathMinute);
		}
		if (fixture.TryGetProperty("expected_longitudes", out var longitudes))
		foreach (var row in longitudes.EnumerateArray()) Assert.AreEqual(row.GetProperty("degrees").GetDouble(), compiled.Evaluate(row.GetProperty("day").GetInt64() * 86400).SyntheticLongitude!.Value / AuthoredMath.Radians, 1e-8);
		var roundTrip = new CompiledAuthoredCelestial(AuthoredCelestialFormat.Parse(compiled.Serialize()), seconds, id == "C1" ? 80 : 60, id == "C1" ? 20 : 24);
		foreach (var ticks in new long[] { -123456, 0, 123456, 1000000000 }) Assert.AreEqual(compiled.Evaluate(ticks), roundTrip.Evaluate(ticks));
	}

	internal static AuthoredCelestialDefinition Basic(bool moon = false) => new()
	{
		Kind = moon ? AuthoredCelestialKind.RailMoon : AuthoredCelestialKind.RailSun,
		CalendarId = 1, AnnualPeriod = 360 * 1440, TimeOfDay = !moon,
		Path = new() { Period = 1440, Rise = moon ? 1080 : 360, Set = moon ? 360 : 1080 },
		LightProfile = [new(-90, 0), new(-12, 0), new(0, 10), new(90, 1000)],
		Phase = moon ? new() { Mode = AuthoredPhaseMode.Regular, Track = new() { Period = 40320 } } : null
	};

	internal static AuthoredCelestialDefinition FromFixture(JsonElement f)
	{
		var d = Basic(f.GetProperty("kind").GetString()!.EndsWith("Moon"));
		d.Kind = Enum.Parse<AuthoredCelestialKind>(f.GetProperty("kind").GetString()!);
		d.Name = f.GetProperty("name").GetString()!;
		if (f.TryGetProperty("path", out var p))
		{
			d.Path.Period = p.GetProperty("period_minutes").GetInt64();
			switch (p.GetProperty("mode").GetString())
			{
				case "simple_rail":
					d.Path.Rise = p.GetProperty("rise_minute").GetInt64(); d.Path.Set = p.GetProperty("set_minute").GetInt64();
					d.Path.RiseBearing = p.GetProperty("rise_bearing_degrees").GetDouble(); d.Path.SetBearing = p.GetProperty("set_bearing_degrees").GetDouble();
					d.Path.MaximumElevation = p.GetProperty("maximum_elevation_degrees").GetDouble();
					if (p.TryGetProperty("tilt_bearing_degrees", out var tilt)) d.Path.TiltBearing = tilt.GetDouble();
					break;
				case "three_point_circle":
					d.Path.Mode = AuthoredPathMode.ThreePointRail;
					d.Path.Rise = p.GetProperty("rise_minute").GetInt64(); d.Path.Set = p.GetProperty("set_minute").GetInt64();
					d.Path.RiseBearing = p.GetProperty("start")[0].GetDouble(); d.Path.SetBearing = p.GetProperty("end")[0].GetDouble();
					d.Path.ViaBearing = p.GetProperty("via")[0].GetDouble(); d.Path.ViaElevation = p.GetProperty("via")[1].GetDouble();
					break;
				case "dense_samples":
					d.Path.Mode = AuthoredPathMode.Dense;
					d.Path.Keys = p.GetProperty("elevation_degrees").EnumerateArray().Select((x, i) => new PositionKey(i, p.GetProperty("azimuth_degrees").GetDouble(), x.GetDouble(), TrackTransition.Jump)).ToList();
					break;
				case "keyframed":
					d.Path.Mode = AuthoredPathMode.Sparse;
					d.Path.Keys = p.GetProperty("elevation_keyframes").EnumerateArray().Select(x => new PositionKey(x.GetProperty("minute").GetInt64(), p.GetProperty("azimuth_degrees").GetDouble(), x.GetProperty("degrees").GetDouble())).ToList();
					break;
				case "stationary":
					d.Path.Mode = AuthoredPathMode.Sparse;
					d.Path.Keys = [new(0, p.GetProperty("azimuth_degrees").GetDouble(), p.GetProperty("elevation_degrees").GetDouble(), TrackTransition.Hold), new(d.Path.Period, p.GetProperty("azimuth_degrees").GetDouble(), p.GetProperty("elevation_degrees").GetDouble())];
					break;
			}
		}
		if (f.TryGetProperty("lighting", out var light))
		{
			d.LightProfile = [];
			d.LightMode = light.GetProperty("mode").GetString()!.StartsWith("phase_scaled") ? AuthoredLightMode.PhaseScaled : AuthoredLightMode.Absolute;
			if (light.TryGetProperty("keyframes", out var keys)) d.LightTrack = new() { Period = d.Path.Period, Keys = keys.EnumerateArray().Select(x => new ScalarKey(x.GetProperty("minute").GetInt64(), x.GetProperty("lux").GetDouble())).ToList() };
			else if (light.TryGetProperty("elevation_lux_knots", out var knots)) d.LightProfile = knots.EnumerateArray().Select(x => new LightKnot(x[0].GetDouble(), x[1].GetDouble())).ToList();
			else d.LightProfile = [new(-90, light.GetProperty("base_lux").GetDouble()), new(90, light.GetProperty("base_lux").GetDouble())];
		}
		if (f.TryGetProperty("phase", out var phase))
		{
			d.Phase = new() { Mode = phase.GetProperty("mode").GetString() == "regular" ? AuthoredPhaseMode.Regular : AuthoredPhaseMode.Authored, Track = new() { Period = phase.GetProperty("period_minutes").GetInt64() } };
			if (phase.TryGetProperty("keyframes", out var keys))
			{
				var transition = phase.GetProperty("interpolation").GetString()!.StartsWith("hold then jump") ? TrackTransition.Jump : TrackTransition.Travel;
				d.Phase.Track.Keys = keys.EnumerateArray().Select(x => new ScalarKey(x.GetProperty("minute").GetInt64(), x.GetProperty("turns").GetDouble(), transition)).ToList();
			}
		}
		if (f.TryGetProperty("annual", out var annual))
		{
			d.AnnualPeriod = annual.GetProperty("period_minutes").GetInt64();
			d.SyntheticLongitude = annual.GetProperty("solar_longitude_enabled").GetBoolean();
			d.LongitudeAtEpoch = annual.GetProperty("longitude_at_epoch_degrees").GetDouble();
		}
		if (f.TryGetProperty("echoes", out var echoes))
		foreach (var echo in echoes.EnumerateArray())
		{
			var key = echo.GetProperty("id").GetString()!;
			d.Milestones.Add(new("custom:" + key, d.Path.Period, echo.GetProperty("minute").GetInt64()));
			d.Echoes.Add(new(key, key, echo.TryGetProperty("audience", out var audience) ? Enum.Parse<CelestialEchoAudience>(audience.GetString()!) : CelestialEchoAudience.BodyVisible, echo.GetProperty("order").GetInt32(), "custom:" + key));
		}
		return d;
	}

	[TestMethod]
	public void SparseAndDenseCrossings_RandomSmallCycles_MatchSampledOracle()
	{
		var random = new Random(8712);
		for (var trial = 0; trial < 150; trial++)
		{
			var d = Basic();
			d.Kind = AuthoredCelestialKind.ScriptedSun;
			d.Path.Mode = AuthoredPathMode.Sparse;
			d.Path.Period = 20;
			d.Path.Keys = Enumerable.Range(0, 5).Select(i => new PositionKey(i * 4, random.Next(360), random.Next(-60, 61), (TrackTransition)(random.Next(2) * 2))).ToList();
			d.Path.Keys.Add(d.Path.Keys[0] with { Minute = 20 });
			var c = new CompiledAuthoredCelestial(d, 60, 60, 24);
			var signs = Enumerable.Range(0, 20).Select(i => AuthoredMath.Sign(c.Evaluate(i * 60).Elevation)).ToArray();
			foreach (var sign in new[] { 1, -1 })
			{
				var expected = new List<long>();
				for (var i = 0; i < 20; i++)
				{
					var previous = (i + 19) % 20;
					if (signs[i] != sign) continue;
					var start = i;
					while (signs[previous] == 0 && previous != i) { start = previous; previous = (previous + 19) % 20; }
					if (signs[previous] == -sign) expected.Add(start * 60);
				}
				CollectionAssert.AreEqual(expected.Distinct().Order().ToArray(), c.EventOffsets(sign > 0 ? AstronomicalEventType.Sunrise : AstronomicalEventType.Sunset).ToArray(), $"Trial {trial}");
			}
		}
	}

	[TestMethod]
	public void RelativelyPrimeChannels_KeepIndependentStorageAndSkyEvaluation()
	{
		var d = Basic();
		d.Kind = AuthoredCelestialKind.ScriptedMoon;
		d.Path = new() { Mode = AuthoredPathMode.Sparse, Period = 1_000_000_007, Keys = [new(0, 90, -20), new(333_333_333, 90, 30), new(777_777_777, 270, 30), new(1_000_000_007, 90, -20)] };
		d.LightProfile.Clear();
		d.LightTrack = new() { Period = 1_000_000_009, Keys = [new(0, 1), new(500_000_000, 2), new(1_000_000_009, 1)] };
		d.LightMode = AuthoredLightMode.Absolute;
		d.Phase = new() { Mode = AuthoredPhaseMode.Regular, Track = new() { Period = 1_000_000_033 } };
		d.AnnualPeriod = 1_000_000_087;
		var c = new CompiledAuthoredCelestial(d, 60, 60, 24);
		Assert.IsTrue(c.StoredEntries < 20, "Storage must track source entries, not the channels' combined repeat period.");
		var changed = c.CopyDefinition();
		changed.AnnualPeriod = 1_000_000_093;
		changed.Phase!.Track.Period = 1_000_000_097;
		var other = new CompiledAuthoredCelestial(changed, 60, 60, 24);
		foreach (var minute in new[] { -1L, 0L, 777_777_777L, 5_000_000_043L })
		{
			var a = c.Evaluate(minute * 60); var b = other.Evaluate(minute * 60);
			Assert.AreEqual(a.DirectionVector, b.DirectionVector);
			Assert.AreEqual(a.SourceLux, b.SourceLux);
		}
	}

	[TestMethod]
	public void VeryLongSparsePath_CompilesWithoutExpandingPeriod()
	{
		var d = Basic();
		d.Kind = AuthoredCelestialKind.ScriptedSun;
		d.Path = new() { Mode = AuthoredPathMode.Sparse, Period = 1_000_000_000_000, Keys = [new(0, 90, -20), new(250_000_000_000, 90, 30), new(750_000_000_000, 270, 30), new(1_000_000_000_000, 90, -20)] };
		var c = new CompiledAuthoredCelestial(d, 100, 80, 20);
		Assert.AreEqual(3, c.Path.StoredEntries);
		Assert.IsTrue(c.StoredEntries < 20);
		Assert.IsTrue(c.FindNext(new(0), new(AstronomicalEventType.Sunrise, 1000)).Found);
	}
}
