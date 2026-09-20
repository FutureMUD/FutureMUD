#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Magic.Environment;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CellLandScarDescriptionTests
{
	[TestMethod]
	[TestCategory("L-T28")]
	public void LandScarAddendum_RequiresProfileOptInAndUsesCurrentScarAndPressureBands()
	{
		Mock<IEnvironmentalMagicProfile> profile = new();
		var state = new EnvironmentalMagicStateSnapshot(new EnvironmentalMagicState
		{
			ScarDamage = 2.0
		}, 1.0);
		Assert.IsNull(Cell.LandScarRoomDescriptionAddendum(profile.Object, state));
		profile.SetupGet(x => x.ShowLandScarAddendum).Returns(true);
		StringAssert.Contains(Cell.LandScarRoomDescriptionAddendum(profile.Object, state)!, "Small patches");
		StringAssert.Contains(Cell.LandScarRoomDescriptionAddendum(profile.Object, state)!, "faint unnatural");
		var severe = new EnvironmentalMagicStateSnapshot(new EnvironmentalMagicState
		{
			ScarDamage = 12.0
		}, 11.0);
		StringAssert.Contains(Cell.LandScarRoomDescriptionAddendum(profile.Object, severe)!, "Wide patches");
		StringAssert.Contains(Cell.LandScarRoomDescriptionAddendum(profile.Object, severe)!, "heavy unnatural");
		var repaired = new EnvironmentalMagicStateSnapshot(new EnvironmentalMagicState(), 0.0);
		Assert.IsNull(Cell.LandScarRoomDescriptionAddendum(profile.Object, repaired));
	}
}
