using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder.Areas;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AutobuilderRectangleTopologyTests
{
	[TestMethod]
	public void GetConnections_TwoByFiveDiagonalRectangle_ReturnsCompleteBoundedTopology()
	{
		var connections = AutobuilderRectangleTopology.GetConnections(2, 5, true).ToList();

		Assert.AreEqual(21, connections.Count);
		Assert.AreEqual(5, connections.Count(x => x.OutboundDirection == CardinalDirection.East));
		Assert.AreEqual(8, connections.Count(x => x.OutboundDirection == CardinalDirection.North));
		Assert.AreEqual(4, connections.Count(x => x.OutboundDirection == CardinalDirection.NorthEast));
		Assert.AreEqual(4, connections.Count(x => x.OutboundDirection == CardinalDirection.SouthEast));
		Assert.IsTrue(connections.Contains(new AutobuilderRectangleConnection(0, 0, 0, 1,
			CardinalDirection.North, CardinalDirection.South)));
		Assert.IsTrue(connections.Contains(new AutobuilderRectangleConnection(0, 0, 1, 1,
			CardinalDirection.NorthEast, CardinalDirection.SouthWest)));
		Assert.IsTrue(connections.All(x =>
			x.OriginX >= 0 && x.OriginX < 2 &&
			x.DestinationX >= 0 && x.DestinationX < 2 &&
			x.OriginY >= 0 && x.OriginY < 5 &&
			x.DestinationY >= 0 && x.DestinationY < 5));
		Assert.AreEqual(connections.Count, connections.Distinct().Count());
	}

	[TestMethod]
	public void GetConnections_TwoByFiveCardinalRectangle_ExcludesDiagonals()
	{
		var connections = AutobuilderRectangleTopology.GetConnections(2, 5, false).ToList();

		Assert.AreEqual(13, connections.Count);
		Assert.IsTrue(connections.All(x => x.OutboundDirection is CardinalDirection.East or CardinalDirection.North));
	}
}
