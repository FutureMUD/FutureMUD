#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonRoomCommandSeederTests
{
	[TestMethod]
	public void GatesAndElevator_PreserveOperationPermissionsAndMissingStopRefusal()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var names = new[] { "arena_gate", "tuluk_gate", "allanak_gate", "luirs_gate", "elevator" };
		var source = new ArmageddonWorldfilePackage { Rooms = archive.Rooms.Where(x => x.Vnum == 66005 || x.Programs.Any(p => names.Any(n => p.EndsWith(" " + n)))).ToArray() };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var rooms = (Dictionary<int, Room>)typeof(ArmageddonWorldfileWriter).GetField("_rooms", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		foreach (var room in source.Rooms)
		{
			var native = new Room { Id = room.Vnum, UniqueName = $"Vnum{room.Vnum}", EffectData = "<Effects/>" };
			db.Rooms.Add(native); rooms.Add(room.Vnum, native);
		}
		var clans = (Dictionary<int, Clan>)typeof(ArmageddonWorldfileWriter).GetField("_clans", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		var ranks = (Dictionary<(int, int), Rank>)typeof(ArmageddonWorldfileWriter).GetField("_ranks", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		foreach (var id in new[] { 4, 8, 12, 23 }) clans.Add(id, new Clan { Id = id });
		ranks.Add((4, 4), new Rank { Id = 44, ClanId = 4, RankNumber = 4 });
		((Dictionary<int, GameItemProto>)typeof(ArmageddonWorldfileWriter).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!).Add(66000, new GameItemProto { Id = 166000 });
		((Dictionary<string, GameItemProto>)typeof(ArmageddonWorldfileWriter).GetField("_resetItemVariants", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!).Add("66000:fixture", new GameItemProto { Id = 266000 });
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallGateCommands", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallStuckElevator", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		Assert.AreEqual(14, db.HooksPerceivables.Count());
		Assert.IsTrue(db.Hooks.All(x => x.Definition.Contains("<UseProgResultForHandling>true</UseProgResultForHandling>")));
		StringAssert.Contains(db.FutureProgs.Single(x => x.FunctionName.EndsWith("luirs_gate")).FunctionText, "torank(44)");
		var elevator = db.FutureProgs.Single(x => x.FunctionName.EndsWith("StuckElevator"));
		StringAssert.Contains(elevator.FunctionText, "@box.proto == 166000");
		StringAssert.Contains(elevator.FunctionText, "@box.proto == 266000");
		StringAssert.Contains(elevator.FunctionText, "won't budge");
		Assert.IsFalse(elevator.FunctionText.Contains("teleport"));
		Assert.AreEqual(source.Rooms.Length, db.Rooms.Count(), "No missing elevator stops are invented.");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
	}

	[TestMethod]
	public void RoomCommands_CompileExistingInputHooksAndPreserveExplicitDestinations()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.TraitDefinitions.Add(new TraitDefinition { Id = 3, Type = 1, Name = "Agility" }); db.SaveChanges();
		db.MagicSchools.Add(new MagicSchool { Id = 1, Name = "Water", SchoolVerb = "armwater" });
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var vnums = new[] { 1003, 50617, 25165, 25167, 25604 };
		var source = new ArmageddonWorldfilePackage { Rooms = archive.Rooms.Where(x => vnums.Contains(x.Vnum) || x.Programs.Contains("C south tal_zen_tunnel")).ToArray(),
			Npcs = [archive.Npcs.Single(x => x.Vnum == 50033)] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var bartender = new NpcTemplate { Id = 10000, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
		db.NpcTemplates.Add(bartender);
		((Dictionary<int, NpcTemplate>)typeof(ArmageddonWorldfileWriter).GetField("_npcs", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!).Add(50033, bartender);
		foreach (var (key, type, id) in new[] { ("arm.class.jihaen_templar", "ChargenRole", 100L), ("arm.capability.water", "MagicCapability", 101L) })
			db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = "Fixture", StableKey = key, EntityType = type, LogicalId = id });
		var clans = (Dictionary<int, Clan>)typeof(ArmageddonWorldfileWriter).GetField("_clans", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		clans.Add(12, new Clan { Id = 12 });
		var rooms = (Dictionary<int, Room>)typeof(ArmageddonWorldfileWriter).GetField("_rooms", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		foreach (var vnum in source.Rooms.Select(x => x.Vnum))
		{
			var room = new Room { Id = vnum + 100000, UniqueName = $"Vnum{vnum}", EffectData = "<Effects/>" };
			rooms.Add(vnum, room); db.Rooms.Add(room);
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallRoomCommands", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallTalZenHazard", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNpcCommandSpecials", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		Assert.AreEqual(4, db.Hooks.Count()); Assert.AreEqual(3, db.HooksPerceivables.Count());
		Assert.IsTrue(db.Hooks.Any(x => x.Definition.Contains("<TargetCommand>*</TargetCommand>")));
		Assert.IsTrue(db.Hooks.All(x => x.Type == "HookOnInput"));
		var hall = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileHallDeparture");
		StringAssert.Contains(hall.FunctionText, "tolocation(150617)");
		Assert.IsFalse(hall.FunctionText.Contains("givetrait"));
		StringAssert.Contains(bartender.Definition, "<Hook id=");
		StringAssert.Contains(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileBartenderCasting").FunctionText, "torole(100)");
		StringAssert.Contains(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileTalZenRefusal").FunctionText, "random(3, 8) * floor");
		Assert.IsTrue(db.Rooms.Any(x => x.EffectData.Contains("DeniedAttemptProg")));
		foreach (var jump in db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileChasmJump")).ToArray())
		{
			StringAssert.Contains(jump.FunctionText, "tolocation(125604)");
			StringAssert.Contains(jump.FunctionText, "random(-5, 5)");
		}
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
	}
}
