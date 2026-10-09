#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StealthPlanarInventorySecurityTests
{
	[DataTestMethod]
	[DataRow("owner")]
	[DataRow("container")]
	[DataRow("item")]
	[DataRow("currency")]
	[DataRow("none")]
	public void InventoryAccess_ChecksEachParticipantAndAllowsOrdinaryTransfers(string blocked)
	{
		var actor = Character(1);
		var owner = Character(blocked == "owner" ? 2 : 1);
		var container = Item(blocked == "container" ? 2 : 1);
		var item = Item(blocked == "item" ? 2 : 1);
		var money = Item(blocked == "currency" ? 2 : 1);
		var currency = Mock.Of<ICurrency>();
		money.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(Mock.Of<ICurrencyPile>(x => x.Currency == currency));
		var result = (bool)typeof(StealthModule).GetMethod("CheckStealthInventoryAccess",
			BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null,
			[actor.Object, owner.Object, container.Object, item.Object, currency, new[] { money.Object }])!;
		Assert.AreEqual(blocked == "none", result);
	}

	[DataTestMethod]
	[DataRow("StealItemFromContainer", "owner")]
	[DataRow("StealItemFromContainer", "container")]
	[DataRow("StealItemFromContainer", "item")]
	[DataRow("StealCurrencyFromContainer", "owner")]
	[DataRow("StealCurrencyFromContainer", "container")]
	[DataRow("StealCurrencyFromContainer", "item")]
	[DataRow("StealBeltedItem", "owner")]
	[DataRow("StealBeltedItem", "container")]
	[DataRow("StealBeltedItem", "item")]
	public void StealTransfer_RejectsCrossPlaneBeforeInventoryOrToolChecks(string method, string blocked)
	{
		var actor = Character(1);
		var owner = Character(blocked == "owner" ? 2 : 1);
		var source = Item(blocked == "container" ? 2 : 1);
		var item = Item(blocked == "item" ? 2 : 1);
		var currency = Mock.Of<ICurrency>();
		item.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(Mock.Of<ICurrencyPile>(x => x.Currency == currency));
		source.Setup(x => x.GetItemType<IContainer>()).Returns(Mock.Of<IContainer>(x => x.Contents == new[] { item.Object }));
		object?[] arguments = method switch
		{
			"StealItemFromContainer" => [actor.Object, owner.Object, item.Object, 0, source.Object],
			"StealCurrencyFromContainer" => [actor.Object, owner.Object, currency, 1m, false, source.Object],
			_ => [actor.Object, owner.Object, source.Object, item.Object]
		};
		typeof(StealthModule).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, arguments);
		Mock.Get(actor.Object.Body).Verify(x => x.CanGet(It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
		Mock.Get(actor.Object.Body).Verify(x => x.CanGet(It.IsAny<IGameItem>(), It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
		Mock.Get(actor.Object.Body).Verify(x => x.CanGet(It.IsAny<ICurrency>(), It.IsAny<IGameItem>(), It.IsAny<decimal>(), It.IsAny<bool>()), Times.Never);
	}

	private static Mock<ICharacter> Character(long plane)
	{
		var body = new Mock<IBody>();
		body.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(plane));
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		return actor;
	}

	private static Mock<IGameItem> Item(long plane)
	{
		var item = new Mock<IGameItem>();
		item.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(plane));
		return item;
	}
}
