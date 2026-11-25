#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.ConcreteTypes;
using MudSharp.Work.Projects.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ProjectActionModel = MudSharp.Models.ProjectAction;

namespace MudSharp_Unit_Tests.Work.Projects;

[TestClass]
public class ResourceDiscoveryProjectActionTests
{
	[TestMethod]
	public void ValidActionTypes_IncludesResourceDiscovery()
	{
		var actionTypes = ProjectFactory.ValidActionTypes.ToList();

		CollectionAssert.Contains(actionTypes, "resourcediscovery");
	}

	[TestMethod]
	public void LoadAction_ResourceDiscovery_ResolvesToActionClass()
	{
		var fixture = CreateConfiguredFixture();

		var action = ProjectFactory.LoadAction(CreateConfiguredModel(), fixture.Gameworld.Object);

		Assert.IsInstanceOfType(action, typeof(ResourceDiscoveryProjectAction));
	}

	[TestMethod]
	public void SaveDefinition_ConfiguredResourceDiscovery_RoundTripsXml()
	{
		var fixture = CreateConfiguredFixture();
		var action = new ResourceDiscoveryProjectAction(CreateConfiguredModel(), fixture.Gameworld.Object);

		var xml = InvokeSaveDefinition(action);

		Assert.AreEqual("1", xml.Element("RequiredLocationTagId")?.Value);
		Assert.AreEqual("2", xml.Element("OutputItemProtoId")?.Value);
		Assert.AreEqual("3", xml.Element("OutputItemProtoRevision")?.Value);
		Assert.AreEqual("4", xml.Element("DuplicatePreventionTagId")?.Value);
		Assert.AreEqual("Ore signs break the surface.", xml.Element("Echo")?.Value);
		Assert.AreEqual("The vein is already marked.", xml.Element("AlreadyPresentEcho")?.Value);
		Assert.AreEqual("No workable sign is found.", xml.Element("FailureEcho")?.Value);
	}

	[TestMethod]
	public void CanSubmit_MissingOutputPrototype_FailsValidation()
	{
		var fixture = CreateConfiguredFixture();
		var action = new ResourceDiscoveryProjectAction(new ProjectActionModel
		{
			Type = "resourcediscovery",
			Definition = "<Action><RequiredLocationTagId>1</RequiredLocationTagId><OutputItemProtoId>0</OutputItemProtoId></Action>"
		}, fixture.Gameworld.Object);

		var result = action.CanSubmit();

		Assert.IsFalse(result.Truth);
		StringAssert.Contains(result.Error, "item prototype");
	}

	[TestMethod]
	public void CompleteAction_WhenLocationHasRequiredTag_CreatesMarkerInProjectLocation()
	{
		var fixture = CreateConfiguredFixture();
		var action = new ResourceDiscoveryProjectAction(CreateConfiguredModel(), fixture.Gameworld.Object);
		var marker = new Mock<IGameItem>();
		marker.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		marker.SetupGet(x => x.Prototype).Returns(fixture.OutputPrototype.Object);
		fixture.OutputPrototype.Setup(x => x.CreateNew(fixture.Owner.Object)).Returns(marker.Object);

		var project = CreateProject(fixture);

		action.CompleteAction(project.Object);

		Assert.AreEqual(RoomLayer.InTrees, marker.Object.RoomLayer);
		fixture.Gameworld.Verify(x => x.Add(marker.Object), Times.Once);
		fixture.Room.Verify(x => x.Insert(marker.Object, true), Times.Once);
		fixture.Room.Verify(x => x.HandleRoomEcho("Ore signs break the surface.", RoomLayer.InTrees), Times.Once);
	}

	[TestMethod]
	public void CompleteAction_WhenMarkerAlreadyPresent_DoesNotCreateDuplicate()
	{
		var fixture = CreateConfiguredFixture();
		var action = new ResourceDiscoveryProjectAction(CreateConfiguredModel(), fixture.Gameworld.Object);
		var existing = new Mock<IGameItem>();
		existing.Setup(x => x.IsA(fixture.DuplicateTag.Object)).Returns(true);
		fixture.Room.SetupGet(x => x.GameItems).Returns(new[] { existing.Object });

		var project = CreateProject(fixture);

		action.CompleteAction(project.Object);

		fixture.OutputPrototype.Verify(x => x.CreateNew(It.IsAny<ICharacter?>()), Times.Never);
		fixture.Gameworld.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
		fixture.Room.Verify(x => x.HandleRoomEcho("The vein is already marked.", RoomLayer.InTrees), Times.Once);
	}

