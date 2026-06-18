#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.Actions;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ProjectActionModel = MudSharp.Models.ProjectAction;

namespace MudSharp_Unit_Tests.Work.Projects;

[TestClass]
public class CommodityOutputProjectActionTests
{
	[TestMethod]
	public void ValidActionTypes_IncludesCommodityOutputAndExistingTypes()
	{
		var actionTypes = ProjectFactory.ValidActionTypes.ToList();

		CollectionAssert.Contains(actionTypes, "prog");
		CollectionAssert.Contains(actionTypes, "skilluse");
		CollectionAssert.Contains(actionTypes, "agriculture");
		CollectionAssert.Contains(actionTypes, "commodityoutput");
	}

	[TestMethod]
	public void LoadAction_ExistingActionTypesStillResolveToExistingActionClasses()
	{
		var gameworld = CreateGameworld().Object;

		Assert.IsInstanceOfType(ProjectFactory.LoadAction(new ProjectActionModel
		{
			Type = "prog",
			Definition = "<Action>0</Action>"
		}, gameworld), typeof(ProgAction));

		Assert.IsInstanceOfType(ProjectFactory.LoadAction(new ProjectActionModel
		{
			Type = "skilluse",
			Definition = "<Action><TraitDefinition>0</TraitDefinition><NumberOfFreeChecks>1</NumberOfFreeChecks><Difficulty>0</Difficulty></Action>"
		}, gameworld), typeof(SkillUseAction));

		Assert.IsInstanceOfType(ProjectFactory.LoadAction(new ProjectActionModel
		{
			Type = "agriculture",
			Definition = "<Action />"
		}, gameworld), typeof(AgricultureOperationAction));
	}

	[TestMethod]
	public void SaveDefinition_ConfiguredCommodityOutput_RoundTripsXml()
	{
		var fixture = CreateConfiguredFixture();
		var action = new CommodityOutputProjectAction(CreateConfiguredModel(), fixture.Gameworld.Object);

		var xml = InvokeSaveDefinition(action);

		Assert.AreEqual("1", xml.Element("MaterialId")?.Value);
		Assert.AreEqual("2500", xml.Element("Weight")?.Value);
		Assert.AreEqual("2", xml.Element("TagId")?.Value);
		Assert.AreEqual("true", xml.Element("UseIndirectDescription")?.Value.ToLowerInvariant());
		Assert.AreEqual("Ore piles slump from the completed working.", xml.Element("Echo")?.Value);

		var characteristic = xml.Element("Characteristics")?.Element("Characteristic");
		Assert.IsNotNull(characteristic);
		Assert.AreEqual("3", characteristic!.Attribute("definition")?.Value);
		Assert.AreEqual("4", characteristic.Attribute("value")?.Value);
	}

	[TestMethod]
	public void CanSubmit_MissingMaterialOrNonPositiveWeight_FailsValidation()
	{
		var gameworld = CreateGameworld().Object;
		var missingMaterial = new CommodityOutputProjectAction(new ProjectActionModel
		{
			Type = "commodityoutput",
			Definition = "<Action><MaterialId>0</MaterialId><Weight>2500</Weight></Action>"
		}, gameworld);
		var nonPositiveWeight = new CommodityOutputProjectAction(new ProjectActionModel
		{
			Type = "commodityoutput",
			Definition = "<Action><MaterialId>1</MaterialId><Weight>0</Weight></Action>"
		}, CreateConfiguredFixture().Gameworld.Object);

		var missingMaterialResult = missingMaterial.CanSubmit();
		var nonPositiveWeightResult = nonPositiveWeight.CanSubmit();

		Assert.IsFalse(missingMaterialResult.Truth);
		StringAssert.Contains(missingMaterialResult.Error, "material");
		Assert.IsFalse(nonPositiveWeightResult.Truth);
		StringAssert.Contains(nonPositiveWeightResult.Error, "positive");
	}

	[TestMethod]
	public void CompleteAction_ConfiguredCommodityOutput_CreatesCommodityInProjectLocation()
	{
		var fixture = CreateConfiguredFixture();
		var action = new CommodityOutputProjectAction(CreateConfiguredModel(), fixture.Gameworld.Object);
		var originalCommodityPrototype = CommodityGameItemComponentProto.ItemPrototype;
		var commodity = new Mock<ICommodity>();
		ISolid? assignedMaterial = null;
		double assignedWeight = 0.0;
		ITag? assignedTag = null;
		bool assignedIndirect = false;
		ICharacteristicDefinition? assignedDefinition = null;
		ICharacteristicValue? assignedValue = null;
		commodity.SetupSet(x => x.Material = It.IsAny<ISolid>()).Callback<ISolid>(x => assignedMaterial = x);
		commodity.SetupSet(x => x.Weight = It.IsAny<double>()).Callback<double>(x => assignedWeight = x);
		commodity.SetupSet(x => x.Tag = It.IsAny<ITag?>()).Callback<ITag?>(x => assignedTag = x);
		commodity.SetupSet(x => x.UseIndirectQuantityDescription = It.IsAny<bool>())
		         .Callback<bool>(x => assignedIndirect = x);
		commodity.Setup(x => x.SetCommodityCharacteristic(It.IsAny<ICharacteristicDefinition>(),
				It.IsAny<ICharacteristicValue>()))
		         .Callback<ICharacteristicDefinition, ICharacteristicValue>((definition, value) =>
		         {
			         assignedDefinition = definition;
			         assignedValue = value;
		         })
		         .Returns(true);

		var item = new Mock<IGameItem>();
		item.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		var prototype = new Mock<IGameItemProto>();
		prototype.Setup(x => x.CreateNew(null)).Returns(item.Object);
		CommodityGameItemComponentProto.ItemPrototype = prototype.Object;

		var cell = new Mock<ICell>();
		IGameItem? insertedItem = null;
		cell.Setup(x => x.Insert(It.IsAny<IGameItem>(), true))
		    .Callback<IGameItem, bool>((createdItem, _) => insertedItem = createdItem);
		var owner = new Mock<ICharacter>();
		owner.SetupGet(x => x.Location).Returns(cell.Object);
		owner.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var worker = new Mock<ICharacter>();
		worker.SetupGet(x => x.Location).Returns(cell.Object);
		worker.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);
		var labour = new Mock<IProjectLabourRequirement>();
		var project = new Mock<IActiveProject>();
		project.SetupGet(x => x.CharacterOwner).Returns(owner.Object);
		project.SetupGet(x => x.ActiveLabour).Returns(new[] { (worker.Object, labour.Object) });

