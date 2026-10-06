#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Testing.EnvironmentalMagic;
using MudSharp.Vehicles;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CellSpatialOwnershipTests
{
	[TestMethod]
	public void OrdinaryHostedMembershipAndCustodyRollback_KeepIntrinsicParentsAndOtherContents()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var cells = fixture.Cells.OfType<Cell>().ToArray();
		var first = cells[0]; var exterior = cells[1];
		var (owner, other, shard, otherShard) = Owners(fixture, first.Id);
		Attach(first, owner); Attach(exterior, other);
		var vehicle = new Mock<IVehicle>(); vehicle.SetupGet(x => x.Id).Returns(9000); vehicle.SetupGet(x => x.Location).Returns(exterior);
		fixture.World.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { vehicle.Object });
		first.SetHostedVehicle(9000, 9001);
		var item = new Mock<IGameItem>(); ICell? location = null;
		item.SetupGet(x => x.Id).Returns(1);
		item.SetupGet(x => x.Location).Returns(() => location);
		item.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponent>());
		item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(x => ReferenceEquals(x, item.Object));
		item.Setup(x => x.MoveTo(It.IsAny<SpatialLocation>(), It.IsAny<ICellExit>(), It.IsAny<bool>()))
			.Callback<SpatialLocation, ICellExit, bool>((point, _, _) => location = point.Cell);
		var unrelated = Mock.Of<IGameItem>(x => x.Id == 9);
		first.SetPreparedCurrencyCellMembership(unrelated, true);
		first.Insert(item.Object, true);
		AssertMembership(first, owner, shard, item.Object, true);
		Assert.IsFalse(other.GameItems.Any(x => ReferenceEquals(x, item.Object)) || otherShard.GameItems.Any(x => ReferenceEquals(x, item.Object)));
		var restore = ((ICustodyRollbackLocation)first).CaptureCustodyMembershipRollback(new[] { item.Object });
		first.Extract(item.Object);
		AssertMembership(first, owner, shard, item.Object, false);
		var calls = item.Invocations.Count;
		restore();
		Assert.AreEqual(calls, item.Invocations.Count, "Membership rollback must invoke no item callbacks.");
		AssertMembership(first, owner, shard, item.Object, true);
		AssertMembership(first, owner, shard, unrelated, true);
		first.Extract(item.Object);
		AssertMembership(first, owner, shard, item.Object, false);
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock }; ICell? actorLocation = null;
		actor.SetupGet(x => x.Id).Returns(2);
		actor.SetupGet(x => x.Location).Returns(() => actorLocation);
		actor.SetupGet(x => x.Body.ExternalItems).Returns(Array.Empty<IGameItem>());
		actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, actor.Object));
		actor.Setup(x => x.MoveTo(It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<ICellExit>(), It.IsAny<bool>()))
			.Callback<ICell, RoomLayer, ICellExit, bool>((target, _, _, _) => actorLocation = target);
		first.Enter(actor.Object, noSave: true);
		Assert.AreEqual(1, first.Characters.Count(x => ReferenceEquals(x, actor.Object)));
		Assert.AreEqual(1, owner.Characters.Count(x => ReferenceEquals(x, actor.Object)));
		Assert.AreEqual(1, shard.Characters.Count(x => ReferenceEquals(x, actor.Object)));
		Assert.IsFalse(other.Characters.Any(x => ReferenceEquals(x, actor.Object)) || otherShard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		first.Leave(actor.Object);
		Assert.IsFalse(first.Characters.Any(x => ReferenceEquals(x, actor.Object)) || owner.Characters.Any(x => ReferenceEquals(x, actor.Object)) || shard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.AreEqual(1, actor.Invocations.Count(x => x.Method.Name == "MoveTo"));
		Assert.IsTrue(actor.Invocations.Any(x => x.Method.Name == "HandleEvent"), "Ordinary movement retains event callbacks.");
		AssertMembership(first, owner, shard, unrelated, true);
	}

	[TestMethod]
	public void SharedShard_RezoneAndInterruptedReconciliationKeepCanonicalMembershipOnce()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var cells = fixture.Cells.OfType<Cell>().ToArray();
		var (owner, other, shard, _) = Owners(fixture, cells[0].Id, sharedShard: true);
		Attach(cells[0], owner); Attach(cells[1], other);
		var item = Mock.Of<IGameItem>(x => x.Id == 1 && x.Location == cells[0]);
		cells[0].SetPreparedCurrencyCellMembership(item, true);
		cells[0].SetNewZone(other);
		AssertMembership(cells[0], other, shard, item, true);
		Assert.IsFalse(owner.GameItems.Any(x => ReferenceEquals(x, item)));
		cells[0].SetNewZone(owner);
		ICell actorLocation = cells[0];
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Id).Returns(2); actor.SetupGet(x => x.Location).Returns(() => actorLocation);
		cells[0].ReconcileNativeCharacterMembership(actor.Object, true);
		actorLocation = cells[1];
		cells[1].ReconcileNativeCharacterMembership(actor.Object, true);
		cells[0].ReconcileNativeCharacterMembership(actor.Object, false);
		Assert.IsFalse(owner.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.AreEqual(1, other.Characters.Count(x => ReferenceEquals(x, actor.Object)));
		Assert.AreEqual(1, shard.Characters.Count(x => ReferenceEquals(x, actor.Object)));
		AssertMembership(cells[0], owner, shard, item, true);
		Assert.IsFalse(actor.Invocations.Any(x => x.Method.Name is "MoveTo" or "HandleEvent"));
	}

	[TestMethod]
	public void ZoneDefaults_PreserveExplicitNonFirstSelectionAndChooseSurvivorOrFirstNewCell()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var cells = fixture.Cells.OfType<Cell>().ToArray();
		var (owner, other, _, _) = Owners(fixture, cells[1].Id);
		Attach(cells[0], owner); Attach(cells[1], owner);
		Assert.AreSame(cells[1], owner.DefaultCell, "Loading a preceding cell must not overwrite an explicit default.");
		owner.Changed = false; other.Changed = false;
		cells[1].SetNewZone(other);
		Assert.AreSame(cells[0], owner.DefaultCell);
		Assert.AreSame(cells[1], other.DefaultCell);
		Assert.IsTrue(owner.Changed && other.Changed);
		other.Unregister(cells[1]); other.Changed = false;
		other.Register(cells[1]);
		Assert.AreSame(cells[1], other.DefaultCell);
		Assert.IsTrue(other.Changed, "First-cell registration must persist its selected default.");
	}

	private static void AssertMembership(Cell cell, Zone zone, Shard shard, IGameItem item, bool present)
	{
		var expected = present ? 1 : 0;
		Assert.AreEqual(expected, cell.GameItems.Count(x => ReferenceEquals(x, item)));
		Assert.AreEqual(expected, zone.GameItems.Count(x => ReferenceEquals(x, item)));
		Assert.AreEqual(expected, shard.GameItems.Count(x => ReferenceEquals(x, item)));
	}

	[TestMethod]
	public void HostedRawMembershipAndRezone_UseStoredOwnerAndPreserveCallbackBoundary()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var cells = fixture.Cells.OfType<Cell>().ToArray();
		var first = cells[0]; var exterior = cells[1];
		var (owner, other, shard, otherShard) = Owners(fixture, first.Id);
		Attach(first, owner); Attach(exterior, other);
		first.SetCoordinates(17, -2, 4); exterior.SetCoordinates(900, 0, 0);
		var vehicle = new Mock<IVehicle>(); vehicle.SetupGet(x => x.Id).Returns(9000); vehicle.SetupGet(x => x.Location).Returns(exterior);
		fixture.World.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { vehicle.Object });
		first.SetHostedVehicle(9000, 9001);
		Assert.AreSame(other, first.Zone); Assert.AreSame(owner, first.OwningZone);
		Assert.AreEqual(900, first.X); Assert.AreEqual((17, -2, 4), first.StoredCoordinates);
		var item = Mock.Of<IGameItem>(x => x.Id == 1 && x.Location == first);
		first.SetPreparedCurrencyCellMembership(item, true);
		Assert.IsTrue(owner.GameItems.Any(x => ReferenceEquals(x, item)) && shard.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.IsFalse(other.GameItems.Any(x => ReferenceEquals(x, item)) || otherShard.GameItems.Any(x => ReferenceEquals(x, item)));
		first.SetPreparedCurrencyCellMembership(item, false);
		Assert.IsFalse(first.GameItems.Any(x => ReferenceEquals(x, item)) || owner.GameItems.Any(x => ReferenceEquals(x, item)) || shard.GameItems.Any(x => ReferenceEquals(x, item)));
		first.SetPreparedCurrencyCellMembership(item, true);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Id).Returns(2); actor.SetupGet(x => x.Location).Returns(first);
		first.ReconcileNativeCharacterMembership(actor.Object, true);
		Assert.IsTrue(owner.Characters.Any(x => ReferenceEquals(x, actor.Object)) && shard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.IsFalse(other.Characters.Any(x => ReferenceEquals(x, actor.Object)) || otherShard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		owner.Changed = false; other.Changed = false;
		first.SetNewZone(other);
		Assert.IsFalse(owner.Cells.Contains(first) || shard.Cells.Contains(first));
		Assert.IsTrue(other.Cells.Contains(first) && otherShard.Cells.Contains(first));
		Assert.IsFalse(owner.GameItems.Any(x => ReferenceEquals(x, item)) || shard.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.IsFalse(owner.Characters.Any(x => ReferenceEquals(x, actor.Object)) || shard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.IsTrue(other.GameItems.Any(x => ReferenceEquals(x, item)) && otherShard.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.IsTrue(other.Characters.Any(x => ReferenceEquals(x, actor.Object)) && otherShard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.IsNull(owner.DefaultCell, "The old persisted default must be cleared when its sole cell moves.");
		Assert.IsTrue(owner.Changed && other.Changed, "Both owners must save their current default selection.");
		Assert.AreEqual((17, -2, 4), first.StoredCoordinates);
		Assert.IsFalse(actor.Invocations.Any(x => x.Method.Name is "MoveTo" or "HandleEvent"), "Rezone/raw reconciliation invokes no movement callbacks.");
	}

	[TestMethod]
	public void DuplicateCoordinatesAndSimulation_HaveNoSiblingOwnerOrGlobalKeyRegistration()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var cells = fixture.Cells.OfType<Cell>().ToArray();
		var (owner, other, shard, _) = Owners(fixture, cells[0].Id);
		foreach (var cell in cells) { Attach(cell, owner); cell.SetCoordinates(17, -2, 4); }
		Assert.IsNull(shard.DetermineCellByCoordinates(17, -2, 4), "Duplicate coordinates are allowed; targeting must refuse ambiguity.");
		Assert.IsTrue(cells[0].TrySetUniqueName("Mirandola:Gate", out _));
		var simulation = new Cell(cells[0], -100);
		Assert.AreSame(owner, simulation.OwningZone);
		Assert.AreEqual(cells[0].StoredCoordinates, simulation.StoredCoordinates);
		Assert.IsNull(simulation.UniqueName);
		Assert.IsFalse(owner.Cells.Contains(simulation) || shard.Cells.Contains(simulation));
		var item = Mock.Of<IGameItem>(x => x.Id == 10);
		simulation.SetPreparedCurrencyCellMembership(item, true);
		Assert.IsTrue(simulation.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.IsFalse(owner.GameItems.Any(x => ReferenceEquals(x, item)) || shard.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.ThrowsException<InvalidOperationException>(() => simulation.SetNewZone(other));
	}

	private static void Attach(Cell cell, Zone zone)
	{
		typeof(Cell).GetField("_owningZone", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(cell, zone);
		zone.Register(cell);
	}

	private static (Zone Owner, Zone Other, Shard Shard, Shard OtherShard) Owners(EnvironmentalMagicTestWorld fixture, long defaultCell, bool sharedShard = false)
	{
		var shard = new Shard(new Db.Shard { Id = 101, Name = "Stored shard", SphericalRadiusMetres = 6371000 }, fixture.World.Object);
		var otherShard = sharedShard ? shard : new Shard(new Db.Shard { Id = 102, Name = "Exterior shard", SphericalRadiusMetres = 6371000 }, fixture.World.Object);
		fixture.World.SetupGet(x => x.Shards).Returns(sharedShard ? new All<IShard> { shard } : new All<IShard> { shard, otherShard });
		fixture.World.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController>());
		var owner = new Zone(new Db.Zone { Id = 201, Name = "Stored", ShardId = shard.Id, DefaultCellId = defaultCell == 0 ? null : defaultCell }, fixture.World.Object);
		var other = new Zone(new Db.Zone { Id = 202, Name = "Exterior", ShardId = otherShard.Id }, fixture.World.Object);
		return (owner, other, shard, otherShard);
	}
}