	[TestMethod]
	public void CompleteAction_WhenLocationLacksRequiredTag_DoesNotCreateMarker()
	{
		var fixture = CreateConfiguredFixture();
		fixture.Room.Setup(x => x.IsA(fixture.RequiredTag.Object)).Returns(false);
		var action = new ResourceDiscoveryProjectAction(CreateConfiguredModel(), fixture.Gameworld.Object);
		var project = CreateProject(fixture);

		action.CompleteAction(project.Object);

		fixture.OutputPrototype.Verify(x => x.CreateNew(It.IsAny<ICharacter?>()), Times.Never);
		fixture.Gameworld.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
		fixture.Room.Verify(x => x.HandleRoomEcho("No workable sign is found.", RoomLayer.InTrees), Times.Once);
	}

	[TestMethod]
	[DoNotParallelize]
	[DataRow("success", false, 2, 1)]
	[DataRow("barren", false, 2, 0)]
	[DataRow("duplicate", false, 2, 0)]
	[DataRow("success", true, null, 1)]
	[DataRow("barren", true, null, 0)]
	[DataRow("duplicate", true, null, 0)]
	[DataRow("barren", true, 0, 1)]
	[DataRow("barren", false, null, 1)]
	[DataRow("sample-first", false, 2, 0)]
	public void CompletePhaseActions_GatesOnlyTheMatchingDiscoverySample(
		string outcome, bool stockProject, int? markerId, int expectedCommodityCreations)
	{
		var fixture = CreateConfiguredFixture();
		if (outcome == "barren")
		{
			fixture.Room.Setup(x => x.IsA(fixture.RequiredTag.Object)).Returns(false);
		}
		else if (outcome == "duplicate")
		{
			var existingMarker = new Mock<IGameItem>();
			existingMarker.Setup(x => x.IsA(fixture.DuplicateTag.Object)).Returns(true);
			fixture.Room.SetupGet(x => x.GameItems).Returns([existingMarker.Object]);
		}

		fixture.OutputPrototype.Setup(x => x.CreateNew(fixture.Owner.Object)).Returns(() =>
		{
			var marker = new Mock<IGameItem>();
			marker.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
			marker.SetupGet(x => x.Prototype).Returns(fixture.OutputPrototype.Object);
			return marker.Object;
		});
		var sample = CreateSample(fixture, markerId, out var commodityPrototype, out var commodityItem);
		if (outcome == "sample-first")
		{
			sample.SortOrder = 0;
		}

		var phase = new ProjectPhase(new MudSharp.Models.ProjectPhase { ProjectActions = { sample, CreateConfiguredModel() } },
			fixture.Gameworld.Object, stockProject ? "Stock Primary Production: Prospect for Iron Deposits" : "Foothills Survey");
		var output = phase.CompletionActions.OfType<CommodityOutputProjectAction>().Single();
		var savedDefinition = (XElement)typeof(CommodityOutputProjectAction)
			.GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(output, [])!;
		Assert.AreEqual(markerId ?? (stockProject ? 2 : (int?)null), (int?)savedDefinition.Element("RequiredDiscoveryMarkerPrototypeId"));

		var project = CreateProject(fixture);
		var unrelatedAction = new Mock<IProjectAction>();
		unrelatedAction.SetupGet(x => x.Id).Returns(30);
		unrelatedAction.SetupGet(x => x.SortOrder).Returns(3);
		var unrelatedActionRan = false;
		unrelatedAction.Setup(x => x.CompleteAction(project.Object)).Callback(() => unrelatedActionRan = true);
		var originalCommodityPrototype = CommodityGameItemComponentProto.ItemPrototype;
		CommodityGameItemComponentProto.ItemPrototype = commodityPrototype.Object;

		try
		{
			ActiveProject.CompletePhaseActions(project.Object, phase.CompletionActions.Prepend(unrelatedAction.Object));
		}
		finally
		{
			CommodityGameItemComponentProto.ItemPrototype = originalCommodityPrototype;
		}

		fixture.Gameworld.Verify(x => x.Add(commodityItem.Object), Times.Exactly(expectedCommodityCreations));
		Assert.IsTrue(unrelatedActionRan, "An unrelated phase action must run regardless of discovery outcome.");
	}

