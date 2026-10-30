#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy.Currency;
using MudSharp.GameItems;
using MudSharp.FutureProg.Functions.Currency;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;
using MudSharp.GameItems.Components;
using DbGameItemComponent = MudSharp.Models.GameItemComponent;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CurrencyDefaultPileTests
{
	[TestMethod]
	public void CurrencyPilePrototype_RejectsIncompleteDefaultConfiguration()
	{
		var prototype = TestObjectFactory.CreateUninitialized<CurrencyGameItemComponentProto>();
		var exception = Assert.ThrowsExactly<TargetInvocationException>(() => LoadDefinition(prototype, "<Definition DefaultCurrency=\"1\"/>"));
		Assert.IsInstanceOfType<FormatException>(exception.InnerException);
	}

	[TestMethod]
	public void CurrencyPilePrototype_RejectsNonPositiveDefaultAmount()
	{
		var prototype = TestObjectFactory.CreateUninitialized<CurrencyGameItemComponentProto>();
		var exception = Assert.ThrowsExactly<TargetInvocationException>(() => LoadDefinition(prototype,
			"<Definition DefaultCurrency=\"1\" DefaultAmount=\"0\"/>"));
		Assert.IsInstanceOfType<FormatException>(exception.InnerException);
	}

	[TestMethod]
	public void ConfiguredCurrencyPile_CreateNewMintsDefaultOnce_AndCopyOrReloadDoNotRefill()
	{
		var coin = new Mock<ICoin>();
		coin.SetupGet(x => x.Id).Returns(81);
		coin.SetupGet(x => x.Value).Returns(1m);
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(77);
		currency.SetupGet(x => x.Coins).Returns([coin.Object]);
		var coins = new Dictionary<ICoin, int> { [coin.Object] = 12 };
		var exact = true;
		currency.Setup(x => x.FindCoinsForAmount(12m, out exact)).Returns(coins);

		var world = new Mock<IFuturemud>();
		var currencies = new Mock<IUneditableAll<ICurrency>>();
		currencies.Setup(x => x.Get(77)).Returns(currency.Object);
		world.SetupGet(x => x.Currencies).Returns(currencies.Object);
		var prototype = TestObjectFactory.CreateUninitialized<CurrencyGameItemComponentProto>();
		typeof(CurrencyGameItemComponentProto).GetProperty(nameof(CurrencyGameItemComponentProto.DefaultCurrency))!
			.SetValue(prototype, currency.Object);
		typeof(CurrencyGameItemComponentProto).GetProperty(nameof(CurrencyGameItemComponentProto.DefaultAmount))!
			.SetValue(prototype, 12m);
		var parent = CreateParent(world.Object);

		var fresh = (CurrencyGameItemComponent)prototype.CreateNew(parent.Object, temporary: true);
		Assert.AreSame(currency.Object, fresh.Currency);
		Assert.AreEqual(12, fresh.Coins.Single().Item2, "A new configured pile should receive exactly its authored default.");
		Assert.AreEqual(12m, fresh.TotalValue);

		Assert.IsFalse(fresh.RemoveCoins([Tuple.Create<ICoin, int>(coin.Object, 12)]));
		Assert.AreEqual(0, fresh.Coins.Count(), "The source pile is now spent.");

		var copy = (CurrencyGameItemComponent)fresh.Copy(CreateParent(world.Object).Object, temporary: true);
		Assert.AreEqual(0, copy.Coins.Count(), "Copying a spent pile must preserve its spent state.");

		var saveMethod = typeof(CurrencyGameItemComponent).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var spentDefinition = (string)saveMethod.Invoke(fresh, null)!;
		var loaded = (CurrencyGameItemComponent)prototype.LoadComponent(
			new DbGameItemComponent { Id = 42, Definition = spentDefinition }, CreateParent(world.Object).Object);
		Assert.AreEqual(0, loaded.Coins.Count(), "Loading persisted empty coins must not reapply prototype defaults.");
		Assert.AreSame(currency.Object, loaded.Currency);
		currency.Verify(x => x.FindCoinsForAmount(12m, out It.Ref<bool>.IsAny), Times.Once,
			"Only the original CreateNew should calculate default coins.");
	}

	[TestMethod]
	public void GiveCurrency_UsesOnlyTargetPileWithMatchingCurrency()
	{
		var currency = new Mock<ICurrency>();
		var otherCurrency = new Mock<ICurrency>();
		var pile = new Mock<ICurrencyPile>();
		pile.SetupGet(x => x.Currency).Returns(otherCurrency.Object);
		var target = new Mock<IGameItem>();
		target.Setup(x => x.IsItemType<ICurrencyPile>()).Returns(true);
		target.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(pile.Object);
		currency.Setup(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());

		Assert.IsFalse(GiveCurrencyFunction.GiveCurrency(currency.Object, 5m, target.Object, false));
		pile.Verify(x => x.AddCoins(It.IsAny<IEnumerable<Tuple<ICoin, int>>>()), Times.Never);
	}

	private static void LoadDefinition(CurrencyGameItemComponentProto prototype, string xml)
	{
		var method = typeof(CurrencyGameItemComponentProto).GetMethod("LoadFromXml", BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(prototype, [XElement.Parse(xml)]);
	}

	private static Mock<IGameItem> CreateParent(IFuturemud world)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(world);
		return parent;
	}
}
