#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyExtensionsTests
{
	[TestMethod]
	public void FunctioningWieldingLocationsAvailableFor_InventoryModelIgnoresOccupiedGrabInventory()
	{
		var (body, _, _) = CreateBodyWithTwoHands();
		var inventory = new Mock<IGrab>();
		var carriedItem = new Mock<IGameItem>();
		body.SetupGet(x => x.HoldLocs).Returns([inventory.Object]);
		body.Setup(x => x.HeldItemsFor(inventory.Object)).Returns([carriedItem.Object]);

		Assert.AreEqual(2, body.Object.FunctioningWieldingLocationsAvailableFor().Count());
	}

	[TestMethod]
	public void FunctioningWieldingLocationsAvailableFor_ItemOccupyingHandRemainsAvailableToItself()
	{
		var (body, leftHand, _) = CreateBodyWithTwoHands();
		var item = new Mock<IGameItem>();
		body.Setup(x => x.WieldedItemsFor(leftHand.Object)).Returns([item.Object]);

		Assert.AreEqual(2, body.Object.FunctioningWieldingLocationsAvailableFor(item.Object).Count());
		Assert.AreEqual(1, body.Object.FunctioningWieldingLocationsAvailableFor().Count());
	}

	[TestMethod]
	public void FunctioningWieldingLocationsAvailableFor_UnrelatedItemOccupyingHandIsUnavailable()
	{
		var (body, leftHand, _) = CreateBodyWithTwoHands();
		var item = new Mock<IGameItem>();
		var otherItem = new Mock<IGameItem>();
		body.Setup(x => x.HeldItemsFor(leftHand.Object)).Returns([otherItem.Object]);

		Assert.AreEqual(1, body.Object.FunctioningWieldingLocationsAvailableFor(item.Object).Count());
	}

	[TestMethod]
	public void FunctioningWieldingLocationsAvailableFor_UnusableHandIsUnavailable()
	{
		var (body, leftHand, _) = CreateBodyWithTwoHands();
		body.Setup(x => x.CanUseBodypart(leftHand.Object)).Returns(CanUseBodypartResult.CantUsePartDamage);

		Assert.AreEqual(1, body.Object.FunctioningWieldingLocationsAvailableFor().Count());
	}

	private static (Mock<IBody> Body, Mock<IWield> LeftHand, Mock<IWield> RightHand) CreateBodyWithTwoHands()
	{
		var body = new Mock<IBody>();
		var leftHand = new Mock<IWield>();
		var rightHand = new Mock<IWield>();
		body.SetupGet(x => x.WieldLocs).Returns([leftHand.Object, rightHand.Object]);
		body.Setup(x => x.CanUseBodypart(It.IsAny<IBodypart>())).Returns(CanUseBodypartResult.CanUse);
		body.Setup(x => x.HeldItemsFor(It.IsAny<IBodypart>())).Returns(Array.Empty<IGameItem>());
		body.Setup(x => x.WieldedItemsFor(It.IsAny<IBodypart>())).Returns(Array.Empty<IGameItem>());
		return (body, leftHand, rightHand);
	}
}
