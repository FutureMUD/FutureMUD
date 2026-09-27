using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSplitAccessTests
{
	[TestMethod]
	public void CommodityPreview_IsTemporaryAndDoesNotChangeStockOrRegisterItems()
	{
		var (item, world) = CreateItem();
		var commodity = AddCommodity(item);
		var save = Mock.Get(world.Object.SaveManager);
		save.Invocations.Clear();
		world.Invocations.Clear();

		var preview = item.PeekSplitByWeight(2.5);

		Assert.AreEqual(2.5, preview.GetItemType<ICommodity>().Weight);
		Assert.AreEqual(10.0, commodity.Weight);
		save.Verify(x => x.AddInitialisation(It.IsAny<ILateInitialisingItem>()), Times.Never);
		save.Verify(x => x.Add(It.IsAny<ISaveable>()), Times.Never);
		world.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
	}

	[TestMethod]
	public void CommoditySplit_PreservesShopStockIdentityAndOnlyDebitsRequestedWeight()
	{
		var (item, _) = CreateItem();
		var commodity = AddCommodity(item);
		var display = AddShopDisplay(item);
		item.OverrideDesc = "Builder-customised goods.";

		var split = item.GetByWeight(null!, 2.5);

		Assert.AreEqual(7.5, commodity.Weight);
		Assert.AreEqual(2.5, split.GetItemType<ICommodity>().Weight);
		Assert.AreEqual(item.OverrideDesc, split.OverrideDesc);
		Assert.AreSame(display, item.EffectsOfType<ItemOnDisplayInShop>().Single());
		var splitDisplay = split.EffectsOfType<ItemOnDisplayInShop>().Single();
		Assert.AreSame(display.Shop, splitDisplay.Shop);
		Assert.AreSame(display.Merchandise, splitDisplay.Merchandise);
		Assert.AreSame(split, splitDisplay.Owner);
		Mock.Get(display.Shop).Verify(x => x.RegisterStockItemSplit(item, split), Times.Once);
	}

	[TestMethod]
	public void StackSplit_PreservesShopStockIdentityOnBothPortions()
	{
		var (item, _) = CreateItem();
		var stack = new StackableGameItemComponent((StackableGameItemComponentProto)null!, item) { Quantity = 10 };
		Components(item).Add(stack);
		var display = AddShopDisplay(item);

		var split = stack.Split(3);

		Assert.AreEqual(7, stack.Quantity);
		Assert.AreEqual(3, split.Quantity);
		Assert.AreSame(display, item.EffectsOfType<ItemOnDisplayInShop>().Single());
		Assert.AreSame(display.Merchandise, split.EffectsOfType<ItemOnDisplayInShop>().Single().Merchandise);
		Mock.Get(display.Shop).Verify(x => x.RegisterStockItemSplit(item, split), Times.Once);
	}

	[TestMethod]
	public void RepeatedCommoditySplits_MergeNotifiesTheShopAndConservesWeight()
	{
		var (item, _) = CreateItem();
		var commodity = AddCommodity(item);
		var display = AddShopDisplay(item);
		var first = item.GetByWeight(null!, 2.0);
		var second = item.GetByWeight(null!, 3.0);

		Assert.IsTrue(first.CanMerge(second));
		first.Merge(second);

		Assert.AreEqual(5.0, commodity.Weight);
		Assert.AreEqual(5.0, first.GetItemType<ICommodity>().Weight);
		Mock.Get(display.Shop).Verify(x => x.RegisterStockItemMerge(first, second), Times.Once);
	}

	private static (GameItem Item, Mock<IFuturemud> World) CreateItem()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		proto.SetupGet(x => x.Gameworld).Returns(world.Object);
		proto.SetupGet(x => x.Name).Returns("goods");
		return (new GameItem(proto.Object), world);
	}

	private static List<IGameItemComponent> Components(GameItem item) =>
		(List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(item)!;

	private static CommodityGameItemComponent AddCommodity(GameItem item)
	{
		var commodity = new CommodityGameItemComponent(new MudSharp.Models.GameItemComponent
		{
			Definition = "<Definition><Material>1</Material><Weight>10</Weight></Definition>"
		}, null!, item);
		Components(item).Add(commodity);
		return commodity;
	}

	private static ItemOnDisplayInShop AddShopDisplay(GameItem item)
	{
		var shop = new Mock<IShop>();
		var merchandise = new Mock<IMerchandise>();
		merchandise.SetupGet(x => x.Id).Returns(1L);
		merchandise.SetupGet(x => x.PermitItemDecayOnStockedItems).Returns(true);
		shop.SetupGet(x => x.Merchandises).Returns([merchandise.Object]);
		var display = new ItemOnDisplayInShop(item, shop.Object, merchandise.Object);
		item.AddEffect(display);
		return display;
	}
}
