#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PersistedRoomReferenceTests
{
	[DataTestMethod]
	[DataRow("RoomOverlay","CellOverlay")]
	[DataRow("RoomOverlayPackage","CellOverlayPackage")]
	[DataRow("RouteRoomLandmark","RouteCellLandmark")]
	public void Matching_RenamedSupportingType_RetainsItsOriginalIdentity(string current,string legacy)
	{
		var item=new Mock<IFrameworkItem>();
		item.SetupGet(x=>x.Id).Returns(9000);
		item.SetupGet(x=>x.FrameworkItemType).Returns(current);
		Assert.IsTrue(item.Object.FrameworkItemEquals(9000,legacy));
		Assert.IsTrue(item.Object.FrameworkItemEquals(9000,current));
		Assert.IsFalse(item.Object.FrameworkItemEquals(8101,legacy));
	}
	[DataTestMethod]
	[DataRow("Cell", true)]
	[DataRow("Room:v2", true)]
	[DataRow("Room", false)]
	[DataRow("Room:v3", false)]
	[DataRow("Room:v2:extra", false)]
	public void Matching_SameNumericId_DistinguishesLegacyParent(string type, bool expected)
	{
		var room = new Mock<IFrameworkItem>();
		room.SetupGet(x => x.Id).Returns(9000);
		room.SetupGet(x => x.FrameworkItemType).Returns("Room");
		Assert.AreEqual(expected, room.Object.FrameworkItemEquals(9000, type));
		Assert.IsFalse(room.Object.FrameworkItemEquals(8101, type));
		Assert.AreEqual("Room:v2", room.Object.GetPersistedReferenceType());
	}

	[TestMethod]
	public void Encoding_NonRoomAndMissingTargets_RetainsEstablishedBehavior()
	{
		var item = new Mock<IFrameworkItem>();
		item.SetupGet(x => x.Id).Returns(9000);
		item.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		Assert.AreEqual("GameItem", item.Object.GetPersistedReferenceType());
		Assert.IsTrue(item.Object.FrameworkItemEquals(9000, "GameItem"));
		Assert.IsFalse(item.Object.FrameworkItemEquals(9000, "Room:v2"));
		Assert.IsTrue(FrameworkItemExtensions.FrameworkItemEquals(null!, null, "None"));
		Assert.IsFalse(FrameworkItemExtensions.FrameworkItemEquals(null!, 9000, "Room:v2"));
	}
}
