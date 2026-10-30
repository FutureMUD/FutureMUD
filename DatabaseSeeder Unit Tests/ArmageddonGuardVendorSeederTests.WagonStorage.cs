#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void WagonStorage_UsesSourceAttendantsIsolatedYardsAndExactBearerTicketRedemption()
	{
		using var db = WagonStorageFixture(out var writer, out var source, out var frontageIds);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallWagonStorage", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(writer, null);
		typeof(ArmageddonWorldfileWriter).GetMethod("FinalizeAuxiliaryRooms", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		foreach (var room in db.Rooms.Local.Where(x => x.UniqueName.StartsWith("armw-wagon-depot", StringComparison.Ordinal)))
			Assert.AreEqual(room.RoomOverlays.Single().Id, room.CurrentOverlayId);
		db.SaveChanges();

		var record = db.SeederManagedRecords.Single(x => x.StableKey == "npc-service.wagon-storage");
		var provenance = JsonDocument.Parse(record.SeedBaseline!).RootElement;
		CollectionAssert.AreEquivalent(new[] { 3842, 20119, 50455, 50605 },
			provenance.GetProperty("Sources").EnumerateArray().Select(x => x.GetInt32()).ToArray());
		Assert.AreEqual(1597, provenance.GetProperty("TicketPrototype").GetInt32());
		Assert.AreEqual(50, provenance.GetProperty("RedemptionBits").GetInt32());
		var yards = db.Rooms.Where(x => x.UniqueName.StartsWith("armw-wagon-depot.", StringComparison.Ordinal)).ToArray();
		Assert.AreEqual(frontageIds.Count, yards.Length, "Each retained source frontage gets one generated depot room.");
		Assert.AreEqual(yards.Length, yards.Select(x => x.UniqueName).Distinct(StringComparer.Ordinal).Count());
		foreach (var roomId in frontageIds)
		{
			var frontageVnum = source.Rooms.Single(x => 100000L + x.Vnum == roomId).Vnum;
			Assert.IsTrue(yards.Any(x => x.UniqueName.Contains(frontageVnum.ToString(), StringComparison.Ordinal)),
				$"Generated depot unique names include frontage Vnum {frontageVnum}.");
		}
		Assert.IsFalse(db.Exits.AsEnumerable().Any(x => yards.Any(y => x.RoomId1 == y.Id || x.RoomId2 == y.Id)), "Storage depots remain unconnected ordinary rooms.");

		var owner = ProgVariableTypes.Item.ToStorageString();
		foreach (var register in new[] { "armw_wagon_depot", "armw_wagon_id", "armw_wagon_ticket" })
		{
			Assert.AreEqual(1, db.VariableDefinitions.Count(x => x.OwnerTypeDefinition == owner && x.Property == register));
			Assert.AreEqual(ProgVariableTypes.Number.ToStorageString(), db.VariableDefinitions.Single(x => x.OwnerTypeDefinition == owner && x.Property == register).ContainedTypeDefinition);
			Assert.AreEqual("<var>0</var>", db.VariableDefaults.Single(x => x.OwnerTypeDefinition == owner && x.Property == register).DefaultValue);
		}

		Assert.AreEqual(8, db.Hooks.Count(x => x.Name.StartsWith("Worldfile Wagon Storage ", StringComparison.Ordinal)));
		foreach (var vnum in new[] { 3842, 20119, 50455, 50605 })
		{
			foreach (var command in new[] { "rent", "offer" })
			{
				var label = command == "rent" ? "Rent" : "Offer";
				var hook = db.Hooks.Single(x => x.Name == $"Worldfile Wagon Storage {label} {vnum}");
				CollectionAssert.AreEquivalent(Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds")[vnum], TemplatesWithHook(db, hook.Id),
					$"Source NPC {vnum} {command} hook is attached across its template family.");
			}
		}

		var rentProgs = db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileWagonRent", StringComparison.Ordinal)).ToArray();
		var offerProgs = db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileWagonOffer", StringComparison.Ordinal)).ToArray();
		var sourceWagon = source.Items.Single(x => x.ItemType == 23);
		var eligibleWagonProto = Field<Dictionary<int, GameItemProto>>(writer, "_items")[sourceWagon.Vnum];
		Assert.AreEqual(4, rentProgs.Length);
		Assert.AreEqual(4, offerProgs.Length);
		foreach (var rent in rentProgs)
		{
			StringAssert.Contains(rent.FunctionText, "targetitem(@actor, @actor.location.items, @query)");
			StringAssert.Contains(rent.FunctionText, "eligible = false");
			StringAssert.Contains(rent.FunctionText, $"@wagon.proto == {eligibleWagonProto.Id}", "Only a source wagon prototype is eligible for storage.");
			StringAssert.Contains(rent.FunctionText, "vehicleoccupied(@wagon)");
			Assert.IsTrue(rent.FunctionText.IndexOf("vehicleoccupied(@wagon)", StringComparison.Ordinal) < rent.FunctionText.IndexOf("loaditem(1597)", StringComparison.Ordinal),
				"Occupied vehicles are refused before a ticket is loaded.");
			StringAssert.Contains(rent.FunctionText, "silentget(@actor, @chit)");
			StringAssert.Contains(rent.FunctionText, "relocateemptyvehicle(@wagon, tolocation(@depot), true)");
			Assert.IsFalse(rent.FunctionText.Contains("purchasevehicletransfer", StringComparison.Ordinal), "Storage does not charge a fee.");
		}
		foreach (var offer in offerProgs)
		{
			StringAssert.Contains(offer.FunctionText, "targetitem(@actor, @actor.inventory, @query)");
			StringAssert.Contains(offer.FunctionText, "getexistingitem(@wagonid)");
			StringAssert.Contains(offer.FunctionText, "getregister(@wagon, \"armw_wagon_ticket\") != @chit.id");
			StringAssert.Contains(offer.FunctionText, "@wagon.location != tolocation(@depot)");
			StringAssert.Contains(offer.FunctionText, "purchasevehicletransfer(@actor, @seller, @wagon, @actor.location, tocurrency(9001), 50)");
			Assert.IsFalse(offer.FunctionText.Contains("relocateemptyvehicle", StringComparison.Ordinal), "Redemption uses the exact native purchase-and-relocation operation.");
		}
		Assert.IsFalse(db.FutureProgs.Any(x => x.FunctionName.Contains("WagonToYard", StringComparison.OrdinalIgnoreCase)));
		Assert.IsFalse(db.Hooks.Any(x => x.Name.Contains("wagon_to_yard", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(source.Npcs.All(x => new[] { 3842, 20119, 50455, 50605 }.Contains(x.Vnum)));
		CompileGeneratedProgs(db);
	}

	private static FuturemudDatabaseContext WagonStorageFixture(out ArmageddonWorldfileWriter writer,
		out ArmageddonWorldfilePackage source, out HashSet<long> frontageIds)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var vnums = new[] { 3842, 20119, 50455, 50605 };
		var ticketItem = archive.Items.Single(x => x.Vnum == 1597);
		var sourceWagonItem = archive.Items.First(x => x.ItemType == 23);
		var npcs = archive.Npcs.Where(x => vnums.Contains(x.Vnum)).ToArray();
		var resets = archive.Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 && x.Arguments.Length > 3 && vnums.Contains(x.Arguments[1])).ToArray();
		var roomVnums = resets.Select(x => x.Arguments[3]).Distinct().ToHashSet();
		var rooms = archive.Rooms.Where(x => roomVnums.Contains(x.Vnum)).ToArray();
		Assert.AreEqual(4, npcs.Length, "Fixture uses the four retained source wagon-storage attendants.");
		Assert.AreEqual(4, roomVnums.Count, "Fixture contains the four source attendant frontage rooms.");
		source = new ArmageddonWorldfilePackage
		{
			Npcs = npcs,
			Items = [ticketItem, sourceWagonItem],
			Resets = resets,
			Rooms = rooms
		};
		writer = new ArmageddonWorldfileWriter(db, source);

		foreach (var vnum in vnums)
		{
			var template = new NpcTemplate { Id = vnum, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			var variant = new NpcTemplate { Id = vnum + 100000, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.AddRange(template, variant);
			Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(vnum, template);
			Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(vnum, [template.Id, variant.Id]);
		}

		var items = Field<Dictionary<int, GameItemProto>>(writer, "_items");
		foreach (var item in source.Items)
		{
			var proto = new GameItemProto { Id = item.Vnum, RevisionNumber = 0 };
			db.GameItemProtos.Add(proto);
			items.Add(item.Vnum, proto);
		}
		var currency = new Currency { Id = 9001, Name = "Obsidian" };
		db.Currencies.Add(currency);
		typeof(ArmageddonWorldfileWriter).GetField("_currency", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, currency);
		var overlayPackage = new RoomOverlayPackage { Id = 8000, Name = "Armageddon Fixture", RevisionNumber = 0 };
		db.RoomOverlayPackages.Add(overlayPackage);
		typeof(ArmageddonWorldfileWriter).GetField("_overlays", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, overlayPackage);
		db.Terrains.Add(new Terrain { Id = 8001, Name = "Urban Street" });
		var zones = Field<Dictionary<int, Zone>>(writer, "_zones");
		foreach (var group in rooms.GroupBy(x => x.Zone))
		{
			var zone = new Zone { Id = 9000 + group.Key, Name = "Source zone " + group.Key };
			db.Zones.Add(zone);
			zones.Add(group.Key, zone);
		}
		frontageIds = [];
		var roomMap = Field<Dictionary<int, Room>>(writer, "_rooms");
		foreach (var room in rooms)
		{
			var nativeRoom = new Room { Id = 100000 + room.Vnum, ZoneId = zones[room.Zone].Id, UniqueName = "Vnum" + room.Vnum, EffectData = "<Effects/>" };
			db.Rooms.Add(nativeRoom);
			roomMap.Add(room.Vnum, nativeRoom);
			frontageIds.Add(nativeRoom.Id);
		}
		db.SaveChanges();
		return db;
	}
}
