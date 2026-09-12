#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Products;
using CraftProduct = MudSharp.Models.CraftProduct;

namespace MudSharp_Unit_Tests.Crafts;

[TestClass]
public class SimpleVariableProductTests
{
	[TestMethod]
	[DataRow(false, false, false)]
	[DataRow(true, false, false)]
	[DataRow(false, true, true)]
	[DataRow(true, true, true)]
	public void ProduceProduct_MixedRules_PreservesSkinValuesQuantityAndQuality(bool stackable, bool failProduct, bool disableQuality)
	{
		var fixture = new Fixture();
		fixture.Proto.Setup(x => x.IsItemType<StackableGameItemComponentProto>()).Returns(stackable);
		fixture.Gameworld.Setup(x => x.GetStaticBool("DisableCraftQualityCalculation")).Returns(disableQuality);
		var product = fixture.Product(failProduct);
		Assert.IsTrue(product.IsValid(), product.WhyNotValid());
		var reloaded = fixture.Product(failProduct, product.Xml());
		var result = (ICraftProductDataWithItems)reloaded.ProduceProduct(fixture.Active.Object, ItemQuality.Excellent);
		CollectionAssert.AreEqual(new[] { fixture.Output.Object }, result.Products.ToArray());
		Assert.AreEqual(2, fixture.Supplied!.Count);
		Assert.AreSame(fixture.Blue.Object, fixture.Supplied.Single(x => x.Definition == fixture.Colour.Object).Value);
		Assert.AreSame(fixture.White.Object, fixture.Supplied.Single(x => x.Definition == fixture.Accent.Object).Value);
		Assert.AreEqual(disableQuality ? ItemQuality.Standard : ItemQuality.Excellent, fixture.Output.Object.Quality);
		Assert.AreEqual(RoomLayer.GroundLevel, fixture.Output.Object.RoomLayer);
		fixture.Proto.Verify(x => x.CreateNew(null!, fixture.Skin.Object, 2,
			It.IsAny<IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>()), Times.Once);
		fixture.Gameworld.Verify(x => x.Add(fixture.Output.Object), Times.Once);
		StringAssert.Contains(reloaded.Name, "Accent = white");
	}

	[TestMethod]
	public void Load_LegacyInputOnlyXml_RemainsValidAndRoundTrips()
	{
		var fixture = new Fixture();
		var xml = fixture.Product().Xml();
		xml.Elements("FixedVariable").Remove();
		var product = fixture.Product(false, xml);
		Assert.IsTrue(product.IsValid(), product.WhyNotValid());
		Assert.AreEqual(1, product.Characteristics.Count);
		Assert.AreEqual(0, product.FixedCharacteristics.Count);
		Assert.AreEqual("0", product.Xml().Element("Variable")!.Attribute("inputindex")!.Value);
		product.ProduceProduct(fixture.Active.Object, ItemQuality.Good);
		Assert.AreEqual(1, fixture.Supplied!.Count);
	}

	[TestMethod]
	public void ProduceProduct_SelectedValuesOnly_DoesNotRequireVariableInputs()
	{
		var fixture = new Fixture();
		var xml = fixture.Product().Xml();
		xml.Elements("Variable").Remove();
		xml.Add(new XElement("FixedVariable", new XAttribute("value", 300), 200));
		fixture.Craft.SetupGet(x => x.Inputs).Returns([]);
		fixture.Consumed.Clear();
		var product = fixture.Product(false, xml);
		Assert.IsTrue(product.IsValid(), product.WhyNotValid());
		product.ProduceProduct(fixture.Active.Object, ItemQuality.Good);
		Assert.AreEqual(2, fixture.Supplied!.Count);
	}

