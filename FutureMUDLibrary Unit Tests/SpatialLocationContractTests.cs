#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpatialLocationContractTests
{
	[TestMethod]
	public void SpatialLocation_OrdinaryRoom_HasLegacyProjectionWithoutRoutePosition()
	{
		var room = Mock.Of<IRoom>();
		var location = new SpatialLocation(room, RoomLayer.InTrees);

		Assert.AreSame(room, location.Room);
		Assert.AreEqual(RoomLayer.InTrees, location.Layer);
		Assert.IsFalse(location.HasRoutePosition);
		Assert.IsNull(location.RoutePositionMetres);
		Assert.AreSame(room, location.InRoomLocation.Location);
		Assert.AreEqual(RoomLayer.InTrees, location.InRoomLocation.RoomLayer);
	}

	[TestMethod]
	public void SpatialLocation_RouteRoom_PreservesCoordinateAndValueEquality()
	{
		var room = Mock.Of<IRoom>();
		var first = new SpatialLocation(room, RoomLayer.GroundLevel, 7_150.25);
		var same = new SpatialLocation(room, RoomLayer.GroundLevel, 7_150.25);
		var elsewhere = new SpatialLocation(room, RoomLayer.GroundLevel, 7_151.25);

		Assert.IsTrue(first.HasRoutePosition);
		Assert.AreEqual(7_150.25, first.RoutePositionMetres);
		Assert.AreEqual(first, same);
		Assert.AreNotEqual(first, elsewhere);
	}

	[TestMethod]
	public void Locateable_DefaultSpatialMembers_PreserveOrdinaryRoomBehaviour()
	{
		var room = Mock.Of<IRoom>();
		ILocateable locateable = new LocateableStub(room, RoomLayer.GroundLevel);
		locateable.SetRoutePosition(500.0);

		Assert.IsNull(locateable.RoutePositionMetres);
		Assert.AreSame(room, locateable.SpatialLocation.Room);
		Assert.AreEqual(RoomLayer.GroundLevel, locateable.SpatialLocation.Layer);
		Assert.IsNull(locateable.SpatialLocation.RoutePositionMetres);
		Assert.AreSame(room, locateable.InRoomLocation.Location);
		Assert.AreEqual(RoomLayer.GroundLevel, locateable.InRoomLocation.RoomLayer);
	}

	[TestMethod]
	public void SharesRoomLayerWith_IgnoresLongitudinalCoordinateByDesign()
	{
		var room = Mock.Of<IRoom>();
		ILocateable first = new LocateableStub(room, RoomLayer.GroundLevel, 10.0);
		ILocateable second = new LocateableStub(room, RoomLayer.GroundLevel, 9_000.0);
		ILocateable otherLayer = new LocateableStub(room, RoomLayer.InAir, 10.0);
		ILocateable otherRoom = new LocateableStub(Mock.Of<IRoom>(), RoomLayer.GroundLevel, 10.0);

		Assert.IsTrue(first.SharesRoomLayerWith(second));
		Assert.IsFalse(first.SharesRoomLayerWith(otherLayer));
		Assert.IsFalse(first.SharesRoomLayerWith(otherRoom));
		Assert.IsFalse(first.SharesRoomLayerWith(null));
	}

	[TestMethod]
	public void RouteExitAnchor_ContainsUsesInclusiveBandEndpoints()
	{
		IRouteExitAnchor anchor = new RouteExitAnchorStub
		{
			MinimumPositionMetres = 7_100.0,
			MaximumPositionMetres = 7_200.0
		};

		Assert.IsFalse(anchor.Contains(7_099.999));
		Assert.IsTrue(anchor.Contains(7_100.0));
		Assert.IsTrue(anchor.Contains(7_150.0));
		Assert.IsTrue(anchor.Contains(7_200.0));
		Assert.IsFalse(anchor.Contains(7_200.001));
	}

	[TestMethod]
	public void RouteRoomDirection_NumericSignMatchesCoordinateDirection()
	{
		Assert.AreEqual(-1, (int)RouteRoomDirection.Negative);
		Assert.AreEqual(1, (int)RouteRoomDirection.Positive);
	}

	private sealed class LocateableStub : ILocateable
	{
		public LocateableStub(IRoom location, RoomLayer layer, double? routePositionMetres = null)
		{
			Location = location;
			RoomLayer = layer;
			RoutePositionMetres = routePositionMetres;
		}

		public string Name => "locateable";
		public long Id => 1;
		public string FrameworkItemType => "LocateableStub";
		public IEnumerable<string> Keywords => ["locateable"];
		public IRoom Location { get; }
		public RoomLayer RoomLayer { get; set; }
		public double? RoutePositionMetres { get; }

		public bool ColocatedWith(IPerceivable otherThing)
		{
			return ((ILocateable)this).SharesRoomLayerWith(otherThing);
		}

#pragma warning disable CS0067
		public event LocatableEvent? OnLocationChanged;
		public event LocatableEvent? OnLocationChangedIntentionally;
#pragma warning restore CS0067
	}

	private sealed class RouteExitAnchorStub : IRouteExitAnchor
	{
		public IRoomExit Exit { get; } = Mock.Of<IRoomExit>();
		public IRoom Room { get; } = Mock.Of<IRoom>();
		public double MinimumPositionMetres { get; init; }
		public double MaximumPositionMetres { get; init; }
		public double ArrivalPositionMetres { get; init; }
	}
}
