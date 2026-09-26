#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuthoredCelestialWorldTests
{
	private static Zone Zone(CelestialTestContext ctx, List<ICelestialObject> bodies, long id, double latitude, double longitude, double pollution)
	{
		var shard = new Mock<IShard>(); shard.SetupGet(x => x.Id).Returns(id);
		shard.SetupGet(x => x.Celestials).Returns(() => bodies); shard.SetupGet(x => x.Clocks).Returns(new[] { ctx.Clock });
		shard.SetupGet(x => x.SphericalRadiusMetres).Returns(id * 1000000.0);
		var shards = new All<IShard>(); shards.Add(shard.Object);
		Mock.Get(ctx.Gameworld).SetupGet(x => x.Shards).Returns(shards);
		Mock.Get(ctx.Gameworld).SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController>());
		return new(new() { Id = id, Name = "Authored Test", ShardId = id, Latitude = latitude, Longitude = longitude, Elevation = id * 300, AmbientLightPollution = pollution }, ctx.Gameworld);
	}

	[TestMethod]
	public void RealZones_PreserveFirstAuthorityLightModifiersDetachAndTimezoneProjection()
	{
		var ctx = CelestialTestFactory.CreateEarthSystem(); ctx.SetDateTime("1/jan/2010", 9, 0, 0);
		using var sun = AuthoredCelestial.Create(50, AuthoredCelestialPresets.Create("MorningStar", 1, 60, 24), ctx.Clock, ctx.Gameworld);
		using var moon = AuthoredCelestial.Create(51, AuthoredCelestialPresets.Create("ScriptedMoon", 1, 60, 24), ctx.Clock, ctx.Gameworld);
		var bodies = new List<ICelestialObject> { ctx.Sun, sun, moon };
		var west = Zone(ctx, bodies, 1, -.6, -3, 1); var east = Zone(ctx, bodies, 2, .8, 3, 9);
		var expectedPhysical = ctx.Sun.CurrentPosition(west.Geography);
		var expected = expectedPhysical.LastAscensionAngle > .05 ? expectedPhysical.Direction == CelestialMoveDirection.Ascending ? TimeOfDay.Morning : TimeOfDay.Afternoon : expectedPhysical.LastAscensionAngle < -.20944 ? TimeOfDay.Night : expectedPhysical.Direction == CelestialMoveDirection.Ascending ? TimeOfDay.Dawn : TimeOfDay.Dusk;
		Assert.AreEqual(expected, west.CurrentTimeOfDay);
		bodies.Remove(ctx.Sun); west.InitialiseCelestials(); east.InitialiseCelestials();
		Assert.AreEqual(TimeOfDay.Dawn, west.CurrentTimeOfDay); Assert.AreEqual(TimeOfDay.Dawn, east.CurrentTimeOfDay);
		Assert.AreEqual(sun.LiveState.Elevation, west.GetInfo(sun).LastAscensionAngle);
		Assert.AreEqual(sun.LiveState.Elevation, east.GetInfo(sun).LastAscensionAngle);
		Assert.AreEqual(8, east.CurrentLightLevel - west.CurrentLightLevel, 1e-10);
		west.GetInfo(sun).Direction = CelestialMoveDirection.Descending;
		Assert.AreEqual(CelestialMoveDirection.Ascending, east.GetInfo(sun).Direction);
		var next = sun.FindNext(ctx.Calendar.CurrentInstant, new(AstronomicalEventType.Sunset)).Instant;
		var westZone = new MudTimeZone(100, -12, 0, "west", "west", ctx.Clock);
		var eastZone = new MudTimeZone(101, 14, 0, "east", "east", ctx.Clock);
		var westDate = next.ToMudDateTime(ctx.Calendar, ctx.Clock, westZone);
		var eastDate = next.ToMudDateTime(ctx.Calendar, ctx.Clock, eastZone);
		Assert.AreNotEqual(westDate.Date.GetDateString(), eastDate.Date.GetDateString());
		Assert.AreEqual(next.Ticks, MudInstant.FromMudDateTime(westDate).Ticks);
		Assert.AreEqual(next.Ticks, MudInstant.FromMudDateTime(eastDate).Ticks);
		bodies.Remove(sun); west.InitialiseCelestials(); east.InitialiseCelestials();
		Assert.AreEqual(TimeOfDay.Night, west.CurrentTimeOfDay);
		var optIn = moon.Compiled.CopyDefinition(); optIn.TimeOfDay = true; moon.Activate(optIn);
		Assert.AreEqual(TimeOfDay.Morning, west.CurrentTimeOfDay);
		bodies.Insert(0, sun); east.InitialiseCelestials(); west.InitialiseCelestials();
		var before = sun.IntrinsicEvaluationCount; ctx.Clock.CurrentTime.AddSeconds(60);
		Assert.AreEqual(before + 1, sun.IntrinsicEvaluationCount);
		Assert.AreEqual(west.GetInfo(sun).Direction, east.GetInfo(sun).Direction);
		west.DeregisterCelestials(); east.DeregisterCelestials();
	}

	private sealed class OutputCelestial(AuthoredCelestialDefinition definition, CelestialTestContext context)
		: AuthoredCelestial(50, definition, context.Clock, context.Gameworld)
	{
		public void Emit(AuthoredEcho echo, ILocation location) => DeliverEcho(echo, location);
	}

	[TestMethod]
	public void RealEchoDelivery_RespectsPerceptionWeatherShelterAudienceAndOutsidePrefix()
	{
		var ctx = CelestialTestFactory.CreateEarthSystem(); ctx.SetDateTime("1/jan/2010", 9, 0, 0);
		using var c = new OutputCelestial(AuthoredCelestialPresets.Create("MorningGlow", 1, 60, 24), ctx);
		var messages = new List<string>();
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string, bool, bool>((x, _, _) => messages.Add(x));
		var character = new Mock<ICharacter>(); character.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var cell = new Mock<ICell>(); character.SetupGet(x => x.Location).Returns(cell.Object);
		var canSee = true; var outdoors = CellOutdoorsType.Outdoors; var obscured = false;
		character.Setup(x => x.CanSee(c, PerceiveIgnoreFlags.None)).Returns(() => canSee);
		cell.SetupGet(x => x.Celestials).Returns(new[] { c }); cell.SetupGet(x => x.Characters).Returns(new[] { character.Object });
		cell.Setup(x => x.OutdoorsType(character.Object)).Returns(() => outdoors);
		var weather = new Mock<IWeatherEvent>(); weather.SetupGet(x => x.ObscuresViewOfSky).Returns(() => obscured);
		cell.Setup(x => x.CurrentWeather(character.Object)).Returns(weather.Object);
		var body = new AuthoredEcho("body", "body echo"); var sky = new AuthoredEcho("sky", "sky echo", CelestialEchoAudience.SkyVisible);
		c.Emit(body, cell.Object); Assert.AreEqual(0, messages.Count, "Body is below the horizon.");
		c.Emit(sky, cell.Object); Assert.AreEqual(1, messages.Count);
		outdoors = CellOutdoorsType.IndoorsWithWindows; c.Emit(sky, cell.Object); StringAssert.Contains(messages.Last(), "[Outside]");
		canSee = false; c.Emit(sky, cell.Object); Assert.AreEqual(2, messages.Count, "Blind/unable-to-see recipient must receive no echo.");
		canSee = true; outdoors = CellOutdoorsType.Indoors; c.Emit(sky, cell.Object); Assert.AreEqual(2, messages.Count);
		outdoors = CellOutdoorsType.Outdoors; obscured = true; c.Emit(sky, cell.Object); Assert.AreEqual(2, messages.Count);
		obscured = false; ctx.Clock.CurrentTime.SetTime(0, 0, 0); Assert.AreEqual(0, c.LiveState.SourceLux);
		c.Emit(sky, cell.Object); Assert.AreEqual(3, messages.Count, "Zero source lux alone does not override recipient perception.");
	}

	[TestMethod]
	public void CompatibleCalendarProjection_PreservesTheUnderlyingAuthoredInstant()
	{
		var ctx = CelestialTestFactory.CreateEarthSystem(); ctx.SetDateTime("1/jan/2010", 9, 0, 0);
		var alternate = new Calendar(2, ctx.Gameworld);
		alternate.LoadFromXml(ctx.Calendar.SaveToXml());
		alternate.FeedClock = ctx.Clock;
		alternate.SetDate("1/jan/2030");
		((All<ICalendar>)ctx.Gameworld.Calendars).Add(alternate);
		using var sun = AuthoredCelestial.Create(50, AuthoredCelestialPresets.Create("RailSun", 1, 60, 24), ctx.Clock, ctx.Gameworld);
		Assert.AreNotEqual(ctx.Calendar.CurrentInstant.Ticks, alternate.CurrentInstant.Ticks);
		Assert.AreEqual(sun.EvaluateAt(ctx.Calendar.CurrentInstant), sun.EvaluateAt(alternate.CurrentInstant));
		var request = new CelestialEventRequest(AstronomicalEventType.Sunrise);
		Assert.AreEqual(sun.FindNext(ctx.Calendar.CurrentInstant, request), sun.FindNext(alternate.CurrentInstant, request));
	}

	[TestMethod]
	public void MidnightDayBoundaryAndRejectedCalendarEdits_AreAtomicAndNonrecursive()
	{
		var ctx = CelestialTestFactory.CreateEarthSystem(); ctx.SetDateTime("1/jan/2010", 0, 0, 0);
		var bodies = new All<ICelestialObject>();
		Mock.Get(ctx.Gameworld).SetupGet(x => x.CelestialObjects).Returns(bodies);
		var d = AuthoredCelestialPresets.Create("RailSun", 1, 60, 24); d.Path.Rise = 0; d.Path.Set = 720;
		using var c = AuthoredCelestial.Create(50, d, ctx.Clock, ctx.Gameworld); bodies.Add(c);
		typeof(Calendar).GetProperty(nameof(Calendar.DayBoundary))!.SetValue(ctx.Calendar, CalendarDayBoundaryType.SunriseAtAuthorityLocation);
		typeof(Calendar).GetProperty(nameof(Calendar.AuthorityLocation))!.SetValue(ctx.Calendar, ctx.ZeroGeography);
		Assert.AreEqual(0L, MudInstant.FromMudDateTime(ctx.Calendar.StartOfCalendarDay(ctx.Calendar.GetDate("1/jan/2010"))).Ticks);
		var otherClock = new Mock<IClock>(); otherClock.SetupGet(x => x.Id).Returns(9);
		((All<IClock>)ctx.Gameworld.Clocks).Add(otherClock.Object);
		Assert.ThrowsException<InvalidOperationException>(() => ctx.Calendar.ClockID = 9);
		Assert.AreEqual(1L, ctx.Calendar.ClockID); Assert.AreSame(ctx.Clock, ctx.Calendar.FeedClock);
		var unavailable = new Mock<ICalendar>(); unavailable.SetupGet(x => x.Id).Returns(99); unavailable.SetupGet(x => x.FeedClock).Returns(ctx.Clock);
		unavailable.SetupGet(x => x.CurrentInstant).Returns(MudInstant.Never); ((All<ICalendar>)ctx.Gameworld.Calendars).Add(unavailable.Object);
		var invalid = c.Compiled.CopyDefinition(); invalid.CalendarId = 99; var original = c.Compiled.Serialize(); var state = c.LiveState;
		Assert.ThrowsException<ArgumentException>(() => c.Activate(invalid));
		Assert.AreEqual(original, c.Compiled.Serialize()); Assert.AreEqual(state, c.LiveState);
		ctx.Calendar.SetDate("1/jan/2100"); Assert.AreEqual(c.EvaluateAt(ctx.Calendar.CurrentInstant), c.LiveState);
		ctx.Calendar.SetDate("1/jan/2000"); Assert.AreEqual(c.EvaluateAt(ctx.Calendar.CurrentInstant), c.LiveState);
	}
}
