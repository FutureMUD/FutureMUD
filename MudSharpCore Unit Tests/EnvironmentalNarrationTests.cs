#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Climate.WeatherEvents;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Testing.EnvironmentalMagic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalNarrationTests
{
	[TestMethod]
	public void RoomPreview_CombinedOverrides_RenderSelectedOverlayOnce()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.First();
		var builder = Mock.Get(world.Builder);
		builder.SetupGet(x => x.Location).Returns(room);
		builder.SetupGet(x => x.InnerLineFormatLength).Returns(100);
		var draft = Mock.Of<IRoomOverlayPackage>();
		builder.SetupGet(x => x.CurrentOverlayPackage).Returns(draft);
		var autumn = Mock.Of<ISeason>(x => x.Id == 3 && x.Name == "late_autumn" && x.SeasonGroup == "autumn");
		world.World.SetupGet(x => x.Seasons).Returns(new All<ISeason> { autumn });
		var light = Mock.Get(world.World.Object.LightModel);
		light.SetupGet(x => x.LightDescriptions).Returns(["very dark"]);
		light.Setup(x => x.GetMinimumIlluminationForDescription("very dark")).Returns(1.0);
		light.Setup(x => x.GetIlluminationDescription(It.Is<double>(n => n > 1 && n < 2))).Returns("very dark");
		var liveWeather = Mock.Of<IWeatherEvent>(x => x.Wind == WindLevel.Breeze);
		var controller = Mock.Of<IWeatherController>(x => x.CurrentWeatherEvent == liveWeather &&
			x.RegionalClimate == Mock.Of<IRegionalClimate>());
		Mock.Get(room.Zone).SetupGet(x => x.WeatherController).Returns(controller);
		Mock.Get(room.Zone).SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Morning);
		room.AddOverlay(Mock.Of<IEditableRoomOverlay>(x => x.Package == draft &&
			x.RoomName == "environment{night,autumn=The autumn night ferry}{The ferry}" &&
			x.RoomDescription == "environment{>wind:strongwind,<wind:hurricanewind,dry,*rain,very dark=The wet ropes strain.}{wind:breeze=The flag stirs.}{The flag hangs limp.}"));

		var saves = world.SaveRequests;
		var commands = RoomBuilderModule.Instance.Commands.TCommands;
		Assert.IsFalse(commands["room"].Execute(world.Builder, "room preview time night", permissionLevel: PermissionLevel.Player));
		Assert.IsTrue(commands["room"].Execute(world.Builder,
			"room preview time night wind galewind rain dry recentrain rain season late_autumn light \"very dark\"",
			permissionLevel: PermissionLevel.Admin));
		Mock.Get(world.Builder.OutputHandler).Verify(x => x.Send(It.Is<string>(s =>
			s.Contains("The autumn night ferry") && s.Contains("The wet ropes strain.")), true, false), Times.Once);

		Assert.IsTrue(commands["cell"].Execute(world.Builder, "cell preview rain dry", permissionLevel: PermissionLevel.Admin));
		Mock.Get(world.Builder.OutputHandler).Verify(x => x.Send(It.Is<string>(s =>
			s.Contains("The ferry") && s.Contains("The flag stirs.")), true, false), Times.Once);
		Assert.AreEqual(saves, world.SaveRequests);
		Assert.AreEqual(TimeOfDay.Morning, room.CurrentTimeOfDay);
		Assert.AreEqual(WindLevel.Breeze, room.CurrentWeather(world.Builder).Wind);
		Assert.AreNotSame(room.CurrentOverlay, room.GetOverlayFor(world.Builder));
	}

	[TestMethod]
	[DataRow("time night rain", "Supply a value")]
	[DataRow("time midnight", "Choose dawn")]
	[DataRow("wind monsoon", "Choose a wind level")]
	[DataRow("recentrain fog", "Choose a precipitation level")]
	[DataRow("light NaN", "finite")]
	[DataRow("light -1", "non-negative")]
	[DataRow("light gloomier", "Use a lux value")]
	[DataRow("season missing", "There is no season")]
	[DataRow("temperature cold", "Unknown preview factor")]
	[DataRow("", "room preview <factor>")]
	public void RoomPreview_InvalidOrMissingConditions_ExplainAndDoNotRender(string input, string expected)
	{
		var builder = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		builder.SetupGet(x => x.Gameworld.Seasons).Returns(new All<ISeason>());
		builder.SetupGet(x => x.Gameworld.LightModel.LightDescriptions).Returns(["bright"]);
		var command = RoomBuilderModule.Instance.Commands.TCommands["room"];
		Assert.IsTrue(command.Execute(builder.Object, $"room preview {input}", permissionLevel: PermissionLevel.Admin));
		Mock.Get(builder.Object.OutputHandler).Verify(x => x.Send(It.Is<string>(s => s.Contains(expected)), true, false), Times.Once);
		Mock.Get(builder.Object.Location).Verify(x => x.PreviewDescription(It.IsAny<IPerceiver>(), It.IsAny<RoomDescriptionOverrides>()), Times.Never);
	}

	[TestMethod]
	public void RoomDescription_NegatedSeasonAndTimeConditions_SelectFirstMatchingBranch()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.First();
		var zone = Mock.Get(room.Zone);
		var season = Mock.Of<ISeason>(x => x.Name == "late_autumn" && x.SeasonGroup == "leaf-fall");
		var climate = new Mock<IRegionalClimate>();
		climate.SetupGet(x => x.Seasons).Returns([season]);
		zone.SetupGet(x => x.WeatherController).Returns(Mock.Of<IWeatherController>(
			x => x.RegionalClimate == climate.Object && x.CurrentSeason == season));
		((IEditableRoomOverlay)room.CurrentOverlay).RoomDescription =
			"environment{night=Dark.}{leaf-fall, !night=Falling leaves.}{Clear.} " +
			"environment{!late_autumn=Warm.}{Cold.}";

		zone.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Dusk);
		Assert.AreEqual("Falling leaves. Cold.", room.PublicLandmarkDescription(false));
		zone.SetupGet(x => x.CurrentTimeOfDay).Returns(TimeOfDay.Night);
		Assert.AreEqual("Dark. Cold.", room.PublicLandmarkDescription(false));
	}

	[TestMethod]
	public void TransitionEcho_QuotedSourceAndText_ResolveOverrideAndRestoreDefault()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var overcast = Mock.Of<IWeatherEvent>(x => x.Id == 10 && x.Name == "Low Cloud");
		var drizzle = Mock.Of<IWeatherEvent>(x => x.Id == 11 && x.CountsAs == overcast);
		world.SetupGet(x => x.WeatherEvents).Returns(new All<IWeatherEvent> { overcast });
		var fog = new SimpleWeatherEvent(new MudSharp.Models.WeatherEvent
		{
			Id = 20,
			Name = "Fog",
			AdditionalInfo = "<Info><Transitions><Default>Fog arrives.</Default></Transitions></Info>"
		}, world.Object);
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		Assert.IsTrue(fog.BuildingCommand(actor.Object, new StringStack("transition \"Low Cloud\" The headland disappears.")));
		Assert.AreEqual("The headland disappears.", fog.DescribeTransitionTo(overcast));
		Assert.AreEqual("The headland disappears.", fog.DescribeTransitionTo(drizzle));
		Assert.IsTrue(fog.BuildingCommand(actor.Object, new StringStack("transition none \"Low Cloud\"")));
		Assert.AreEqual("Fog arrives.", fog.DescribeTransitionTo(overcast));
	}
}
