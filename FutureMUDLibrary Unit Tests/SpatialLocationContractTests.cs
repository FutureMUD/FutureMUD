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
	public void SpatialLocation_OrdinaryCell_HasLegacyProjectionWithoutRoutePosition()
	{
		var cell = Mock.Of<ICell>();
		var location = new SpatialLocation(cell, RoomLayer.InTrees);

		Assert.AreSame(cell, location.Cell);
		Assert.AreEqual(RoomLayer.InTrees, location.Layer);
		Assert.IsFalse(location.HasRoutePosition);
		Assert.IsNull(location.RoutePositionMetres);
		Assert.AreSame(cell, location.InRoomLocation.Location);
		Assert.AreEqual(RoomLayer.InTrees, location.InRoomLocation.RoomLayer);
	}

	[TestMethod]
	public void SpatialLocation_RouteCell_PreservesCoordinateAndValueEquality()
	{
		var cell = Mock.Of<ICell>();
		var first = new SpatialLocation(cell, RoomLayer.GroundLevel, 7_150.25);
		var same = new SpatialLocation(cell, RoomLayer.GroundLevel, 7_150.25);
		var elsewhere = new SpatialLocation(cell, RoomLayer.GroundLevel, 7_151.25);

		Assert.IsTrue(first.HasRoutePosition);
		Assert.AreEqual(7_150.25, first.RoutePositionMetres);
		Assert.AreEqual(first, same);
		Assert.AreNotEqual(first, elsewhere);
	}

	[TestMethod]
	public void Locateable_DefaultSpatialMembers_PreserveOrdinaryCellBehaviour()
	{
		var cell = Mock.Of<ICell>();
		ILocateable locateable = new LocateableStub(cell, RoomLayer.GroundLevel);
		locateable.SetRoutePosition(500.0);

		Assert.IsNull(locateable.RoutePositionMetres);
		Assert.AreSame(cell, locateable.SpatialLocation.Cell);
		Assert.AreEqual(RoomLayer.GroundLevel, locateable.SpatialLocation.Layer);
		Assert.IsNull(locateable.SpatialLocation.RoutePositionMetres);
		Assert.AreSame(cell, locateable.InRoomLocation.Location);
		Assert.AreEqual(RoomLayer.GroundLevel, locateable.InRoomLocation.RoomLayer);
	}

	[TestMethod]
	public void SharesCellLayerWith_IgnoresLongitudinalCoordinateByDesign()
	{
		var cell = Mock.Of<ICell>();
		ILocateable first = new LocateableStub(cell, RoomLayer.GroundLevel, 10.0);
		ILocateable second = new LocateableStub(cell, RoomLayer.GroundLevel, 9_000.0);
		ILocateable otherLayer = new LocateableStub(cell, RoomLayer.InAir, 10.0);
		ILocateable otherCell = new LocateableStub(Mock.Of<ICell>(), RoomLayer.GroundLevel, 10.0);

		Assert.IsTrue(first.SharesCellLayerWith(second));
		Assert.IsFalse(first.SharesCellLayerWith(otherLayer));
		Assert.IsFalse(first.SharesCellLayerWith(otherCell));
		Assert.IsFalse(first.SharesCellLayerWith(null));
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
	public void RouteCellDirection_NumericSignMatchesCoordinateDirection()
	{
		Assert.AreEqual(-1, (int)RouteCellDirection.Negative);
		Assert.AreEqual(1, (int)RouteCellDirection.Positive);
	}

	private sealed class LocateableStub : ILocateable
	{
		public LocateableStub(ICell location, RoomLayer layer, double? routePositionMetres = null)
		{
			Location = location;
			RoomLayer = layer;
			RoutePositionMetres = routePositionMetres;
		}

		public string Name => "locateable";
		public long Id => 1;
		public string FrameworkItemType => "LocateableStub";
		public IEnumerable<string> Keywords => ["locateable"];
		public ICell Location { get; }
		public RoomLayer RoomLayer { get; set; }
		public double? RoutePositionMetres { get; }

		public bool ColocatedWith(IPerceivable otherThing)
		{
			return ((ILocateable)this).SharesCellLayerWith(otherThing);
		}

#pragma warning disable CS0067
		public event LocatableEvent? OnLocationChanged;
		public event LocatableEvent? OnLocationChangedIntentionally;
#pragma warning restore CS0067
	}

	private sealed class RouteExitAnchorStub : IRouteExitAnchor
	{
		public ICellExit Exit { get; } = Mock.Of<ICellExit>();
		public ICell Cell { get; } = Mock.Of<ICell>();
		public double MinimumPositionMetres { get; init; }
		public double MaximumPositionMetres { get; init; }
		public double ArrivalPositionMetres { get; init; }
	}
}
