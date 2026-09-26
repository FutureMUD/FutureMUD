#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuthoredCelestialValidationTests
{
	[TestMethod]
	public void RaisedSourceBudget_RemainsEffectiveWhenCopyingCompiledDefinition()
	{
		var definition = AuthoredCelestialNumericalTests.Basic();
		definition.Description = new string('a', 16 * 1024 * 1024);
		Assert.ThrowsException<ArgumentException>(() => new CompiledAuthoredCelestial(definition, 60, 60, 24));
		var compiled = new CompiledAuthoredCelestial(definition, 60, 60, 24, new(SourceBytes: 17 * 1024 * 1024));
		Assert.AreEqual(definition.Description.Length, compiled.CopyDefinition().Description.Length);
	}

	public static IEnumerable<object[]> InvalidDefinitions()
	{
		var changes = new (string, Action<AuthoredCelestialDefinition>)[]
		{
			("version", d => d.Version = 2), ("type", d => d.Kind = (AuthoredCelestialKind)999),
			("context", d => d.CalendarId = 0), ("anchor", d => d.AnchorTicks = 1),
			("zero period", d => d.Path.Period = 0), ("negative period", d => d.AnnualPeriod = -1),
			("period overflow", d => d.AnnualPeriod = long.MaxValue), ("nonfinite", d => d.LongitudeAtEpoch = double.NaN),
			("infinite", d => d.LightProfile[0] = new(-90, double.PositiveInfinity)),
			("negative lux", d => d.LightProfile[0] = new(-90, -1)), ("profile duplicate", d => d.LightProfile.Add(d.LightProfile[0])),
			("elevation", d => d.Path.MaximumElevation = 91), ("endpoints", d => d.Path.SetBearing = 240),
			("tilt", d => d.Path.TiltBearing = 90), ("same time", d => d.Path.Set = d.Path.Rise),
			("offset", d => d.Path.Rise = d.Path.Period), ("ineligible phase light", d => d.LightMode = AuthoredLightMode.PhaseScaled),
			("no light", d => d.LightProfile.Clear()), ("two lights", d => d.LightTrack = new() { Period = 2, Keys = [new(0, 1), new(2, 1)] }),
			("sun phase", d => d.Phase = new()), ("missing channel", d => d.Path = null!),
			("null keys", d => d.Path.Keys = null!), ("null entry", d => d.LightProfile.Add(null!)),
			("duplicate event", d => d.Milestones = [new("custom:x", 10, 1), new("custom:X", 10, 2)]),
			("reserved event", d => d.Milestones = [new("sunrise", 10, 1)]),
			("event offset", d => d.Milestones = [new("custom:x", 10, 10)]),
			("echo reference", d => d.Echoes = [new("x", "hello", Milestone: "custom:absent")]),
			("duplicate echo", d => d.Echoes = [new("x", "hello", Period: 10), new("x", "world", Period: 20)]),
			("ambiguous echo", d => d.Echoes = [new("x", "hello", Period: 10, Threshold: 0)])
		};
		foreach (var (name, change) in changes)
		{
			var d = AuthoredCelestialNumericalTests.Basic(); change(d);
			yield return [name, d];
		}
	}

	[DataTestMethod]
	[DynamicData(nameof(InvalidDefinitions), DynamicDataSourceType.Method)]
	public void InvalidSources_AreRejectedWithoutActivation(string category, AuthoredCelestialDefinition definition)
	{
		try { _ = new CompiledAuthoredCelestial(definition, 60, 60, 24); }
		catch (Exception ex) when (ex is ArgumentException or OverflowException or JsonException) { return; }
		Assert.Fail($"Accepted invalid {category} definition.");
	}

	[TestMethod]
	public void TrackValidation_RejectsClosureDuplicatesAntipodesDenseGapsAndPhaseWinding()
	{
		void Reject(Action<AuthoredCelestialDefinition> edit)
		{
			var d = AuthoredCelestialPresets.Create("ScriptedMoon", 1, 60, 24);
			edit(d);
			Assert.ThrowsException<ArgumentException>(() => new CompiledAuthoredCelestial(d, 60, 60, 24));
		}
		Reject(d => d.Path.Keys.Add(new(0, 0, 0)));
		Reject(d => d.Path.Keys[^1] = new(1440, 0, 80));
		Reject(d => d.Path.Keys = [new(0, 90, 0), new(720, 270, 0), new(1440, 90, 0)]);
		Reject(d => d.Path.Mode = AuthoredPathMode.Dense);
		Reject(d => d.Path.Keys = [new(0, 90, 0, TrackTransition.Hold), new(1440, 90, 20)]);
		Reject(d => { d.Phase!.Mode = AuthoredPhaseMode.Authored; d.Phase.Track = new() { Period = 10, Keys = [new(0, 0), new(10, .7)] }; });
		Reject(d => { d.Phase!.Mode = AuthoredPhaseMode.Authored; d.Phase.Track = new() { Period = 10, Keys = [new(0, 0), new(10, 2)] }; });
		Reject(d => { d.Phase!.Mode = AuthoredPhaseMode.Authored; d.Phase.Track = new() { Period = 10, Keys = [new(0, 0), new(0, .5), new(10, 1)] }; });
		Reject(d => d.Phase!.Track.Period = 1);
		Reject(d => d.Phase!.CrescentSunId = 99);
		Reject(d => { d.Phase!.CrescentSunId = 99; d.Phase.CrescentMilestones.Add("custom:missing"); });
		Reject(d => { d.LightProfile.Clear(); d.LightTrack = new() { Period = 10, Keys = [new(0, 1), new(10, 2)] }; });
		var rail = AuthoredCelestialNumericalTests.Basic(); rail.Path.Mode = AuthoredPathMode.ThreePointRail; rail.Path.SetBearing = rail.Path.RiseBearing;
		Assert.ThrowsException<ArgumentException>(() => new CompiledAuthoredCelestial(rail, 60, 60, 24));
		Assert.ThrowsException<ArgumentException>(() => new CompiledAuthoredCelestial(AuthoredCelestialNumericalTests.Basic(), 0, 60, 24));
	}

	[TestMethod]
	public void ResourceAndImportGuards_RejectBeforeExpansion_AndBoundOverflow()
	{
		var d = AuthoredCelestialNumericalTests.Basic();
		Assert.ThrowsException<ArgumentException>(() => new CompiledAuthoredCelestial(d, 60, 60, 24, new(SourceBytes: 100)));
		Assert.ThrowsException<ArgumentException>(() => new CompiledAuthoredCelestial(d, 60, 60, 24, new(Entries: 1)));
		Assert.ThrowsException<JsonException>(() => AuthoredCelestialFormat.Parse("<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///missing'>]><x>&e;</x>"));
		Assert.ThrowsException<JsonException>(() => AuthoredCelestialFormat.Parse("{\"NotAField\":1}"));
		var c = new CompiledAuthoredCelestial(d, 60, 60, 24);
		Assert.AreEqual(CelestialEventStatus.OutOfRange, c.FindNext(new(MudInstant.CurrentEpoch, 0, 1, 1), new(AstronomicalEventType.Sunrise, long.MaxValue)).Status);
		Assert.AreEqual(CelestialEventStatus.Found, c.FindNext(new(MudInstant.CurrentEpoch, 0, 1, 1), new(AstronomicalEventType.Sunrise, 1000000)).Status);
	}

	[TestMethod]
	public void SlowRailsAndShortestArc_PreserveDirectionAndDefaults()
	{
		var d = AuthoredCelestialNumericalTests.Basic();
		d.Path.Period = 10000000000000; d.Path.Rise = 0; d.Path.Set = d.Path.Period / 2;
		var c = new CompiledAuthoredCelestial(d, 60, 60, 24);
		var state = c.Evaluate(checked(d.Path.Period * 3 / 8 * 60));
		Assert.AreEqual(CelestialMoveDirection.Descending, state.Direction);
		Assert.AreEqual(CelestialMotion.Falling, state.Motion);
		Assert.AreEqual(TimeOfDay.Afternoon, AuthoredMath.TimeOfDay(state, true));
		Assert.IsFalse(new AuthoredCelestialDefinition { Kind = AuthoredCelestialKind.RailMoon }.TimeOfDay);
		Assert.IsFalse(new AuthoredCelestialDefinition { Kind = AuthoredCelestialKind.ScriptedMoon }.TimeOfDay);
		d.Kind = AuthoredCelestialKind.ScriptedSun;
		d.Path = new() { Mode = AuthoredPathMode.Sparse, Period = 4, Keys = [new(0, 359, 0), new(2, 1, 0, TrackTransition.Jump), new(4, 359, 0)] };
		c = new(d, 60, 60, 24); state = c.Evaluate(60);
		Assert.AreEqual(0, state.Azimuth, 1e-12); Assert.AreEqual(CelestialMotion.Level, state.Motion);
		Assert.AreEqual(CelestialEventStatus.NoFutureOccurrence, c.FindNext(new(0), new(AstronomicalEventType.Sunrise)).Status);
	}

	[TestMethod]
	public void RandomThreePointCircles_StayUnitAndOnIndependentPlane()
	{
		var random = new Random(650215);
		for (var trial = 0; trial < 100; trial++)
		{
			var rotation = random.NextDouble() * 360;
			var spread = 10 + random.NextDouble() * 140;
			var height = .1 + random.NextDouble() * 80;
			var d = AuthoredCelestialNumericalTests.Basic();
			d.Path.Mode = AuthoredPathMode.ThreePointRail; d.Path.Period = 1000; d.Path.Rise = 100; d.Path.Set = 700;
			d.Path.RiseBearing = rotation - spread; d.Path.SetBearing = rotation + spread; d.Path.ViaBearing = rotation; d.Path.ViaElevation = height;
			var c = new CompiledAuthoredCelestial(d, 60, 60, 24);
			// Rotating the horizontal frame by -rotation yields endpoints (sin +/-spread, cos spread, 0).
			// Therefore the circle plane is y + [(cos spread-cos height)/sin height] z = cos spread.
			var slope = (Math.Cos(spread * AuthoredMath.Radians) - Math.Cos(height * AuthoredMath.Radians)) / Math.Sin(height * AuthoredMath.Radians);
			for (var minute = 0; minute < 1000; minute += 13)
			{
				var v = c.Evaluate(minute * 60L).DirectionVector;
				var y = v.North * Math.Cos(rotation * AuthoredMath.Radians) + v.East * Math.Sin(rotation * AuthoredMath.Radians);
				Assert.AreEqual(Math.Cos(spread * AuthoredMath.Radians), y + slope * v.Up, 1e-10);
				Assert.AreEqual(1, v.East * v.East + v.North * v.North + v.Up * v.Up, 1e-11);
			}
		}
	}

	[TestMethod]
	public void DenseZeroPlateausRotatedSeams_MatchIndependentStrictSignOracle()
	{
		var random = new Random(92511);
		for (var trial = 0; trial < 200; trial++)
		{
			var signs = Enumerable.Range(0, random.Next(3, 30)).Select(_ => random.Next(3) - 1).ToArray();
			var d = AuthoredCelestialPresets.Create("ScriptedSun", 1, 60, 24);
			d.Path.Mode = AuthoredPathMode.Dense; d.Path.Period = signs.Length;
			d.Path.Keys = signs.Select((x, i) => new PositionKey(i, 90, x)).ToList();
			var c = new CompiledAuthoredCelestial(d, 60, 60, 24);
			foreach (var sign in new[] { -1, 1 })
			{
				var expected = new HashSet<long>();
				for (var i = 0; i < signs.Length; i++)
				{
					if (signs[i] != sign) continue;
					var prior = (i + signs.Length - 1) % signs.Length; var firstZero = i;
					while (signs[prior] == 0 && prior != i) { firstZero = prior; prior = (prior + signs.Length - 1) % signs.Length; }
					if (signs[prior] == -sign) expected.Add(firstZero * 60L);
				}
				CollectionAssert.AreEqual(expected.Order().ToArray(), c.EventOffsets(sign > 0 ? AstronomicalEventType.Sunrise : AstronomicalEventType.Sunset).ToArray());
			}
		}
	}

	[TestMethod]
	public void RandomRails_IndependentVectorIdentitiesEndpointsAndPeriodicity()
	{
		var random = new Random(910347);
		for (var i = 0; i < 100; i++)
		{
			var d = AuthoredCelestialNumericalTests.Basic();
			d.Path.Period = random.Next(20, 3000); d.Path.Rise = random.Next((int)d.Path.Period);
			d.Path.Set = (d.Path.Rise + random.Next(1, (int)d.Path.Period)) % d.Path.Period;
			d.Path.RiseBearing = random.NextDouble() * 720 - 360; d.Path.SetBearing = d.Path.RiseBearing + 180;
			d.Path.TiltBearing = d.Path.RiseBearing + (i % 2 == 0 ? 90 : -90); d.Path.MaximumElevation = 1 + random.NextDouble() * 89;
			var c = new CompiledAuthoredCelestial(d, 60, 60, 24);
			var duration = AuthoredMath.Mod(d.Path.Set - d.Path.Rise, d.Path.Period);
			for (var n = 0; n < 10; n++)
			{
				var m = random.Next((int)duration); var theta = Math.PI * m / duration;
				var state = c.Evaluate((d.Path.Rise + m) * 60);
				Assert.AreEqual(Math.Sin(theta) * Math.Sin(d.Path.MaximumElevation * AuthoredMath.Radians), state.DirectionVector.Up, 1e-12);
				Assert.AreEqual(1, state.DirectionVector.East * state.DirectionVector.East + state.DirectionVector.North * state.DirectionVector.North + state.DirectionVector.Up * state.DirectionVector.Up, 1e-12);
				Assert.AreEqual(state.DirectionVector, c.Evaluate((d.Path.Rise + m - d.Path.Period) * 60).DirectionVector);
			}
			Assert.AreEqual(0, c.Evaluate(d.Path.Rise * 60).Elevation, 1e-12);
			Assert.AreEqual(0, c.Evaluate(d.Path.Set * 60).Elevation, 1e-12);
		}
	}
}
