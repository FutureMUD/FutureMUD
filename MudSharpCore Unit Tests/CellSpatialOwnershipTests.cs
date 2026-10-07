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
public class RoomSpatialOwnershipTests
{
	[TestMethod]
	public void OrdinaryHostedMembershipAndCustodyRollback_KeepIntrinsicParentsAndOtherContents()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var rooms = fixture.Rooms.OfType<Room>().ToArray();
		var first = rooms[0]; var exterior = rooms[1];
		var (owner, other, shard, otherShard) = Owners(fixture, first.Id);
		Attach(first, owner); Attach(exterior, other);
		var vehicle = new Mock<IVehicle>(); vehicle.SetupGet(x => x.Id).Returns(9000); vehicle.SetupGet(x => x.Location).Returns(exterior);
		fixture.World.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { vehicle.Object });
		first.SetHostedVehicle(9000, 9001);
		var item = new Mock<IGameItem>(); IRoom? location = null;
		item.SetupGet(x => x.Id).Returns(1);
		item.SetupGet(x => x.Location).Returns(() => location);
		item.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponent>());
		item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(x => ReferenceEquals(x, item.Object));
		item.Setup(x => x.MoveTo(It.IsAny<SpatialLocation>(), It.IsAny<IRoomExit>(), It.IsAny<bool>()))
			.Callback<SpatialLocation, IRoomExit, bool>((point, _, _) => location = point.Room);
		var unrelated = Mock.Of<IGameItem>(x => x.Id == 9);
		first.SetPreparedCurrencyRoomMembership(unrelated, true);
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
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock }; IRoom? actorLocation = null;
		actor.SetupGet(x => x.Id).Returns(2);
		actor.SetupGet(x => x.Location).Returns(() => actorLocation);
		actor.SetupGet(x => x.Body.ExternalItems).Returns(Array.Empty<IGameItem>());
		actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, actor.Object));
		actor.Setup(x => x.MoveTo(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), It.IsAny<IRoomExit>(), It.IsAny<bool>()))
			.Callback<IRoom, RoomLayer, IRoomExit, bool>((target, _, _, _) => actorLocation = target);
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
		var rooms = fixture.Rooms.OfType<Room>().ToArray();
		var (owner, other, shard, _) = Owners(fixture, rooms[0].Id, sharedShard: true);
		Attach(rooms[0], owner); Attach(rooms[1], other);
		var item = Mock.Of<IGameItem>(x => x.Id == 1 && x.Location == rooms[0]);
		rooms[0].SetPreparedCurrencyRoomMembership(item, true);
		rooms[0].SetNewZone(other);
		AssertMembership(rooms[0], other, shard, item, true);
		Assert.IsFalse(owner.GameItems.Any(x => ReferenceEquals(x, item)));
		rooms[0].SetNewZone(owner);
		IRoom actorLocation = rooms[0];
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Id).Returns(2); actor.SetupGet(x => x.Location).Returns(() => actorLocation);
		rooms[0].ReconcileNativeCharacterMembership(actor.Object, true);
		actorLocation = rooms[1];
		rooms[1].ReconcileNativeCharacterMembership(actor.Object, true);
		rooms[0].ReconcileNativeCharacterMembership(actor.Object, false);
		Assert.IsFalse(owner.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.AreEqual(1, other.Characters.Count(x => ReferenceEquals(x, actor.Object)));
		Assert.AreEqual(1, shard.Characters.Count(x => ReferenceEquals(x, actor.Object)));
		AssertMembership(rooms[0], owner, shard, item, true);
		Assert.IsFalse(actor.Invocations.Any(x => x.Method.Name is "MoveTo" or "HandleEvent"));
	}

	[TestMethod]
	public void ZoneDefaults_PreserveExplicitNonFirstSelectionAndChooseSurvivorOrFirstNewRoom()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var rooms = fixture.Rooms.OfType<Room>().ToArray();
		var (owner, other, _, _) = Owners(fixture, rooms[1].Id);
		Attach(rooms[0], owner); Attach(rooms[1], owner);
		Assert.AreSame(rooms[1], owner.DefaultRoom, "Loading a preceding cell must not overwrite an explicit default.");
		owner.Changed = false; other.Changed = false;
		rooms[1].SetNewZone(other);
		Assert.AreSame(rooms[0], owner.DefaultRoom);
		Assert.AreSame(rooms[1], other.DefaultRoom);
		Assert.IsTrue(owner.Changed && other.Changed);
		other.Unregister(rooms[1]); other.Changed = false;
		other.Register(rooms[1]);
		Assert.AreSame(rooms[1], other.DefaultRoom);
		Assert.IsTrue(other.Changed, "First-cell registration must persist its selected default.");
	}

	private static void AssertMembership(Room room, Zone zone, Shard shard, IGameItem item, bool present)
	{
		var expected = present ? 1 : 0;
		Assert.AreEqual(expected, room.GameItems.Count(x => ReferenceEquals(x, item)));
		Assert.AreEqual(expected, zone.GameItems.Count(x => ReferenceEquals(x, item)));
		Assert.AreEqual(expected, shard.GameItems.Count(x => ReferenceEquals(x, item)));
	}

	[TestMethod]
	public void HostedRawMembershipAndRezone_UseStoredOwnerAndPreserveCallbackBoundary()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var rooms = fixture.Rooms.OfType<Room>().ToArray();
		var first = rooms[0]; var exterior = rooms[1];
		var (owner, other, shard, otherShard) = Owners(fixture, first.Id);
		Attach(first, owner); Attach(exterior, other);
		first.SetCoordinates(17, -2, 4); exterior.SetCoordinates(900, 0, 0);
		var vehicle = new Mock<IVehicle>(); vehicle.SetupGet(x => x.Id).Returns(9000); vehicle.SetupGet(x => x.Location).Returns(exterior);
		fixture.World.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { vehicle.Object });
		first.SetHostedVehicle(9000, 9001);
		Assert.AreSame(other, first.Zone); Assert.AreSame(owner, first.OwningZone);
		Assert.AreEqual(900, first.X); Assert.AreEqual((17, -2, 4), first.StoredCoordinates);
		var item = Mock.Of<IGameItem>(x => x.Id == 1 && x.Location == first);
		first.SetPreparedCurrencyRoomMembership(item, true);
		Assert.IsTrue(owner.GameItems.Any(x => ReferenceEquals(x, item)) && shard.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.IsFalse(other.GameItems.Any(x => ReferenceEquals(x, item)) || otherShard.GameItems.Any(x => ReferenceEquals(x, item)));
		first.SetPreparedCurrencyRoomMembership(item, false);
		Assert.IsFalse(first.GameItems.Any(x => ReferenceEquals(x, item)) || owner.GameItems.Any(x => ReferenceEquals(x, item)) || shard.GameItems.Any(x => ReferenceEquals(x, item)));
		first.SetPreparedCurrencyRoomMembership(item, true);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Id).Returns(2); actor.SetupGet(x => x.Location).Returns(first);
		first.ReconcileNativeCharacterMembership(actor.Object, true);
		Assert.IsTrue(owner.Characters.Any(x => ReferenceEquals(x, actor.Object)) && shard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.IsFalse(other.Characters.Any(x => ReferenceEquals(x, actor.Object)) || otherShard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		owner.Changed = false; other.Changed = false;
		first.SetNewZone(other);
		Assert.IsFalse(owner.Rooms.Contains(first) || shard.Rooms.Contains(first));
		Assert.IsTrue(other.Rooms.Contains(first) && otherShard.Rooms.Contains(first));
		Assert.IsFalse(owner.GameItems.Any(x => ReferenceEquals(x, item)) || shard.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.IsFalse(owner.Characters.Any(x => ReferenceEquals(x, actor.Object)) || shard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.IsTrue(other.GameItems.Any(x => ReferenceEquals(x, item)) && otherShard.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.IsTrue(other.Characters.Any(x => ReferenceEquals(x, actor.Object)) && otherShard.Characters.Any(x => ReferenceEquals(x, actor.Object)));
		Assert.IsNull(owner.DefaultRoom, "The old persisted default must be cleared when its sole cell moves.");
		Assert.IsTrue(owner.Changed && other.Changed, "Both owners must save their current default selection.");
		Assert.AreEqual((17, -2, 4), first.StoredCoordinates);
		Assert.IsFalse(actor.Invocations.Any(x => x.Method.Name is "MoveTo" or "HandleEvent"), "Rezone/raw reconciliation invokes no movement callbacks.");
	}

	[TestMethod]
	public void DuplicateCoordinatesAndSimulation_HaveNoSiblingOwnerOrGlobalKeyRegistration()
	{
		using var fixture = new EnvironmentalMagicTestWorld(count: 2);
		var rooms = fixture.Rooms.OfType<Room>().ToArray();
		var (owner, other, shard, _) = Owners(fixture, rooms[0].Id);
		foreach (var room in rooms) { Attach(room, owner); room.SetCoordinates(17, -2, 4); }
		Assert.IsNull(shard.DetermineRoomByCoordinates(17, -2, 4), "Duplicate coordinates are allowed; targeting must refuse ambiguity.");
		Assert.IsTrue(rooms[0].TrySetUniqueName("Mirandola:Gate", out _));
		var simulation = new Room(rooms[0], -100);
		Assert.AreSame(owner, simulation.OwningZone);
		Assert.AreEqual(rooms[0].StoredCoordinates, simulation.StoredCoordinates);
		Assert.IsNull(simulation.UniqueName);
		Assert.IsFalse(owner.Rooms.Contains(simulation) || shard.Rooms.Contains(simulation));
		var item = Mock.Of<IGameItem>(x => x.Id == 10);
		simulation.SetPreparedCurrencyRoomMembership(item, true);
		Assert.IsTrue(simulation.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.IsFalse(owner.GameItems.Any(x => ReferenceEquals(x, item)) || shard.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.ThrowsException<InvalidOperationException>(() => simulation.SetNewZone(other));
	}

	private static void Attach(Room room, Zone zone)
	{
		typeof(Room).GetField("_owningZone", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(room, zone);
		zone.Register(room);
	}

	private static (Zone Owner, Zone Other, Shard Shard, Shard OtherShard) Owners(EnvironmentalMagicTestWorld fixture, long defaultRoom, bool sharedShard = false)
	{
		var shard = new Shard(new Db.Shard { Id = 101, Name = "Stored shard", SphericalRadiusMetres = 6371000 }, fixture.World.Object);
		var otherShard = sharedShard ? shard : new Shard(new Db.Shard { Id = 102, Name = "Exterior shard", SphericalRadiusMetres = 6371000 }, fixture.World.Object);
		fixture.World.SetupGet(x => x.Shards).Returns(sharedShard ? new All<IShard> { shard } : new All<IShard> { shard, otherShard });
		fixture.World.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController>());
		var owner = new Zone(new Db.Zone { Id = 201, Name = "Stored", ShardId = shard.Id, DefaultRoomId = defaultRoom == 0 ? null : defaultRoom }, fixture.World.Object);
		var other = new Zone(new Db.Zone { Id = 202, Name = "Exterior", ShardId = otherShard.Id }, fixture.World.Object);
		return (owner, other, shard, otherShard);
	}
}
