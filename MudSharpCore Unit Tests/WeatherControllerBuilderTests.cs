using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Celestial;
using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Time;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeatherControllerBuilderTests
{
	[TestMethod]
	public void WeatherController_NoPermittedInitialEvent_RejectsBeforeSubscriptionsOrPersistence()
	{
		var (world, climate, zone, clock, _) = NoInitialWeatherFixture();
		var error = Assert.ThrowsException<ArgumentException>(() =>
			new WeatherController(world.Object, "Test Weather", climate.Object, zone.Object));
		Assert.AreEqual("climate", error.ParamName);
		clock.VerifyAdd(x => x.MinutesUpdated += It.IsAny<ClockEventHandler>(), Times.Never);
		world.VerifyGet(x => x.HeartbeatManager, Times.Never);
	}

	[TestMethod]
	public void WeatherControllerBuilder_NoPermittedInitialEvent_ReturnsErrorWithoutRegistration()
	{
		var (world, _, _, clock, model) = NoInitialWeatherFixture();
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		EditableItemHelper.WeatherControllerHelper.EditableNewAction(actor.Object,
			new StringStack("\"Test Weather\" \"Test Climate\" \"Test Zone\""));
		world.Verify(x => x.Add(It.IsAny<IWeatherController>()), Times.Never);
		clock.VerifyAdd(x => x.MinutesUpdated += It.IsAny<ClockEventHandler>(), Times.Never);
		model.Verify(x => x.HandleWeatherTick(null, It.IsAny<ISeason>(), TimeOfDay.Night, 0), Times.Once);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains("no initial weather event")), true, false), Times.Once);
	}

	private static (Mock<IFuturemud> World, Mock<IRegionalClimate> Climate, Mock<IZone> Zone,
		Mock<IClock> Clock, Mock<IClimateModel> Model) NoInitialWeatherFixture()
	{
		var season = Mock.Of<ISeason>(x => x.Id == 1 && x.Name == "Summer");
		var model = new Mock<IClimateModel>();
		model.Setup(x => x.HandleWeatherTick(null, season, TimeOfDay.Night, 0)).Returns((IWeatherEvent)null!);
		var climate = new Mock<IRegionalClimate>();
		climate.SetupGet(x => x.Id).Returns(1);
		climate.SetupGet(x => x.Name).Returns("Test Climate");
		climate.SetupGet(x => x.ClimateModel).Returns(model.Object);
		climate.SetupGet(x => x.Seasons).Returns([season]);
		climate.SetupGet(x => x.SeasonRotation).Returns(new CircularRange<ISeason>(365, new[] { (season, 0.0) }));
		var zone = new Mock<IZone>();
		zone.SetupGet(x => x.Id).Returns(1);
		zone.SetupGet(x => x.Name).Returns("Test Zone");
		zone.SetupGet(x => x.Geography).Returns(new GeographicCoordinate(0, 0, 0, 6371000));
		var clock = new Mock<IClock>();
		clock.SetupGet(x => x.Id).Returns(1);
		clock.SetupGet(x => x.PrimaryTimezone).Returns(Mock.Of<IMudTimeZone>());
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Clocks).Returns(new All<IClock> { clock.Object });
		world.SetupGet(x => x.CelestialObjects).Returns(new All<ICelestialObject>());
		world.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController>());
		world.SetupGet(x => x.RegionalClimates).Returns(new All<IRegionalClimate> { climate.Object });
		world.SetupGet(x => x.Zones).Returns(new All<IZone> { zone.Object });
		return (world, climate, zone, clock, model);
	}

	[TestMethod]
	public void ParseWeatherControllerCreationArguments_ZoneName_ResolvesZone()
	{
		var regionalClimate = new Mock<IRegionalClimate>();
		regionalClimate.SetupGet(x => x.Id).Returns(2);
		regionalClimate.SetupGet(x => x.Name).Returns("Subpolar Oceanic");

		var zone = new Mock<IZone>();
		zone.SetupGet(x => x.Id).Returns(17);
		zone.SetupGet(x => x.Name).Returns("Sydney Zone");

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController>());
		gameworld.SetupGet(x => x.RegionalClimates)
			.Returns(new All<IRegionalClimate> { regionalClimate.Object });
		gameworld.SetupGet(x => x.Zones).Returns(new All<IZone> { zone.Object });

		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var result = EditableItemHelper.ParseWeatherControllerCreationArguments(
			actor.Object,
			new StringStack("\"ZZZ Pilot Weather\" \"Subpolar Oceanic\" \"Sydney Zone\""));

		Assert.IsTrue(result.HasValue);
		Assert.AreEqual("ZZZ Pilot Weather", result.Value.Name);
		Assert.AreSame(regionalClimate.Object, result.Value.RegionalClimate);
		Assert.AreSame(zone.Object, result.Value.Zone);
	}
}
