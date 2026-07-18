using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeatherControllerBuilderTests
{
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
