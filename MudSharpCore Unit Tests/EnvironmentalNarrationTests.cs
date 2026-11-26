#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Climate.WeatherEvents;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Testing.EnvironmentalMagic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalNarrationTests
{
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