	private static ProjectActionModel CreateSample(ResourceDiscoveryFixture fixture, int? markerId,
		out Mock<IGameItemProto> commodityPrototype, out Mock<IGameItem> commodityItem)
	{
		var material = new Mock<ISolid>();
		material.SetupGet(x => x.Id).Returns(8);
		material.SetupGet(x => x.Name).Returns("hematite");
		material.SetupGet(x => x.FrameworkItemType).Returns("Solid");
		material.SetupGet(x => x.ResidueColour).Returns(Telnet.Red);
		var materials = new All<ISolid>();
		materials.Add(material.Object);
		fixture.Gameworld.SetupGet(x => x.Materials).Returns(materials);

		var commodity = new Mock<ICommodity>();
		commodityItem = new Mock<IGameItem>();
		commodityItem.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		commodityItem.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		commodityPrototype = new Mock<IGameItemProto>();
		commodityPrototype.Setup(x => x.CreateNew(null)).Returns(commodityItem.Object);
		return new ProjectActionModel
		{
			Id = 25,
			Name = "Sample Iron Ore",
			Description = "Create an iron sample",
			SortOrder = 2,
			Type = "commodityoutput",
			Definition = new XElement("Action", new XElement("MaterialId", 8), new XElement("Weight", 1500),
				markerId.HasValue ? new XElement("RequiredDiscoveryMarkerPrototypeId", markerId) : null).ToString()
		};
	}

	private static ProjectActionModel CreateConfiguredModel()
	{
		return new ProjectActionModel
		{
			Id = 20,
			Name = "Reveal Hematite",
			Description = "Reveal the hematite marker",
			SortOrder = 1,
			Type = "resourcediscovery",
			Definition =
				"<Action><RequiredLocationTagId>1</RequiredLocationTagId><OutputItemProtoId>2</OutputItemProtoId><OutputItemProtoRevision>3</OutputItemProtoRevision><DuplicatePreventionTagId>4</DuplicatePreventionTagId><Echo>Ore signs break the surface.</Echo><AlreadyPresentEcho>The vein is already marked.</AlreadyPresentEcho><FailureEcho>No workable sign is found.</FailureEcho></Action>"
		};
	}

	private static XElement InvokeSaveDefinition(ResourceDiscoveryProjectAction action)
	{
		return (XElement)typeof(ResourceDiscoveryProjectAction)
		                 .GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
		                 .Invoke(action, [])!;
	}

	private static Mock<IActiveProject> CreateProject(ResourceDiscoveryFixture fixture)
	{
		var labour = new Mock<IProjectLabourRequirement>();
		var project = new Mock<IActiveProject>();
		project.SetupGet(x => x.CharacterOwner).Returns(fixture.Owner.Object);
		project.SetupGet(x => x.ActiveLabour).Returns(new[] { (fixture.Worker.Object, labour.Object) });
		return project;
	}

	private static ResourceDiscoveryFixture CreateConfiguredFixture()
	{
		var requiredTag = MockFrameworkItem<ITag>(1, "Hematite Resource");
		requiredTag.SetupGet(x => x.FullName).Returns("Primary Production / Hematite Resource");
		var duplicateTag = MockFrameworkItem<ITag>(4, "Hematite Resource");
		duplicateTag.SetupGet(x => x.FullName).Returns("Primary Production / Hematite Resource");
		var outputPrototype = MockFrameworkItem<IGameItemProto>(2, "hematite deposit");
		outputPrototype.SetupGet(x => x.RevisionNumber).Returns(3);
		outputPrototype.SetupGet(x => x.ShortDescription).Returns("a hematite ore vein");
		outputPrototype.SetupGet(x => x.UniqueName).Returns("primary_production_hematite_deposit");

		var tags = new All<ITag>();
		tags.Add(requiredTag.Object);
		tags.Add(duplicateTag.Object);
		var itemProtos = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		itemProtos.Setup(x => x.Get(2, 3)).Returns(outputPrototype.Object);
		itemProtos.Setup(x => x.Get(2)).Returns(outputPrototype.Object);
		itemProtos.Setup(x => x.GetEnumerator()).Returns(() => new List<IGameItemProto> { outputPrototype.Object }.GetEnumerator());

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Tags).Returns(tags);
		gameworld.SetupGet(x => x.ItemProtos).Returns(itemProtos.Object);

		var room = new Mock<IRoom>();
		room.Setup(x => x.IsA(requiredTag.Object)).Returns(true);
		room.SetupGet(x => x.GameItems).Returns([]);
		var owner = new Mock<ICharacter>();
		owner.SetupGet(x => x.Location).Returns(room.Object);
		owner.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var worker = new Mock<ICharacter>();
		worker.SetupGet(x => x.Location).Returns(room.Object);
		worker.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);

		return new ResourceDiscoveryFixture(gameworld, requiredTag, duplicateTag, outputPrototype, room, owner, worker);
	}

	private static Mock<T> MockFrameworkItem<T>(long id, string name) where T : class, IFrameworkItem
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns(typeof(T).Name);
		return mock;
	}

	private sealed record ResourceDiscoveryFixture(
		Mock<IFuturemud> Gameworld,
		Mock<ITag> RequiredTag,
		Mock<ITag> DuplicateTag,
		Mock<IGameItemProto> OutputPrototype,
		Mock<IRoom> Room,
		Mock<ICharacter> Owner,
		Mock<ICharacter> Worker);
}
