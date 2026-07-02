using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class InventoryExtensionsTests
{
	[TestMethod]
	public void GetInventoryString_AttachedItemOnCoveredBelt_ShowsCoveredAttachedItem()
	{
		var (inventory, perceiver, scabbard, trousers) = AttachedItemInventory(WearableItemCoverStatus.Covered);

		var result = inventory.Object.GetInventoryString(perceiver.Object);

		StringAssert.Contains(result, "<attached to belt>");
		StringAssert.Contains(result, "a faux-leather scabbard");
		StringAssert.Contains(result, "(covered)");
		trousers.Verify(x => x.HowSeen(perceiver.Object, false, DescriptionType.Short, false, PerceiveIgnoreFlags.None), Times.Once);
		scabbard.Verify(x => x.HowSeen(perceiver.Object, false, DescriptionType.Short, true, PerceiveIgnoreFlags.None), Times.Once);
	}

	[TestMethod]
	public void GetInventoryString_AttachedItemOnUncoveredBelt_ShowsPlainAttachedItem()
	{
		var (inventory, perceiver, _, _) = AttachedItemInventory(WearableItemCoverStatus.Uncovered);

		var result = inventory.Object.GetInventoryString(perceiver.Object);

		StringAssert.Contains(result, "<attached to belt>");
		StringAssert.Contains(result, "a faux-leather scabbard");
		Assert.IsFalse(result.Contains("(covered)", StringComparison.InvariantCultureIgnoreCase));
		Assert.IsFalse(result.Contains("(partially covered)", StringComparison.InvariantCultureIgnoreCase));
	}

	private static (Mock<IInventory> Inventory, Mock<IPerceiver> Perceiver, Mock<IGameItem> Scabbard, Mock<IGameItem> Coverer)
		AttachedItemInventory(WearableItemCoverStatus beltCoverStatus)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticBool("HeldAndWieldedDisplayAtTop")).Returns(false);

		var perceiver = new Mock<IPerceiver>();
		perceiver.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var belt = new Mock<IGameItem>();
		belt.SetupGet(x => x.Name).Returns("belt");
		var wearable = new Mock<IWearable>();
		wearable.SetupGet(x => x.DisplayInventoryWhenWorn).Returns(true);
		belt.Setup(x => x.IsItemType<IWearable>()).Returns(true);
		belt.Setup(x => x.GetItemType<IWearable>()).Returns(wearable.Object);
		perceiver.Setup(x => x.CanSee(belt.Object, PerceiveIgnoreFlags.None)).Returns(true);

		var beltComponent = new Mock<IBelt>();
		beltComponent.SetupGet(x => x.Parent).Returns(belt.Object);

		var scabbard = new Mock<IGameItem>();
		scabbard.Setup(x => x.HowSeen(perceiver.Object, false, DescriptionType.Short, true, PerceiveIgnoreFlags.None))
		        .Returns("a faux-leather scabbard");
		var beltable = new Mock<IBeltable>();
		beltable.SetupGet(x => x.Parent).Returns(scabbard.Object);
		beltable.SetupGet(x => x.ConnectedTo).Returns(beltComponent.Object);
		scabbard.Setup(x => x.GetItemType<IBeltable>()).Returns(beltable.Object);

		var coverer = new Mock<IGameItem>();
		coverer.Setup(x => x.HowSeen(perceiver.Object, false, DescriptionType.Short, false, PerceiveIgnoreFlags.None))
		       .Returns("a pair of trousers");

		var inventory = new Mock<IInventory>();
		inventory.Setup(x => x.HeldOrWieldedItems).Returns(Enumerable.Empty<IGameItem>());
		inventory.Setup(x => x.WornItems).Returns(new[] { belt.Object, scabbard.Object });
		inventory.Setup(x => x.OrderedWornItems).Returns(new[] { scabbard.Object });
		inventory.Setup(x => x.CoverInformation(belt.Object))
		         .Returns(new[] { Tuple.Create(beltCoverStatus, coverer.Object) });

		return (inventory, perceiver, scabbard, coverer);
	}
}
