#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Form.Material;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.Testing.EnvironmentalMagic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeatherHazardTests
{
	[TestMethod]
	public void RoomSubscriptions_ReassignmentAndSharedControllers_TickEffectiveRoomExactlyOnce()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = world.Rooms.OfType<Room>().First();
		var zone = Mock.Get(room.Zone);
		var first = new Mock<IWeatherController>();
		var second = new Mock<IWeatherController>();
		zone.SetupGet(x => x.WeatherController).Returns(first.Object);
		room.RefreshWeatherSubscriptions();
		room.RefreshWeatherSubscriptions();
		var ticks = 0;
		Action<IRoom> visit = _ => ticks++;
		first.Raise(x => x.WeatherRoomTick += null, first.Object, visit);
		Assert.AreEqual(1, ticks);
		var area = Mock.Of<IArea>(x => x.WeatherController == first.Object);
		room.AddArea(area);
		zone.SetupGet(x => x.WeatherController).Returns(second.Object);
		room.RefreshWeatherSubscriptions();
		second.Raise(x => x.WeatherRoomTick += null, second.Object, visit);
		Assert.AreEqual(1, ticks, "An inactive zone controller must not tick the area-controlled cell.");
		first.Raise(x => x.WeatherRoomTick += null, first.Object, visit);
		Assert.AreEqual(2, ticks);
		room.RemoveArea(area);
		first.Raise(x => x.WeatherRoomTick += null, first.Object, visit);
		second.Raise(x => x.WeatherRoomTick += null, second.Object, visit);
		Assert.AreEqual(3, ticks);
	}

	[TestMethod]
	public void RoomAtmosphere_MagicThenExposedWeatherThenOverlay_RestoresWithoutMutation()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = world.Rooms.OfType<Room>().First();
		var overlay = (IEditableRoomOverlay)room.CurrentOverlay;
		var normal = Mock.Of<IGas>();
		var dust = Mock.Of<IGas>(x => x.Id == 1);
		var magic = Mock.Of<IGas>();
		world.World.SetupGet(x => x.Gases).Returns(new All<IGas> { dust });
		overlay.Atmosphere = normal;
		overlay.OutdoorsType = RoomOutdoorsType.Outdoors;
		var weather = Mock.Of<IWeatherEvent>(x => x.Hazards == new WeatherHazardSettings { AtmosphereGasId = 1 });
		Mock.Get(room.Zone).SetupGet(x => x.WeatherController).Returns(Mock.Of<IWeatherController>(x => x.CurrentWeatherEvent == weather));
		Assert.AreSame(dust, room.Atmosphere);
		var effect = new Mock<IAffectAtmosphere>();
		effect.SetupGet(x => x.Atmosphere).Returns(magic);
		effect.Setup(x => x.Applies()).Returns(true);
		effect.Setup(x => x.IsEffectType<IAffectAtmosphere>()).Returns(true);
		effect.Setup(x => x.GetSubtype<IAffectAtmosphere>()).Returns(effect.Object);
		room.AddEffect(effect.Object);
		Assert.AreSame(magic, room.Atmosphere);
		room.RemoveEffect(effect.Object);
		Assert.AreSame(dust, room.Atmosphere);
		overlay.OutdoorsType = RoomOutdoorsType.IndoorsWithWindows;
		Assert.AreSame(normal, room.Atmosphere);
		overlay.OutdoorsType = RoomOutdoorsType.IndoorsClimateExposed;
		Assert.AreSame(dust, room.Atmosphere);
		Mock.Get(room.Zone).SetupGet(x => x.WeatherController).Returns((IWeatherController?)null);
		Assert.AreSame(normal, room.Atmosphere);
		Assert.AreSame(normal, overlay.Atmosphere);
	}

	[DataTestMethod]
	[DataRow(RoomOutdoorsType.Indoors)]
	[DataRow(RoomOutdoorsType.IndoorsWithWindows)]
	[DataRow(RoomOutdoorsType.IndoorsClimateExposed)]
	public void Lightning_ProtectedRoom_DoesNotRollOrInjure(RoomOutdoorsType outdoors)
	{
		var fixture = new Fixture(new() { LightningChance = 1 });
		fixture.Room.Setup(x => x.OutdoorsType(null)).Returns(outdoors);
		WeatherHazardService.Tick(fixture.Weather.Object, fixture.Room.Object, () => throw new AssertFailedException("Protected cell rolled lightning."));
		fixture.Actor.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void Lightning_InactiveControllerEvent_DoesNotRoll()
	{
		var fixture = new Fixture(new() { LightningChance = 1 });
		WeatherHazardService.Tick(Mock.Of<IWeatherEvent>(), fixture.Room.Object, () => throw new AssertFailedException("Inactive weather rolled lightning."));
	}

	[TestMethod]
	public void Lightning_DirectCharacter_UsesElectricalDamageAndAudioPropagation()
	{
		var fixture = new Fixture(new() { LightningChance = 1, GroundWeight = 0, ItemWeight = 0, CharacterWeight = 1, Damage = 150 });
		WeatherHazardService.Tick(fixture.Weather.Object, fixture.Room.Object, () => 0.1);
		fixture.Actor.Verify(x => x.SufferDamage(It.Is<IDamage>(d => d.DamageType == DamageType.Electrical && d.DamageAmount == 150)), Times.Once);
		fixture.Room.Verify(x => x.HandleAudioEcho(It.IsAny<string>(), AudioVolume.ExtremelyLoud, 10,
			AudioPropagationMode.Topological, It.IsAny<IPerceiver>(), RoomLayer.GroundLevel, false, "thunder"), Times.Once);
	}

	[TestMethod]
	public void Lightning_DirectItem_UsesOrdinaryItemWounds()
	{
		var fixture = new Fixture(new() { LightningChance = 1, GroundWeight = 0, ItemWeight = 1, CharacterWeight = 0, Damage = 80 });
		WeatherHazardService.Tick(fixture.Weather.Object, fixture.Room.Object, () => 0.1);
		fixture.Item.Verify(x => x.PassiveSufferDamage(It.Is<IDamage>(d => d.DamageType == DamageType.Electrical && d.DamageAmount == 80)), Times.Once);
		fixture.Actor.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(0.0, 0)]
	[DataRow(0.2, 1)]
	public void Lightning_GroundSplash_ConfigurableAndExcludesSubmergedTargets(double factor, int count)
	{
		var fixture = new Fixture(new() { LightningChance = 1, GroundWeight = 1, ItemWeight = 0, CharacterWeight = 0, Damage = 100, GroundDamageFactor = factor });
		fixture.Item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.Underwater);
		fixture.Room.Setup(x => x.IsUnderwaterLayer(RoomLayer.Underwater)).Returns(true);
		WeatherHazardService.Tick(fixture.Weather.Object, fixture.Room.Object, () => 0.1);
		fixture.Actor.Verify(x => x.SufferDamage(It.Is<IDamage>(d => d.DamageAmount == 20)), Times.Exactly(count));
		fixture.Item.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void Lightning_AtmosphericOnly_EchoesWithoutDamage()
	{
		var fixture = new Fixture(new() { AtmosphericLightningChance = 1 });
		WeatherHazardService.Tick(fixture.Weather.Object, fixture.Room.Object, () => 0.1);
		fixture.Actor.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
		fixture.Item.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Never);
		fixture.Output.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
	}

	private sealed class Fixture
	{
		public Mock<IRoom> Room { get; } = new();
		public Mock<IWeatherEvent> Weather { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public Mock<IOutputHandler> Output { get; } = new();
		public Fixture(WeatherHazardSettings settings)
		{
			Weather.SetupGet(x => x.Hazards).Returns(settings);
			Room.Setup(x => x.CurrentWeather(null)).Returns(Weather.Object);
			Room.Setup(x => x.OutdoorsType(null)).Returns(RoomOutdoorsType.Outdoors);
			Room.SetupGet(x => x.Characters).Returns([Actor.Object]);
			Room.SetupGet(x => x.GameItems).Returns([Item.Object]);
			Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
			Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
			Actor.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Actor.Setup(x => x.CanSee(Room.Object, PerceiveIgnoreFlags.None)).Returns(true);
			Actor.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns(Array.Empty<IWound>());
			Item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
			Item.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns(Array.Empty<IWound>());
		}
	}
}
