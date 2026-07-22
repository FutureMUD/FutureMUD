#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteSpatialServiceTests
{
	private static readonly RouteSpatialConfiguration Configuration =
		new(3.0, 10.0, 100.0, 500.0, 100.0);

	[TestMethod]
	public void RouteCellDefinition_ModelSnapshot_LoadsOrderedValidatedContent()
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(42L);
		var model = new MudSharp.Models.RouteCell
		{
			CellId = 42L,
			LengthMetres = 10_000.000M,
			DefaultPositionMetres = 125.000M,
			PositiveDirectionName = "townward",
			NegativeDirectionName = "stationward",
			MetresPerRoomEquivalent = 100.000M,
			TopologyVersion = 7L
		};
		model.Landmarks.Add(new MudSharp.Models.RouteCellLandmark
		{
			Id = 2L,
			Name = "Old Bridge",
			Keywords = "old bridge crossing",
			Description = "An old stone bridge.",
			PositionMetres = 7_500.000M,
			DisplayOrder = 2
		});
		model.Landmarks.Add(new MudSharp.Models.RouteCellLandmark
		{
			Id = 1L,
			Name = "Milepost",
			Keywords = "milepost marker",
			Description = "A weathered milepost.",
			PositionMetres = 500.000M,
			DisplayOrder = 1
		});
		model.ExitAnchors.Add(new MudSharp.Models.RouteExitAnchor
		{
			ExitId = 99L,
			MinimumPositionMetres = 7_100.000M,
			MaximumPositionMetres = 7_200.000M,
			ArrivalPositionMetres = 7_150.000M
		});

		var definition = new RouteCellDefinition(cell.Object, model);

		Assert.AreEqual(10_000.0, definition.LengthMetres);
		Assert.AreEqual(125.0, definition.DefaultPositionMetres);
		Assert.AreEqual(7L, definition.TopologyVersion);
		Assert.AreEqual(1L, definition.Landmarks[0].Id);
		Assert.IsTrue(definition.Landmarks[1].Keywords.Contains("bridge"));
		var anchor = (RouteCellExitAnchor)definition.ExitAnchors.Single();
		Assert.AreEqual(99L, anchor.ExitId);
		Assert.AreEqual(7_150.0, anchor.ArrivalPositionMetres);
	}

	[TestMethod]
	public void TryValidateLocation_OrdinaryAndRouteCoordinates_EnforcesSpatialModel()
	{
		var service = new RouteSpatialService(Configuration);
		var ordinary = new Mock<ICell>();
		ordinary.SetupGet(x => x.Id).Returns(1L);
		ordinary.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		var (route, _) = CreateRouteCell(2L, 100.0, 50.0);

		Assert.IsTrue(service.TryValidateLocation(
			new SpatialLocation(ordinary.Object, RoomLayer.GroundLevel), out _));
		Assert.IsFalse(service.TryValidateLocation(
			new SpatialLocation(ordinary.Object, RoomLayer.GroundLevel, 1.0), out _));
		Assert.IsTrue(service.TryValidateLocation(
			new SpatialLocation(route.Object, RoomLayer.GroundLevel, 0.0), out _));
		Assert.IsTrue(service.TryValidateLocation(
			new SpatialLocation(route.Object, RoomLayer.GroundLevel, 100.0), out _));
		Assert.IsFalse(service.TryValidateLocation(
			new SpatialLocation(route.Object, RoomLayer.GroundLevel), out _));
		Assert.IsFalse(service.TryValidateLocation(
			new SpatialLocation(route.Object, RoomLayer.GroundLevel, double.NaN), out _));
		Assert.IsFalse(service.TryValidateLocation(
			new SpatialLocation(route.Object, RoomLayer.GroundLevel, 100.001), out _));
	}

	[TestMethod]
	public void GetProximity_ThresholdBoundariesAndLayers_UseConfiguredMetricBands()
	{
		var service = new RouteSpatialService(Configuration);
		var (cell, _) = CreateRouteCell(3L, 1_000.0, 0.0);
		var origin = CreateLocateable(cell.Object, RoomLayer.GroundLevel, 100.0);

		Assert.AreEqual(Proximity.Immediate,
			service.GetProximity(origin.Object, CreateLocateable(cell.Object, RoomLayer.GroundLevel, 103.0).Object));
		Assert.AreEqual(Proximity.Proximate,
			service.GetProximity(origin.Object, CreateLocateable(cell.Object, RoomLayer.GroundLevel, 103.001).Object));
		Assert.AreEqual(Proximity.Proximate,
			service.GetProximity(origin.Object, CreateLocateable(cell.Object, RoomLayer.GroundLevel, 110.0).Object));
		Assert.AreEqual(Proximity.Distant,
			service.GetProximity(origin.Object, CreateLocateable(cell.Object, RoomLayer.GroundLevel, 200.0).Object));
		Assert.AreEqual(Proximity.VeryDistant,
			service.GetProximity(origin.Object, CreateLocateable(cell.Object, RoomLayer.GroundLevel, 600.0).Object));
		Assert.AreEqual(Proximity.Unapproximable,
			service.GetProximity(origin.Object, CreateLocateable(cell.Object, RoomLayer.GroundLevel, 600.001).Object));
		Assert.AreEqual(Proximity.VeryDistant,
			service.GetProximity(origin.Object, CreateLocateable(cell.Object, RoomLayer.InTrees, 100.0).Object));
	}

	[TestMethod]
	public void SharesLongitudinalVicinityWith_IgnoresLayerButNotRouteDistance()
	{
		var (cell, _) = CreateRouteCell(31L, 10_000.0, 0.0);
		var source = CreateLocateable(cell.Object, RoomLayer.GroundLevel, 7_150.0);
		var nearbyOtherLayer = CreateLocateable(cell.Object, RoomLayer.InTrees, 7_153.0);
		var farOtherLayer = CreateLocateable(cell.Object, RoomLayer.InTrees, 9_000.0);

		Assert.IsTrue(source.Object.SharesLongitudinalVicinityWith(nearbyOtherLayer.Object));
		Assert.IsFalse(source.Object.SharesLongitudinalVicinityWith(farOtherLayer.Object));
	}

	[TestMethod]
	public void GetPerceivablesWithin_IndexedRouteCell_ReturnsOnlyCoordinateRange()
	{
		var service = new RouteSpatialService(Configuration);
		var occupants = new List<IPerceivable>();
		var (cell, _) = CreateRouteCell(4L, 20_000.0, 0.0, occupants);
		for (var index = 0; index < 2_000; index++)
		{
			var perceivable = new Mock<IPerceivable>();
			var position = index * 10.0;
			perceivable.SetupGet(x => x.Location).Returns(cell.Object);
			perceivable.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
			perceivable.SetupGet(x => x.RoutePositionMetres).Returns(position);
			perceivable.SetupGet(x => x.SpatialLocation)
				.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, position));
			occupants.Add(perceivable.Object);
		}

		var results = service.GetPerceivablesWithin(
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 5_000.0),
			10.0);

		Assert.AreEqual(3, results.Count);
		Assert.IsTrue(results.All(x => x.RoutePositionMetres is >= 4_990.0 and <= 5_010.0));
	}

	[TestMethod]
	public void IsExitVisible_RouteCellRequiresPerceptionAndLongitudinalRange()
	{
		var service = new RouteSpatialService(Configuration);
		var (cell, definition) = CreateRouteCell(41L, 10_000.0, 0.0);
		var exit = new Mock<MudSharp.Construction.Boundary.ICellExit>();
		var sharedExit = new Mock<MudSharp.Construction.Boundary.IExit>();
		sharedExit.SetupGet(x => x.Id).Returns(99L);
		exit.SetupGet(x => x.Exit).Returns(sharedExit.Object);
		var anchor = new Mock<IRouteExitAnchor>();
		anchor.SetupGet(x => x.Exit).Returns(exit.Object);
		anchor.SetupGet(x => x.MinimumPositionMetres).Returns(7_100.0);
		anchor.SetupGet(x => x.MaximumPositionMetres).Returns(7_200.0);
		anchor.Setup(x => x.Contains(It.IsAny<double>()))
			.Returns((double position) => position is >= 7_100.0 and <= 7_200.0);
		definition.SetupGet(x => x.ExitAnchors).Returns([anchor.Object]);
		var voyeur = new Mock<IPerceiver>();
		voyeur.SetupGet(x => x.Location).Returns(cell.Object);
		voyeur.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		voyeur.SetupGet(x => x.RoutePositionMetres).Returns(6_500.0);
		voyeur.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 6_500.0));
		cell.Setup(x => x.IsExitVisible(
			voyeur.Object,
			exit.Object,
			PerceptionTypes.DirectVisual,
			PerceiveIgnoreFlags.None)).Returns(true);

		Assert.IsFalse(service.IsExitVisible(voyeur.Object, exit.Object, 500.0),
			"An otherwise visible portal beyond the longitudinal view range must remain undiscoverable.");
		Assert.IsTrue(service.IsExitVisible(voyeur.Object, exit.Object, 600.0));

		cell.Setup(x => x.IsExitVisible(
			voyeur.Object,
			exit.Object,
			PerceptionTypes.DirectVisual,
			PerceiveIgnoreFlags.None)).Returns(false);
		Assert.IsFalse(service.IsExitVisible(voyeur.Object, exit.Object, 600.0),
			"Longitudinal range must not bypass an authored hidden-exit effect.");
	}

	[TestMethod]
	public void GetPerceivablesWithin_ActiveMovementIndexDoesNotScanOtherRouteCells()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var service = new RouteSpatialService(Configuration, clock);
		var (queriedCell, _) = CreateRouteCell(42L, 10_000.0, 0.0);
		var (foreignCell, _) = CreateRouteCell(43L, 10_000.0, 0.0);
		var queriedMover = CreateActivePerceivable(queriedCell.Object, 5_000.0);
		var queriedSamples = 0;
		var queriedSegment = CreateCountingSegment(queriedCell.Object, () => queriedSamples++);
		service.BeginActiveMovement(queriedMover.Object, queriedSegment.Object);

		var foreignSamples = 0;
		var foreignSegment = CreateCountingSegment(foreignCell.Object, () => foreignSamples++);
		for (var index = 0; index < 2_000; index++)
		{
			var foreignMover = CreateActivePerceivable(foreignCell.Object, 5_000.0);
			service.BeginActiveMovement(foreignMover.Object, foreignSegment.Object);
		}

		var results = service.GetPerceivablesWithin(
			new SpatialLocation(queriedCell.Object, RoomLayer.GroundLevel, 5_000.0),
			10.0);

		Assert.AreEqual(1, results.Count);
		Assert.AreSame(queriedMover.Object, results.Single());
		Assert.AreEqual(1, queriedSamples);
		Assert.AreEqual(0, foreignSamples,
			"A local query must not evaluate active movers from unrelated RouteCells.");
	}

	[TestMethod]
	public void ActiveMovement_FakeClock_PauseDelayResumeAndMaterialiseAreLazy()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var service = new RouteSpatialService(Configuration, clock);
		var (cell, _) = CreateRouteCell(5L, 100.0, 0.0);
		var mover = new TestPerceivedItem(100L);
		mover.MoveTo(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 0.0), noSave: true);
		var segment = new Mock<ISpatialMovementSegment>();
		segment.SetupGet(x => x.Origin)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 0.0));
		segment.SetupGet(x => x.Destination)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 100.0));
		segment.SetupGet(x => x.Direction).Returns(RouteCellDirection.Positive);
		segment.SetupGet(x => x.DistanceMetres).Returns(100.0);
		segment.SetupGet(x => x.SpeedMetresPerSecond).Returns(10.0);
		segment.SetupGet(x => x.Duration).Returns(TimeSpan.FromSeconds(10.0));
		segment.Setup(x => x.PositionAt(It.IsAny<TimeSpan>()))
			.Returns((TimeSpan elapsed) => new SpatialLocation(
				cell.Object,
				RoomLayer.GroundLevel,
				Math.Clamp(elapsed.TotalSeconds * 10.0, 0.0, 100.0)));

		service.BeginActiveMovement(mover, segment.Object);
		clock.SetUtcNow(new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero));
		clock.Advance(TimeSpan.FromSeconds(4.0));
		Assert.AreEqual(40.0, service.GetEffectiveLocation(mover).RoutePositionMetres);
		clock.SetUtcNow(new DateTimeOffset(2200, 1, 1, 0, 0, 0, TimeSpan.Zero));
		Assert.AreEqual(40.0, service.GetEffectiveLocation(mover).RoutePositionMetres,
			"Wall-clock jumps must not reverse or fast-forward a live RouteCell movement.");
		Assert.AreEqual(0.0, mover.RoutePositionMetres, "Lazy movement must not mutate the durable coordinate.");
		Assert.IsTrue(service.PauseActiveMovement(mover));
		clock.Advance(TimeSpan.FromSeconds(6.0));
		Assert.AreEqual(40.0, service.GetEffectiveLocation(mover).RoutePositionMetres);
		Assert.IsTrue(service.ResumeActiveMovement(mover, TimeSpan.FromSeconds(2.0)));
		clock.Advance(TimeSpan.FromSeconds(1.0));
		Assert.AreEqual(40.0, service.GetEffectiveLocation(mover).RoutePositionMetres);
		clock.Advance(TimeSpan.FromSeconds(4.0));
		Assert.AreEqual(70.0, service.GetEffectiveLocation(mover).RoutePositionMetres);

		var materialised = service.MaterialiseActiveMovement(mover, noSave: true);

		Assert.AreEqual(70.0, materialised?.RoutePositionMetres);
		Assert.AreEqual(70.0, mover.RoutePositionMetres);
		RouteSpatialService.Instance.UntrackPerceivable(mover);
	}

	[TestMethod]
	public void PerceivedItem_ColocatedWith_UsesRouteDistanceAndExplicitRelationshipOverrides()
	{
		var (cell, _) = CreateRouteCell(6L, 1_000.0, 0.0);
		var first = new TestPerceivedItem(201L);
		var second = new TestPerceivedItem(202L);
		first.MoveTo(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 100.0), noSave: true);
		second.MoveTo(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 104.0), noSave: true);

		Assert.IsFalse(first.ColocatedWith(second));
		second.SetRoutePosition(103.0);
		Assert.IsTrue(first.ColocatedWith(second));
		second.SetRoutePosition(900.0);
		first.SetTarget(second);
		Assert.AreEqual(Proximity.Immediate, first.GetProximity(second));
		Assert.IsTrue(first.ColocatedWith(second));

		RouteSpatialService.Instance.UntrackPerceivable(first);
		RouteSpatialService.Instance.UntrackPerceivable(second);
	}

	[TestMethod]
	public void PerceivedItem_SetRoutePosition_ValidatesAndRaisesSpatialEvent()
	{
		var (cell, _) = CreateRouteCell(7L, 100.0, 25.0);
		var item = new TestPerceivedItem(301L);
		item.MoveTo(cell.Object, RoomLayer.GroundLevel, noSave: true);
		SpatialLocation? previous = null;
		SpatialLocation? current = null;
		item.OnSpatialPositionChanged += (_, oldLocation, newLocation) =>
		{
			previous = oldLocation;
			current = newLocation;
		};

		item.SetRoutePosition(75.0);

		Assert.AreEqual(25.0, previous?.RoutePositionMetres);
		Assert.AreEqual(75.0, current?.RoutePositionMetres);
		Assert.AreEqual(75.0, item.RoutePositionMetres);
		Assert.ThrowsException<ArgumentException>(() => item.SetRoutePosition(101.0));
		Assert.AreEqual(75.0, item.RoutePositionMetres);
		RouteSpatialService.Instance.UntrackPerceivable(item);
	}

	[TestMethod]
	public void RouteSpatialConfiguration_InvalidThresholdOrder_ThrowsDiagnostic()
	{
		var invalid = new RouteSpatialConfiguration(3.0, 3.0, 100.0, 500.0, 100.0);

		Assert.ThrowsException<InvalidOperationException>(invalid.Validate);
	}

	private static (Mock<ICell> Cell, Mock<IRouteCellDefinition> Definition) CreateRouteCell(
		long id,
		double length,
		double defaultPosition,
		IEnumerable<IPerceivable>? perceivables = null)
	{
		var cell = new Mock<ICell>();
		var definition = new Mock<IRouteCellDefinition>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		cell.SetupGet(x => x.SpatialType).Returns(CellSpatialType.LinearRoute);
		cell.SetupGet(x => x.Perceivables).Returns(() => perceivables ?? []);
		definition.SetupGet(x => x.Cell).Returns(cell.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(length);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(defaultPosition);
		definition.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		definition.SetupGet(x => x.PositiveDirectionName).Returns("forward");
		definition.SetupGet(x => x.NegativeDirectionName).Returns("backward");
		definition.SetupGet(x => x.Landmarks).Returns([]);
		definition.SetupGet(x => x.ExitAnchors).Returns([]);
		return (cell, definition);
	}

	private static Mock<ILocateable> CreateLocateable(ICell cell, RoomLayer layer, double? position)
	{
		var locateable = new Mock<ILocateable>();
		locateable.SetupGet(x => x.Location).Returns(cell);
		locateable.SetupGet(x => x.RoomLayer).Returns(layer);
		locateable.SetupGet(x => x.RoutePositionMetres).Returns(position);
		locateable.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(cell, layer, position));
		return locateable;
	}

	private static Mock<IPerceivable> CreateActivePerceivable(ICell cell, double position)
	{
		var perceivable = new Mock<IPerceivable>();
		perceivable.SetupGet(x => x.Location).Returns(cell);
		perceivable.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		perceivable.SetupGet(x => x.RoutePositionMetres).Returns(position);
		perceivable.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(cell, RoomLayer.GroundLevel, position));
		return perceivable;
	}

	private static Mock<ISpatialMovementSegment> CreateCountingSegment(ICell cell, Action sampled)
	{
		var segment = new Mock<ISpatialMovementSegment>();
		var origin = new SpatialLocation(cell, RoomLayer.GroundLevel, 5_000.0);
		var destination = new SpatialLocation(cell, RoomLayer.GroundLevel, 5_100.0);
		segment.SetupGet(x => x.Origin).Returns(origin);
		segment.SetupGet(x => x.Destination).Returns(destination);
		segment.SetupGet(x => x.Direction).Returns(RouteCellDirection.Positive);
		segment.SetupGet(x => x.DistanceMetres).Returns(100.0);
		segment.SetupGet(x => x.SpeedMetresPerSecond).Returns(1.0);
		segment.SetupGet(x => x.Duration).Returns(TimeSpan.FromSeconds(100.0));
		segment.Setup(x => x.PositionAt(It.IsAny<TimeSpan>()))
			.Returns((TimeSpan elapsed) =>
			{
				sampled();
				return new SpatialLocation(cell, RoomLayer.GroundLevel,
					5_000.0 + Math.Clamp(elapsed.TotalSeconds, 0.0, 100.0));
			});
		return segment;
	}

	private sealed class TestPerceivedItem : PerceivedItem
	{
		public TestPerceivedItem(long id)
			: base(id)
		{
			_name = $"test item {id}";
			_keywords = new Lazy<List<string>>(() => ["test", "item"]);
			Gameworld = null!;
		}

		public override string FrameworkItemType => "TestPerceivedItem";
		public override ProgVariableTypes Type => ProgVariableTypes.Perceivable;

		public override void Register(IOutputHandler handler)
		{
		}

		public override object DatabaseInsert()
		{
			return this;
		}

		public override void SetIDFromDatabase(object dbitem)
		{
		}
	}
}
