#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleBarrierExposureTests
{
	[TestMethod]
	public void PrototypeWithoutExposureSetting_DefaultsToSheltered()
	{
		Assert.IsFalse(new DB.VehicleProto().ExposesOccupantsToExitBarriers);
		Assert.IsFalse(CreatePrototype().ExposesOccupantsToExitBarriers);
	}

	[DataTestMethod]
	[DataRow("exposed", true)]
	[DataRow("sheltered", false)]
	public void BuilderExposure_SaveAndRevisionCopyRoundTripRetainsExplicitPolicy(string text, bool expected)
	{
		var prototype = CreatePrototype(!expected);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		Assert.IsTrue(prototype.BuildingCommand(actor.Object, new StringStack($"barrierexposure {text}")));
		Assert.AreEqual(expected, prototype.ExposesOccupantsToExitBarriers);

		var saved = Row();
		prototype.SaveDefinition(saved);
		var loaded = new VehiclePrototype(saved, prototype.Gameworld);
		Assert.AreEqual(expected, loaded.ExposesOccupantsToExitBarriers);
		var newRevision = Row();
		newRevision.RevisionNumber = 2;
		loaded.SaveDefinition(newRevision); // The same definition writer used by CreateNewRevision.
		Assert.AreEqual(expected, new VehiclePrototype(newRevision, prototype.Gameworld).ExposesOccupantsToExitBarriers);
		Assert.AreEqual(2, newRevision.RevisionNumber);
	}

	[TestMethod]
	public void BuilderExposure_InvalidOrTrailingArgumentDoesNotChangePolicy()
	{
		var prototype = CreatePrototype();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		Assert.IsFalse(prototype.BuildingCommand(actor.Object, new StringStack("barrierexposure water")));
		Assert.IsFalse(prototype.BuildingCommand(actor.Object, new StringStack("barrierexposure exposed extra")));
		Assert.IsFalse(prototype.ExposesOccupantsToExitBarriers);
	}

	[TestMethod]
	public void ExposedOccupants_SelectsEnabledTrainMembersAndProtectsHostedInterior()
	{
		var origin = new Mock<IRoom>().Object;
		var interior = new Mock<IRoom>().Object;
		var exposed = Occupant(origin);
		var sheltered = Occupant(origin);
		var hosted = Occupant(interior);
		var compartment = new Mock<IVehicleCompartment>();
		compartment.SetupGet(x => x.InteriorRoom).Returns(interior);
		var root = Vehicle(1L, origin, true, [exposed, hosted]);
		root.SetupGet(x => x.Compartments).Returns([compartment.Object]);
		var trailer = Vehicle(2L, origin, false, [sheltered]);
		var plan = new VehicleHitchGraphMovePlan(root.Object,
			[new VehicleHitchGraphTrainMember(root.Object, 0, null), new VehicleHitchGraphTrainMember(trailer.Object, 1, null)], [], [], 0.0);

		CollectionAssert.AreEqual(new[] { exposed }, VehicleExitBarrierTraversal.ExposedOccupants(plan).ToArray());
	}

	private static ICharacter Occupant(IRoom location)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(location);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		return actor.Object;
	}

	private static Mock<IVehicle> Vehicle(long id, IRoom location, bool exposed, ICharacter[] occupants)
	{
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.ExposesOccupantsToExitBarriers).Returns(exposed);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(id);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Location).Returns(location);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.Occupants).Returns(occupants);
		return vehicle;
	}

	private static VehiclePrototype CreatePrototype(bool exposed = false)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		var row = Row();
		row.ExposesOccupantsToExitBarriers = exposed;
		return new VehiclePrototype(row, world.Object);
	}

	private static DB.VehicleProto Row() => new()
	{
		Id = 10L, RevisionNumber = 1, Name = "Test wagon", Description = "A test wagon.",
		EditableItem = new DB.EditableItem(), VehicleScale = (int)VehicleScale.RoomContainer
	};
}
