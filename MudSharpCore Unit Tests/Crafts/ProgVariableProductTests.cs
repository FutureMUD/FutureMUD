using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
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

namespace MudSharp_Unit_Tests.Crafts;

[TestClass]
public class ProgVariableProductTests
{
	[TestMethod]
	public void ProduceProduct_SuppliesCraftOrderedInputsThatMatchEachVariableProgContract()
	{
		var single = new Mock<IGameItem>();
		single.SetupGet(x => x.IsSingleEntity).Returns(true);
		var liquidDummy = new DummyPerceivable("liquid");
		var firstMember = new Mock<IGameItem>();
		firstMember.SetupGet(x => x.IsSingleEntity).Returns(true);
		var secondMember = new Mock<IGameItem>();
		secondMember.SetupGet(x => x.IsSingleEntity).Returns(true);
		var group = new Mock<IPerceivableGroup>();
		group.SetupGet(x => x.IsSingleEntity).Returns(false);
		group.SetupGet(x => x.Members).Returns([firstMember.Object, secondMember.Object]);
		IReadOnlyList<IPerceivable> perceivablesSupplied = null;
		IReadOnlyList<IPerceivable> itemsSupplied = null;
		IReadOnlyList<IPerceivable> anyParametersSupplied = null;

		var perceivableProg = new Mock<IFutureProg>();
		perceivableProg.SetupGet(x => x.Id).Returns(900);
		perceivableProg.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Collection | ProgVariableTypes.Perceivable]);
		perceivableProg.Setup(x => x.ExecuteLong(0L, It.IsAny<object[]>()))
			.Callback((long _, object[] arguments) =>
				perceivablesSupplied = ((IEnumerable<IPerceivable>)arguments.Single()).ToArray())
			.Returns(300);
		var itemProg = new Mock<IFutureProg>();
		itemProg.SetupGet(x => x.Id).Returns(901);
		itemProg.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Collection | ProgVariableTypes.Item]);
		itemProg.Setup(x => x.ExecuteLong(0L, It.IsAny<object[]>()))
			.Callback((long _, object[] arguments) =>
				itemsSupplied = ((IEnumerable<IPerceivable>)arguments.Single()).ToArray())
			.Returns(300);
		var anyParametersProg = new Mock<IFutureProg>();
		anyParametersProg.SetupGet(x => x.Id).Returns(902);
		anyParametersProg.SetupGet(x => x.AcceptsAnyParameters).Returns(true);
		anyParametersProg.Setup(x => x.ExecuteLong(0L, It.IsAny<object[]>()))
			.Callback((long _, object[] arguments) =>
				anyParametersSupplied = ((IEnumerable<IPerceivable>)arguments.Single()).ToArray())
			.Returns(300);
		var progs = UneditableRepository([perceivableProg.Object, itemProg.Object, anyParametersProg.Object]);

		var definition = new Mock<ICharacteristicDefinition>();
		definition.SetupGet(x => x.Id).Returns(200);
		var definitions = UneditableRepository([definition.Object]);
		var value = new Mock<ICharacteristicValue>();
		value.SetupGet(x => x.Id).Returns(300);
		var values = UneditableRepository([value.Object]);

		var output = new Mock<IGameItem>();
		output.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		output.SetupProperty(x => x.Quality, ItemQuality.Standard);
		output.SetupProperty(x => x.Material);
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(100);
		proto.Setup(x => x.IsItemType<StackableGameItemComponentProto>()).Returns(false);
		proto.Setup(x => x.CreateNew(null, null, 1,
			It.IsAny<IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>()))
			.Returns([output.Object]);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		gameworld.SetupGet(x => x.Characteristics).Returns(definitions.Object);
		gameworld.SetupGet(x => x.CharacteristicValues).Returns(values.Object);
		gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository([proto.Object]).Object);
		gameworld.SetupGet(x => x.ItemSkins).Returns(RevisableRepository(Array.Empty<IGameItemSkin>()).Object);
		gameworld.Setup(x => x.GetStaticBool("DisableCraftQualityCalculation")).Returns(false);

		var firstInput = new Mock<ICraftInput>();
		var secondInput = new Mock<ICraftInput>();
		var thirdInput = new Mock<ICraftInput>();
		var craft = new Mock<ICraft>();
		craft.SetupGet(x => x.Inputs).Returns([firstInput.Object, secondInput.Object, thirdInput.Object]);
		var consumed = new Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)>
		{
			[thirdInput.Object] = (group.Object, new TestCraftInputData(group.Object)),
			[secondInput.Object] = (liquidDummy, new TestCraftInputData(liquidDummy)),
			[firstInput.Object] = (single.Object, new TestCraftInputData(single.Object))
		};
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var component = new Mock<IActiveCraftGameItemComponent>();
		component.SetupGet(x => x.Parent).Returns(parent.Object);
		component.SetupGet(x => x.ConsumedInputs).Returns(consumed);

		Product(gameworld.Object, craft.Object, perceivableProg.Object.Id).ProduceProduct(component.Object, ItemQuality.Good);
		Product(gameworld.Object, craft.Object, itemProg.Object.Id).ProduceProduct(component.Object, ItemQuality.Good);
		Product(gameworld.Object, craft.Object, anyParametersProg.Object.Id).ProduceProduct(component.Object, ItemQuality.Good);

		CollectionAssert.AreEqual(
			new IPerceivable[] { single.Object, liquidDummy, firstMember.Object, secondMember.Object },
			perceivablesSupplied!.ToArray());
		CollectionAssert.AreEqual(
			new IPerceivable[] { single.Object, firstMember.Object, secondMember.Object },
			itemsSupplied!.ToArray());
		CollectionAssert.AreEqual(
			new IPerceivable[] { single.Object, liquidDummy, firstMember.Object, secondMember.Object },
			anyParametersSupplied!.ToArray());
		perceivableProg.Verify(x => x.ExecuteLong(0L, It.IsAny<object[]>()), Times.Once);
		itemProg.Verify(x => x.ExecuteLong(0L, It.IsAny<object[]>()), Times.Once);
		anyParametersProg.Verify(x => x.ExecuteLong(0L, It.IsAny<object[]>()), Times.Once);
	}

	private static ProgVariableProduct Product(IFuturemud gameworld, ICraft craft, long progId)
	{
		var product = new CraftProduct
		{
			Id = 1,
			ProductType = "ProgVariableProduct",
			Definition = new XElement("Definition",
				new XElement("ProductProducedId", 100),
				new XElement("Quantity", 1),
				new XElement("Skin", 0),
				new XElement("Variable", new XAttribute("prog", progId), 200)).ToString(),
			OriginalAdditionTime = DateTime.UtcNow
		};
		return (ProgVariableProduct)Activator.CreateInstance(
			typeof(ProgVariableProduct),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[product, craft, gameworld],
			null)!;
	}

	private static Mock<IUneditableAll<T>> UneditableRepository<T>(IEnumerable<T> items)
		where T : class, IFrameworkItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableAll<T>>();
		mock.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		mock.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => list.FirstOrDefault(x => x.Id == id));
		return mock;
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

	private sealed class TestCraftInputData(IPerceivable perceivable) : ICraftInputData
	{
		public XElement SaveToXml() => new("Data");
		public IPerceivable Perceivable => perceivable;
		public ItemQuality InputQuality => ItemQuality.Standard;
		public void FinaliseLoadTimeTasks() { }
		public void Delete() { }
		public void Quit() { }
	}
}
