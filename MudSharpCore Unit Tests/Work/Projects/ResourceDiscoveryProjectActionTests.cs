#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Work.Projects;
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
		fixture.Cell.Verify(x => x.Insert(marker.Object, true), Times.Once);
		fixture.Cell.Verify(x => x.HandleRoomEcho("Ore signs break the surface.", RoomLayer.InTrees), Times.Once);
	}

	[TestMethod]
	public void CompleteAction_WhenMarkerAlreadyPresent_DoesNotCreateDuplicate()
	{
		var fixture = CreateConfiguredFixture();
		var action = new ResourceDiscoveryProjectAction(CreateConfiguredModel(), fixture.Gameworld.Object);
		var existing = new Mock<IGameItem>();
		existing.Setup(x => x.IsA(fixture.DuplicateTag.Object)).Returns(true);
		fixture.Cell.SetupGet(x => x.GameItems).Returns(new[] { existing.Object });

		var project = CreateProject(fixture);

		action.CompleteAction(project.Object);

		fixture.OutputPrototype.Verify(x => x.CreateNew(It.IsAny<ICharacter?>()), Times.Never);
		fixture.Gameworld.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
		fixture.Cell.Verify(x => x.HandleRoomEcho("The vein is already marked.", RoomLayer.InTrees), Times.Once);
	}

	[TestMethod]
	public void CompleteAction_WhenLocationLacksRequiredTag_DoesNotCreateMarker()
	{
		var fixture = CreateConfiguredFixture();
		fixture.Cell.Setup(x => x.IsA(fixture.RequiredTag.Object)).Returns(false);
		var action = new ResourceDiscoveryProjectAction(CreateConfiguredModel(), fixture.Gameworld.Object);
		var project = CreateProject(fixture);

		action.CompleteAction(project.Object);

		fixture.OutputPrototype.Verify(x => x.CreateNew(It.IsAny<ICharacter?>()), Times.Never);
		fixture.Gameworld.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
		fixture.Cell.Verify(x => x.HandleRoomEcho("No workable sign is found.", RoomLayer.InTrees), Times.Once);
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

		var cell = new Mock<ICell>();
		cell.Setup(x => x.IsA(requiredTag.Object)).Returns(true);
		cell.SetupGet(x => x.GameItems).Returns([]);
		var owner = new Mock<ICharacter>();
		owner.SetupGet(x => x.Location).Returns(cell.Object);
		owner.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var worker = new Mock<ICharacter>();
		worker.SetupGet(x => x.Location).Returns(cell.Object);
		worker.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);

		return new ResourceDiscoveryFixture(gameworld, requiredTag, duplicateTag, outputPrototype, cell, owner, worker);
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
		Mock<ICell> Cell,
		Mock<ICharacter> Owner,
		Mock<ICharacter> Worker);
}
