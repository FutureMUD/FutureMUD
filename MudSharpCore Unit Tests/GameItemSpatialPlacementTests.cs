#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat.AuxiliaryEffects;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.Work.Projects;

namespace MudSharp_Unit_Tests;

[TestClass]
public class GameItemSpatialPlacementTests
{
	[TestMethod]
	public void InsertAtSource_RouteCell_InheritsEffectiveCoordinateAndPreservesItemLayer()
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.RouteDefinition).Returns(Mock.Of<IRouteCellDefinition>());
		var source = new Mock<ILocateable>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);
		var spatialService = new Mock<IRouteSpatialService>();
		spatialService.Setup(x => x.GetEffectiveLocation(source.Object))
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 7_150.0));
		spatialService.Setup(x => x.TryValidateLocation(
				It.IsAny<SpatialLocation>(),
				out It.Ref<string>.IsAny))
			.Returns((SpatialLocation _, out string error) =>
			{
				error = string.Empty;
				return true;
			});

		item.Object.InsertAtSource(source.Object, true, spatialService.Object);

		item.Verify(x => x.MoveTo(
			new SpatialLocation(cell.Object, RoomLayer.InTrees, 7_150.0),
			null,
			false), Times.Once);
		cell.Verify(x => x.Insert(item.Object, true), Times.Once);
	}

	[TestMethod]
	public void InsertAtSource_OrdinaryCell_UsesExistingInsertionWithoutSpatialMove()
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		var source = new Mock<ILocateable>();
		var item = new Mock<IGameItem>();
		var spatialService = new Mock<IRouteSpatialService>();
		spatialService.Setup(x => x.GetEffectiveLocation(source.Object))
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel));
		spatialService.Setup(x => x.TryValidateLocation(
				It.IsAny<SpatialLocation>(),
				out It.Ref<string>.IsAny))
			.Returns((SpatialLocation _, out string error) =>
			{
				error = string.Empty;
				return true;
			});

		item.Object.InsertAtSource(source.Object, false, spatialService.Object);

		item.Verify(x => x.MoveTo(
			It.IsAny<SpatialLocation>(),
			It.IsAny<MudSharp.Construction.Boundary.ICellExit?>(),
			It.IsAny<bool>()), Times.Never);
		cell.Verify(x => x.Insert(item.Object, false), Times.Once);
	}

	[TestMethod]
	public void PlaceDisarmedItem_RouteCell_InheritsTargetCoordinateAndLayer()
	{
		var (cell, source, item) = CreateRoutePlacementFixture(7_150.0, RoomLayer.GroundLevel);

		Disarm.PlaceDisarmedItem(item.Object, source.Object);

		Assert.AreEqual(RoomLayer.GroundLevel, item.Object.RoomLayer);
		item.Verify(x => x.MoveTo(
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 7_150.0),
			null,
			false), Times.Once);
		cell.Verify(x => x.Insert(item.Object, false), Times.Once);
	}

	[TestMethod]
	public void ButcherySpatialPlacement_RouteCell_InheritsWorkerCoordinateAndProductLayer()
	{
		var (cell, source, item) = CreateRoutePlacementFixture(4_275.5, RoomLayer.GroundLevel);

		ButcherySpatialPlacement.Place(item.Object, source.Object, RoomLayer.Underwater, true);

		Assert.AreEqual(RoomLayer.Underwater, item.Object.RoomLayer);
		item.Verify(x => x.MoveTo(
			new SpatialLocation(cell.Object, RoomLayer.Underwater, 4_275.5),
			null,
			false), Times.Once);
		cell.Verify(x => x.Insert(item.Object, true), Times.Once);
	}

	[TestMethod]
	public void ProjectCashFallback_RouteCell_InheritsRecipientCoordinateAndMergesStack()
	{
		var (cell, source, item) = CreateRoutePlacementFixture(9_999.0, RoomLayer.OnRooftops);

		ProjectPaymentService.PlaceCashAtActor(item.Object, source.Object);

		Assert.AreEqual(RoomLayer.OnRooftops, item.Object.RoomLayer);
		item.Verify(x => x.MoveTo(
			new SpatialLocation(cell.Object, RoomLayer.OnRooftops, 9_999.0),
			null,
			false), Times.Once);
		cell.Verify(x => x.Insert(item.Object, true), Times.Once);
	}

	[DataTestMethod]
	[DataRow("weapon unload", 1_250.0, false)]
	[DataRow("outfit removal", 5_500.0, false)]
	[DataRow("builder full-hands fallback", 8_875.0, false)]
	[DataRow("cash change fallback", 9_250.0, true)]
	public void SourceRelativeFallbackScenarios_RouteCell_InheritExactActorCoordinate(
		string scenario,
		double coordinate,
		bool newStack)
	{
		var (cell, source, item) = CreateRoutePlacementFixture(coordinate, RoomLayer.GroundLevel);
		item.Object.RoomLayer = source.Object.RoomLayer;

		item.Object.InsertAtSource(source.Object, newStack);

		item.Verify(x => x.MoveTo(
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, coordinate),
			null,
			false), Times.Once, scenario);
		cell.Verify(x => x.Insert(item.Object, newStack), Times.Once, scenario);
	}

	[TestMethod]
	public void InsertAtSpatialLocation_CapturedPositionSurvivesSourceRelocation()
	{
		var routeDefinition = new Mock<IRouteCellDefinition>();
		routeDefinition.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		var originalCell = new Mock<ICell>();
		originalCell.SetupGet(x => x.RouteDefinition).Returns(routeDefinition.Object);
		var movedCell = new Mock<ICell>();
		movedCell.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		var source = new Mock<ICharacter>();
		source.SetupGet(x => x.Location).Returns(movedCell.Object);
		var item = new Mock<IGameItem>();
		item.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		var captured = new SpatialLocation(originalCell.Object, RoomLayer.InAir, 3_333.25);

		item.Object.InsertAtSpatialLocation(captured, true);

		Assert.AreEqual(RoomLayer.InAir, item.Object.RoomLayer);
		item.Verify(x => x.MoveTo(captured, null, false), Times.Once);
		originalCell.Verify(x => x.Insert(item.Object, true), Times.Once);
		movedCell.Verify(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void ComponentLifecycleRelease_UsesPreCallbackCoordinateAfterParentRelocates()
	{
		var routeDefinition = new Mock<IRouteCellDefinition>();
		routeDefinition.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		var originalCell = new Mock<ICell>();
		originalCell.SetupGet(x => x.RouteDefinition).Returns(routeDefinition.Object);
		var movedCell = new Mock<ICell>();
		movedCell.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Location).Returns(movedCell.Object);
		var releasedItem = new Mock<IGameItem>();
		releasedItem.SetupProperty(x => x.RoomLayer, RoomLayer.InTrees);
		IGameItemComponent component = new PlacementTestComponent(parent.Object, releasedItem.Object);
		var captured = new SpatialLocation(originalCell.Object, RoomLayer.GroundLevel, 4_444.5);

		component.HandleDieOrMorph(Mock.Of<IGameItem>(), originalCell.Object, captured);

		releasedItem.Verify(x => x.MoveTo(
			new SpatialLocation(originalCell.Object, RoomLayer.InTrees, 4_444.5),
			null,
			false), Times.Once);
		originalCell.Verify(x => x.Insert(releasedItem.Object, false), Times.Once);
		movedCell.Verify(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void ComponentActorRelease_PrefersEmptierCoordinateOverContainerCoordinate()
	{
		var routeDefinition = new Mock<IRouteCellDefinition>();
		routeDefinition.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.RouteDefinition).Returns(routeDefinition.Object);
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Location).Returns(cell.Object);
		parent.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 1_000.0));
		var emptier = new Mock<ICharacter>();
		emptier.SetupGet(x => x.Location).Returns(cell.Object);
		emptier.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 1_002.5));
		var releasedItem = new Mock<IGameItem>();
		releasedItem.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		var component = new PlacementTestComponent(parent.Object);

		component.PlaceReleasedItem(releasedItem.Object, cell.Object, emptier.Object);

		releasedItem.Verify(x => x.MoveTo(
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 1_002.5),
			null,
			false), Times.Once);
		cell.Verify(x => x.Insert(releasedItem.Object, false), Times.Once);
	}

	private static (Mock<ICell> Cell, Mock<ICharacter> Source, Mock<IGameItem> Item)
		CreateRoutePlacementFixture(double coordinate, RoomLayer sourceLayer)
	{
		var routeDefinition = new Mock<IRouteCellDefinition>();
		routeDefinition.SetupGet(x => x.LengthMetres).Returns(10_000.0);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.RouteDefinition).Returns(routeDefinition.Object);
		var source = new Mock<ICharacter>();
		source.SetupGet(x => x.Location).Returns(cell.Object);
		source.SetupGet(x => x.RoomLayer).Returns(sourceLayer);
		source.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, sourceLayer, coordinate));
		var item = new Mock<IGameItem>();
		item.SetupProperty(x => x.RoomLayer, RoomLayer.InTrees);
		return (cell, source, item);
	}

	private sealed class PlacementTestComponent : GameItemComponent
	{
		private readonly IGameItem? _releasedItem;
		private readonly IGameItemComponentProto _prototype = Mock.Of<IGameItemComponentProto>();

		public PlacementTestComponent(IGameItem parent, IGameItem? releasedItem = null)
			: base(parent, Mock.Of<IGameItemComponentProto>(), true)
		{
			_releasedItem = releasedItem;
		}

		public override IGameItemComponentProto Prototype => _prototype;

		public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
		{
			if (_releasedItem is not null)
			{
				InsertAtParentSpatialLocation(_releasedItem, location);
			}

			return false;
		}

		public void PlaceReleasedItem(IGameItem item, ICell location, ILocateable preferredSource)
		{
			InsertAtParentSpatialLocation(item, location, preferredSource: preferredSource);
		}

		public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
		{
			return new PlacementTestComponent(newParent, _releasedItem);
		}

		protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
		{
		}

		protected override string SaveToXml()
		{
			return "<Definition />";
		}
	}
}
