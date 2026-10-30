#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Vehicles;
using MudSharp.Planes;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EmptyVehicleRelocationTests
{
	[TestMethod]
	public void Occupancy_UnassignedInteriorPassengerBlocksButCargoDoesNot()
	{
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		room.SetupGet(x => x.GameItems).Returns([Mock.Of<IGameItem>()]);
		var compartment = Mock.Of<IVehicleCompartment>(x => x.InteriorRoom == room.Object);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Occupancies).Returns(Array.Empty<IVehicleOccupancy>());
		vehicle.SetupGet(x => x.Occupants).Returns(Array.Empty<ICharacter>());
		vehicle.SetupGet(x => x.Compartments).Returns([compartment]);
		Assert.IsFalse(EmptyVehicleRelocation.HasLoadedPassengers(vehicle.Object));
		room.SetupGet(x => x.Characters).Returns([Mock.Of<ICharacter>()]);
		Assert.IsTrue(EmptyVehicleRelocation.HasLoadedPassengers(vehicle.Object));
	}

	[TestMethod]
	public void Occupancy_AssignedSlotBlocksEvenWithoutResolvedCharacter()
	{
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Occupancies).Returns([Mock.Of<IVehicleOccupancy>()]);
		Assert.IsTrue(EmptyVehicleRelocation.HasLoadedPassengers(vehicle.Object));
	}

	[TestMethod]
	public void OfflineResidents_UnknownInstanceBlocksAndLoadedLocationOverridesStaleRow()
	{
		var outside = Mock.Of<IRoom>(x => x.Id == 20);
		var inside = Mock.Of<IRoom>(x => x.Id == 10);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.InstanceId).Returns(3);
		actor.SetupGet(x => x.Location).Returns(outside);
		Assert.IsTrue(EmptyVehicleRelocation.PersistedPassengersRemain([10], [(3, 1)], [], []));
		Assert.IsFalse(EmptyVehicleRelocation.PersistedPassengersRemain([10], [(3, 1)], [], [actor.Object]));
		actor.SetupGet(x => x.Location).Returns(inside);
		Assert.IsTrue(EmptyVehicleRelocation.PersistedPassengersRemain([10], [(3, 1)], [], [actor.Object]));
		actor.SetupGet(x => x.Location).Returns(outside);
		Assert.IsTrue(EmptyVehicleRelocation.PersistedPassengersRemain([10], [(4, 1)], [], [actor.Object]),
			"A loaded primary must not hide an offline secondary instance in the wagon.");
	}

	[TestMethod]
	public void LegacyOfflineResident_RemainsOccupiedWithoutLoadedIdentity()
	{
		Assert.IsTrue(EmptyVehicleRelocation.PersistedPassengersRemain([10], [], [7], []));
		Assert.IsFalse(EmptyVehicleRelocation.PersistedPassengersRemain([10], [], [], []));
	}

	[TestMethod]
	public void UnhitchCharacters_ReleasesNativeEffectsAndLeavesCharactersOutside()
	{
		var actors = new All<ICharacter>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(1);
		actors.Add(actor.Object);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Actors).Returns(actors);
		world.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
		world.SetupGet(x => x.VehicleHitchLinks).Returns(new All<IVehicleHitchLink>());
		var vehicle = Mock.Of<IVehicle>(x => x.Gameworld == world.Object && x.ExteriorItem == Mock.Of<IGameItem>());
		EmptyVehicleRelocation.UnhitchCharacters(vehicle, new VehicleHitchService());
		actor.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<Dragging>>(), true), Times.Once);
		actor.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<CharacterHitch>>(), true), Times.Once);
		Assert.IsFalse(actor.Invocations.Any(x => x.Method.Name is "Teleport" or "MoveTo" or "Moved"));
	}

	[TestMethod]
	public void LoadedInstances_SharedIdentityDoesNotCollapseDistinctBodiesDuringUnhitch()
	{
		var first = new Mock<ICharacter>();
		var second = new Mock<ICharacter>();
		foreach (var actor in new[] { first, second })
		{
			actor.SetupGet(x => x.Id).Returns(1);
			actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns(true);
		}
		var firstInstance = first.As<ICharacterInstance>();
		var secondInstance = second.As<ICharacterInstance>();
		first.SetupGet(x => x.InstanceId).Returns(11);
		second.SetupGet(x => x.InstanceId).Returns(12);
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.Instances).Returns([firstInstance.Object, secondInstance.Object]);
		first.SetupGet(x => x.Identity).Returns(identity.Object);
		var actors = new All<ICharacter>();
		actors.Add(first.Object);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Actors).Returns(actors);
		world.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
		world.SetupGet(x => x.VehicleHitchLinks).Returns(new All<IVehicleHitchLink>());
		var vehicle = Mock.Of<IVehicle>(x => x.Gameworld == world.Object && x.ExteriorItem == Mock.Of<IGameItem>());
		EmptyVehicleRelocation.UnhitchCharacters(vehicle, new VehicleHitchService());
		first.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<CharacterHitch>>(), true), Times.Once);
		second.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<CharacterHitch>>(), true), Times.Once);
	}

	[TestMethod]
	public void NativeCommit_MovesSameExteriorAndCanonicalVehicleAndPreservesInteriorCargo()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var origin = NativeRoom(10, world.Object);
		var destination = NativeRoom(20, world.Object);
		var rooms = new Mock<IUneditableAll<IRoom>>();
		rooms.Setup(x => x.Get(10)).Returns(origin);
		rooms.Setup(x => x.Get(20)).Returns(destination);
		world.SetupGet(x => x.Rooms).Returns(rooms.Object);
		var prototype = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		prototype.SetupGet(x => x.Gameworld).Returns(world.Object);
		prototype.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		prototype.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponentProto>());
		var exterior = new GameItem(prototype.Object);
		exterior.SetPreparedVehicleLocation(new SpatialLocation(origin, RoomLayer.GroundLevel, null));
		origin.SetPreparedCurrencyRoomMembership(exterior, true);
		var vehicle = new Vehicle(new Db.Vehicle { Id = 77, CurrentRoomId = 10,
			LocationType = (int)VehicleLocationType.Room, MovementStatus = (int)VehicleMovementStatus.Stationary }, world.Object);
		Set(vehicle, "_exteriorItem", exterior);
		var cargo = Mock.Of<IGameItem>();
		var interior = Mock.Of<IRoom>(x => x.GameItems == new[] { cargo });
		var compartment = Mock.Of<IVehicleCompartment>(x => x.InteriorRoom == interior);
		((List<IVehicleCompartment>)Get(vehicle, "_compartments")).Add(compartment);
		vehicle.CommitPreparedRelocation(destination, RoomLayer.GroundLevel);
		Assert.AreSame(destination, vehicle.Location);
		Assert.AreSame(destination, exterior.DirectLocation);
		Assert.IsFalse(origin.GameItems.Contains(exterior));
		CollectionAssert.AreEqual(new IGameItem[] { exterior }, destination.GameItems.ToArray());
		Assert.AreSame(exterior, vehicle.ExteriorItem);
		Assert.AreSame(interior, vehicle.Compartments.Single().InteriorRoom);
		Assert.AreSame(cargo, interior.GameItems.Single());
		Assert.AreEqual(VehicleMovementStatus.Stationary, vehicle.MovementState.MovementStatus);
		vehicle.CommitPreparedRelocation(origin, RoomLayer.GroundLevel);
		Assert.AreSame(origin, vehicle.Location);
		CollectionAssert.AreEqual(new IGameItem[] { exterior }, origin.GameItems.ToArray());
		Assert.IsFalse(destination.GameItems.Any());
	}

	private static Room NativeRoom(long id, IFuturemud world)
	{
		var room = (Room)RuntimeHelpers.GetUninitializedObject(typeof(Room));
		Set(room, "_id", id);
		Set(room, "_gameItems", new List<IGameItem>());
		Set(room, "_isCombatSimulationRoom", true);
		typeof(Room).GetProperty(nameof(Room.Gameworld))!.SetValue(room, world);
		return room;
	}
	private static object Get(object target, string name)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) is { } field)
				return field.GetValue(target)!;
		throw new InvalidOperationException(name);
	}
	private static void Set(object target, string name, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) is { } field)
			{
				field.SetValue(target, value);
				return;
			}
		throw new InvalidOperationException(name);
	}
}
