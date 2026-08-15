using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Work.Crafts;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests.Crafts;

[TestClass]
public class CraftInputPersistenceTests
{
	[TestMethod]
	public void ActiveCraft_RoundTripsStandardAndLiquidConsumedInputData()
	{
		var standardXml = new XElement("Data",
			new XElement("Item", 101),
			new XElement("Quantity", 2));
		var liquidXml = new XElement("Input",
			new XElement("Liquid", 202),
			new XElement("Amount", 1.25),
			new XElement("Quality", 7),
			new XElement("Mix", new XElement("Liquid", new XAttribute("id", 202), 1.25)),
			new XElement("OriginalItem", 303));
		var taggedLiquidXml = new XElement("Input",
			new XElement("TagId", 404),
			new XElement("Amount", 2.5),
			new XElement("Quality", 5),
			new XElement("Mix", new XElement("Liquid", new XAttribute("id", 405), 2.5)),
			new XElement("OriginalItem", 505),
			new XElement("OriginalItem", 506));

		var standard = Input(1, standardXml);
		var liquid = Input(2, liquidXml);
		var taggedLiquid = Input(3, taggedLiquidXml);
		var craft = new Mock<ICraft>();
		craft.SetupGet(x => x.Id).Returns(42);
		craft.SetupGet(x => x.RevisionNumber).Returns(6);
		craft.SetupGet(x => x.Inputs).Returns([standard.Input.Object, liquid.Input.Object, taggedLiquid.Input.Object]);

		var crafts = new Mock<IUneditableRevisableAll<ICraft>>();
		crafts.Setup(x => x.Get(42, 6)).Returns(craft.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Crafts).Returns(crafts.Object);
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var source = new TestActiveCraftComponent(parent.Object) { Craft = craft.Object };
		source.ConsumedInputs[standard.Input.Object] = (standard.Data.Perceivable, standard.Data);
		source.ConsumedInputs[liquid.Input.Object] = (liquid.Data.Perceivable, liquid.Data);
		source.ConsumedInputs[taggedLiquid.Input.Object] = (taggedLiquid.Data.Perceivable, taggedLiquid.Data);

		var saved = XElement.Parse(source.SaveDefinition());
		var loaded = new TestActiveCraftComponent(parent.Object);
		loaded.LoadDefinition(saved);

		Assert.AreEqual(3, loaded.ConsumedInputs.Count);
		standard.Input.Verify(x => x.LoadDataFromXml(It.Is<XElement>(e => XElement.DeepEquals(e, standardXml)), gameworld.Object), Times.Once);
		liquid.Input.Verify(x => x.LoadDataFromXml(It.Is<XElement>(e => XElement.DeepEquals(e, liquidXml)), gameworld.Object), Times.Once);
		taggedLiquid.Input.Verify(x => x.LoadDataFromXml(It.Is<XElement>(e => XElement.DeepEquals(e, taggedLiquidXml)), gameworld.Object), Times.Once);
	}

	private static (Mock<ICraftInput> Input, TestCraftInputData Data) Input(long id, XElement xml)
	{
		var perceivable = new Mock<IPerceivable>().Object;
		var input = new Mock<ICraftInput>();
		input.SetupGet(x => x.Id).Returns(id);
		input.Setup(x => x.LoadDataFromXml(It.IsAny<XElement>(), It.IsAny<IFuturemud>()))
			.Returns((XElement _, IFuturemud _) => new TestCraftInputData(perceivable, new XElement(xml)));
		return (input, new TestCraftInputData(perceivable, xml));
	}

	private sealed class TestActiveCraftComponent(IGameItem parent)
		: ActiveCraftGameItemComponent((ActiveCraftGameItemComponentProto)null!, parent, true)
	{
		public string SaveDefinition() => SaveToXml();
		public void LoadDefinition(XElement root) => LoadFromXml(root);
	}

	private sealed class TestCraftInputData(IPerceivable perceivable, XElement xml) : ICraftInputData
	{
		public XElement SaveToXml() => new(xml);
		public IPerceivable Perceivable => perceivable;
		public ItemQuality InputQuality => ItemQuality.Standard;
		public void FinaliseLoadTimeTasks() { }
		public void Delete() { }
		public void Quit() { }
	}
}
