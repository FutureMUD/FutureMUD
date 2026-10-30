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
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Planes;
using MudSharp.Testing.EnvironmentalMagic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomWeatherResidueTests
{
	[TestMethod]
	public void LocalWeather_SaveRestoreAndRemoval_UsesTheRoomEventWithoutChangingRegionalWeather()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = world.Rooms.OfType<Room>().First();
		var regional = Mock.Of<IWeatherEvent>(x => x.Id == 1);
		var local = Mock.Of<IWeatherEvent>(x => x.Id == 2 && x.Wind == WindLevel.GaleWind);
		var controller = Mock.Of<IWeatherController>(x => x.CurrentWeatherEvent == regional);
		Mock.Get(room.Zone).SetupGet(x => x.WeatherController).Returns(controller);
		world.World.SetupGet(x => x.WeatherEvents).Returns(new All<IWeatherEvent> { regional, local });
		var effect = new RoomWeatherEffect(room, local);
		room.AddEffect(effect);
		Assert.AreSame(local, room.CurrentWeather(null));
		Assert.AreSame(regional, room.WeatherController.CurrentWeatherEvent);
		Assert.AreEqual(WindLevel.GaleWind, room.CurrentWeather(null).Wind);
		RoomWeatherEffect.InitialiseEffectType();
		var restored = (RoomWeatherEffect)Effect.LoadEffect(effect.SaveToXml(new Dictionary<IEffect, TimeSpan>()), room);
		Assert.AreSame(local, restored.WeatherEvent);
		room.RemoveEffect(effect);
		Assert.AreSame(regional, room.CurrentWeather(null));
		room.AddEffect(restored);
		Assert.AreSame(local, room.CurrentWeather(null));
	}

	[TestMethod]
	public void WeatherResidue_UsesNativeBodyAndItemSurfaces_CapsAccumulationAndSkipsShelteredRooms()
	{
		var world = new Mock<IFuturemud>();
		var material = Mock.Of<ISolid>(x => x.Id == 7 && x.ResidueSdesc == "(dusty)");
		world.SetupGet(x => x.Materials).Returns(new All<ISolid> { material });
		var room = new Mock<IRoom>(); room.SetupGet(x => x.Gameworld).Returns(world.Object);
		room.Setup(x => x.OutdoorsType(null)).Returns(RoomOutdoorsType.Outdoors);
		var actor = new Mock<ICharacter>(); var body = new Mock<IBody>();
		body.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		actor.SetupGet(x => x.Body).Returns(body.Object); actor.SetupGet(x => x.Location).Returns(room.Object);
		actor.SetupGet(x => x.Merits).Returns([]);
		body.SetupGet(x => x.Merits).Returns([]);
		var changes = 0;
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1);
		var bodyState = new BodySurfaceLiquidState(world.Object, () => new[] { part }, () => changes++);
		var previousDryingTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		bodyState.ForPart(part).LastResolvedUtc = previousDryingTime;
		body.SetupGet(x => x.SurfaceLiquidState).Returns(bodyState);
		var item = new Mock<IGameItem>(); var itemState = new SurfaceLiquidState(world.Object, () => changes++);
		item.SetupGet(x => x.SurfaceLiquidState).Returns(itemState); item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		body.SetupGet(x => x.WornItems).Returns(new[] { item.Object });
		body.SetupGet(x => x.HeldItems).Returns(Array.Empty<IGameItem>()); body.SetupGet(x => x.WieldedItems).Returns(Array.Empty<IGameItem>());
		room.SetupGet(x => x.Characters).Returns(new[] { actor.Object }); room.SetupGet(x => x.GameItems).Returns(Array.Empty<IGameItem>());
		var config = new WeatherHazardSettings { ResidueMaterialId = 7, ResiduePerMinute = 0.1, ResidueMaximum = 0.15 };
		var weather = Mock.Of<IWeatherEvent>(x => x.Hazards == config);
		room.Setup(x => x.CurrentWeather(null)).Returns(weather);
		room.Setup(x => x.OutdoorsType(null)).Returns(RoomOutdoorsType.Indoors);
		WeatherHazardService.Tick(weather, room.Object, () => 1);
		Assert.AreEqual(0.0, bodyState.ResidueWeight);
		Assert.AreEqual(0.0, itemState.ResidueWeight);
		room.Setup(x => x.OutdoorsType(null)).Returns(RoomOutdoorsType.Outdoors);
		WeatherHazardService.Tick(weather, room.Object, () => throw new AssertFailedException("Residue must not roll for lightning."));
		Assert.AreEqual(0.1, bodyState.ResidueWeight, 1e-10);
		Assert.AreEqual(previousDryingTime, bodyState.ForPart(part).LastResolvedUtc, "Dry deposition must preserve the existing drying clock.");
		Assert.AreEqual(0.1, itemState.ResidueWeight, 1e-10);
		WeatherHazardService.Tick(weather, room.Object, () => 1);
		Assert.AreEqual(0.15, bodyState.ResidueWeight, 1e-10);
		Assert.AreEqual(0.15, itemState.ResidueWeight, 1e-10);
		Assert.IsTrue(changes > 0, "Dry deposition must notify native surface persistence/effect maintenance.");
		Assert.AreEqual("(dusty)", itemState.GetAddendumText(1, 1, false));
	}
}
