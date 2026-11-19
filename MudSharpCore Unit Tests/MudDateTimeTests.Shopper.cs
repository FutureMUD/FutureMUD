#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Shoppers;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

public partial class MudDateTimeTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DoShop_ItemCallbacksReceiveItemOrLegacyShop(bool legacyShopSignature)
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = new Mock<IFuturemud>();
		var shop = new Mock<IShop>();
		var item = Mock.Of<IGameItem>(x => x.Quantity == 1);
		var merchandise = Mock.Of<IMerchandise>();
		const decimal price = 5.0m;
		var zone = Mock.Of<IEconomicZone>(x => x.Id == 1 &&
			x.FinancialPeriodReferenceCalendar == _testCalendar && x.Currency == Mock.Of<ICurrency>());
		shop.SetupGet(x => x.Id).Returns(1);
		shop.SetupGet(x => x.Name).Returns("Market Bakery");
		shop.SetupGet(x => x.EconomicZone).Returns(zone);
		shop.SetupGet(x => x.IsReadyToDoBusiness).Returns(true);
		shop.SetupGet(x => x.IsTrading).Returns(true);
		shop.SetupGet(x => x.AllMerchandiseForVirtualShoppers).Returns([(item, merchandise, price)]);
		world.SetupGet(x => x.Shops).Returns(new All<IShop> { shop.Object });
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var selection = CreateShopperProg(1, ProgVariableTypes.Boolean, ProgVariableTypes.Shop);
		var shopWeight = CreateShopperProg(2, ProgVariableTypes.Number, ProgVariableTypes.Shop);
		selection.Setup(x => x.ExecuteBool(shop.Object)).Returns(true);
		shopWeight.Setup(x => x.ExecuteDouble(1.0, shop.Object)).Returns(1.0);
		world.SetupGet(x => x.AlwaysTrueProg).Returns(selection.Object);
		world.SetupGet(x => x.AlwaysOneProg).Returns(shopWeight.Object);
		var firstParameter = legacyShopSignature ? ProgVariableTypes.Shop : ProgVariableTypes.Item;
		var eligibility = CreateShopperProg(3, ProgVariableTypes.Boolean, firstParameter,
			ProgVariableTypes.Merchandise, ProgVariableTypes.Number);
		var weight = CreateShopperProg(4, ProgVariableTypes.Number, firstParameter,
			ProgVariableTypes.Merchandise, ProgVariableTypes.Number);
		object expected = legacyShopSignature ? shop.Object : item;
		eligibility.Setup(x => x.ExecuteBool(expected, merchandise, price)).Returns(true);
		weight.Setup(x => x.ExecuteDouble(1.0, expected, merchandise, price)).Returns(1.0);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>
		{
			selection.Object, shopWeight.Object, eligibility.Object, weight.Object
		});
		var builder = Mock.Of<ICharacter>(x => x.Gameworld == world.Object &&
			x.OutputHandler == Mock.Of<IOutputHandler>());
		using var shopper = new TestShopper(world.Object, zone);
		shopper.LoadCallbacks(legacyShopSignature ? 3 : 1, legacyShopSignature ? 4 : 2);
		if (!legacyShopSignature)
		{
			Assert.IsTrue(shopper.BuildingCommand(builder, new StringStack("item 3")));
			Assert.IsTrue(shopper.BuildingCommand(builder, new StringStack("itemweight 4")));
		}
		else
		{
			Assert.IsFalse(shopper.BuildingCommand(builder, new StringStack("item 3")));
			Assert.IsFalse(shopper.BuildingCommand(builder, new StringStack("itemweight 4")));
		}

		shopper.DoShop();

		eligibility.Verify(x => x.ExecuteBool(expected, merchandise, price), Times.Once);
		weight.Verify(x => x.ExecuteDouble(1.0, expected, merchandise, price), Times.Once);
		shop.Verify(x => x.BuyVirtualShopper(merchandise, item, 1), Times.Once);
	}

	private static Mock<IFutureProg> CreateShopperProg(long id, ProgVariableTypes result,
		params ProgVariableTypes[] parameters)
	{
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(id);
		prog.SetupGet(x => x.Name).Returns($"ShopperProg{id}");
		prog.SetupGet(x => x.Public).Returns(true);
		prog.SetupGet(x => x.ReturnType).Returns(result);
		prog.SetupGet(x => x.Parameters).Returns(parameters);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
			.Returns((IEnumerable<ProgVariableTypes> arguments) => arguments.SequenceEqual(parameters));
		return prog;
	}

	private sealed class TestShopper(IFuturemud world, IEconomicZone zone) : SimpleShopper(world, "Bakery Customer", zone),
		IDisposable
	{
		public void LoadCallbacks(long eligibility, long weight)
		{
			LoadFromDefinition(new XElement("Shopper",
				new XElement("BudgetPerShop", 5),
				new XElement("WillShopAtShopProg", 1),
				new XElement("ShopSelectionWeightProg", 2),
				new XElement("WillBuyItemProg", eligibility),
				new XElement("ItemBuyWeightProg", weight)));
		}

		public void Dispose() => ReleaseListener();
	}
}
