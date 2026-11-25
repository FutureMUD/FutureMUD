using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LandmarkEffectTests
{
	[TestMethod]
	public void MissingSavedFilter_StaysPrivateAcrossSaveUntilExplicitlyCleared()
	{
		var room = GetRoom();
		Mock.Get(room.Object.Gameworld).SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var landmark = new LandmarkEffect(room.Object, false, "");
		landmark.LandmarkDescriptionTexts.Add((91, "Restricted detail"));
		var saved = landmark.SaveToXml(new());
		saved.Element("ApplicabilityProg")!.Value = "42";
		LandmarkEffect.InitialiseEffectType();
		var loaded = (LandmarkEffect)Effect.LoadEffect(saved, room.Object);
		Assert.IsFalse(loaded.IsPublicOnDiscord());
		var resaved = loaded.SaveToXml(new());
		Assert.AreEqual("42", resaved.Element("ApplicabilityProg")!.Value);
		var extra = resaved.Descendants("ExtraText").Single();
		Assert.AreEqual("91", extra.Attribute("prog")!.Value);
		Assert.AreEqual("Restricted detail", extra.Value);
		loaded.ApplicabilityProg = null;
		Assert.IsTrue(loaded.IsPublicOnDiscord());
	}

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
