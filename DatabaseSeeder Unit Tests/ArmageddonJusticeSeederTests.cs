#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;
using CrimeTypes = MudSharp.RPG.Law.CrimeTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonJusticeSeederTests
{
	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;

	[TestMethod]
	[DataRow("luir_soldier", 64, 6.0)]
	[DataRow("red_storm_soldier", 48, 1.0)]
	public void Justice_GeneratesCustodyAndFixedLawsAndCompilesOnloadEligibilityCall(string special, int zoneNumber, double clockRate)
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.Clocks.Add(new Clock { Id = 20, Definition = new System.Xml.Linq.XElement("Clock",
			new System.Xml.Linq.XElement("InGameSecondsPerRealSecond", clockRate)).ToString() });
		db.Calendars.Single().FeedClockId = 20;
		var guard = ArmageddonWorldfileTestData.Load().Npcs.First(x => x.Programs.Any(p => p.EndsWith(" " + special)));
		var source = new ArmageddonWorldfilePackage { Npcs = [guard], Resets =
			[new(new("fixture.cmd", 1), zoneNumber, "M", [0, guard.Vnum, 1, 10], [], [])] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var currency = new Currency { Id = 10, Name = "Obsidian" }; db.Currencies.Add(currency);
		typeof(ArmageddonWorldfileWriter).GetField("_currency", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, currency);
		var package = new RoomOverlayPackage { Id = 10, Name = "Imported", RevisionNumber = 0 };
		db.RoomOverlayPackages.Add(package);
		typeof(ArmageddonWorldfileWriter).GetField("_overlays", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, package);
		var zone = new Zone { Id = zoneNumber, Name = "Guarded Settlement" }; db.Zones.Add(zone); Field<Dictionary<int, Zone>>(writer, "_zones").Add(zoneNumber, zone);
		db.Terrains.Add(new Terrain { Id = 10, Name = "Urban Street" });
		var room = new Room { Id = 10, ZoneId = zoneNumber, UniqueName = "Vnum10", EffectData = "<Effects/>" };
		room.RoomOverlays.Add(new RoomOverlay { Id = 10, RoomId = 10, RoomName = "Guard Post" });
		db.Rooms.Add(room); Field<Dictionary<int, Room>>(writer, "_rooms").Add(10, room);
		var npc = new NpcTemplate { Id = 10, RevisionNumber = 0, Name = "Guard", UniqueName = $"Vnum{guard.Vnum}" };
		db.NpcTemplates.Add(npc); Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(guard.Vnum, npc);
		Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(guard.Vnum, [10]);
		var cash = new FutureProg { Id = 10, FunctionName = "ArmageddonWorldfileNpcCash" + guard.Vnum,
			FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
		cash.FutureProgsParameters.Add(new FutureProgsParameter { ParameterIndex = 0, ParameterName = "npc",
			ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(cash); Field<List<FutureProg>>(writer, "_progs").Add(cash); db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallJustice", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		Assert.AreEqual(1, db.LegalAuthorities.Count()); Assert.AreEqual(3, db.Laws.Count());
		foreach (var law in db.Laws)
			Assert.AreEqual(6000.0 * clockRate, law.ActivePeriod,
				$"{law.Name} must use the maximum ten source duration units, converted to native clock seconds.");
		var authority = db.LegalAuthorities.Single(); Assert.IsTrue(authority.AutomaticallyConvict);
		Assert.AreEqual(300.0, authority.AutomaticConvictionTime); Assert.AreEqual(6, db.Rooms.Count());
		Assert.IsNotNull(authority.PrisonReleaseLocationId); Assert.IsNotNull(authority.PrisonBelongingsLocationId);
		Assert.IsTrue(db.Rooms.Where(x => x.Id != 10).All(x => x.CurrentOverlayId.HasValue));
		Assert.AreEqual(1, db.PatrolRoutes.Count()); Assert.AreEqual(1, npc.NpctemplatesArtificalIntelligences.Count);
		StringAssert.Contains(cash.FunctionText, $"if (@ArmageddonWorldfileEnforcerRole{special}(@npc))");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
	}

	[TestMethod]
	public void SpiceTraffickingDuration_UsesTheMaximumFiveSourceFlagUnits()
	{
		Assert.AreEqual(3000.0, ArmageddonWorldfileWriter.SourceCrimeMaximumRealSeconds(CrimeTypes.TrafficingContraband));
	}
}
