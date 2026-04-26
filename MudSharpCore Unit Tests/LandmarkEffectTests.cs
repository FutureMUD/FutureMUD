using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LandmarkEffectTests
{
	[TestMethod]
	public void Constructor_MeetingPlaceFlagTrue_SetsMeetingPlace()
	{
		var cell = GetCell();

		var effect = new LandmarkEffect(cell.Object, true, "public");

		Assert.IsTrue(effect.IsMeetingPlace);
		Assert.AreEqual("public", effect.Sphere);
	}

	[TestMethod]
	public void Constructor_MeetingPlaceFlagFalse_LeavesLandmarkOnly()
	{
		var cell = GetCell();

		var effect = new LandmarkEffect(cell.Object, false, "private");

		Assert.IsFalse(effect.IsMeetingPlace);
		Assert.AreEqual("private", effect.Sphere);
	}

	private static Mock<ICell> GetCell()
	{
		var gameworld = new Mock<IFuturemud>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		return cell;
	}
}