	[TestMethod]
	[DataRow("missing-definition")]
	[DataRow("missing-value")]
	[DataRow("wrong-value")]
	[DataRow("duplicate-fixed")]
	[DataRow("duplicate-mixed")]
	[DataRow("missing-input")]
	[DataRow("negative-input")]
	[DataRow("incapable-input")]
	public void InvalidRules_ReportReasonAndNeverCreateProducts(string fault)
	{
		var fixture = new Fixture();
		var xml = fixture.Product().Xml();
		switch (fault)
		{
			case "missing-definition": xml.Element("FixedVariable")!.Value = "999"; break;
			case "missing-value": xml.Element("FixedVariable")!.SetAttributeValue("value", 999); break;
			case "wrong-value": xml.Element("FixedVariable")!.SetAttributeValue("value", 300); break;
			case "duplicate-fixed": xml.Add(new XElement(xml.Element("FixedVariable")!)); break;
			case "duplicate-mixed": xml.Add(new XElement("FixedVariable", new XAttribute("value", 300), 200)); break;
			case "missing-input": xml.Element("Variable")!.SetAttributeValue("inputindex", 12); break;
			case "negative-input": xml.Element("Variable")!.SetAttributeValue("inputindex", -1); break;
			case "incapable-input": fixture.Input.Setup(x => x.DeterminesVariable(fixture.Colour.Object)).Returns(false); break;
		}
		var product = fixture.Product(false, xml);
		Assert.IsFalse(product.IsValid());
		Assert.IsFalse(string.IsNullOrWhiteSpace(product.WhyNotValid()));
		Assert.ThrowsException<ApplicationException>(() => product.ProduceProduct(fixture.Active.Object, ItemQuality.Good));
		Assert.IsNull(fixture.Supplied);
		fixture.Gameworld.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
		Assert.IsFalse(fixture.Product(false, product.Xml()).IsValid(), "Saving must not silently discard invalid configured rules.");
	}

	[TestMethod]
	[DataRow("unconsumed")]
	[DataRow("missing-value")]
	[DataRow("incompatible-value")]
	public void ProduceProduct_BrokenConsumedData_FailsBeforeCreatingAnyItem(string fault)
	{
		var fixture = new Fixture();
		if (fault == "unconsumed") fixture.Consumed.Clear();
		else fixture.Input.Setup(x => x.GetValueForVariable(fixture.Colour.Object, It.IsAny<ICraftInputData>()))
			.Returns(fault == "missing-value" ? null! : fixture.White.Object);
		var error = Assert.ThrowsException<ApplicationException>(() => fixture.Product().ProduceProduct(fixture.Active.Object, ItemQuality.Good));
		StringAssert.Contains(error.Message, "$i1");
		Assert.IsNull(fixture.Supplied);
		fixture.Gameworld.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
	}

	private sealed class ProductUnderTest(CraftProduct model, ICraft craft, IFuturemud world) : SimpleVariableProduct(model, craft, world)
	{
		public XElement Xml() => XElement.Parse(SaveDefinition());
	}

