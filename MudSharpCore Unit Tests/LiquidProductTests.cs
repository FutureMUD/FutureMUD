using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LiquidProductTests
{
	[TestMethod]
	public void LiquidProduct_ProduceProduct_FillsCreatedContainer()
	{
		var liquid = Liquid(1, "barley beer").Object;
		var container = LiquidContainer(capacity: 5.0, currentVolume: 0.0);
		var item = Item(container.Object);
		var proto = Proto(10, "a sealed amphora", item.Object, hasLiquidContainerPrototype: true).Object;
		var product = Product(proto, liquid, volume: 3.0);
		var component = ActiveCraftComponent();

		var data = product.ProduceProduct(component.Object, ItemQuality.Good);

		var products = ((ICraftProductDataWithItems)data).Products.ToList();
		Assert.AreEqual(1, products.Count);
		Assert.AreSame(item.Object, products[0]);
		container.Verify(x => x.MergeLiquid(It.Is<LiquidMixture>(m => Math.Abs(m.TotalVolume - 3.0) < 0.0001), null, "craft"), Times.Once);
		item.Verify(x => x.HandleEvent(EventType.ItemFinishedLoading, item.Object), Times.Never);

		var location = new Mock<ICell>();
		data.ReleaseProducts(location.Object, RoomLayer.GroundLevel);

		item.Verify(x => x.HandleEvent(EventType.ItemFinishedLoading, item.Object), Times.Once);
		item.Verify(x => x.Login(), Times.Once);
	}

	[TestMethod]
	public void LiquidProduct_ProduceProduct_RejectsNonContainerTargets()
	{
		var liquid = Liquid(1, "barley beer").Object;
		var item = Item(null);
		var proto = Proto(10, "a sealed amphora", item.Object, hasLiquidContainerPrototype: false).Object;
		var product = Product(proto, liquid, volume: 3.0);

		var exception = Assert.ThrowsException<ApplicationException>(() =>
			product.ProduceProduct(ActiveCraftComponent().Object, ItemQuality.Standard));

		StringAssert.Contains(exception.Message, "is not a liquid container");
		item.Verify(x => x.Delete(), Times.Once);
	}

	[TestMethod]
	public void LiquidProduct_ProduceProduct_RejectsInsufficientCapacity()
	{
		var liquid = Liquid(1, "barley beer").Object;
		var container = LiquidContainer(capacity: 2.0, currentVolume: 0.5);
		var item = Item(container.Object);
		var proto = Proto(10, "a sealed amphora", item.Object, hasLiquidContainerPrototype: true).Object;
		var product = Product(proto, liquid, volume: 3.0);

		var exception = Assert.ThrowsException<ApplicationException>(() =>
			product.ProduceProduct(ActiveCraftComponent().Object, ItemQuality.Standard));

		StringAssert.Contains(exception.Message, "only has");
		container.Verify(x => x.MergeLiquid(It.IsAny<LiquidMixture>(), null, "craft"), Times.Never);
		item.Verify(x => x.Delete(), Times.Once);
	}

	private static LiquidProduct Product(IGameItemProto proto, ILiquid liquid, double volume)
	{
		var gameworld = Gameworld(proto, liquid).Object;
		var craftProduct = new CraftProduct
		{
			Id = 1,
			ProductType = "LiquidProduct",
			Definition = new XElement("Definition",
				new XElement("ProductProducedId", proto.Id),
				new XElement("Quantity", 1),
				new XElement("Liquid", liquid.Id),
				new XElement("LiquidVolume", volume),
				new XElement("Skin", 0)
			).ToString(),
			OriginalAdditionTime = DateTime.UtcNow
		};
		return (LiquidProduct)Activator.CreateInstance(
			typeof(LiquidProduct),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { craftProduct, new Mock<ICraft>().Object, gameworld },
			null)!;
	}

	private static Mock<IFuturemud> Gameworld(IGameItemProto proto, ILiquid liquid)
	{
		var unitManager = new Mock<IUnitManager>();
		unitManager.Setup(x => x.DescribeMostSignificantExact(It.IsAny<double>(), UnitType.FluidVolume, It.IsAny<MudSharp.Accounts.IAccount>()))
		           .Returns<double, UnitType, MudSharp.Accounts.IAccount>((value, _, _) => $"{value:N1} litres");
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository(new[] { proto }).Object);
		gameworld.SetupGet(x => x.Liquids).Returns(Repository(new[] { liquid }).Object);
		gameworld.SetupGet(x => x.ItemSkins).Returns(RevisableRepository(Array.Empty<IGameItemSkin>()).Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		gameworld.Setup(x => x.GetStaticBool("DisableCraftQualityCalculation")).Returns(false);
		gameworld.Setup(x => x.Add(It.IsAny<IGameItem>()));
		return gameworld;
	}

	private static Mock<ILiquid> Liquid(long id, string name)
	{
		var mock = new Mock<ILiquid>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.DisplayColour).Returns(Telnet.Yellow);
		return mock;
	}

	private static Mock<ILiquidContainer> LiquidContainer(double capacity, double currentVolume)
	{
		var mock = new Mock<ILiquidContainer>();
		mock.SetupGet(x => x.LiquidCapacity).Returns(capacity);
		mock.SetupGet(x => x.LiquidVolume).Returns(currentVolume);
		return mock;
	}

	private static Mock<IGameItem> Item(ILiquidContainer container)
	{
		var mock = new Mock<IGameItem>();
		mock.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		mock.SetupProperty(x => x.Quality, ItemQuality.Standard);
		mock.SetupProperty(x => x.Skin);
		mock.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(container);
		return mock;
	}

	private static Mock<IGameItemProto> Proto(long id, string sdesc, IGameItem item, bool hasLiquidContainerPrototype)
	{
		var mock = new Mock<IGameItemProto>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(sdesc);
		mock.SetupGet(x => x.ShortDescription).Returns(sdesc);
		mock.Setup(x => x.IsItemType<ILiquidContainerPrototype>()).Returns(hasLiquidContainerPrototype);
		mock.Setup(x => x.CreateNew(null)).Returns(item);
		Mock.Get(item).SetupGet(x => x.Prototype).Returns(mock.Object);
		return mock;
	}

	private static Mock<IActiveCraftGameItemComponent> ActiveCraftComponent()
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var component = new Mock<IActiveCraftGameItemComponent>();
		component.SetupGet(x => x.Parent).Returns(parent.Object);
		return component;
	}

	private static Mock<IUneditableAll<T>> Repository<T>(IEnumerable<T> items) where T : class, IFrameworkItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableAll<T>>();
		mock.As<IEnumerable<T>>()
		    .Setup(x => x.GetEnumerator())
		    .Returns(() => list.GetEnumerator());
		mock.Setup(x => x.Get(It.IsAny<long>()))
		    .Returns((long id) => list.FirstOrDefault(x => x.Id == id));
		mock.Setup(x => x.GetByName(It.IsAny<string>()))
		    .Returns((string name) => list.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
		mock.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
		    .Returns((string text, bool _) =>
			    long.TryParse(text, out var id)
				    ? list.FirstOrDefault(x => x.Id == id)
				    : list.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)));
		mock.SetupGet(x => x.Count).Returns(list.Count);
		return mock;
	}

	private static Mock<IUneditableRevisableAll<T>> RevisableRepository<T>(IEnumerable<T> items) where T : class, IRevisableItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableRevisableAll<T>>();
		mock.As<IEnumerable<T>>()
		    .Setup(x => x.GetEnumerator())
		    .Returns(() => list.GetEnumerator());
		mock.Setup(x => x.Get(It.IsAny<long>()))
		    .Returns((long id) => list.FirstOrDefault(x => x.Id == id));
		mock.Setup(x => x.GetByName(It.IsAny<string>(), It.IsAny<bool>()))
		    .Returns((string name, bool ignoreCase) => list.FirstOrDefault(x => x.Name.Equals(name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture)));
		mock.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
		    .Returns((string text, bool _) =>
			    long.TryParse(text, out var id)
				    ? list.FirstOrDefault(x => x.Id == id)
				    : list.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)));
		return mock;
	}
}
