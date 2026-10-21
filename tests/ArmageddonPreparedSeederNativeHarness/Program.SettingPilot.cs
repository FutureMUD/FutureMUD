#nullable enable

// Native combined Setting driving qualification.
// Uses the existing TestDatabase, HarnessClock, Require, SetPrivateMember and
// NewIndependentContext helpers. All database/runtime mutations below are for
// the harness-owned disposable restored database, never an existing world.
//
// Integration:
// 1. After combined Setting installation, before LoadProductionRestartWorld,
//    allocate TWO empty disposable rooms with current overlays, then call Seed... .
//    Pass an explicitly selected Core solid material ID; no Useful/Item data needed.
// 2. Boot the real production world. Call LoadArmageddonPilotCatalogue ONCE.
//    Materialise the real Character through the combined harness's normal path.
// 3. RunArmageddonNativePilot(world, clock, ids, realDriver, roomScale:false).
//    Optionally run roomScale:true with a SECOND real driver initially in Origin.
//    Driver must already own Pilot (approved role), effective Pilot >=60 and an
//    appropriate cap. This draft does not qualify chargen grants by adding Pilot.
// 4. Serialize the receipt into the harness's existing child-process input.
//    In a separate process boot with LoadProductionRestartWorld, materialise the
//    same real Character, call LoadArmageddonPilotCatalogue ONCE, then
//    LoadArmageddonPilotInstances ONCE and AssertArmageddonNativePilotReload.
//    Do not call LoadVehicle* a second time if the production helper gains them.
//
// Evidence limits: authored native obstruction prog, NOT real storm/weather;
// supported legacy room-exit propulsion compatibility, NOT animal/hitch/fuel
// qualification; explicit native Board/TakeControl setup, actual "drive" command
// for acceptance. Room-scale exercises hosted interior + exterior movement.

