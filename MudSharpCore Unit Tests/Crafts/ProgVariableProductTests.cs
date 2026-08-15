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
using MudSharp.PerceptionEngine.Lists;
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
	public void GetIndividualPerceivables_FlattensMixedSinglesAndGroupsInOrder()
	{
		var single = new DummyPerceivable("single");
		var liquidDummy = new DummyPerceivable("liquid");
		var firstMember = new DummyPerceivable("first member");
		var secondMember = new DummyPerceivable("second member");
		var group = new PerceivableGroup([firstMember, secondMember]);

		var result = new IPerceivable[] { single, liquidDummy, group }
			.GetIndividualPerceivables()
			.ToArray();

		CollectionAssert.AreEqual(
			new IPerceivable[] { single, liquidDummy, firstMember, secondMember },
			result);
	}

	[TestMethod]
	public void ProduceProduct_SuppliesFlattenedConsumedPerceivablesToVariableProg()
	{
		var single = new DummyPerceivable("single");
		var liquidDummy = new DummyPerceivable("liquid");
		var firstMember = new DummyPerceivable("first member");
		var secondMember = new DummyPerceivable("second member");
		var group = new PerceivableGroup([firstMember, secondMember]);
		IReadOnlyList<IPerceivable> supplied = null;

		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(900);
		prog.Setup(x => x.ExecuteLong(0L, It.IsAny<object[]>()))
			.Callback((long _, object[] arguments) =>
				supplied = ((IEnumerable<IPerceivable>)arguments.Single()).ToArray())
			.Returns(300);
		var progs = UneditableRepository([prog.Object]);

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
		var consumed = new Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)>
		{
			[firstInput.Object] = (single, new TestCraftInputData(single)),
			[secondInput.Object] = (liquidDummy, new TestCraftInputData(liquidDummy)),
			[thirdInput.Object] = (group, new TestCraftInputData(group))
		};
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var component = new Mock<IActiveCraftGameItemComponent>();
		component.SetupGet(x => x.Parent).Returns(parent.Object);
		component.SetupGet(x => x.ConsumedInputs).Returns(consumed);

		Product(gameworld.Object).ProduceProduct(component.Object, ItemQuality.Good);

		CollectionAssert.AreEqual(
			new IPerceivable[] { single, liquidDummy, firstMember, secondMember },
			supplied!.ToArray());
		prog.Verify(x => x.ExecuteLong(0L, It.IsAny<object[]>()), Times.Once);
	}

	private static ProgVariableProduct Product(IFuturemud gameworld)
	{
		var product = new CraftProduct
		{
			Id = 1,
			ProductType = "ProgVariableProduct",
			Definition = new XElement("Definition",
				new XElement("ProductProducedId", 100),
				new XElement("Quantity", 1),
				new XElement("Skin", 0),
				new XElement("Variable", new XAttribute("prog", 900), 200)).ToString(),
			OriginalAdditionTime = DateTime.UtcNow
		};
		return (ProgVariableProduct)Activator.CreateInstance(
			typeof(ProgVariableProduct),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[product, new Mock<ICraft>().Object, gameworld],
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
