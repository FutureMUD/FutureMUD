#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteLegacyVicinityTests
{
	[TestMethod]
	public void CellsInVicinity_SourceRouteCell_DoesNotFlattenRouteIntoOneRoom()
	{
		var route = new Mock<ICell>();
		var definition = new Mock<IRouteCellDefinition>();
		var ordinary = new Mock<ICell>();
		var exit = new Mock<ICellExit>();
		route.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		route.Setup(x => x.ExitsFor(null!, true)).Returns([exit.Object]);
		exit.SetupGet(x => x.Destination).Returns(ordinary.Object);
		var source = new Mock<IPerceivable>();
		source.SetupGet(x => x.Location).Returns(route.Object);

		var cells = source.Object.CellsInVicinity(10, false, false).ToArray();

		CollectionAssert.AreEqual(new[] { route.Object }, cells);
	}

	[TestMethod]
	public void CellsInVicinity_OrdinarySource_DoesNotEnterRouteCellShortcut()
	{
		var ordinary = new Mock<ICell>();
		var route = new Mock<ICell>();
		var definition = new Mock<IRouteCellDefinition>();
		var exit = new Mock<ICellExit>();
		route.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		ordinary.Setup(x => x.ExitsFor(null!, true)).Returns([exit.Object]);
		exit.SetupGet(x => x.Destination).Returns(route.Object);
		var source = new Mock<IPerceivable>();
		source.SetupGet(x => x.Location).Returns(ordinary.Object);

		var cells = source.Object.CellsInVicinity(10, false, false).ToArray();

		CollectionAssert.AreEqual(new[] { ordinary.Object }, cells);
	}
}