	[TestMethod]
	public void Builder_SelectedAndInheritedRules_ReplaceAndRemoveWithoutDuplicates()
	{
		var fixture = new Fixture();
		var actor = Mock.Of<MudSharp.Character.ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());
		var product = fixture.Product();
		Assert.IsTrue(product.BuildingCommand(actor, new StringStack("variable Colour value blue")));
		Assert.AreEqual(0, product.Characteristics.Count);
		Assert.AreEqual(2, product.FixedCharacteristics.Count);
		Assert.IsTrue(product.BuildingCommand(actor, new StringStack("variable Colour 1")));
		Assert.AreEqual(1, product.Characteristics.Count);
		Assert.AreEqual(1, product.FixedCharacteristics.Count);
		Assert.IsTrue(product.BuildingCommand(actor, new StringStack("variable Accent")));
		Assert.AreEqual(0, product.FixedCharacteristics.Count);
		Assert.IsTrue(product.BuildingCommand(actor, new StringStack("variable Colour")));
		Assert.AreEqual(0, product.Characteristics.Count);
		fixture.Craft.Verify(x => x.CalculateCraftIsValid(), Times.Exactly(4));
	}

	[TestMethod]
	[DataRow("variable Colour value white")]
	[DataRow("variable Colour value blu")]
	[DataRow("variable Colour value")]
	[DataRow("variable Accent 1")]
	[DataRow("variable Colour 2")]
	public void Builder_InvalidRule_PreservesPreviousConfiguration(string command)
	{
		var fixture = new Fixture();
		var actor = Mock.Of<MudSharp.Character.ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());
		var product = fixture.Product();
		var before = product.Xml().ToString();
		Assert.IsFalse(product.BuildingCommand(actor, new StringStack(command)));
		Assert.AreEqual(before, product.Xml().ToString());
		fixture.Craft.Verify(x => x.CalculateCraftIsValid(), Times.Never);
	}

	private sealed class Fixture
	{
		public Mock<ICharacteristicDefinition> Colour { get; } = new();
		public Mock<ICharacteristicDefinition> Accent { get; } = new();
		public Mock<ICharacteristicValue> Blue { get; } = new();
		public Mock<ICharacteristicValue> White { get; } = new();
		public Mock<IGameItemProto> Proto { get; } = new();
		public Mock<IGameItemSkin> Skin { get; } = new();
		public Mock<IGameItem> Output { get; } = new();
		public Mock<IVariableInput> Input { get; } = new();
		public Mock<ICraft> Craft { get; } = new();
		public Mock<IFuturemud> Gameworld { get; } = new();
		public Mock<IActiveCraftGameItemComponent> Active { get; } = new();
		public Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)> Consumed { get; } = new();
		public List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>? Supplied { get; private set; }

		public Fixture()
		{
			Colour.SetupGet(x => x.Id).Returns(200);
			Colour.SetupGet(x => x.Name).Returns("Colour");
			Accent.SetupGet(x => x.Id).Returns(201);
			Accent.SetupGet(x => x.Name).Returns("Accent");
			Blue.SetupGet(x => x.Id).Returns(300);
			Blue.SetupGet(x => x.Name).Returns("blue");
			White.SetupGet(x => x.Id).Returns(301);
			White.SetupGet(x => x.Name).Returns("white");
			Colour.Setup(x => x.IsValue(Blue.Object)).Returns(true);
			Accent.Setup(x => x.IsValue(White.Object)).Returns(true);
			Gameworld.SetupGet(x => x.Characteristics).Returns(Repository([Colour.Object, Accent.Object]));
			Gameworld.SetupGet(x => x.CharacteristicValues).Returns(Repository([Blue.Object, White.Object]));
			Gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			Proto.SetupGet(x => x.Id).Returns(100);
			Proto.SetupGet(x => x.ShortDescription).Returns("a garment");
			Skin.SetupGet(x => x.Id).Returns(500);
			Skin.SetupGet(x => x.ShortDescription).Returns("a trimmed garment");
			Gameworld.SetupGet(x => x.ItemProtos).Returns(RevisableRepository([Proto.Object]));
			Gameworld.SetupGet(x => x.ItemSkins).Returns(RevisableRepository([Skin.Object]));
			Output.SetupProperty(x => x.Quality, ItemQuality.Standard);
			Output.SetupProperty(x => x.RoomLayer);
			Proto.Setup(x => x.CreateNew(null!, Skin.Object, 2,
				It.IsAny<IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>()))
				.Callback((MudSharp.Character.ICharacter? _, IGameItemSkin _, int _, IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> values) => Supplied = values.ToList())
				.Returns([Output.Object]);
			Input.Setup(x => x.DeterminesVariable(Colour.Object)).Returns(true);
			Input.Setup(x => x.GetValueForVariable(Colour.Object, It.IsAny<ICraftInputData>())).Returns(Blue.Object);
			Craft.SetupGet(x => x.Inputs).Returns([Input.Object]);
			Consumed.Add(Input.Object, (Mock.Of<IGameItem>(), Mock.Of<ICraftInputData>()));
			Active.SetupGet(x => x.ConsumedInputs).Returns(Consumed);
			Active.SetupGet(x => x.Parent).Returns(Mock.Of<IGameItem>(x => x.RoomLayer == RoomLayer.GroundLevel));
		}

		public ProductUnderTest Product(bool fail = false, XElement? xml = null) => new(new CraftProduct
		{
			Id = 1,
			ProductType = "SimpleVariableProduct",
			OriginalAdditionTime = DateTime.UtcNow,
			IsFailProduct = fail,
			Definition = (xml ?? new XElement("Definition", new XElement("ProductProducedId", 100),
				new XElement("Quantity", 2), new XElement("Skin", 500),
				new XElement("Variable", new XAttribute("inputindex", 0), 200),
				new XElement("FixedVariable", new XAttribute("value", 301), 201))).ToString()
		}, Craft.Object, Gameworld.Object);
	}

	private static IUneditableAll<T> Repository<T>(IReadOnlyList<T> items) where T : class, IFrameworkItem
	{
		var result = new Mock<IUneditableAll<T>>();
		result.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => items.FirstOrDefault(x => x.Id == id)!);
		result.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
			.Returns((string name, bool _) => items.FirstOrDefault(x => x.Name == name));
		result.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => items.GetEnumerator());
		return result.Object;
	}

	private static IUneditableRevisableAll<T> RevisableRepository<T>(IReadOnlyList<T> items) where T : class, IRevisableItem
	{
		var result = new Mock<IUneditableRevisableAll<T>>();
		result.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => items.FirstOrDefault(x => x.Id == id)!);
		return result.Object;
	}
}
