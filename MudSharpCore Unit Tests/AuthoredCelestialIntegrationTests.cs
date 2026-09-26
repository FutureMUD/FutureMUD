#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuthoredCelestialIntegrationTests
{
	[DataTestMethod]
	[DataRow(1440)]
	[DataRow(40000)]
	public void DenseDefinition_LargerThanLegacyText_LoadsAndCopiesWithProductionBudget(int samples)
	{
		var ctx = CelestialTestFactory.CreateEarthSystem();
		var definition = Definition(ctx, "ScriptedSun");
		definition.Path.Mode = AuthoredPathMode.Dense;
		definition.Path.Period = samples;
		definition.Path.Keys = Enumerable.Range(0, samples)
			.Select(i => new PositionKey(i, i % 360, 60 * Math.Sin(i * 2 * Math.PI / samples)))
			.ToList();
		var source = AuthoredCelestialFormat.Serialize(definition);
		Assert.IsTrue(Encoding.UTF8.GetByteCount(source) > 65535);
		using var celestial = AuthoredCelestial.Load(new()
		{
			Id = 902, CelestialType = "ScriptedSun", FeedClockId = ctx.Clock.Id, Definition = source
		}, ctx.Gameworld);
		Assert.AreEqual(samples, celestial.Compiled.CopyDefinition().Path.Keys.Count);
		Assert.AreEqual(source, celestial.Compiled.Serialize());
		Assert.AreEqual(0.0, celestial.EvaluateAt(Instant(0)).Elevation, 1e-10);
	}

	[TestMethod]
	public void SourceBudget_HonoursConfiguredLimits_AndDefaultsToSixteenMiB()
	{
		var world = new Mock<IFuturemud>();
		Assert.AreEqual(16 * 1024 * 1024, AuthoredCelestial.Limits(world.Object).SourceBytes);
		world.Setup(x => x.GetStaticConfiguration("AuthoredCelestialSourceBytes")).Returns("65535");
		Assert.AreEqual(65535, AuthoredCelestial.Limits(world.Object).SourceBytes);
		world.Setup(x => x.GetStaticConfiguration("AuthoredCelestialSourceBytes")).Returns("33554432");
		Assert.AreEqual(32 * 1024 * 1024, AuthoredCelestial.Limits(world.Object).SourceBytes);
		world.Setup(x => x.GetStaticConfiguration("AuthoredCelestialSourceBytes")).Returns("-1");
		Assert.AreEqual(16 * 1024 * 1024, AuthoredCelestial.Limits(world.Object).SourceBytes);
	}

	private static AuthoredCelestialDefinition Definition(CelestialTestContext context, string preset = "MorningStar") =>
		AuthoredCelestialPresets.Create(preset, context.Calendar.Id, context.Clock.MinutesPerHour, context.Clock.HoursPerDay);
	private static MudInstant Instant(long ticks) => new(MudInstant.CurrentEpoch, ticks, 1, 1);

	[DataTestMethod]
	[DataRow("RailSun")][DataRow("RailMoon")][DataRow("ScriptedSun")][DataRow("ScriptedMoon")]
	public void FourTypes_LoadEnvelopeAndEvaluate_IgnoreGeography(string preset)
	{
		var ctx = CelestialTestFactory.CreateEarthSystem();
		ctx.SetDateTime("1/jan/2010", 9, 0, 0);
		var definition = Definition(ctx, preset);
		using var celestial = AuthoredCelestial.Load(new() { Id = 901, CelestialType = definition.Kind.ToString(), Definition = AuthoredCelestialFormat.Serialize(definition), FeedClockId = 1 }, ctx.Gameworld);
		Assert.AreEqual(preset, celestial.GetType().Name);
		Assert.AreEqual(901L, celestial.Id);
		var state = celestial.EvaluateAt(ctx.Calendar.CurrentInstant);
		foreach (var geography in new[] { new GeographicCoordinate(0, 0, 0, 0), new GeographicCoordinate(-1.5, 3.1, 8000, 6000000), new GeographicCoordinate(1.5, -3.1, -3000, 9000000) })
		{
			Assert.AreEqual(state.Elevation, celestial.CurrentElevationAngle(geography));
			Assert.AreEqual(state.Azimuth, celestial.CurrentAzimuthAngle(geography, double.NaN));
			Assert.AreEqual(state.SourceLux, celestial.CurrentIllumination(geography));
			Assert.AreEqual(state.Direction, celestial.CurrentPosition(geography).Direction);
		}
		Assert.AreEqual(preset.EndsWith("Sun"), celestial.CelestialAngleIsUsedToDetermineTimeOfDay);
		Assert.IsFalse(celestial is ICelestialEphemeris, "Authored coordinates must never masquerade as physical ephemerides.");
	}

	[TestMethod]
	public void LiveMinute_Fanout1000_EvaluatesOnceAndDoesNotShareMutableWrappers()
	{
		var ctx = CelestialTestFactory.CreateEarthSystem();
		ctx.SetDateTime("1/jan/2010", 8, 59, 0);
		using var celestial = AuthoredCelestial.Create(50, Definition(ctx), ctx.Clock, ctx.Gameworld);
		var locations = Enumerable.Range(0, 1000).Select(_ => new Mock<ILocation>().Object).ToArray();
		var information = locations.Select(x => celestial.ReturnNewCelestialInformation(x, null!, ctx.ZeroGeography)).ToArray();
		celestial.MinuteUpdateEvent += sender =>
		{
			for (var i = 0; i < locations.Length; i++)
			{
				information[i] = celestial.ReturnNewCelestialInformation(locations[i], information[i], ctx.ZeroGeography);
				_ = celestial.CurrentIllumination(ctx.DefaultGeography);
			}
		};
		var before = celestial.IntrinsicEvaluationCount;
		ctx.Clock.CurrentTime.AddSeconds(60);
		Assert.AreEqual(before + 1, celestial.IntrinsicEvaluationCount);
		Assert.AreEqual(TimeOfDay.Dawn, celestial.CurrentTimeOfDay(ctx.ZeroGeography));
		Assert.AreEqual(CelestialMoveDirection.Ascending, information[0].Direction);
		information[0].Direction = CelestialMoveDirection.Descending;
		Assert.AreEqual(CelestialMoveDirection.Ascending, information[1].Direction);
		celestial.AddMinutes();
		Assert.AreEqual(before + 1, celestial.IntrinsicEvaluationCount);
	}

	private sealed class RecordingCelestial(long id, AuthoredCelestialDefinition definition, CelestialTestContext ctx)
		: AuthoredCelestial(id, definition, ctx.Clock, ctx.Gameworld)
	{
		public List<(ILocation Location, string Id)> Delivered { get; } = [];
		protected override void DeliverEcho(AuthoredEcho echo, ILocation location) => Delivered.Add((location, echo.Id));
	}

	[TestMethod]
	public void EchoLifecycle_TwoSubscribersQueriesJumpsAndRewind_ExactlyOncePerTraversal()
	{
		var ctx = CelestialTestFactory.CreateEarthSystem();
		ctx.SetDateTime("1/jan/2010", 0, 9, 0);
		var d = Definition(ctx, "ScriptedSun");
		d.Echoes = [new("b", "second", Order: 1, Period: 60, Minute: 10), new("a", "first", Order: 0, Period: 60, Minute: 10), new("c", "third", Period: 60, Minute: 12)];
		using var celestial = new RecordingCelestial(50, d, ctx);
		var a = new Mock<ILocation>().Object;
		var b = new Mock<ILocation>().Object;
		var ai = celestial.ReturnNewCelestialInformation(a, null!, ctx.ZeroGeography);
		var bi = celestial.ReturnNewCelestialInformation(b, null!, ctx.ZeroGeography);
		celestial.MinuteUpdateEvent += _ =>
		{
			ai = celestial.ReturnNewCelestialInformation(a, ai, ctx.ZeroGeography);
			bi = celestial.ReturnNewCelestialInformation(b, bi, ctx.ZeroGeography);
			celestial.ReturnNewCelestialInformation(a, ai, ctx.ZeroGeography);
		};
		ctx.Clock.CurrentTime.AddSeconds(60);
		CollectionAssert.AreEqual(new[] { "a", "b", "a", "b" }, celestial.Delivered.Select(x => x.Id).ToArray());
		var live = celestial.LiveState;
		celestial.EvaluateAt(Instant(-100));
		celestial.FindNext(Instant(0), new(AstronomicalEventType.Sunrise, 1000000));
		celestial.AddMinutes();
		celestial.ReturnNewCelestialInformation(new Mock<ILocation>().Object, null!, ctx.ZeroGeography);
		Assert.AreEqual(live, celestial.LiveState);
		Assert.AreEqual(4, celestial.Delivered.Count);
		ctx.Clock.CurrentTime.AddMinutes(2);
		Assert.AreEqual(4, celestial.Delivered.Count, "Bulk minute shift must be silent.");
		ctx.Clock.CurrentTime.SetTime(0, 9, 0);
		celestial.AddMinutes(-1000000);
		ctx.Clock.CurrentTime.AddSeconds(60);
		Assert.AreEqual(8, celestial.Delivered.Count);
		celestial.Activate(d);
		Assert.AreEqual(8, celestial.Delivered.Count);
		celestial.ForgetSubscriber(b);
		bi = celestial.ReturnNewCelestialInformation(b, null!, ctx.ZeroGeography);
		ctx.Clock.CurrentTime.AddSeconds(120);
		Assert.AreEqual(8, celestial.Delivered.Count, "Discontinuous tick must not replay the skipped batch.");
	}

	[TestMethod]
	public void Activation_InvalidCandidatePreservesSource_ValidEditRefreshesSilently()
	{
		var ctx = CelestialTestFactory.CreateEarthSystem();
		ctx.SetDateTime("1/jan/2010", 9, 59, 0);
		using var c = AuthoredCelestial.Create(50, Definition(ctx), ctx.Clock, ctx.Gameworld);
		var original = c.Compiled.Serialize(); var live = c.LiveState;
		var invalid = c.Compiled.CopyDefinition(); invalid.Path.Keys[3] = invalid.Path.Keys[3] with { Elevation = 99 };
		Assert.ThrowsException<ArgumentException>(() => c.Activate(invalid));
		Assert.AreEqual(original, c.Compiled.Serialize()); Assert.AreEqual(live, c.LiveState);
		var valid = c.Compiled.CopyDefinition(); valid.LightTrack!.Keys[3] = valid.LightTrack.Keys[3] with { Value = 80 }; valid.LightTrack.Keys[4] = valid.LightTrack.Keys[4] with { Value = 80 };
		c.Activate(valid);
		Assert.AreEqual(80, c.CurrentIllumination(ctx.ZeroGeography), 1e-10);
		Assert.AreEqual(CelestialMoveDirection.Ascending, c.LiveState.Direction);
	}

	[TestMethod]
	public void ObjectEventDispatch_AllCapabilities_StrictNextAndErrors()
	{
		var ctx = CelestialTestFactory.CreateEarthSystem();
		ctx.SetDateTime("1/jan/2010", 0, 0, 0);
		var sd = Definition(ctx, "RailSun"); sd.SyntheticLongitude = true;
		using var sun = AuthoredCelestial.Create(51, sd, ctx.Clock, ctx.Gameworld);
		var md = Definition(ctx, "RailMoon");
		md.Milestones = [new("custom:crescent", 40320, 24180)]; md.Phase!.CrescentSunId = 51; md.Phase.CrescentMilestones.Add("custom:crescent");
		using var moon = AuthoredCelestial.Create(52, md, ctx.Clock, ctx.Gameworld);
		CelestialEventResult Query(ICelestialObject target, CelestialEventRequest request, long ticks = 0, ICelestialObject? secondary = null) => AstronomicalEventService.Instance.FindNextForCelestial(Instant(ticks), request, target, ctx.ZeroGeography, secondary);
		Assert.AreEqual(360 * 60L, Query(sun, new(AstronomicalEventType.Sunrise)).Instant.Ticks);
		Assert.AreEqual((360 + 1440) * 60L, Query(sun, new(AstronomicalEventType.Sunrise), 360 * 60).Instant.Ticks);
		Assert.AreEqual(20160 * 60L, Query(moon, new(AstronomicalEventType.NewMoon)).Instant.Ticks);
		Assert.AreEqual(40320 * 60L, Query(moon, new(AstronomicalEventType.FullMoon)).Instant.Ticks);
		Assert.AreEqual(129600 * 60L, Query(sun, new(AstronomicalEventType.SolarLongitude, TargetLongitude: Math.PI / 2)).Instant.Ticks);
		Assert.AreEqual(24180 * 60L, Query(sun, new(AstronomicalEventType.VisibleCrescent), secondary: moon).Instant.Ticks);
		Assert.AreEqual(CelestialEventStatus.Unsupported, Query(ctx.Sun, new(AstronomicalEventType.VisibleCrescent), secondary: moon).Status);
		Assert.AreEqual(CelestialEventStatus.Unsupported, Query(sun, new(AstronomicalEventType.VisibleCrescent), secondary: ctx.Moon).Status);
		Assert.AreEqual(CelestialEventStatus.Unsupported, Query(moon, new(AstronomicalEventType.LunarConjunction)).Status);
		Assert.AreEqual(CelestialEventStatus.InvalidRequest, Query(sun, new(AstronomicalEventType.Sunrise, 0)).Status);
		Assert.AreEqual(CelestialEventStatus.IncompatibleTimeContext, sun.FindNext(new(MudInstant.CurrentEpoch, 0, 1, 999), new(AstronomicalEventType.Sunrise)).Status);
		Assert.AreEqual(CelestialEventStatus.IncompatibleTimeContext, sun.FindNext(new(0), new(AstronomicalEventType.Sunrise)).Status);
	}
}
