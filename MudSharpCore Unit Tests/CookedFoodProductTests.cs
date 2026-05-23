using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
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
public class CookedFoodProductTests
{
	[TestMethod]
	public void CookedFoodProduct_ProduceProduct_UsesCommodityWeightAndMaterialForIngredients()
	{
		var material = Material(10, "wheat", "ground wheat").Object;
		var tag = Tag(20, "flour").Object;
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Material).Returns(material);
		commodity.SetupGet(x => x.Weight).Returns(500.0);
		commodity.SetupGet(x => x.Tag).Returns(tag);
		commodity.SetupGet(x => x.CommodityCharacteristics)
		         .Returns(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>());

		var sourceProto = Proto(100, "a pile of ground wheat").Object;
		var sourceItem = Item(sourceProto);
		sourceItem.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		sourceItem.SetupGet(x => x.Quality).Returns(ItemQuality.Good);

		FoodIngredientInstance captured = null;
		var prepared = new Mock<IPreparedFood>();
		prepared.Setup(x => x.AddIngredient(It.IsAny<FoodIngredientInstance>()))
		        .Callback<FoodIngredientInstance>(x => captured = x.Clone());

		var productProto = Proto(200, "a flatbread").Object;
		var productItem = Item(productProto);
		productItem.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		productItem.SetupProperty(x => x.Quality, ItemQuality.Standard);
		productItem.SetupProperty(x => x.Skin);
		productItem.Setup(x => x.GetItemType<IPreparedFood>()).Returns(prepared.Object);
		Mock.Get(productProto).Setup(x => x.CreateNew(null)).Returns(productItem.Object);

		var input = new Mock<ICraftInput>();
		input.SetupGet(x => x.Id).Returns(1);
		var consumedInputs = new Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)>
		{
			{ input.Object, (sourceItem.Object, new TestCraftInputData(sourceItem.Object)) }
		};
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var component = new Mock<IActiveCraftGameItemComponent>();
		component.SetupGet(x => x.Parent).Returns(parent.Object);
		component.SetupGet(x => x.ConsumedInputs).Returns(consumedInputs);

		var product = Product(productProto);

		product.ProduceProduct(component.Object, ItemQuality.Good);

		Assert.IsNotNull(captured);
		Assert.AreEqual("ingredient", captured.Role);
		Assert.AreEqual(material.Id, captured.MaterialId);
		Assert.AreEqual(500.0, captured.Weight);
		Assert.AreEqual(ItemQuality.Good, captured.Quality);
		StringAssert.Contains(captured.Description, "ground wheat");
	}

	private static CookedFoodProduct Product(IGameItemProto proto)
	{
		var gameworld = Gameworld(proto).Object;
		var craftProduct = new CraftProduct
		{
			Id = 1,
			ProductType = "CookedFoodProduct",
			Definition = new XElement("Definition",
				new XElement("ProductProducedId", proto.Id),
				new XElement("Quantity", 1),
				new XElement("Skin", 0),
				new XElement("RemoveDrugsAndFoodEffects", false),
				new XElement("IngredientSlots")
			).ToString(),
			OriginalAdditionTime = DateTime.UtcNow
		};
		return (CookedFoodProduct)Activator.CreateInstance(
			typeof(CookedFoodProduct),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { craftProduct, new Mock<ICraft>().Object, gameworld },
			null)!;
	}

	private static Mock<IFuturemud> Gameworld(IGameItemProto proto)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository(new[] { proto }).Object);
		gameworld.SetupGet(x => x.ItemSkins).Returns(RevisableRepository(Array.Empty<IGameItemSkin>()).Object);
		gameworld.Setup(x => x.GetStaticBool("DisableCraftQualityCalculation")).Returns(false);
		gameworld.Setup(x => x.Add(It.IsAny<IGameItem>()));
		return gameworld;
	}

	private static Mock<ISolid> Material(long id, string name, string description)
	{
		var mock = new Mock<ISolid>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.MaterialDescription).Returns(description);
		return mock;
	}

	private static Mock<ITag> Tag(long id, string name)
	{
		var mock = new Mock<ITag>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		return mock;
	}

	private static Mock<IGameItem> Item(IGameItemProto proto)
	{
		var mock = new Mock<IGameItem>();
		mock.SetupGet(x => x.Prototype).Returns(proto);
		return mock;
	}

	private static Mock<IGameItemProto> Proto(long id, string sdesc)
	{
		var mock = new Mock<IGameItemProto>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(sdesc);
		mock.SetupGet(x => x.ShortDescription).Returns(sdesc);
		mock.Setup(x => x.IsItemType<StackableGameItemComponentProto>()).Returns(false);
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

	private sealed class TestCraftInputData(IGameItem item) : ICraftInputDataWithItems
	{
		public XElement SaveToXml()
		{
			return new XElement("Data");
		}

		public IPerceivable Perceivable => item;
		public ItemQuality InputQuality => item.Quality;
		public IEnumerable<IGameItem> ConsumedItems => [item];

		public void FinaliseLoadTimeTasks()
		{
		}

		public void Delete()
		{
		}

		public void Quit()
		{
		}
	}
}
