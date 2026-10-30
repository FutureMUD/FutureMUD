#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfilePostalTests
{
	[TestMethod]
	public void PostalRouteMatrixMatchesSourceAndGeneratedProgsCompile()
	{
		Assert.AreEqual(4, ArmageddonWorldfileWriter.SourcePostalOffices.Count);
		Assert.AreEqual((50033, 50937), (ArmageddonWorldfileWriter.SourcePostalOffices[0].NpcVnum,
			ArmageddonWorldfileWriter.SourcePostalOffices[0].RoomVnum));
		Assert.AreEqual((20115, 28028), (ArmageddonWorldfileWriter.SourcePostalOffices[1].NpcVnum,
			ArmageddonWorldfileWriter.SourcePostalOffices[1].RoomVnum));
		Assert.AreEqual((64007, 64027), (ArmageddonWorldfileWriter.SourcePostalOffices[2].NpcVnum,
			ArmageddonWorldfileWriter.SourcePostalOffices[2].RoomVnum));
		Assert.AreEqual((48001, 48061), (ArmageddonWorldfileWriter.SourcePostalOffices[3].NpcVnum,
			ArmageddonWorldfileWriter.SourcePostalOffices[3].RoomVnum));
		Assert.AreEqual(new ArmageddonWorldfileWriter.PostalRoute(5, 90, 10), ArmageddonWorldfileWriter.SourcePostalRoutes[0, 1]);
		Assert.AreEqual(new ArmageddonWorldfileWriter.PostalRoute(2, 98, 5), ArmageddonWorldfileWriter.SourcePostalRoutes[0, 3]);
		Assert.AreEqual(new ArmageddonWorldfileWriter.PostalRoute(8, 88, 12), ArmageddonWorldfileWriter.SourcePostalRoutes[3, 1]);

		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.Clocks.Add(new Clock { Id = 20, PrimaryTimezoneId = 20, Definition = "<Clock><InGameSecondsPerRealSecond>12</InGameSecondsPerRealSecond><SecondsPerMinute>60</SecondsPerMinute></Clock>" });
		db.Timezones.Add(new Timezone { Id = 20, ClockId = 20, Name = "AST" });
		db.Calendars.Single().FeedClockId = 20;
		db.Calendars.Single().Date = "1/1/1";
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var officeVnums = ArmageddonWorldfileWriter.SourcePostalOffices.Select(x => x.NpcVnum).ToHashSet();
		var officeResets = archive.Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 &&
			x.Arguments.Length > 3 && officeVnums.Contains(x.Arguments[1]) &&
			ArmageddonWorldfileWriter.SourcePostalOffices.Any(y => y.NpcVnum == x.Arguments[1] && y.RoomVnum == x.Arguments[3])).ToArray();
		var officeRoomVnums = officeResets.Select(x => x.Arguments[3]).Distinct().ToHashSet();
		var source = new ArmageddonWorldfilePackage
		{
			Npcs = archive.Npcs.Where(x => officeVnums.Contains(x.Vnum)).ToArray(),
			Resets = officeResets,
			Rooms = archive.Rooms.Where(x => officeRoomVnums.Contains(x.Vnum)).ToArray()
		};
		var writer = new ArmageddonWorldfileWriter(db, source);
		var zones = Field<Dictionary<int, Zone>>(writer, "_zones");
		var rooms = Field<Dictionary<int, Room>>(writer, "_rooms");
		var npcTemplates = Field<Dictionary<int, NpcTemplate>>(writer, "_npcs");
		var npcFamilies = Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds");
		long nextZoneId = (db.Zones.Max(x => (long?)x.Id) ?? 0) + 9000;
		long nextRoomId = (db.Rooms.Max(x => (long?)x.Id) ?? 0) + 9000;
		long nextNpcId = (db.NpcTemplates.Max(x => (long?)x.Id) ?? 0) + 9000;
		foreach (var sourceRoom in source.Rooms)
		{
			if (!zones.TryGetValue(sourceRoom.Zone, out var zone))
			{
				zone = new Zone { Id = nextZoneId++, Name = "Postal test zone " + sourceRoom.Zone };
				db.Zones.Add(zone);
				zones.Add(sourceRoom.Zone, zone);
			}
			var room = new Room { Id = nextRoomId++, ZoneId = zone.Id, UniqueName = "PostalTestRoom" + sourceRoom.Vnum, EffectData = "<Effects/>" };
			db.Rooms.Add(room);
			rooms.Add(sourceRoom.Vnum, room);
		}
		foreach (var office in source.Npcs)
		{
			var template = new NpcTemplate
			{
				Id = nextNpcId++, RevisionNumber = 0,
				Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>"
			};
			db.NpcTemplates.Add(template);
			npcTemplates.Add(office.Vnum, template);
			npcFamilies.Add(office.Vnum, [template.Id]);
		}
		db.Terrains.Add(new Terrain { Id = (db.Terrains.Max(x => (long?)x.Id) ?? 0) + 1, Name = "Urban Street" });
		var overlayPackage = new RoomOverlayPackage
		{
			Id = (db.RoomOverlayPackages.Max(x => (long?)x.Id) ?? 0) + 1, Name = "Postal test", RevisionNumber = 0
		};
		db.RoomOverlayPackages.Add(overlayPackage);
		Set(writer, "_overlays", overlayPackage);
		var currency = new Currency { Id = (db.Currencies.Max(x => (long?)x.Id) ?? 0) + 1, Name = "Postal test currency" };
		db.Currencies.Add(currency);
		Set(writer, "_currency", currency);
		db.FutureProgs.Add(new MudSharp.Models.FutureProg
		{
			Id = 99001, FunctionName = "ArmageddonWorldfileClan28Leader", FunctionText = "return true",
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
			Category = "Fixture", Subcategory = "Fixture",
			FutureProgsParameters = { new() { ParameterIndex = 0, ParameterName = "actor", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() } }
		});
		db.SaveChanges();

		typeof(ArmageddonWorldfileWriter).GetMethod("InstallPostal", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(),
			db.VariableDefinitions.ToArray());
		var generated = db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfilePostal", StringComparison.Ordinal) ||
			x.FunctionName == "ArmageddonWorldfilePostalDelivery").ToArray();
		Assert.IsTrue(generated.Length >= 7);
		foreach (var prog in generated) compiler.Compile(prog.Id);
		Assert.IsTrue(db.ProgSchedules.Any(x => x.Name == "Armageddon physical post transit"));
		Assert.AreEqual(2, db.VariableDefinitions.Count(x => x.Property == "armw_postal_addresses" ||
			x.Property == "armw_postal_address_offices"));
	}

	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;

	private static void Set(ArmageddonWorldfileWriter writer, string name, object value) =>
		typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, value);
}