		try
		{
			action.CompleteAction(project.Object);
		}
		finally
		{
			CommodityGameItemComponentProto.ItemPrototype = originalCommodityPrototype;
		}

		Assert.AreSame(fixture.Material.Object, assignedMaterial);
		Assert.AreEqual(2500.0, assignedWeight);
		Assert.AreSame(fixture.Tag.Object, assignedTag);
		Assert.IsTrue(assignedIndirect);
		Assert.AreSame(fixture.CharacteristicDefinition.Object, assignedDefinition);
		Assert.AreSame(fixture.CharacteristicValue.Object, assignedValue);
		Assert.AreSame(item.Object, insertedItem);
		Assert.AreEqual(RoomLayer.InTrees, item.Object.RoomLayer);
		fixture.Gameworld.Verify(x => x.Add(item.Object), Times.Once);
		cell.Verify(x => x.HandleRoomEcho("Ore piles slump from the completed working.", RoomLayer.InTrees), Times.Once);
	}

	private static ProjectActionModel CreateConfiguredModel()
	{
		return new ProjectActionModel
		{
			Id = 10,
			Name = "Output Ore",
			Description = "Create ore output",
			SortOrder = 1,
			Type = "commodityoutput",
			Definition =
				"<Action><MaterialId>1</MaterialId><Weight>2500</Weight><TagId>2</TagId><UseIndirectDescription>true</UseIndirectDescription><Echo>Ore piles slump from the completed working.</Echo><Characteristics><Characteristic definition=\"3\" value=\"4\" /></Characteristics></Action>"
		};
	}

	private static XElement InvokeSaveDefinition(CommodityOutputProjectAction action)
	{
		return (XElement)typeof(CommodityOutputProjectAction)
		                 .GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
		                 .Invoke(action, [])!;
	}

	private static CommodityOutputFixture CreateConfiguredFixture()
	{
		var material = MockFrameworkItem<ISolid>(1, "hematite");
		material.SetupGet(x => x.ResidueColour).Returns(Telnet.Red);
		var tag = MockFrameworkItem<ITag>(2, "raw ore");
		tag.SetupGet(x => x.FullName).Returns("Primary Production / Raw Ore");
		var definition = MockFrameworkItem<ICharacteristicDefinition>(3, "grade");
		var value = MockFrameworkItem<ICharacteristicValue>(4, "high grade");
		value.SetupGet(x => x.GetValue).Returns("high grade");
		value.SetupGet(x => x.Definition).Returns(definition.Object);
		definition.Setup(x => x.IsValue(value.Object)).Returns(true);

		var gameworld = CreateGameworld(material.Object, tag.Object, definition.Object, value.Object);
		return new CommodityOutputFixture(gameworld, material, tag, definition, value);
	}

	private static Mock<IFuturemud> CreateGameworld(
		ISolid? material = null,
		ITag? tag = null,
		ICharacteristicDefinition? definition = null,
		ICharacteristicValue? value = null)
	{
		var materials = new All<ISolid>();
		if (material is not null)
		{
			materials.Add(material);
		}

		var tags = new All<ITag>();
		if (tag is not null)
		{
			tags.Add(tag);
		}

		var characteristics = new All<ICharacteristicDefinition>();
		if (definition is not null)
		{
			characteristics.Add(definition);
		}

		var characteristicValues = new All<ICharacteristicValue>();
		if (value is not null)
		{
			characteristicValues.Add(value);
		}

		var futureProgs = new All<IFutureProg>();
		var traits = new All<MudSharp.Body.Traits.ITraitDefinition>();
		var unitManager = new Mock<IUnitManager>();
		unitManager.Setup(x => x.DescribeExact(It.IsAny<double>(), UnitType.Mass, It.IsAny<IPerceiver>()))
		           .Returns<double, UnitType, IPerceiver>((amount, _, _) => $"{amount:N0}g");

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Materials).Returns(materials);
		gameworld.SetupGet(x => x.Tags).Returns(tags);
		gameworld.SetupGet(x => x.Characteristics).Returns(characteristics);
		gameworld.SetupGet(x => x.CharacteristicValues).Returns(characteristicValues);
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs);
		gameworld.SetupGet(x => x.Traits).Returns(traits);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		return gameworld;
	}

	private static Mock<T> MockFrameworkItem<T>(long id, string name) where T : class, IFrameworkItem
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns(typeof(T).Name);
		return mock;
	}

	private sealed record CommodityOutputFixture(
		Mock<IFuturemud> Gameworld,
		Mock<ISolid> Material,
		Mock<ITag> Tag,
		Mock<ICharacteristicDefinition> CharacteristicDefinition,
		Mock<ICharacteristicValue> CharacteristicValue);
}
