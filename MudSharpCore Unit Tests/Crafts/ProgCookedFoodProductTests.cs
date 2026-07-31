using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Products;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests.Crafts;

[TestClass]
public class ProgCookedFoodProductTests
{
	[TestMethod]
	public void ProductFactory_RegistersTheGeneralizedCookedFoodProduct()
	{
		Assert.IsTrue(CraftProductFactory.Factory.ValidBuilderTypes.Contains("progcookedfood"));
		Assert.IsTrue(CraftProductFactory.Factory.ValidBuilderTypes.Contains("prog cooked food"));
	}

	[TestMethod]
	public void SelectorProgShape_CompilesAsASingleSelectedItem()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new MudSharp.FutureProg.FutureProg(
			FutureProgTestBootstrap.Gameworld,
			"TestCatalogueSelector",
			ProgVariableTypes.Item,
			[],
			"var products as item collection\nadditem products loaditem(\"test_food_proto\")\nreturn collectionfirst(collectionshuffle(@products))");

		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.IsTrue(prog.ReturnType.CompatibleWith(ProgVariableTypes.Item));
	}

	[TestMethod]
	public void SelectorProgShape_AlsoAcceptsACollectionOfPreparedFoodItems()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new MudSharp.FutureProg.FutureProg(
			FutureProgTestBootstrap.Gameworld,
			"TestCatalogueSelectorCollection",
			ProgVariableTypes.Collection | ProgVariableTypes.Item,
			[],
			"var products as item collection\nadditem products loaditem(\"test_food_proto_one\")\nadditem products loaditem(\"test_food_proto_two\")\nreturn @products");

		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.IsTrue(prog.ReturnType.CompatibleWith(ProgVariableTypes.Collection | ProgVariableTypes.Item));
	}

	[TestMethod]
	public void ProductMaterialisesEveryCollectionSelectionThroughTheCookedFoodLedger()
	{
		PreparedCounts.Clear();
		var firstProto = PreparedProto(101, "a barley flatbread");
		var secondProto = PreparedProto(102, "a millet flatbread");
		var firstSelected = Item(firstProto);
		var secondSelected = Item(secondProto);
		var firstOutput = PreparedOutput(firstProto);
		var secondOutput = PreparedOutput(secondProto);
		firstProto.Setup(x => x.CreateNew(null)).Returns(firstOutput.Object);
		secondProto.Setup(x => x.CreateNew(null)).Returns(secondOutput.Object);

		var selector = new Mock<IFutureProg>();
		selector.SetupGet(x => x.Id).Returns(9001);
		selector.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Collection | ProgVariableTypes.Item);
		selector.Setup(x => x.ExecuteCollection<IGameItem>(It.IsAny<object[]>()))
			.Returns([firstSelected.Object, secondSelected.Object]);

		var futureProgs = new Mock<IUneditableAll<IFutureProg>>();
		futureProgs.Setup(x => x.Get(9001)).Returns(selector.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
		gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository([firstProto.Object, secondProto.Object]).Object);
		gameworld.SetupGet(x => x.ItemSkins).Returns(RevisableRepository(Array.Empty<IGameItemSkin>()).Object);
		gameworld.Setup(x => x.GetStaticBool("DisableCraftQualityCalculation")).Returns(false);
		gameworld.Setup(x => x.Add(It.IsAny<IGameItem>()));

		var sourceProto = Proto(103, "a pile of grain");
		var source = Item(sourceProto);
		source.SetupGet(x => x.Quality).Returns(ItemQuality.Good);
		var input = new Mock<ICraftInput>();
		input.SetupGet(x => x.Id).Returns(1);
		var craft = new Mock<ICraft>();
		craft.SetupGet(x => x.Inputs).Returns([input.Object]);
		var component = new Mock<IActiveCraftGameItemComponent>();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		component.SetupGet(x => x.Parent).Returns(parent.Object);
		component.SetupGet(x => x.ConsumedInputs).Returns(
			new Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)>
			{
				[input.Object] = (source.Object, new TestCraftInputData(source.Object))
			});

		var product = Product(gameworld.Object, craft.Object);
		var result = product.ProduceProduct(component.Object, ItemQuality.Good);

		Assert.IsInstanceOfType(result, typeof(ICraftProductDataWithItems));
		var produced = ((ICraftProductDataWithItems)result).Products.ToArray();
		Assert.AreEqual(2, produced.Length);
		Assert.AreEqual(1, IngredientCount(firstOutput));
		Assert.AreEqual(1, IngredientCount(secondOutput));
		firstSelected.Verify(x => x.Delete(), Times.Once);
		secondSelected.Verify(x => x.Delete(), Times.Once);
	}

	private static ProgCookedFoodProduct Product(IFuturemud gameworld, ICraft craft)
	{
		var craftProduct = new CraftProduct
		{
			Id = 1,
			ProductType = "ProgCookedFoodProduct",
			Definition = new XElement("Definition",
				new XElement("ItemProg", 9001),
				new XElement("Quantity", 1),
				new XElement("Skin", 0),
				new XElement("RemoveDrugsAndFoodEffects", false),
				new XElement("IngredientSlots")
			).ToString(),
			OriginalAdditionTime = DateTime.UtcNow
		};
		return (ProgCookedFoodProduct)Activator.CreateInstance(
			typeof(ProgCookedFoodProduct),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[craftProduct, craft, gameworld],
			null)!;
	}

	private static Mock<IGameItemProto> PreparedProto(long id, string sdesc)
	{
		var proto = Proto(id, sdesc);
		proto.Setup(x => x.IsItemType<PreparedFoodGameItemComponentProto>()).Returns(true);
		return proto;
	}

	private static Mock<IGameItem> PreparedOutput(Mock<IGameItemProto> proto)
	{
		var prepared = new Mock<IPreparedFood>();
		var output = Item(proto);
		output.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		output.SetupProperty(x => x.Quality, ItemQuality.Standard);
		output.SetupProperty(x => x.Skin);
		output.Setup(x => x.GetItemType<IPreparedFood>()).Returns(prepared.Object);
		prepared.Setup(x => x.AddIngredient(It.IsAny<FoodIngredientInstance>()))
			.Callback<FoodIngredientInstance>(_ => PreparedCounts[proto.Object] = PreparedCounts.GetValueOrDefault(proto.Object) + 1);
		return output;
	}

	private static readonly Dictionary<IGameItemProto, int> PreparedCounts = new();

	private static int IngredientCount(Mock<IGameItem> output)
	{
		return PreparedCounts.GetValueOrDefault(output.Object.Prototype);
	}

	private static Mock<IGameItem> Item(Mock<IGameItemProto> proto)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Prototype).Returns(proto.Object);
		return item;
	}

	private static Mock<IGameItemProto> Proto(long id, string sdesc)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id);
		proto.SetupGet(x => x.Name).Returns(sdesc);
		proto.SetupGet(x => x.ShortDescription).Returns(sdesc);
		proto.SetupGet(x => x.Weight).Returns(100.0);
		proto.Setup(x => x.IsItemType<StackableGameItemComponentProto>()).Returns(false);
		return proto;
	}

	private static Mock<IUneditableRevisableAll<T>> RevisableRepository<T>(IEnumerable<T> items)
		where T : class, IRevisableItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableRevisableAll<T>>();
		mock.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		mock.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => list.FirstOrDefault(x => x.Id == id));
		return mock;
	}

	private sealed class TestCraftInputData(IGameItem item) : ICraftInputDataWithItems
	{
		public XElement SaveToXml() => new("Data");
		public IPerceivable Perceivable => item;
		public ItemQuality InputQuality => item.Quality;
		public IEnumerable<IGameItem> ConsumedItems => [item];
		public void FinaliseLoadTimeTasks() { }
		public void Delete() { }
		public void Quit() { }
	}
}