using System.Globalization;
using System.Text;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.Vehicles;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record ArmPilotFixture(long Origin, long Destination, long Exit,
		long ItemPrototype, long RoomPrototype, long ObscurationProg);

	private sealed record ArmPilotReceipt(long Vehicle, long Exterior, long Prototype,
		long Driver, long Instance, long Body, long Destination, long DriverRoom,
		long? HostedInterior, bool RoomScale);

	private static ArmPilotFixture SeedArmageddonNativePilotFixture(TestDatabase database,
		long originId, long destinationId, long coreSolidMaterialId)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		Require(originId != destinationId, "Pilot fixture needs two distinct disposable rooms.");
		var origin = db.Rooms.Single(x => x.Id == originId);
		var destination = db.Rooms.Single(x => x.Id == destinationId);
		Require(origin.CurrentOverlayId.HasValue && destination.CurrentOverlayId.HasValue,
			"Pilot rooms must have native current overlays before seeding.");
		Require(!db.Exits.Any(x => x.RoomId1 == originId || x.RoomId2 == originId ||
			x.RoomId1 == destinationId || x.RoomId2 == destinationId),
			"Pilot fixture requires isolated disposable rooms, not removal of existing exits.");
		Require(db.Materials.Any(x => x.Id == coreSolidMaterialId), "Explicit Core material is missing.");
		var account = db.Accounts.OrderBy(x => x.Id).Select(x => x.Id).First();
		var originOverlay = db.RoomOverlays.Single(x => x.Id == origin.CurrentOverlayId);
		var destinationOverlay = db.RoomOverlays.Single(x => x.Id == destination.CurrentOverlayId);
		// Fixture visibility is independent of calendar/weather lighting.
		originOverlay.AddedLight = destinationOverlay.AddedLight = 1000;
		var exit = new Db.Exit {
			RoomId1 = originId, RoomId2 = destinationId,
			Direction1 = (int)CardinalDirection.North, Direction2 = (int)CardinalDirection.South,
			TimeMultiplier = 1, MaximumSizeToEnter = (int)SizeCategory.Titanic,
			MaximumSizeToEnterUpright = (int)SizeCategory.Titanic, BlockedLayers = "" };
		db.Exits.Add(exit); db.SaveChanges();
		db.RoomOverlaysExits.AddRange(
			new Db.RoomOverlayExit { RoomOverlayId = originOverlay.Id, ExitId = exit.Id },
			new Db.RoomOverlayExit { RoomOverlayId = destinationOverlay.Id, ExitId = exit.Id });
		var prog = new Db.FutureProg {
			FunctionName = "armHarnessPilotObscuration", Category = "Harness", Subcategory = "Pilot",
			FunctionComment = "Authored disposable Pilot acceptance obstruction; no weather claim.",
			FunctionText = $"return @room.id == {destinationId.ToString(CultureInfo.InvariantCulture)}",
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
			StaticType = (int)FutureProgStaticType.NotStatic };
		prog.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "actor",
			ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		prog.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "room",
			ParameterTypeDefinition = ProgVariableTypes.Location.ToStorageString() });
		db.FutureProgs.Add(prog); db.SaveChanges();
		// Preserve enable flags: acceptance must fail if the actual installation left them off.
		var obstructionSetting = db.StaticConfigurations.SingleOrDefault(x => x.SettingName == "DirectionSenseObscurationProg");
		if (obstructionSetting is null)
		{
			obstructionSetting = new Db.StaticConfiguration { SettingName = "DirectionSenseObscurationProg" };
			db.StaticConfigurations.Add(obstructionSetting);
		}
		obstructionSetting.Definition = prog.Id.ToString(CultureInfo.InvariantCulture);

		Db.EditableItem Revision() => new() { RevisionNumber = 0, BuilderAccountId = account,
			BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current };
		long Prototype(VehicleScale scale)
		{
			var label = scale == VehicleScale.RoomScale ? "hosted cart" : "cart";
			var proto = new Db.VehicleProto {
				Id = (db.VehicleProtos.Select(x => (long?)x.Id).Max() ?? 0) + 1,
				Name = "ARMHarness Pilot " + label, Description = "Disposable native Pilot acceptance fixture.",
				VehicleScale = (int)scale, EditableItem = Revision() };
			db.VehicleProtos.Add(proto); db.SaveChanges();
			var compartment = new Db.VehicleCompartmentProto { VehicleProtoId = proto.Id,
				Name = "driver deck", Description = "A native acceptance deck.",
				InteriorTerrainId = originOverlay.TerrainId, InteriorOutdoorsType = (int)RoomOutdoorsType.Indoors };
			db.VehicleCompartmentProtos.Add(compartment); db.SaveChanges();
			var slot = new Db.VehicleOccupantSlotProto { VehicleProtoId = proto.Id,
				VehicleCompartmentProtoId = compartment.Id, Name = "driver",
				SlotType = (int)VehicleOccupantSlotType.Driver, Capacity = 1 };
			db.VehicleOccupantSlotProtos.Add(slot); db.SaveChanges();
			db.VehicleControlStationProtos.Add(new() { VehicleProtoId = proto.Id,
				VehicleOccupantSlotProtoId = slot.Id, Name = "driver station", IsPrimary = true });
			db.VehicleMovementProfileProtos.Add(new() { VehicleProtoId = proto.Id,
				Name = "ordinary room-exit", MovementType = (int)VehicleMovementProfileType.RoomExit,
				IsDefault = true, RequiredInstalledRole = "" });
			Db.VehicleAccessPointProto? access = null;
			if (scale == VehicleScale.RoomScale)
			{
				access = new() { VehicleProtoId = proto.Id,
					VehicleCompartmentProtoId = compartment.Id, Name = "open boarding ramp",
					Description = "A fixture ramp.", AccessPointType = (int)VehicleAccessPointType.Ramp,
					StartsOpen = true, MustBeClosedForMovement = false };
				db.VehicleAccessPointProtos.Add(access); db.SaveChanges();
			}
			var component = new Db.GameItemComponentProto {
				Id = (db.GameItemComponentProtos.Select(x => (long?)x.Id).Max() ?? 0) + 1,
				Name = "ARMHarness Pilot " + label + " exterior", Type = "Vehicle Exterior",
				Description = "Native fixture exterior.", EditableItem = Revision(),
				Definition = $"<Definition><VehiclePrototypeId>{proto.Id}</VehiclePrototypeId></Definition>" };
			db.GameItemComponentProtos.Add(component); db.SaveChanges();
			var item = new Db.GameItemProto {
				Id = (db.GameItemProtos.Select(x => (long?)x.Id).Max() ?? 0) + 1,
				Name = "ARMHarness Pilot " + label, Keywords = "acceptance cart",
				ShortDescription = "an acceptance " + label, FullDescription = "A disposable native cart.",
				MaterialId = coreSolidMaterialId, Size = (int)SizeCategory.Small, Weight = 1,
				BaseItemQuality = (int)ItemQuality.Standard, EditableItem = Revision() };
			item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id });
			db.GameItemProtos.Add(item); db.SaveChanges();
			proto.ExteriorItemProtoId = item.Id; proto.ExteriorItemProtoRevision = 0; db.SaveChanges();
			if (access is not null)
			{
				// Factory requires an actual access projection even with a native docking.
				var accessComponent = new Db.GameItemComponentProto {
					Id = (db.GameItemComponentProtos.Select(x => (long?)x.Id).Max() ?? 0) + 1,
					Name = "ARMHarness Pilot ramp component", Type = "Vehicle Access Point",
					Description = "Native ramp projection.", EditableItem = Revision(),
					Definition = $"<Definition><VehiclePrototypeId>{proto.Id}</VehiclePrototypeId><AccessPointPrototypeId>{access.Id}</AccessPointPrototypeId></Definition>" };
				db.GameItemComponentProtos.Add(accessComponent); db.SaveChanges();
				var accessItem = new Db.GameItemProto {
					Id = (db.GameItemProtos.Select(x => (long?)x.Id).Max() ?? 0) + 1,
					Name = "ARMHarness Pilot ramp", Keywords = "acceptance ramp",
					ShortDescription = "an acceptance boarding ramp", FullDescription = "A disposable native boarding ramp.",
					MaterialId = coreSolidMaterialId, Size = (int)SizeCategory.Small, Weight = 1,
					BaseItemQuality = (int)ItemQuality.Standard, EditableItem = Revision() };
				accessItem.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = accessComponent.Id });
				db.GameItemProtos.Add(accessItem); db.SaveChanges();
				access.ProjectionItemProtoId = accessItem.Id; access.ProjectionItemProtoRevision = 0; db.SaveChanges();
			}
			return proto.Id;
		}
		var itemProto = Prototype(VehicleScale.ItemScale);
		var roomProto = Prototype(VehicleScale.RoomScale);
		db.SaveChanges();
		return new(originId, destinationId, exit.Id, itemProto, roomProto, prog.Id);
	}

	// ProductionRestart currently omits these loader calls. No mock catalogue setup.
	private static void LoadArmageddonPilotCatalogue(Futuremud world)
	{
		Require(!world.VehiclePrototypes.Any(), "Vehicle prototypes already loaded; integrate loader exactly once.");
		((IFuturemudLoader)world).LoadVehiclePrototypes();
	}

	private static void LoadArmageddonPilotInstances(Futuremud world)
	{
		Require(!world.Vehicles.Any(), "Vehicles already loaded; integrate loader exactly once.");
		var loader = (IFuturemudLoader)world;
		loader.LoadVehicleRoutes(); loader.LoadVehicles(); loader.LoadVehicleHitchLinks(); loader.LoadVehicleOperations();
	}

	private static ArmPilotReceipt RunArmageddonNativePilot(Futuremud world, HarnessClock clock,
		ArmPilotFixture fixture, ICharacter driver, bool roomScale)
	{
		using var runtimeTime = RuntimeClock.Push(clock);
		Require(driver is Character && ReferenceEquals(driver.Gameworld, world), "Acceptance requires a native production Character.");
		Require(world.GetStaticBool(VehiclePilot.EnabledSetting) && world.GetStaticBool("DirectionSenseEnabled"),
			"Installed Pilot and Direction Sense flags must be enabled; do not repair flags in acceptance.");
		Require(driver.Location.Id == fixture.Origin && driver.Movement is null && driver.Combat is null,
			"Provide a fresh, free, mobile real driver in the fixture origin.");
		var pilot = world.Traits.Get(world.GetStaticLong(VehiclePilot.TraitSetting));
		Require(pilot is ISkillDefinition && driver.HasTrait(pilot) && driver.TraitValue(pilot) >= 60,
			"Provide an approved-role Pilot owner with effective proficiency >=60 for deterministic 1d60 success.");
		var direction = world.GetCheck(MudSharp.RPG.Checks.CheckType.DirectionSenseCheck)
			.TargetNumberExpression.Parameters.Values.Single().Trait;
		Require(direction.Id != pilot.Id, "Pilot must remain separate from navigation.");
		var origin = world.Rooms.Get(fixture.Origin); var destination = world.Rooms.Get(fixture.Destination);
		var obstruction = world.FutureProgs.Get(fixture.ObscurationProg);
		Require(obstruction is not null && obstruction.ExecuteBool(driver, origin) == false &&
			obstruction.ExecuteBool(driver, destination) == true, "Native fixture obstruction must compile and select only the destination.");
		Require(!DirectionSenseMovement.IsActive(driver, origin), "Success origin unexpectedly obscured.");
		var proto = world.VehiclePrototypes.Get(roomScale ? fixture.RoomPrototype : fixture.ItemPrototype, 0);
		Require(proto is not null && proto.CanCreateVehicle(out _), "Native fixture vehicle validation failed.");
		var vehicle = VehicleFactory.CreateVehicle(proto!, origin, driver.RoomLayer, driver);
		Require(!roomScale || vehicle.Dockings.Any(x => x.State == VehicleDockingState.BoardingOpen),
			"Native room-scale factory must create open dockings before Board.");
		var slot = vehicle.Prototype.OccupantSlots.Single(x => x.SlotType == VehicleOccupantSlotType.Driver);
		Require(vehicle.CanBoard(driver, slot, out var boardReason), "Native boarding refused: " + boardReason);
		Require(vehicle.Board(driver, slot), "Native Board failed after permission.");
		Require(vehicle.TakeControl(driver) || ReferenceEquals(vehicle.Controller, driver), "Native driver control failed.");
		var hosted = roomScale ? vehicle.Compartments.Single().InteriorRoomId : null;
		Require(!roomScale || hosted.HasValue && driver.Location.Id == hosted.Value, "Room-scale Board did not place driver in hosted interior.");
		var north = origin.GetExit(CardinalDirection.North, vehicle.ExteriorItem);
		Require(north is not null && vehicle.CanMove(driver, north, out _) && driver.Body.CanSee(origin, north),
			"Native cart must be move-ready with a visible north exit before exercising Pilot.");

		var oldOutput = driver.OutputHandler; var oldTree = driver.CommandTree;
		var output = new StringBuilder(); driver.Register(new PlayerOutputHandler(output, driver));
		SetPrivateMember(driver, "CommandTree", PlayerCommandTree.Instance);
		var pilotRaw = driver.TraitRawValue(pilot!);
		var hadDirection = driver.HasTrait(direction); var directionRaw = driver.TraitRawValue(direction);
		try
		{
			driver.ExecuteCommand("drive north");
			Require(driver.Movement is not null, "Actual drive command did not start: " + output);
			for (var i = 0; i < 240 && driver.Movement is not null; i++)
			{
				clock.Advance(TimeSpan.FromSeconds(1)); world.Scheduler.CheckSchedules(); world.EffectScheduler.CheckSchedules();
			}
			Require(driver.Movement is null && vehicle.Location.Id == destination.Id &&
				vehicle.ExteriorItem.Location.Id == destination.Id &&
				vehicle.MovementState.MovementStatus == VehicleMovementStatus.Stationary,
				"Native scheduled drive did not complete at exterior destination: " + output);
			Require(driver.Location.Id == (hosted ?? destination.Id) && ReferenceEquals(vehicle.Controller, driver),
				"Completed drive lost real driver/control or moved hosted interior occupant outside.");

			var south = destination.GetExit(CardinalDirection.South, vehicle.ExteriorItem);
			Require(south is not null && vehicle.CanMove(driver, south, out _), "Return leg must be natively ready.");
			Require(driver.RemoveTrait(pilot!), "Could not remove owned Pilot for negative control.");
			output.Clear(); driver.ExecuteCommand("drive south");
			Require(output.ToString().Contains(VehiclePilot.NoProgressMessage, StringComparison.Ordinal) &&
				driver.Movement is null && vehicle.Location.Id == destination.Id && !driver.HasTrait(pilot!),
				"Missing Pilot did not refuse without movement or branch grant: " + output);
			Require(driver.AddTrait(pilot!, pilotRaw) && driver.TraitValue(pilot!) >= 60,
				"Could not restore original Pilot owner/proficiency.");
			if (hadDirection) Require(driver.RemoveTrait(direction), "Could not remove Direction Sense.");
			Require(DirectionSenseMovement.IsActive(driver, destination), "Return leg must actually be obscured.");
			Require(destination.ExitsFor(vehicle.ExteriorItem).Count(x => DirectionSenseMovement.IsCompassDirection(x.OutboundDirection)) == 1,
				"Obscured refusal requires only the intended compass exit (no safe wrong turn).");
			output.Clear(); driver.ExecuteCommand("drive south");
			Require(output.ToString().Contains(DirectionSenseMovement.NoProgressMessage, StringComparison.Ordinal) &&
				driver.Movement is null && vehicle.Location.Id == destination.Id && !driver.HasTrait(direction),
				"Obscured missing navigation did not refuse independently of restored Pilot: " + output);
			Console.WriteLine($"ARM-PILOT-{(roomScale ? "room" : "item")}=passed native-factory/control/drive missing-Pilot-refused obscured-DirectionSense-refused");
		}
		finally
		{
			if (!driver.HasTrait(pilot!)) driver.AddTrait(pilot!, pilotRaw); else driver.SetTraitValue(pilot!, pilotRaw);
			if (hadDirection && !driver.HasTrait(direction)) driver.AddTrait(direction, directionRaw);
			else if (hadDirection) driver.SetTraitValue(direction, directionRaw);
			driver.Register(oldOutput); SetPrivateMember(driver, "CommandTree", oldTree);
		}
		// Expire the real command delay before the cold checkpoint; no real-time sleep.
		var delay = world.GetStaticDouble("DirectionSenseNoProgressDelaySeconds");
		Require(double.IsFinite(delay) && delay >= 0 && delay <= 3600, "Unexpected fixture navigation delay.");
		clock.Advance(TimeSpan.FromSeconds(delay + 1)); world.EffectScheduler.CheckSchedules(); world.SaveManager.Flush();
		return new(vehicle.Id, vehicle.ExteriorItem.Id, vehicle.Prototype.Id, driver.Id,
			driver.InstanceId, driver.Body.Id, destination.Id, driver.Location.Id, hosted, roomScale);
	}

	private static void AssertArmageddonNativePilotReload(Futuremud world, ICharacter driver, ArmPilotReceipt receipt)
	{
		Require(driver is Character && driver.Id == receipt.Driver && driver.InstanceId == receipt.Instance &&
			driver.Body.Id == receipt.Body && driver.Location.Id == receipt.DriverRoom, "Native driver checkpoint did not reload.");
		var vehicle = world.Vehicles.Get(receipt.Vehicle);
		Require(vehicle is not null && vehicle.Prototype.Id == receipt.Prototype && vehicle.ExteriorItem.Id == receipt.Exterior &&
			vehicle.Location.Id == receipt.Destination && vehicle.ExteriorItem.Location.Id == receipt.Destination &&
			vehicle.MovementState.MovementStatus == VehicleMovementStatus.Stationary &&
			vehicle.Controller?.SamePhysicalInstance(driver) == true && vehicle.IsOccupant(driver),
			"Production vehicle loaders did not preserve completed drive, canonical driver/control and exterior.");
		Require(!receipt.RoomScale || vehicle!.Compartments.Single().InteriorRoomId == receipt.HostedInterior &&
			driver.Location.Id == receipt.HostedInterior, "Cold room-scale checkpoint changed hosted interior identity.");
		Console.WriteLine("ARM-PILOT-cold=passed production-loaders real-character vehicle/exterior/controller/occupancy/hosted-room preserved");
	}
}
