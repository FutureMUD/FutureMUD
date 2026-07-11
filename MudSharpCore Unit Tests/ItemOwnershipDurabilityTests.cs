#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemOwnershipDurabilityTests
{
	[TestMethod]
	public void CopyOwnerFrom_CopiesDurableReference()
	{
		var (first, second) = CreateItems();
		var owner = CreateOwner(100L, "Hospital");
		first.SetOwner(owner.Object);

		second.CopyOwnerFrom(first);

		Assert.AreEqual(new ItemOwnershipReference("Hospital", 100L), second.OwnershipReference);
		Assert.IsTrue(second.HasSameOwnerAs(first));
	}

	[TestMethod]
	public void CanMerge_DifferentOwners_IsRejected()
	{
		var (first, second) = CreateItems();
		first.SetOwner(CreateOwner(100L, "Character").Object);
		second.SetOwner(CreateOwner(101L, "Character").Object);

		Assert.IsFalse(first.CanMerge(second));
	}

	[TestMethod]
	public void CanMerge_SameOwner_IsAllowed()
	{
		var (first, second) = CreateItems();
		var owner = CreateOwner(100L, "Character");
		first.SetOwner(owner.Object);
		second.SetOwner(owner.Object);

		Assert.IsTrue(first.CanMerge(second));
	}

	[TestMethod]
	public void FindCurrencyPreservingOwnership_MultipleOwners_SelectsSingleExactOwnerGroup()
	{
		var (first, second) = CreateItems();
		var (third, _) = CreateItems();
		var firstOwner = CreateOwner(100L, "Character");
		var secondOwner = CreateOwner(101L, "Character");
		first.SetOwner(firstOwner.Object);
		second.SetOwner(firstOwner.Object);
		third.SetOwner(secondOwner.Object);

		var coin = new Mock<ICoin>();
		coin.SetupGet(x => x.Value).Returns(1.0M);
		var firstPile = CreateCurrencyPile(first);
		var secondPile = CreateCurrencyPile(second);
		var thirdPile = CreateCurrencyPile(third);
		var currency = new Mock<ICurrency>();
		currency
			.Setup(x => x.FindCurrency(It.IsAny<IEnumerable<ICurrencyPile>>(), 10.0M))
			.Returns((IEnumerable<ICurrencyPile> piles, decimal _) =>
			{
				var group = piles.ToList();
				return group.First().Parent.HasSameOwnerAs(first)
					? new Dictionary<ICurrencyPile, Dictionary<ICoin, int>>
					{
						[firstPile.Object] = new Dictionary<ICoin, int> { [coin.Object] = 6 },
						[secondPile.Object] = new Dictionary<ICoin, int> { [coin.Object] = 4 }
					}
					: new Dictionary<ICurrencyPile, Dictionary<ICoin, int>>
					{
						[thirdPile.Object] = new Dictionary<ICoin, int> { [coin.Object] = 8 }
					};
			});

		var result = Body.FindCurrencyPreservingOwnership(currency.Object,
			[firstPile.Object, secondPile.Object, thirdPile.Object], 10.0M);

		Assert.AreEqual(2, result.Count);
		Assert.IsTrue(result.Keys.All(x => x.Parent.HasSameOwnerAs(first)));
	}

	private static (GameItem First, GameItem Second) CreateItems()
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);

		var stack = new Mock<IStackable>();
		stack.Setup(x => x.PreventsMerging(It.IsAny<IGameItemComponent>())).Returns(false);
		stack.Setup(x => x.FinaliseLoad());
		var componentProto = new Mock<IGameItemComponentProto>();
		componentProto.Setup(x => x.CreateNew(It.IsAny<IGameItem>(), It.IsAny<ICharacter?>(), false))
		              .Returns(stack.Object);

		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(1L);
		proto.SetupGet(x => x.Name).Returns("stack");
		proto.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		proto.SetupGet(x => x.Components).Returns([componentProto.Object]);
		proto.SetupGet(x => x.Morphs).Returns(false);
		proto.SetupGet(x => x.Keywords).Returns(["stack"]);

		return (new GameItem(proto.Object), new GameItem(proto.Object));
	}

	private static Mock<IFrameworkItem> CreateOwner(long id, string type)
	{
		var owner = new Mock<IFrameworkItem>();
		owner.SetupGet(x => x.Id).Returns(id);
		owner.SetupGet(x => x.Name).Returns($"{type} {id}");
		owner.SetupGet(x => x.FrameworkItemType).Returns(type);
		return owner;
	}

	private static Mock<ICurrencyPile> CreateCurrencyPile(IGameItem parent)
	{
		var pile = new Mock<ICurrencyPile>();
		pile.SetupGet(x => x.Parent).Returns(parent);
		return pile;
	}
}
