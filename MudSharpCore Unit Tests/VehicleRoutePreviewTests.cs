#nullable enable

using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleRoutePreviewTests
{
	[TestMethod]
	public void DescribeCompiledRouteDetails_ListsTypedStepsCoordinatesCostsAndTopologyPins()
	{
		var actor = CreateActor();
		var routeCell = new Mock<ICell>();
		routeCell.SetupGet(x => x.Id).Returns(101L);
		routeCell.SetupGet(x => x.Name).Returns("Intertown Main Line");
		var routeDefinition = new Mock<IRouteCellDefinition>();
		routeDefinition.SetupGet(x => x.Cell).Returns(routeCell.Object);
		routeDefinition.SetupGet(x => x.TopologyVersion).Returns(7L);
		routeDefinition.SetupGet(x => x.PositiveDirectionName).Returns("eastbound");
		routeDefinition.SetupGet(x => x.NegativeDirectionName).Returns("westbound");
		routeCell.SetupGet(x => x.RouteDefinition).Returns(routeDefinition.Object);

		var townCell = new Mock<ICell>();
		townCell.SetupGet(x => x.Id).Returns(202L);
		townCell.SetupGet(x => x.Name).Returns("East Town Station");

		var originStop = CreateStop(11L, "West Town", routeCell.Object, 250.0);
		var destinationStop = CreateStop(12L, "East Town", townCell.Object, null);
		var leg = new Mock<IVehicleRouteLeg>();
		leg.SetupGet(x => x.Id).Returns(21L);
		leg.SetupGet(x => x.Sequence).Returns(0);
		leg.SetupGet(x => x.OriginStop).Returns(originStop.Object);
		leg.SetupGet(x => x.DestinationStop).Returns(destinationStop.Object);
		leg.SetupGet(x => x.RouteDistanceMetres).Returns(500.0);
		leg.SetupGet(x => x.RoomEquivalentCost).Returns(6.25);

		var linear = new Mock<IVehicleRouteLinearStep>();
		linear.SetupGet(x => x.Id).Returns(301L);
		linear.SetupGet(x => x.Sequence).Returns(0);
		linear.SetupGet(x => x.StepType).Returns(VehicleRouteStepType.LinearRoute);
		linear.SetupGet(x => x.RouteCell).Returns(routeDefinition.Object);
		linear.SetupGet(x => x.Direction).Returns(RouteCellDirection.Positive);
		linear.SetupGet(x => x.DistanceMetres).Returns(500.0);
		linear.SetupGet(x => x.RoomEquivalentCost).Returns(5.0);
		linear.SetupGet(x => x.Origin).Returns(new SpatialLocation(routeCell.Object, RoomLayer.GroundLevel, 250.0));
		linear.SetupGet(x => x.Destination).Returns(new SpatialLocation(routeCell.Object, RoomLayer.GroundLevel, 750.0));
		linear.SetupGet(x => x.OriginTopologyVersion).Returns(7L);
		linear.SetupGet(x => x.DestinationTopologyVersion).Returns(7L);

		var persistentExit = new Mock<IExit>();
		persistentExit.SetupGet(x => x.Id).Returns(401L);
		var cellExit = new Mock<ICellExit>();
		cellExit.SetupGet(x => x.Exit).Returns(persistentExit.Object);
		cellExit.SetupGet(x => x.OutboundDirectionDescription).Returns("the station gate");
		var exitStep = new Mock<IVehicleRouteExitStep>();
		exitStep.SetupGet(x => x.Id).Returns(302L);
		exitStep.SetupGet(x => x.Sequence).Returns(1);
		exitStep.SetupGet(x => x.StepType).Returns(VehicleRouteStepType.CellExit);
		exitStep.SetupGet(x => x.Exit).Returns(cellExit.Object);
		exitStep.SetupGet(x => x.RoomEquivalentCost).Returns(1.25);
		exitStep.SetupGet(x => x.Origin).Returns(new SpatialLocation(routeCell.Object, RoomLayer.GroundLevel, 750.0));
		exitStep.SetupGet(x => x.Destination).Returns(new SpatialLocation(townCell.Object, RoomLayer.GroundLevel));
		exitStep.SetupGet(x => x.OriginTopologyVersion).Returns(7L);
		exitStep.SetupGet(x => x.DestinationTopologyVersion).Returns((long?)null);

		leg.SetupGet(x => x.Steps).Returns([linear.Object, exitStep.Object]);
		var pin = new VehicleRouteTopologyPin(routeCell.Object, 7L);

		var result = VehicleRoute.DescribeCompiledRouteDetails(actor.Object, [leg.Object], [pin])
			.StripANSIColour();

		StringAssert.Contains(result, "LinearRouteStep #301");
		StringAssert.Contains(result, "cell #101 Intertown Main Line / GroundLevel at local-length-250");
		StringAssert.Contains(result, "cell #101 Intertown Main Line / GroundLevel at local-length-750");
		StringAssert.Contains(result, "Direction eastbound; distance local-length-500; cost 5.000 room-equivalents");
		StringAssert.Contains(result, "topology pins: origin cell #101 v7, destination cell #101 v7");
		StringAssert.Contains(result, "CellExitStep #302 via exit #401 (the station gate)");
		StringAssert.Contains(result, "cell #202 East Town Station / GroundLevel");
		StringAssert.Contains(result, "Cost 1.250 room-equivalents");
		StringAssert.Contains(result, "RouteCell Topology Pins:");
		StringAssert.Contains(result, "Cell #101 Intertown Main Line: pinned v7, current v7 [current]");
	}

	[TestMethod]
	public void DescribeCompiledRouteDetails_NoLegsOrPins_ReportsEmptySections()
	{
		var actor = CreateActor();

		var result = VehicleRoute.DescribeCompiledRouteDetails(
			actor.Object,
			Array.Empty<IVehicleRouteLeg>(),
			Array.Empty<IVehicleRouteTopologyPin>());

		StringAssert.Contains(result, "Compiled Legs:");
		StringAssert.Contains(result, "RouteCell Topology Pins:");
		Assert.AreEqual(2, result.Split("\t(none)").Length - 1);
	}

	private static Mock<ICharacter> CreateActor()
	{
		var actor = new Mock<ICharacter>();
		actor
			.Setup(x => x.GetFormat(It.IsAny<Type>()))
			.Returns((Type type) => CultureInfo.InvariantCulture.GetFormat(type));
		var unitManager = new Mock<IUnitManager>();
		unitManager.SetupGet(x => x.BaseHeightToMetres).Returns(1.0);
		unitManager
			.Setup(x => x.DescribeMostSignificantExact(It.IsAny<double>(), UnitType.Length, actor.Object))
			.Returns((double value, UnitType _, IPerceiver _) => $"local-length-{value:0.###}");
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		return actor;
	}

	private static Mock<IVehicleRouteStop> CreateStop(long id, string name, ICell cell,
		double? routePositionMetres)
	{
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.Id).Returns(id);
		stop.SetupGet(x => x.Name).Returns(name);
		stop.SetupGet(x => x.Location)
			.Returns(new SpatialLocation(cell, RoomLayer.GroundLevel, routePositionMetres));
		return stop;
	}
}
