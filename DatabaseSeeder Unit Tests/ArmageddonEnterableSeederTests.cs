#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonEnterableSeederTests
{
	[TestMethod]
	public void ImportSelectionExcludesPlaygroundAndBrokenInteriorsButPreservesReusedObjects()
	{
		var archive = ArmageddonWorldfileTestData.Load();
		var selected = archive.ForInstallation();
		Assert.AreEqual(26051, selected.Rooms.Length);
		Assert.AreEqual(7299, selected.Items.Length);
		Assert.AreEqual(27661, selected.Resets.Length);
		Assert.AreEqual(79, selected.Zones.Length);
		Assert.AreEqual(2459, selected.Npcs.Length);
		Assert.IsFalse(selected.Rooms.Any(x => x.Zone == 57));
		Assert.IsFalse(selected.Resets.Any(x => x.Zone == 57));
		Assert.IsFalse(selected.Items.Any(x => x.Source.Path == "objects/z57.obj"));
		Assert.IsFalse(selected.Npcs.Any(x => x.Source.Path == "npcs/z57.npc"));
		Assert.IsFalse(selected.Items.Any(x => ArmageddonWorldfilePackage.ExcludedEnterables.Contains(x.Vnum)));
		Assert.IsFalse(selected.Rooms.Any(x => x.Zone == 76));
		Assert.IsFalse(selected.Resets.Any(x => x.Zone == 76));
		Assert.IsTrue(selected.Items.Any(x => x.Vnum == 76031));
		CollectionAssert.AreEquivalent(new[] { 76018, 76088, 76090 }, selected.Items
			.Where(x => x.Vnum is 76018 or 76088 or 76090).Select(x => x.Vnum).ToArray());
		Assert.IsFalse(selected.Items.Any(x => x.Vnum == 76999));
		Assert.AreEqual(45, selected.Items.Count(x => x.Source.Path == "objects/z76.obj"));
		Assert.IsTrue(selected.Resets.Any(x => x.Opcode == "O" && x.Arguments[1] == 76031));
		Assert.AreEqual(26137, archive.Rooms.Length);
		Assert.AreEqual(7415, archive.Items.Length);
	}

	[TestMethod]
	public void InteriorDiscoveryRejectsExternalIncomingLinksAndUnboundedWorldGraphs()
	{
		var room1 = Room(10, [new ArmWorldExit(0, 11, "", 0, -1, "")]);
		var room2 = Room(11, [new ArmWorldExit(2, 10, "", 0, -1, "")]);
		var source = new ArmageddonWorldfilePackage { Rooms = [room1, room2] };
		CollectionAssert.AreEqual(new[] { 10, 11 }, ArmageddonWorldfileWriter.FindInteriorGraph(source, 10)!);
		Assert.IsNull(ArmageddonWorldfileWriter.FindInteriorGraph(source, 0));
		Assert.IsNull(ArmageddonWorldfileWriter.FindInteriorGraph(new ArmageddonWorldfilePackage
		{ Rooms = [room1, room2, Room(12, [new ArmWorldExit(0, 10, "", 0, -1, "")])] }, 10));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		Assert.IsNull(ArmageddonWorldfileWriter.FindInteriorGraph(archive, 64991));
		Assert.AreEqual(4, ArmageddonWorldfileWriter.FindInteriorGraph(archive, 37991)!.Length);
		Assert.IsNull(ArmageddonWorldfileWriter.FindInteriorGraph(archive, 6066),
			"The sparring circle is reset inside its own referenced graph and must stay shared.");
	}

	[TestMethod]
	public void SharedArgosyAndUnusedAlcoveCannotRedirectPopulationIntoUncreatedInstances()
	{
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var shared = ArmageddonWorldfileWriter.FindSharedInteriorRoots(source);
		Assert.IsTrue(shared.Contains(13532), "The placed argosy references shared world geography.");
		Assert.IsTrue(shared.Contains(13533), "The unused alcove overlaps that geography and must also stay shared.");
		Assert.IsTrue(shared.Contains(28993), "The vineyard contains a fixed bank branch and retains shared geography.");
		Assert.IsTrue(source.Resets.Any(x => x.Opcode == "O" && x.Arguments[3] == 13525));
	}

	[TestMethod]
	public void VehicleWritesNativeCompartmentsControlsPropulsionAndProjectionWithTemplateIdentity()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = new ArmageddonWorldfilePackage { Rooms = [Room(10, [new ArmWorldExit(0, 11, "", 0, -1, "")]), Room(11, [new ArmWorldExit(2, 10, "", 0, -1, "")])] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var rooms = (Dictionary<int, MudSharp.Models.Room>)typeof(ArmageddonWorldfileWriter)
			.GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(writer)!;
		foreach (var vnum in new[] { 10, 11 })
		{
			var room = new MudSharp.Models.Room { Id = vnum, UniqueName = $"Vnum{vnum}", ZoneId = 1, EffectData = "<Effects/>" };
			room.RoomOverlays.Add(new RoomOverlay { Id = vnum, RoomId = vnum, RoomName = $"Interior {vnum}", RoomDescription = "Source prose.", TerrainId = 1 });
			rooms.Add(vnum, room);
		}
		var item = new GameItemProto { Id = 10000, UniqueName = "Vnum37950", Name = "skimmer", Keywords = "skimmer",
			ShortDescription = "a skimmer", FullDescription = "A skimmer.", LongDescription = "A skimmer is here.",
			MaterialId = 1, EditableItem = new EditableItem() };
		db.GameItemProtos.Add(item);
		var definition = new ArmWorldItem(new("objects/z37.obj", 1), 37950, -1, "skimmer", "a skimmer", "A skimmer is here.", "Source prose.",
			[23, 0, 0, 0, 3], [10, 2, 11, 2, 0, 0], [100, 0, 0], [0, 0, 0, 0], [], [], []);
		typeof(ArmageddonWorldfileWriter).GetMethod("AddEnterable", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(writer, [item, definition, definition.Values]);
		db.SaveChanges();
		var vehicle = db.VehicleProtos.Single();
		Assert.AreEqual((int)VehicleScale.RoomScale, vehicle.VehicleScale);
		Assert.AreEqual("Vnum37950", vehicle.Name);
		Assert.AreEqual(2, db.VehicleCompartmentProtos.Count());
		var driver = db.VehicleOccupantSlotProtos.Single();
		Assert.AreEqual(db.VehicleCompartmentProtos.Single(x => x.Name == "Interior 11").Id, driver.VehicleCompartmentProtoId);
		Assert.IsTrue(db.VehicleControlStationProtos.Single().IsPrimary);
		var access = db.VehicleAccessPointProtos.Single();
		Assert.IsTrue(db.GameItemProtos.Single(x => x.Id == access.ProjectionItemProtoId).IsHiddenFromPlayers);
		Assert.AreEqual(1, db.VehicleCompartmentLinkProtos.Count());
		Assert.IsFalse(HitchGearRules.TowPointRequiresHitchItem(new VehicleTowPointPrototype(db.VehicleTowPointProtos.Single(), [])),
			"Source hitching must not acquire an unseeded hitch-gear requirement.");
		Assert.IsTrue(db.VehicleMovementProfileProtos.Any(x => x.MovementEnvironment == (int)VehicleMovementEnvironment.Silt && x.IsDefault));
		Assert.IsTrue(db.VehiclePropulsionProfileProtos.Any(x => x.PropulsionType == (int)VehiclePropulsionType.ExternallyPulled));
		var exterior = db.GameItemComponentProtos.Single(x => x.Type == "Vehicle Exterior");
		var templates = XElement.Parse(exterior.Definition).Element("InteriorRoomTemplates")!.Elements("Room").ToArray();
		CollectionAssert.AreEquivalent(new long[] { 10, 11 }, templates.Select(x => (long)x.Attribute("template")!).ToArray());
	}

	private static ArmWorldRoom Room(int vnum, ArmWorldExit[] exits) => new(new("rooms/z0.wld", 1), 0, $"Room {vnum}", vnum,
		0, 0, 0, exits, [], [], 0, [], "Room prose.");
}
