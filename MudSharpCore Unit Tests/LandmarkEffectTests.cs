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
		var room = GetRoom();

		var effect = new LandmarkEffect(room.Object, true, "public");

		Assert.IsTrue(effect.IsMeetingPlace);
		Assert.AreEqual("public", effect.Sphere);
	}

	[TestMethod]
	public void Constructor_MeetingPlaceFlagFalse_LeavesLandmarkOnly()
	{
		var room = GetRoom();

		var effect = new LandmarkEffect(room.Object, false, "private");

		Assert.IsFalse(effect.IsMeetingPlace);
		Assert.AreEqual("private", effect.Sphere);
	}

	private static Mock<IRoom> GetRoom()
	{
		var gameworld = new Mock<IFuturemud>();
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		return room;
	}
}
