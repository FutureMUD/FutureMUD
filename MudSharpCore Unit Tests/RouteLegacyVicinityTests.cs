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
	public void RoomsInVicinity_SourceRouteRoom_DoesNotFlattenRouteIntoOneRoom()
	{
		var route = new Mock<IRoom>();
		var definition = new Mock<IRouteRoomDefinition>();
		var ordinary = new Mock<IRoom>();
		var exit = new Mock<IRoomExit>();
		route.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		route.Setup(x => x.ExitsFor(null!, true)).Returns([exit.Object]);
		exit.SetupGet(x => x.Destination).Returns(ordinary.Object);
		var source = new Mock<IPerceivable>();
		source.SetupGet(x => x.Location).Returns(route.Object);

		var rooms = source.Object.RoomsInVicinity(10, false, false).ToArray();

		CollectionAssert.AreEqual(new[] { route.Object }, rooms);
	}

	[TestMethod]
	public void RoomsInVicinity_OrdinarySource_DoesNotEnterRouteRoomShortcut()
	{
		var ordinary = new Mock<IRoom>();
		var route = new Mock<IRoom>();
		var definition = new Mock<IRouteRoomDefinition>();
		var exit = new Mock<IRoomExit>();
		route.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		ordinary.Setup(x => x.ExitsFor(null!, true)).Returns([exit.Object]);
		exit.SetupGet(x => x.Destination).Returns(route.Object);
		var source = new Mock<IPerceivable>();
		source.SetupGet(x => x.Location).Returns(ordinary.Object);

		var rooms = source.Object.RoomsInVicinity(10, false, false).ToArray();

		CollectionAssert.AreEqual(new[] { ordinary.Object }, rooms);
	}
}
