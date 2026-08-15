using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Inputs;
using System.Collections.Generic;
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

	[TestMethod]
	public void ReloadedLiquidUseData_HasPerceivableAndCanEnterResumeClashSolver()
	{
		var setup = LiquidWorld();
		var xml = LiquidXml("Liquid", setup.Liquid.Object.Id, setup.Container.Object.Id);

		var data = new LiquidUseInput.LiquidUseInputData(xml, setup.Gameworld.Object);

		Assert.IsNotNull(data.Perceivable);
		Assert.AreEqual(setup.Liquid.Object, data.Liquid);
		Assert.AreEqual(2.0, data.Amount);
		Assert.AreEqual(1, data.OriginalItems.Count);
		AssertResumeClashSolverAccepts(data.Perceivable);
	}

	[TestMethod]
	public void ReloadedLiquidTagUseData_HasPerceivableAndCanEnterResumeClashSolver()
	{
		var setup = LiquidWorld();
		var tag = new Mock<ITag>();
		tag.SetupGet(x => x.Id).Returns(99);
		var tags = new Mock<IUneditableAll<ITag>>();
		tags.Setup(x => x.Get(99)).Returns(tag.Object);
		setup.Gameworld.SetupGet(x => x.Tags).Returns(tags.Object);
		var xml = LiquidXml("TagId", tag.Object.Id, setup.Container.Object.Id);

		var data = new LiquidTagUseInput.LiquidUseInputData(xml, setup.Gameworld.Object);

		Assert.IsNotNull(data.Perceivable);
		Assert.AreEqual(tag.Object, data.Target);
		Assert.AreEqual(2.0, data.Amount);
		Assert.AreEqual(1, data.OriginalItems.Count);
		AssertResumeClashSolverAccepts(data.Perceivable);
	}

	private static XElement LiquidXml(string targetElement, long targetId, long containerId)
	{
		return new XElement("Input",
			new XElement(targetElement, targetId),
			new XElement("Amount", 2.0),
			new XElement("Quality", (int)ItemQuality.Good),
			new XElement("Mix", new XElement("Liquid",
				new XAttribute("id", 14),
				new XAttribute("amount", 2.0))),
			new XElement("OriginalItem", containerId));
	}

	private static (Mock<IFuturemud> Gameworld, Mock<ILiquid> Liquid, Mock<IGameItem> Container) LiquidWorld()
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Id).Returns(14);
		liquid.SetupGet(x => x.Name).Returns("soapy water");
		liquid.SetupGet(x => x.MaterialDescription).Returns("soapy water");
		liquid.SetupGet(x => x.Description).Returns("soapy water");
		liquid.SetupGet(x => x.Density).Returns(1.0);
		liquid.SetupGet(x => x.DisplayColour).Returns(Telnet.Cyan);
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(14)).Returns(liquid.Object);

		var container = new Mock<IGameItem>();
		container.SetupGet(x => x.Id).Returns(303);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		gameworld.Setup(x => x.TryGetItem(303, false)).Returns(container.Object);
		return (gameworld, liquid, container);
	}

	private static void AssertResumeClashSolverAccepts(IPerceivable liquidPerceivable)
	{
		var liquidInput = new Mock<ICraftInput>().Object;
		var otherInput = new Mock<ICraftInput>().Object;
		var solver = new OptionSolver<ICraftInput, IPerceivable>(
			new List<IChoice<ICraftInput, IPerceivable>>
			{
				new Choice<ICraftInput, IPerceivable>(liquidInput,
					[new PerceivableOption(liquidPerceivable)]),
				new Choice<ICraftInput, IPerceivable>(otherInput,
					[new PerceivableOption(new DummyPerceivable("later input"))])
			});

		var result = solver.SolveOptions();

		Assert.IsTrue(result.Success);
		Assert.AreEqual(2, result.Solution.Count);
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
